using Dil.Core.Models;

namespace Dil.Core;

public class RutAnalyzer
{
    /// <summary>
    /// Полный расчёт: от исходных измерений до списка карт ремонта.
    /// </summary>
    public List<RepairCard> Analyze(List<RutMeasurement> measurements, AnalysisParameters p)
    {
        if (measurements.Count == 0)
            return [];

        var (avgLeft, avgRight) = ComputeSlidingAverages(measurements, p.WindowSize);
        var unsatisfactory = MarkUnsatisfactory(avgLeft, avgRight, p.Threshold);
        var groups = GroupIntoCards(unsatisfactory, p.MergeDistance);

        var cards = new List<RepairCard>();
        foreach (var (start, end) in groups)
        {
            int coverageStart = start;
            int coverageEnd = Math.Min(end + p.WindowSize - 1, measurements.Count - 1);
            int coverageLength = coverageEnd - coverageStart + 1;

            if (coverageLength < p.MinCardLength)
            {
                var (extStart, extEnd) = ExtendCard(
                    measurements, coverageStart, coverageEnd, p.MinCardLength);
                cards.Add(BuildCard(measurements, extStart, extEnd, wasExtended: true));
            }
            else
            {
                var (refStart, refEnd) = RefineAddresses(
                    measurements, coverageStart, coverageEnd, p.Threshold);
                cards.Add(BuildCard(measurements, refStart, refEnd, wasExtended: false));
            }
        }

        return cards;
    }

    /// <summary>
    /// Шаг 1. Скользящее среднее с окном windowSize вперёд.
    /// На конце данных окно сужается до оставшихся значений.
    /// </summary>
    public (double[] Left, double[] Right) ComputeSlidingAverages(
        List<RutMeasurement> measurements, int windowSize)
    {
        int n = measurements.Count;
        var left = new double[n];
        var right = new double[n];

        for (int i = 0; i < n; i++)
        {
            int end = Math.Min(i + windowSize, n);
            double sumL = 0, sumR = 0;
            for (int j = i; j < end; j++)
            {
                sumL += measurements[j].LeftRut;
                sumR += measurements[j].RightRut;
            }
            int count = end - i;
            left[i] = sumL / count;
            right[i] = sumR / count;
        }

        return (left, right);
    }

    /// <summary>
    /// Шаг 2. Метр неудовлетворителен, если хотя бы одна колея >= порога.
    /// </summary>
    public bool[] MarkUnsatisfactory(double[] avgLeft, double[] avgRight, double threshold)
    {
        var result = new bool[avgLeft.Length];
        for (int i = 0; i < avgLeft.Length; i++)
            result[i] = avgLeft[i] >= threshold || avgRight[i] >= threshold;
        return result;
    }

    /// <summary>
    /// Шаги 3-4. Группировка неудовлетворительных метров в карты.
    /// Если разрыв между соседними неудовлетворительными метрами &lt; mergeDistance — объединяем.
    /// Возвращает пары (startIndex, endIndex) в массиве measurements —
    /// индексы первой и последней неудовлетворительной десятиметровки в группе.
    /// </summary>
    public List<(int Start, int End)> GroupIntoCards(bool[] unsatisfactory, int mergeDistance)
    {
        var badIndices = new List<int>();
        for (int i = 0; i < unsatisfactory.Length; i++)
        {
            if (unsatisfactory[i])
                badIndices.Add(i);
        }

        if (badIndices.Count == 0)
            return [];

        var groups = new List<(int Start, int End)>();
        int groupStart = badIndices[0];
        int groupEnd = badIndices[0];

        for (int i = 1; i < badIndices.Count; i++)
        {
            int gap = badIndices[i] - badIndices[i - 1];
            if (gap < mergeDistance)
            {
                groupEnd = badIndices[i];
            }
            else
            {
                groups.Add((groupStart, groupEnd));
                groupStart = badIndices[i];
                groupEnd = badIndices[i];
            }
        }

        groups.Add((groupStart, groupEnd));
        return groups;
    }

    /// <summary>
    /// Шаг 5. Удлинение короткой карты.
    /// Расширяем область поиска, ищем окно minLength с наибольшим MAX(avg_left, avg_right).
    /// </summary>
    public (int Start, int End) ExtendCard(
        List<RutMeasurement> measurements, int coverageStart, int coverageEnd, int minLength)
    {
        int coverageLength = coverageEnd - coverageStart + 1;
        int deficit = minLength - coverageLength;

        int searchStart = Math.Max(0, coverageStart - deficit);
        int searchEnd = Math.Min(measurements.Count - 1, coverageEnd + deficit);

        int bestPos = searchStart;
        double bestValue = double.MinValue;

        int windowValues = minLength;

        for (int pos = searchStart; pos + windowValues - 1 <= searchEnd; pos++)
        {
            double sumL = 0, sumR = 0;
            for (int j = pos; j < pos + windowValues; j++)
            {
                sumL += measurements[j].LeftRut;
                sumR += measurements[j].RightRut;
            }
            double avgL = sumL / windowValues;
            double avgR = sumR / windowValues;
            double worstRut = Math.Max(avgL, avgR);

            if (worstRut > bestValue)
            {
                bestValue = worstRut;
                bestPos = pos;
            }
        }

        return (bestPos, bestPos + windowValues - 1);
    }

    /// <summary>
    /// Шаг 6. Уточнение адресов по фактическим метровым значениям.
    /// Внутри диапазона coverageStart..coverageEnd находим первый и последний метр,
    /// где фактическое значение хотя бы одной колеи >= порога.
    /// </summary>
    public (int Start, int End) RefineAddresses(
        List<RutMeasurement> measurements, int coverageStart, int coverageEnd, double threshold)
    {
        int first = coverageStart;
        int last = coverageEnd;

        for (int i = coverageStart; i <= coverageEnd; i++)
        {
            if (measurements[i].LeftRut >= threshold || measurements[i].RightRut >= threshold)
            {
                first = i;
                break;
            }
        }

        for (int i = coverageEnd; i >= coverageStart; i--)
        {
            if (measurements[i].LeftRut >= threshold || measurements[i].RightRut >= threshold)
            {
                last = i;
                break;
            }
        }

        return (first, last);
    }

    private static RepairCard BuildCard(
        List<RutMeasurement> measurements, int start, int end, bool wasExtended)
    {
        double sumL = 0, sumR = 0;
        double maxL = double.MinValue, maxR = double.MinValue;

        for (int i = start; i <= end; i++)
        {
            sumL += measurements[i].LeftRut;
            sumR += measurements[i].RightRut;
            maxL = Math.Max(maxL, measurements[i].LeftRut);
            maxR = Math.Max(maxR, measurements[i].RightRut);
        }

        int count = end - start + 1;

        return new RepairCard
        {
            Start = new Distance(measurements[start].Km, measurements[start].Meter),
            End = new Distance(measurements[end].Km, measurements[end].Meter),
            StartIndex = start,
            EndIndex = end,
            WasExtended = wasExtended,
            AvgLeftRut = sumL / count,
            AvgRightRut = sumR / count,
            MaxLeftRut = maxL,
            MaxRightRut = maxR,
        };
    }
}
