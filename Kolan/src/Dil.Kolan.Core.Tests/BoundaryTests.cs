using Dil.Core;
using Dil.Core.Models;

namespace Dil.Kolan.Core.Tests;

/// <summary>
/// Атомарные тесты граничных условий для каждого шага алгоритма.
/// </summary>
public class BoundaryTests
{
    private readonly RutAnalyzer _a = new();

    private static List<RutMeasurement> Make(double[] left, double[] right)
    {
        var list = new List<RutMeasurement>(left.Length);
        int km = 1, m = 0;
        for (int i = 0; i < left.Length; i++)
        {
            list.Add(new RutMeasurement(km, m, left[i], right[i]));
            m++;
            if (m >= 1000) { m = 0; km++; }
        }
        return list;
    }

    private static List<RutMeasurement> MakeConst(int count, double left, double right)
    {
        var l = new double[count];
        var r = new double[count];
        Array.Fill(l, left);
        Array.Fill(r, right);
        return Make(l, r);
    }

    // ================================================================
    // ШАГ 1. СКОЛЬЗЯЩЕЕ СРЕДНЕЕ — граничные условия
    // ================================================================

    [Fact]
    public void Avg_SingleValue_ReturnsThatValue()
    {
        var data = MakeConst(1, 15.0, 8.0);
        var (left, right) = _a.ComputeSlidingAverages(data, 10);
        Assert.Equal(15.0, left[0]);
        Assert.Equal(8.0, right[0]);
    }

    [Fact]
    public void Avg_LeftAndRight_ComputedIndependently()
    {
        // Левая растёт, правая падает
        var data = Make([10, 20, 30], [30, 20, 10]);
        var (left, right) = _a.ComputeSlidingAverages(data, 3);
        Assert.Equal(20.0, left[0], precision: 10);  // avg(10,20,30)
        Assert.Equal(20.0, right[0], precision: 10);  // avg(30,20,10)
    }

    [Fact]
    public void Avg_LastElement_AlwaysEqualsRawValue()
    {
        var data = Make([5, 10, 99], [1, 2, 77]);
        var (left, right) = _a.ComputeSlidingAverages(data, 10);
        // Последний элемент: окно из 1 значения
        Assert.Equal(99.0, left[2]);
        Assert.Equal(77.0, right[2]);
    }

    [Fact]
    public void Avg_WindowSize1_ReturnsRawValues()
    {
        var data = Make([5, 10, 15], [1, 2, 3]);
        var (left, _) = _a.ComputeSlidingAverages(data, 1);
        Assert.Equal(5.0, left[0]);
        Assert.Equal(10.0, left[1]);
        Assert.Equal(15.0, left[2]);
    }

    // ================================================================
    // ШАГ 2. ПОРОГ — граничные условия >= (не >)
    // ================================================================

    [Fact]
    public void Threshold_ExactlyEqual_IsUnsatisfactory()
    {
        var result = _a.MarkUnsatisfactory([20.0], [0.0], 20.0);
        Assert.True(result[0]);
    }

    [Fact]
    public void Threshold_JustBelow_IsSatisfactory()
    {
        var result = _a.MarkUnsatisfactory([19.999999], [19.999999], 20.0);
        Assert.False(result[0]);
    }

    [Fact]
    public void Threshold_LeftAtThreshold_RightBelow_Unsatisfactory()
    {
        var result = _a.MarkUnsatisfactory([20.0], [10.0], 20.0);
        Assert.True(result[0]);
    }

    [Fact]
    public void Threshold_LeftBelow_RightAtThreshold_Unsatisfactory()
    {
        var result = _a.MarkUnsatisfactory([10.0], [20.0], 20.0);
        Assert.True(result[0]);
    }

    [Fact]
    public void Threshold_BothBelow_Satisfactory()
    {
        var result = _a.MarkUnsatisfactory([19.9], [19.9], 20.0);
        Assert.False(result[0]);
    }

    // ================================================================
    // ШАГИ 3-4. ГРУППИРОВКА — граничные условия расстояния
    // ================================================================

    [Fact]
    public void Group_GapExactlyMergeDistance_NotMerged()
    {
        // gap = 100 при mergeDistance = 100 → НЕ объединяем (< 100, а не <= 100)
        var unsat = new bool[200];
        unsat[0] = true;
        unsat[100] = true; // gap = 100
        var groups = _a.GroupIntoCards(unsat, 100);
        Assert.Equal(2, groups.Count);
    }

