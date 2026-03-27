using Dil.Core;
using Dil.Core.Models;

namespace Dil.Kolan.Core.Tests;

public class RutAnalyzerTests
{
    private readonly RutAnalyzer _analyzer = new();

    // --- Вспомогательные методы ---

    /// <summary>
    /// Создаёт список измерений заданной длины.
    /// Км и метр нарастают последовательно, между км столбами — metersPerKm метров.
    /// leftRut/rightRut задают значения для всех точек.
    /// </summary>
    private static List<RutMeasurement> MakeConstant(
        int count, double leftRut, double rightRut, int startKm = 1, int startMeter = 0, int metersPerKm = 1000)
    {
        var list = new List<RutMeasurement>(count);
        int km = startKm;
        int m = startMeter;
        for (int i = 0; i < count; i++)
        {
            list.Add(new RutMeasurement(km, m, leftRut, rightRut));
            m++;
            if (m >= metersPerKm)
            {
                m = 0;
                km++;
            }
        }
        return list;
    }

    /// <summary>
    /// Создаёт список измерений с заданными значениями левой колеи.
    /// Правая колея = 0. Адресация последовательная.
    /// </summary>
    private static List<RutMeasurement> MakeFromLeftValues(double[] leftValues)
    {
        var list = new List<RutMeasurement>(leftValues.Length);
        for (int i = 0; i < leftValues.Length; i++)
            list.Add(new RutMeasurement(1, i, leftValues[i], 0));
        return list;
    }

    private static List<RutMeasurement> MakeFromValues(double[] leftValues, double[] rightValues)
    {
        var list = new List<RutMeasurement>(leftValues.Length);
        for (int i = 0; i < leftValues.Length; i++)
            list.Add(new RutMeasurement(1, i, leftValues[i], rightValues[i]));
        return list;
    }

    // =============================================
    // Шаг 1. Скользящее среднее
    // =============================================

    [Fact]
    public void SlidingAverage_FullWindow()
    {
        // 10 значений: 1,2,3,...,10. Окно=10. Первый элемент = среднее всех = 5.5
        var values = Enumerable.Range(1, 10).Select(v => (double)v).ToArray();
        var data = MakeFromLeftValues(values);

        var (left, _) = _analyzer.ComputeSlidingAverages(data, 10);

        Assert.Equal(5.5, left[0], precision: 10);
    }

    [Fact]
    public void SlidingAverage_WindowShrinksAtEnd()
    {
        // 10 значений: все по 10.0. Окно=10.
        // Последний элемент: окно из 1 значения = 10.0
        // Предпоследний: окно из 2 = 10.0
        var data = MakeConstant(10, leftRut: 10.0, rightRut: 5.0);

        var (left, right) = _analyzer.ComputeSlidingAverages(data, 10);

        Assert.Equal(10.0, left[9], precision: 10);
        Assert.Equal(10.0, left[8], precision: 10);
        Assert.Equal(5.0, right[9], precision: 10);
    }

    [Fact]
    public void SlidingAverage_InputShorterThanWindow()
    {
        var data = MakeConstant(3, leftRut: 6.0, rightRut: 4.0);

        var (left, right) = _analyzer.ComputeSlidingAverages(data, 10);

        Assert.Equal(3, left.Length);
        // Первый элемент: среднее 3 значений = 6.0
        Assert.Equal(6.0, left[0], precision: 10);
        // Последний: 1 значение = 6.0
        Assert.Equal(6.0, left[2], precision: 10);
    }

    [Fact]
    public void SlidingAverage_VerifyMiddleValues()
    {
        // 20 значений: 1..20. Окно=10.
        // Индекс 5: среднее(6,7,8,9,10,11,12,13,14,15) = 10.5
        var values = Enumerable.Range(1, 20).Select(v => (double)v).ToArray();
        var data = MakeFromLeftValues(values);

        var (left, _) = _analyzer.ComputeSlidingAverages(data, 10);

        Assert.Equal(10.5, left[5], precision: 10);
    }

