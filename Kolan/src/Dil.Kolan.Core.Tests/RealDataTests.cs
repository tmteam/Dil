using Dil.Core;
using Dil.Core.Models;

namespace Dil.Kolan.Core.Tests;

/// <summary>
/// Тест на реальных данных из дедушкиного примера расчёта.
/// Файл /tmp/dil_test_data.csv должен быть предварительно экспортирован из Excel.
/// </summary>
public class RealDataTests
{
    private static List<RutMeasurement> LoadTestData()
    {
        var lines = File.ReadAllLines("/tmp/dil_test_data.csv").Skip(1);
        var data = new List<RutMeasurement>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            data.Add(new RutMeasurement(
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture),
                double.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture)));
        }
        return data;
    }

    // ==========================================================
    // Тесты с окном 10 (правильное, подтверждённое дедушкой)
    // ==========================================================

    [Fact]
    public void Window10_ProducesFourCards()
    {
        var data = LoadTestData();
        var analyzer = new RutAnalyzer();
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = analyzer.Analyze(data, p);

        // С окном 10 получается 4 карты (вместо 3 с окном 9),
        // потому что средние в районе 4+274 опустились ниже 20 мм,
        // разбив вторую группу примера на две.
        Assert.Equal(4, cards.Count);
    }

    [Fact]
    public void Window10_Card1_MatchesExample()
    {
        var data = LoadTestData();
        var analyzer = new RutAnalyzer();
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = analyzer.Analyze(data, p);
        var card1 = cards[0];

        // Совпадает с примером: 3+903 .. 4+039
        Assert.Equal(new Distance(3, 903), card1.Start);
        Assert.Equal(new Distance(4, 39), card1.End);
        Assert.False(card1.WasExtended);
    }

    [Fact]
    public void Window10_Card4_ExtendedCorrectly()
    {
        var data = LoadTestData();
        var analyzer = new RutAnalyzer();
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = analyzer.Analyze(data, p);
        var card4 = cards[3];

        // Совпадает с примером: удлинённая карта начинается с 5+501
        Assert.True(card4.WasExtended);
        Assert.Equal(100, card4.LengthMeters);
        Assert.Equal(5, card4.Start.Km);
        Assert.Equal(501, card4.Start.M);
    }

    // ==========================================================
    // Тесты с окном 9 (как в Excel-примере — для верификации)
    // ==========================================================

    [Fact]
    public void Window9_ProducesThreeGroups_AsInExample()
    {
        var data = LoadTestData();
        var analyzer = new RutAnalyzer();
        // Окно 9 = как в Excel-примере (с ошибкой)
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 9, MinCardLength: 100, MergeDistance: 100);

        var (avgL, avgR) = analyzer.ComputeSlidingAverages(data, 9);
        var unsat = analyzer.MarkUnsatisfactory(avgL, avgR, 20.0);
        var groups = analyzer.GroupIntoCards(unsat, 100);

        // С окном 9 — 3 группы, как в примере
        Assert.Equal(3, groups.Count);
    }

    [Fact]
    public void Window9_YellowCount_MatchesExample()
    {
        var data = LoadTestData();
        var analyzer = new RutAnalyzer();

        var (avgL, avgR) = analyzer.ComputeSlidingAverages(data, 9);
        var unsat = analyzer.MarkUnsatisfactory(avgL, avgR, 20.0);

        // В примере 948 жёлтых строк
        Assert.Equal(948, unsat.Count(x => x));
    }

    [Fact]
    public void Window9_Card1_RefinedAddressesMatchExample()
    {
        var data = LoadTestData();
        var analyzer = new RutAnalyzer();
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 9, MinCardLength: 100, MergeDistance: 100);

        var cards = analyzer.Analyze(data, p);
        var card1 = cards[0];

        // Пример: 3+903 .. 4+039
        Assert.Equal(new Distance(3, 903), card1.Start);
        Assert.Equal(new Distance(4, 39), card1.End);
    }
}