    [Fact]
    public void Group_GapOneLessThanMergeDistance_Merged()
    {
        // gap = 99 при mergeDistance = 100 → объединяем
        var unsat = new bool[200];
        unsat[0] = true;
        unsat[99] = true; // gap = 99 < 100
        var groups = _a.GroupIntoCards(unsat, 100);
        Assert.Single(groups);
        Assert.Equal((0, 99), groups[0]);
    }

    [Fact]
    public void Group_AdjacentBadMeters_GapIs1_Merged()
    {
        var unsat = new bool[] { true, true, true };
        var groups = _a.GroupIntoCards(unsat, 100);
        Assert.Single(groups);
        Assert.Equal((0, 2), groups[0]);
    }

    [Fact]
    public void Group_SingleBadMeter_OneCard()
    {
        var unsat = new bool[100];
        unsat[50] = true;
        var groups = _a.GroupIntoCards(unsat, 100);
        Assert.Single(groups);
        Assert.Equal((50, 50), groups[0]);
    }

    [Fact]
    public void Group_AllBadMeters_OneCard()
    {
        var unsat = Enumerable.Repeat(true, 500).ToArray();
        var groups = _a.GroupIntoCards(unsat, 100);
        Assert.Single(groups);
        Assert.Equal((0, 499), groups[0]);
    }

    // ================================================================
    // ПОКРЫТИЕ КАРТЫ — граничные условия длины
    // ================================================================

    [Fact]
    public void Coverage_ExactlyMinLength_NotExtended()
    {
        // Нужен coverage = ровно 100.
        // Значения ровно 20.0 (на пороге) в позициях 0-99, далее 0.0.
        // Десятиметровка в позиции 90: avg(90..99) = 20.0 >= 20 → последняя неудовлетворительная.
        // Десятиметровка в позиции 91: avg(91..100) = (9*20 + 0)/10 = 18 < 20 → удовлетворительная.
        // coverage = [0, 99], длина = 100.
        var left = new double[200];
        var right = new double[200];
        Array.Fill(left, 0.0);
        Array.Fill(right, 0.0);
        for (int i = 0; i <= 99; i++) left[i] = 20.0;

        var data = Make(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);

        Assert.Single(cards);
        Assert.False(cards[0].WasExtended);
    }

    [Fact]
    public void Coverage_OneLessThanMinLength_Extended()
    {
        // coverage = 99 → должен удлиняться
        // end - start + 10 = 99 → end - start = 89 → 90 подряд (0-89)
        var left = new double[300];
        var right = new double[300];
        Array.Fill(left, 10.0);
        Array.Fill(right, 5.0);
        for (int i = 0; i <= 89; i++) left[i] = 25.0;

        var data = Make(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);

        Assert.Single(cards);
        Assert.True(cards[0].WasExtended);
        Assert.Equal(100, cards[0].LengthMeters);
    }

    [Fact]
    public void Coverage_ClampsToDataEnd()
    {
        // Плохая десятиметровка в самом конце данных
        // end = последний индекс, coverage = min(end + 9, count-1) - start + 1
        var left = new double[50];
        var right = new double[50];
        Array.Fill(left, 10.0);
        Array.Fill(right, 5.0);
        // Плохой участок: последние 5 значений (45-49)
        for (int i = 45; i < 50; i++) left[i] = 25.0;

        var data = Make(left, right);
        // minCardLength=10 чтобы не удлинять (coverage = min(49+9,49) - 45 + 1 = 5)
        // С minCardLength=5: coverage=5 >= 5, не удлиняется
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 5, MergeDistance: 100);
        var cards = _a.Analyze(data, p);