    // =============================================
    // Шаг 2. Выявление неудовлетворительных метров
    // =============================================

    [Fact]
    public void MarkUnsatisfactory_LeftExceedsThreshold()
    {
        double[] avgL = [21.0, 19.0, 20.0];
        double[] avgR = [10.0, 10.0, 10.0];

        var result = _analyzer.MarkUnsatisfactory(avgL, avgR, 20.0);

        Assert.True(result[0]);   // 21 >= 20
        Assert.False(result[1]);  // 19 < 20
        Assert.True(result[2]);   // 20 >= 20 (граничное значение)
    }

    [Fact]
    public void MarkUnsatisfactory_RightExceedsThreshold()
    {
        double[] avgL = [10.0, 10.0];
        double[] avgR = [25.0, 19.9];

        var result = _analyzer.MarkUnsatisfactory(avgL, avgR, 20.0);

        Assert.True(result[0]);
        Assert.False(result[1]);
    }

    [Fact]
    public void MarkUnsatisfactory_BothExceed()
    {
        double[] avgL = [22.0];
        double[] avgR = [21.0];

        var result = _analyzer.MarkUnsatisfactory(avgL, avgR, 20.0);

        Assert.True(result[0]);
    }

    [Fact]
    public void MarkUnsatisfactory_NeitherExceeds()
    {
        double[] avgL = [19.0];
        double[] avgR = [18.0];

        var result = _analyzer.MarkUnsatisfactory(avgL, avgR, 20.0);

        Assert.False(result[0]);
    }

    [Fact]
    public void MarkUnsatisfactory_ExactlyAtThreshold()
    {
        double[] avgL = [20.0];
        double[] avgR = [19.0];

        var result = _analyzer.MarkUnsatisfactory(avgL, avgR, 20.0);

        Assert.True(result[0]); // >= 20.0
    }

    // =============================================
    // Шаги 3-4. Группировка в карты
    // =============================================

    [Fact]
    public void GroupIntoCards_SingleBlock()
    {
        // 5 подряд неудовлетворительных
        bool[] unsat = [true, true, true, true, true];

        var groups = _analyzer.GroupIntoCards(unsat, mergeDistance: 100);

        Assert.Single(groups);
        Assert.Equal((0, 4), groups[0]);
    }

    [Fact]
    public void GroupIntoCards_TwoGroups_LargeGap()
    {
        // Две плохие точки с разрывом 100 (>= mergeDistance=100 → НЕ объединяем)
        var unsat = new bool[200];
        unsat[0] = true;
        unsat[100] = true;

        var groups = _analyzer.GroupIntoCards(unsat, mergeDistance: 100);

        Assert.Equal(2, groups.Count);
        Assert.Equal((0, 0), groups[0]);
        Assert.Equal((100, 100), groups[1]);
    }

    [Fact]
    public void GroupIntoCards_TwoGroups_Merged()
    {
        // Две плохие точки с разрывом 99 (< 100 → объединяем)
        var unsat = new bool[200];
        unsat[0] = true;
        unsat[99] = true;

        var groups = _analyzer.GroupIntoCards(unsat, mergeDistance: 100);

        Assert.Single(groups);
        Assert.Equal((0, 99), groups[0]);
    }

    [Fact]
    public void GroupIntoCards_NoUnsatisfactory()
    {
        bool[] unsat = [false, false, false];

        var groups = _analyzer.GroupIntoCards(unsat, mergeDistance: 100);

        Assert.Empty(groups);
    }

    [Fact]
    public void GroupIntoCards_MultipleSmallGaps_AllMerged()
    {
        // Имитация: плохие точки в позициях 0, 22, 26, 30 (все разрывы < 100)
        var unsat = new bool[50];
        unsat[0] = true;
        unsat[22] = true;
        unsat[26] = true;
        unsat[30] = true;

        var groups = _analyzer.GroupIntoCards(unsat, mergeDistance: 100);

        Assert.Single(groups);
        Assert.Equal((0, 30), groups[0]);
    }

    [Fact]
    public void GroupIntoCards_ThreeGroups()
    {
        // Три группы с разрывами >= 100
        var unsat = new bool[500];
        // Группа 1: 0-5
        for (int i = 0; i <= 5; i++) unsat[i] = true;
        // Разрыв 200 (>= 100)
        // Группа 2: 206-210
        for (int i = 206; i <= 210; i++) unsat[i] = true;
        // Разрыв 150 (>= 100)
        // Группа 3: 361
        unsat[361] = true;

        var groups = _analyzer.GroupIntoCards(unsat, mergeDistance: 100);

        Assert.Equal(3, groups.Count);
        Assert.Equal((0, 5), groups[0]);
        Assert.Equal((206, 210), groups[1]);
        Assert.Equal((361, 361), groups[2]);
    }

    // =============================================
    // Шаг 5. Удлинение короткой карты
    // =============================================

    [Fact]
    public void ExtendCard_AlreadyLongEnough()
    {
        var data = MakeConstant(200, 15.0, 10.0);

        var (start, end) = _analyzer.ExtendCard(data, coverageStart: 10, coverageEnd: 120, minLength: 100);

        // 111 метров >= 100, не нужно менять
        // Но ExtendCard всё равно ищет лучшее окно. Проверяем что длина = minLength
        Assert.Equal(100, end - start + 1);
    }

    [Fact]
    public void ExtendCard_ShortCard_FindsBestWindow()
    {
        // 200 метров: везде колея = 10, кроме участка 80-119 где левая = 25
        var left = new double[200];
        var right = new double[200];
        for (int i = 0; i < 200; i++)
        {
            left[i] = 10.0;
            right[i] = 5.0;
        }
        for (int i = 80; i < 120; i++)
            left[i] = 25.0;

        var data = MakeFromValues(left, right);

        // Короткая карта покрывает индексы 90-100 (11 метров)
        var (start, end) = _analyzer.ExtendCard(data, coverageStart: 90, coverageEnd: 100, minLength: 100);

        Assert.Equal(100, end - start + 1);
        // Лучшее окно должно максимально захватить участок 80-119
        // Оптимально: начало около 60-80, чтобы включить максимум «плохих» метров
        Assert.True(start <= 80, $"Start {start} should be <= 80 to capture the deep rut area");
        Assert.True(end >= 119, $"End {end} should be >= 119 to capture the deep rut area");
    }

    [Fact]
    public void ExtendCard_UsesMaxOfTwoRuts()
    {
        // Два «пика»: один с глубокой левой, другой с глубокой правой.
        // Правый пик глубже → окно должно выбрать его.
        var left = new double[300];
        var right = new double[300];
        for (int i = 0; i < 300; i++)
        {
            left[i] = 5.0;
            right[i] = 5.0;
        }
        // Пик 1: левая = 18 в районе 50-60
        for (int i = 50; i <= 60; i++) left[i] = 18.0;
        // Пик 2: правая = 22 в районе 180-190
        for (int i = 180; i <= 190; i++) right[i] = 22.0;

        var data = MakeFromValues(left, right);

        // Короткая карта где-то посередине
        var (start, end) = _analyzer.ExtendCard(data, coverageStart: 120, coverageEnd: 130, minLength: 100);

        // Окно 100м должно покрывать пик правой колеи (180-190),
        // потому что MAX(avg_L, avg_R) там выше, чем у левого пика (50-60).
        Assert.True(start <= 180 && end >= 190,
            $"Window [{start},{end}] should cover the right rut peak at 180-190");
    }