        Assert.Single(cards);
        Assert.False(cards[0].WasExtended);
        // Конец карты не выходит за массив
        Assert.True(cards[0].EndIndex < 50);
    }

    // ================================================================
    // ШАГ 5. УДЛИНЕНИЕ — граничные условия
    // ================================================================

    [Fact]
    public void Extend_ResultLengthEqualsMinLength()
    {
        var data = MakeConst(300, 15.0, 10.0);
        var (s, e) = _a.ExtendCard(data, 100, 110, minLength: 100);
        Assert.Equal(100, e - s + 1);
    }

    [Fact]
    public void Extend_CardAtVeryStart_DoesNotGoNegative()
    {
        var data = MakeConst(200, 15.0, 10.0);
        var (s, e) = _a.ExtendCard(data, 0, 5, minLength: 100);
        Assert.True(s >= 0);
        Assert.Equal(100, e - s + 1);
    }

    [Fact]
    public void Extend_CardAtVeryEnd_DoesNotExceedArray()
    {
        var data = MakeConst(200, 15.0, 10.0);
        var (s, e) = _a.ExtendCard(data, 195, 199, minLength: 100);
        Assert.True(e < 200);
        Assert.Equal(100, e - s + 1);
    }

    [Fact]
    public void Extend_CriterionIsMaxOfTwoRuts_NotSum()
    {
        // Два пика в зоне поиска:
        // Пик А: только левая глубокая → max(L,R) высокий
        // Пик Б: обе умеренные, но сумма больше → sum выше, max ниже
        // MAX критерий должен выбрать A
        var left = new double[200];
        var right = new double[200];
        Array.Fill(left, 5.0);
        Array.Fill(right, 5.0);

        // Пик А: глубокая ЛЕВАЯ в районе 60-70 (в зоне поиска)
        for (int i = 60; i <= 70; i++) left[i] = 25.0;
        // Пик Б: обе средние в районе 120-130 (в зоне поиска)
        for (int i = 120; i <= 130; i++) { left[i] = 12.0; right[i] = 12.0; }

        var data = Make(left, right);
        // Карта в середине, deficit покроет оба пика
        var (s, e) = _a.ExtendCard(data, 90, 100, minLength: 50);

        // Пик А: avg_left для окна покрывающего 60-70 = (11*25 + 39*5)/50 = 8.7
        // max(8.7, 5.0) = 8.7
        // Пик Б: avg_left = (11*12+39*5)/50 = 6.54, avg_right = 6.54
        // max(6.54, 6.54) = 6.54
        // A > B, поэтому окно должно покрывать пик А
        Assert.True(s <= 60 && e >= 70, $"Window [{s},{e}] should cover peak A at 60-70");
    }

    [Fact]
    public void Extend_EqualRuts_StillWorks()
    {
        // Все значения одинаковые — любое окно допустимо
        var data = MakeConst(200, 15.0, 15.0);
        var (s, e) = _a.ExtendCard(data, 80, 90, minLength: 50);
        Assert.Equal(50, e - s + 1);
    }

    // ================================================================
    // ШАГ 6. УТОЧНЕНИЕ АДРЕСОВ — граничные условия
    // ================================================================

    [Fact]
    public void Refine_AllAboveThreshold_NoTrimming()
    {
        var data = MakeConst(20, 25.0, 25.0);
        var (s, e) = _a.RefineAddresses(data, 0, 19, 20.0);
        Assert.Equal(0, s);
        Assert.Equal(19, e);
    }

    [Fact]
    public void Refine_OnlyRightRutExceeds_CountsForBoundary()
    {
        var left = new double[20];
        var right = new double[20];
        Array.Fill(left, 10.0);
        Array.Fill(right, 10.0);
        right[5] = 22.0;  // единственный метр выше порога — по правой колее
        var data = Make(left, right);
        var (s, e) = _a.RefineAddresses(data, 0, 19, 20.0);
        Assert.Equal(5, s);
        Assert.Equal(5, e);
    }

    [Fact]
    public void Refine_AverageAboveThreshold_ButNoMeterAbove_ReturnsOriginalBounds()
    {
        // Среднее по 10 метрам >= 20, но каждый отдельный метр < 20.
        // Это возможно? Нет: если avg(a1..a10) >= 20 и каждый ai < 20, невозможно.
        // Но если данные подобраны специально — все ровно 19.5 — среднее = 19.5 < 20.
        // Для того чтобы avg >= 20 при каждом < 20 — невозможно.
        // Однако при уточнении мы смотрим на ВЕСЬ диапазон coverage (не только десятиметровки).
        // Если десятиметровка на позиции X имеет avg >= 20, но все отдельные метры X..X+9 < 20 — невозможно.
        // Тем не менее проверим, что код не падает и возвращает исходные границы, если ни один метр не >= порога.
        var data = MakeConst(20, 19.0, 19.0);
        var (s, e) = _a.RefineAddresses(data, 0, 19, 20.0);
        // Нет метров >= 20 → код вернёт first=0 и last=19 (значения по умолчанию)
        Assert.Equal(0, s);
        Assert.Equal(19, e);
    }

    // ================================================================
    // ИНТЕГРАЦИЯ: УДЛИНЁННЫЕ КАРТЫ НЕ УТОЧНЯЮТСЯ
    // ================================================================

    [Fact]
    public void ExtendedCard_IncludesMetersBelow_Threshold()
    {
        // Короткий плохой участок, удлинённая карта будет содержать метры с колеёй < 20
        var left = new double[300];
        var right = new double[300];
        Array.Fill(left, 10.0);
        Array.Fill(right, 5.0);
        for (int i = 150; i <= 160; i++) left[i] = 25.0;

        var data = Make(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);

        Assert.Single(cards);
        Assert.True(cards[0].WasExtended);
        // Удлинённая карта включает метры где колея < 20
        Assert.True(cards[0].AvgLeftRut < 20.0,
            "Extended card average should include sub-threshold meters");
    }

    [Fact]
    public void NonExtendedCard_IsRefined_TrimsBadMeters()
    {
        // Длинный участок (> 100м) где первые и последние несколько метров < порога
        var left = new double[200];
        var right = new double[200];
        Array.Fill(left, 25.0);
        Array.Fill(right, 25.0);
        // Начало: первые 5 метров < порога
        for (int i = 0; i < 5; i++) { left[i] = 10.0; right[i] = 10.0; }
        // Конец: последние 5 метров < порога
        for (int i = 195; i < 200; i++) { left[i] = 10.0; right[i] = 10.0; }

        var data = Make(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);

        Assert.Single(cards);
        Assert.False(cards[0].WasExtended);
        // Уточнение должно обрезать начало и конец
        Assert.True(cards[0].StartIndex >= 5,
            $"Refined start {cards[0].StartIndex} should be >= 5");
    }

    // ================================================================
    // ИНТЕГРАЦИЯ: КМ-ГРАНИЦА
    // ================================================================

    [Fact]
    public void Card_SpansKmBoundary_HasCorrectStartEndKm()
    {
        var data = new List<RutMeasurement>();
        // Км 3: 800 метров (нестандартный!)
        for (int m = 0; m < 800; m++)
            data.Add(new RutMeasurement(3, m, 25.0, 25.0));
        // Км 4: 800 метров
        for (int m = 0; m < 800; m++)
            data.Add(new RutMeasurement(4, m, 25.0, 25.0));

        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);

        Assert.Single(cards);
        Assert.Equal(3, cards[0].Start.Km);
        Assert.Equal(4, cards[0].End.Km);
    }

    [Fact]
    public void Card_EntirelyWithinOneKm_CorrectAddress()
    {
        var data = new List<RutMeasurement>();
        for (int m = 0; m < 500; m++)
            data.Add(new RutMeasurement(5, m, 10.0, 5.0));
        // Плохой участок: метры 200-350 (151 метр)
        for (int i = 200; i <= 350; i++)
            data[i] = data[i] with { LeftRut = 25.0 };

        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);

        Assert.Single(cards);
        Assert.Equal(5, cards[0].Start.Km);
        Assert.Equal(5, cards[0].End.Km);
        Assert.Equal(200, cards[0].Start.M);
    }

    // ================================================================
    // ШАГ 7. СТАТИСТИКА
    // ================================================================

    [Fact]
    public void Stats_AvgAndMax_ComputedOverCardRange()
    {
        var left = new double[120];
        var right = new double[120];
        Array.Fill(left, 22.0);
        Array.Fill(right, 18.0);
        left[60] = 35.0;
        right[70] = 30.0;
        var data = Make(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);
        var card = cards[0];
        Assert.Equal(35.0, card.MaxLeftRut);
        Assert.Equal(30.0, card.MaxRightRut);
        Assert.True(card.AvgLeftRut > 22.0);
        Assert.True(card.AvgRightRut > 18.0);
    }

    [Fact]
    public void Stats_ExtendedCard_StatsOverFullExtendedRange()
    {
        var left = new double[300];
        var right = new double[300];
        Array.Fill(left, 10.0);
        Array.Fill(right, 5.0);
        for (int i = 140; i <= 155; i++) left[i] = 25.0;
        var data = Make(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);
        var cards = _a.Analyze(data, p);
        Assert.Single(cards);
        Assert.True(cards[0].WasExtended);
        Assert.Equal(25.0, cards[0].MaxLeftRut);
        Assert.True(cards[0].AvgLeftRut < 15.0);
    }
}