    [Fact]
    public void ExtendCard_DoesNotGoOutOfBounds()
    {
        var data = MakeConstant(50, 15.0, 10.0);

        // Карта в начале данных, deficit большой
        var (start, end) = _analyzer.ExtendCard(data, coverageStart: 0, coverageEnd: 5, minLength: 40);

        Assert.True(start >= 0);
        Assert.True(end < data.Count);
        Assert.Equal(40, end - start + 1);
    }

    // =============================================
    // Шаг 6. Уточнение адресов
    // =============================================

    [Fact]
    public void RefineAddresses_TrimsBothEnds()
    {
        // 20 метров. Фактические значения > 20 только в индексах 5-14.
        var left = new double[20];
        for (int i = 0; i < 20; i++) left[i] = 10.0;
        for (int i = 5; i <= 14; i++) left[i] = 22.0;

        var data = MakeFromLeftValues(left);

        var (start, end) = _analyzer.RefineAddresses(data, 0, 19, threshold: 20.0);

        Assert.Equal(5, start);
        Assert.Equal(14, end);
    }

    [Fact]
    public void RefineAddresses_RightRutCounts()
    {
        // Левая везде < 20, правая > 20 в индексах 3-7
        var left = new double[15];
        var right = new double[15];
        for (int i = 0; i < 15; i++) { left[i] = 10.0; right[i] = 10.0; }
        for (int i = 3; i <= 7; i++) right[i] = 21.0;

        var data = MakeFromValues(left, right);

        var (start, end) = _analyzer.RefineAddresses(data, 0, 14, threshold: 20.0);

        Assert.Equal(3, start);
        Assert.Equal(7, end);
    }

    [Fact]
    public void RefineAddresses_AllAboveThreshold_NoChange()
    {
        var data = MakeConstant(10, leftRut: 25.0, rightRut: 25.0);

        var (start, end) = _analyzer.RefineAddresses(data, 0, 9, threshold: 20.0);

        Assert.Equal(0, start);
        Assert.Equal(9, end);
    }

    // =============================================
    // Шаг 7. Итоговая статистика (BuildCard через Analyze)
    // =============================================

    [Fact]
    public void Analyze_ComputesCorrectStats()
    {
        // Все значения выше порога, чтобы получить одну карту без удлинения.
        // 120 метров (> minCardLength=100), все одинаковые кроме нескольких.
        var left = new double[120];
        var right = new double[120];
        for (int i = 0; i < 120; i++) { left[i] = 22.0; right[i] = 21.0; }
        left[50] = 30.0;  // максимум левой
        right[60] = 28.0; // максимум правой

        var data = MakeFromValues(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = _analyzer.Analyze(data, p);

        Assert.Single(cards);
        var card = cards[0];
        Assert.Equal(30.0, card.MaxLeftRut);
        Assert.Equal(28.0, card.MaxRightRut);
        Assert.True(card.AvgLeftRut > 22.0); // чуть выше 22 из-за 30
        Assert.False(card.WasExtended);
    }

    // =============================================
    // Сквозные тесты
    // =============================================

    [Fact]
    public void Analyze_NoUnsatisfactory_EmptyResult()
    {
        var data = MakeConstant(500, leftRut: 10.0, rightRut: 8.0);
        var p = new AnalysisParameters();

        var cards = _analyzer.Analyze(data, p);

        Assert.Empty(cards);
    }

    [Fact]
    public void Analyze_EmptyInput_EmptyResult()
    {
        var cards = _analyzer.Analyze([], new AnalysisParameters());

        Assert.Empty(cards);
    }

    [Fact]
    public void Analyze_ExtendedCardNotRefined()
    {
        // Короткий плохой участок (15 м при minCard=100) среди хороших данных.
        var left = new double[500];
        var right = new double[500];
        for (int i = 0; i < 500; i++) { left[i] = 10.0; right[i] = 8.0; }
        // Плохой участок: индексы 200-214 (15 метров)
        for (int i = 200; i <= 214; i++) left[i] = 25.0;

        var data = MakeFromValues(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = _analyzer.Analyze(data, p);

        Assert.Single(cards);
        var card = cards[0];
        Assert.True(card.WasExtended);
        Assert.Equal(100, card.LengthMeters);
        // Удлинённая карта НЕ уточняется — включает метры где колея < 20
    }

    [Fact]
    public void Analyze_TwoGroupsSeparated_TwoCards()
    {
        // Два плохих участка с разрывом >= mergeDistance
        var left = new double[500];
        var right = new double[500];
        for (int i = 0; i < 500; i++) { left[i] = 10.0; right[i] = 8.0; }
        // Участок 1: индексы 50-180 (131 метр, > min 100)
        for (int i = 50; i <= 180; i++) left[i] = 22.0;
        // Разрыв: 181-380 (200 метров >= 100)
        // Участок 2: индексы 381-490 (110 метров, > min 100)
        for (int i = 381; i <= 490; i++) left[i] = 23.0;

        var data = MakeFromValues(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = _analyzer.Analyze(data, p);

        Assert.Equal(2, cards.Count);
        Assert.False(cards[0].WasExtended);
        Assert.False(cards[1].WasExtended);
    }

    [Fact]
    public void Analyze_TwoGroupsMerged_OneCard()
    {
        // Два плохих участка с разрывом < mergeDistance
        var left = new double[300];
        var right = new double[300];
        for (int i = 0; i < 300; i++) { left[i] = 10.0; right[i] = 8.0; }
        // Участок 1: 10-40
        for (int i = 10; i <= 40; i++) left[i] = 22.0;
        // Разрыв: 50 метров (< 100)
        // Участок 2: 90-200
        for (int i = 90; i <= 200; i++) left[i] = 22.0;

        var data = MakeFromValues(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = _analyzer.Analyze(data, p);

        Assert.Single(cards);
    }

    [Fact]
    public void Analyze_NonStandardKmLength()
    {
        // Километровый столб с 800 метрами (не 1000!)
        // Проверяем что программа не ломается и не считает абсолютные координаты.
        var data = new List<RutMeasurement>();
        // Км 3: метры 0-799 (800 м)
        for (int m = 0; m < 800; m++)
            data.Add(new RutMeasurement(3, m, 10.0, 8.0));
        // Км 4: метры 0-799 (800 м)
        for (int m = 0; m < 800; m++)
            data.Add(new RutMeasurement(4, m, 10.0, 8.0));

        // Плохой участок на стыке километров: последние 50 метров км 3 + первые 80 метров км 4
        for (int i = 750; i < 800; i++) data[i] = data[i] with { LeftRut = 25.0 };
        for (int i = 800; i < 880; i++) data[i] = data[i] with { LeftRut = 25.0 };

        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 100, MergeDistance: 100);

        var cards = _analyzer.Analyze(data, p);

        Assert.Single(cards);
        // Карта начинается в км 3 и заканчивается в км 4
        Assert.Equal(3, cards[0].Start.Km);
        Assert.Equal(4, cards[0].End.Km);
    }

    [Fact]
    public void Analyze_MergeDistanceBoundary_ExactlyEqual_NotMerged()
    {
        // Разрыв ровно = mergeDistance → НЕ объединяем (условие: < mergeDistance)
        var left = new double[300];
        var right = new double[300];
        for (int i = 0; i < 300; i++) { left[i] = 10.0; right[i] = 8.0; }
        // Участок 1: 0-9
        for (int i = 0; i <= 9; i++) left[i] = 22.0;
        // Разрыв ровно 100
        // Участок 2: 110-119
        for (int i = 110; i <= 119; i++) left[i] = 22.0;

        var data = MakeFromValues(left, right);
        var p = new AnalysisParameters(Threshold: 20.0, WindowSize: 10, MinCardLength: 10, MergeDistance: 100);

        var cards = _analyzer.Analyze(data, p);

        Assert.Equal(2, cards.Count);
    }
}
