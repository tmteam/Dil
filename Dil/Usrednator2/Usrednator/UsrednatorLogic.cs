using System.Text;
using Dil.Core.Entities;

namespace Usrednator;

public static class UsrednatorLogic
{
    public static void SetZeroPivot(LinkedList<NumberDataItem> entries, int averageMetters)
    {
        /*
         * При появлении нового километра в этой же строке должен прописаться 0 метров
         * и суммирование должно идти от нуля до, например, 10 метров.
         *
         *  В том случае, когда в исходном файле ноль не прописан (такое бывает нечасто, но бывает),
         *  а напротив введенного километра стоит 1 или 2 метра нулю нужно присвоить значение
         *  предыдущей строки (примыкающей к столбу).
         *
         *  Если длина участка суммирования перед новым столбом будет меньше,
         *  двух третей установленной оператором длины участка суммирования,
         *  этот кусок приобщается к предыдущему участку, который усредняется с большим количеством метров,
         *  чем установлено в программе обработки.
         *
         *  Если участок суммирования, примыкающий к километровому столбу, будет больше 2/3,
         *  то он усредняется по фактической длине.
         *
         *  В случае пропуска километрового столба, или двух, или трёх алгоритм сохраняется,
         *  при появлении столба в метрах снова должны пройти нули.
         */

        var first = entries.First();
        if (first.Distance.M != 0 && entries.Count>1)
        {
            /*
             * В том случае, когда в исходном файле ноль не прописан(такое бывает нечасто, но бывает),
             * а напротив введенного километра стоит 1 или 2 метра нулю нужно присвоить значение
             * предыдущей строки(примыкающей к столбу).
             */
            var second = entries.ElementAt(1);
            if (second.Distance.Km == first.Distance.Km && second.Distance.M <= averageMetters * 0.25) 
            {
                var zeroEntry = new NumberDataItem(new Distance(first.Distance.Km,0), second.Data );
                entries.AddFirst(zeroEntry);
            }
        }

    }
    //Этот фильтр использовался раньше. Когда усреднятор был усреднятором а не аппроксиматором
    public static LinkedList<NumberDataItem> AverageFilterOld(
        IEnumerable<NumberDataItem> origin, int averageMetters, bool useZeroPivots)
    {
        var answer = new LinkedList<NumberDataItem>();
        if (!origin.Any())
            return answer;
            
        var startOfInterval = origin.First().Distance;
        var endOfInterval = startOfInterval.AppendMeters(averageMetters);
            
        var buffer = new LinkedList<NumberDataItem>();
        var previousBuffer = new LinkedList<NumberDataItem>();

        int bufferStartIndex = 0;
        int currentKm = startOfInterval.Km;
        int lastMeterInterval = averageMetters;
        int i = -1;

        var previousDistance = new Distance();
        var prePreviousDistance = new Distance();
            
        foreach (var current in origin)
        {
            i++;
            if (current.Distance.Km > currentKm)
            {
                #region Обработка перехода на другой киллометр

                currentKm = current.Distance.Km;

                if (i > 1)
                {
                    //Если две последних записи в одном километре - то можем скорректироваться по ним
                    if (prePreviousDistance.Km == previousDistance.Km)
                    {
                        lastMeterInterval = previousDistance.M - prePreviousDistance.M;
                    }

                    //Если нужна привязка к нулевому метру киллометра
                    if (useZeroPivots)
                    {
                        endOfInterval = new Distance(current.Distance.Km, 0);
                            
                        /*
                         *  Если длина участка суммирования перед новым столбом < 0.66*averageMeters,
                         *  то этот кусок приобщается к предыдущему участку,
                         *  который усредняется с большим количеством метров,
                         *  чем установлено в программе обработки.
                         *
                         * Если участок суммирования, примыкающий к километровому столбу, будет больше 2 / 3 { averageMeters},
                         * то он усредняется по фактической длине.
                         */
                        if (lastMeterInterval < averageMetters * 0.66 && answer.Any())
                        {
                            //Сливаем два буфера
                            foreach (var entry in previousBuffer)
                                buffer.AddFirst(entry);
                            previousBuffer.Clear();

                            //Отменяем последнее значение. Его нужно посчитать заново
                            startOfInterval = answer.Last().Distance;
                            answer.RemoveLast();
                        }

                    }
                    else
                    {
                        //Предполагаемая длина километра
                        var lastMeterOfKm = previousDistance.M + lastMeterInterval;
                        //Корректируем концовку интервала
                        endOfInterval = endOfInterval.ConvertToNextKm(current.Distance.Km, lastMeterOfKm);
                    }
                }
                #endregion
            }
            
            if (current.Distance >= endOfInterval)
            {
                #region Обработка полученного интервала
                if (buffer.Any())
                {
                    var resultData = new double[buffer.First().Data.Length];
                    int j = -1;
                    foreach (var entry in buffer)
                    {
                        j++;
                        resultData.ArrayInplaceSummOrThrow(entry.Data, bufferStartIndex + j);
                    }

                    answer.AddLast(new NumberDataItem(startOfInterval, resultData.Select(r => r / buffer.Count).ToArray()));
                    previousBuffer = buffer;
                    buffer = new LinkedList<NumberDataItem>();
                }
                bufferStartIndex = i;
                startOfInterval = endOfInterval;
                endOfInterval = endOfInterval.AppendMeters(averageMetters);
                #endregion
            }

            if (current.Distance < endOfInterval)
                buffer.AddLast(current);

            prePreviousDistance = previousDistance;
            previousDistance = current.Distance;

        }
        return answer;
    }

    private static LinkedList<NumberDataItem> Deduplication(IEnumerable<NumberDataItem> origin)
    {
        var result = new LinkedList<NumberDataItem>();
        var current = origin.First();
        var acc = current.Data;
        var countOfSame = 1;
        //Написано ужасно но зато работает
        foreach (var value in origin.Skip(1))
        {
            if (Equals(value.Distance, current.Distance))
            {
                acc.ArrayInplaceSummOrThrow(value.Data);
                countOfSame++;
            }
            else
            {
                result.AddLast(new NumberDataItem(current.Distance, acc.ArrayDivide(countOfSame)));
                countOfSame = 1;
                acc = value.Data;
                current = value;
               
            }
            
            if (value == origin.Last()) 
                result.AddLast(new NumberDataItem(current.Distance, acc.ArrayDivide(countOfSame)));
        }

        return result;
    }

    private static LinkedList<NumberDataItem> RemoveKilometerTails(IEnumerable<NumberDataItem> origin)
    {
        var result = new LinkedList<NumberDataItem>();
        var array = origin.ToArray();
        var i = 0;
        while (i < array.Length)
        {
            var current = array[i];
            //Специальный случай когда участок начинается с большого числа
            if (current.Distance.M >= 1000)
            {
                i++;
                continue;
            }
            var count = 1;
            var acc = array[i].Data;

            if (i < array.Length -1)
            {
                var next = array[i + 1];
                while (next.Distance.M >= 1000)
                {
                    acc.ArrayInplaceSummOrThrow(next.Data);
                    count++;
                    if (i + count >= array.Length)
                        break;
                    next = array[i + count];
                }
            }
            result.AddLast(new NumberDataItem(current.Distance, acc.ArrayDivide(count)));
            i = i + count ;
        }

        return result;
    }

    
    public static LinkedList<NumberDataItem> ApproximationFilter(
        IEnumerable<NumberDataItem> origin, int averageMetters, bool useZeroPivots)
    {
        var answer = new LinkedList<NumberDataItem>();
        if (!origin.Any())
            return answer;
        var deduplicated = Deduplication(origin);
        if (useZeroPivots)
            deduplicated = RemoveKilometerTails(deduplicated);
        
        //В начале интерполируем все по одному метру
        var interpolatedByMeter = OneMeterInterpolation(deduplicated);
        //Теперь считаем среднее по отрезку
        if (averageMetters <= 1)
            return interpolatedByMeter;
        var interpolatedAsArray = interpolatedByMeter.ToArray();
        var current = interpolatedByMeter.First;
        while (current != null)
        {
            var startPoint = current.Value.Distance;
            var nextPoint = current.Value.Distance.AppendMeters(averageMetters);
            if (nextPoint.M > 1000)
            {
                if (useZeroPivots)
                {
                    nextPoint = new Distance(nextPoint.Km + 1, 0);
                }
                else
                {
                    // Корректируем киллометраж на границе километров
                    var lastMInKm = interpolatedAsArray
                        .Where(a => a.Distance.Km == current.Value.Distance.Km)
                        .Max(x => x.Distance.M);
                    if (lastMInKm < 1000)
                        nextPoint = nextPoint.ConvertToNextKm(nextPoint.Km + 1, 1000);
                }
            }

            var acc = new double[current.Value.Data.Length];
            var itemCount = 0;
            while (current != null && current.Value.Distance < nextPoint)
            {
                acc.ArrayInplaceSummOrThrow(current.Value.Data);
                itemCount++;
                current = current.Next;
            }

            if (itemCount != 0)
            {
                var averageData = acc.ArrayDivide(itemCount);
                answer.AddLast(new NumberDataItem(startPoint, averageData.ToArray()));
            }
        }

        return answer;
    }

    private static LinkedList<NumberDataItem> OneMeterInterpolation(IEnumerable<NumberDataItem> origin)
    {
        var answer = new LinkedList<NumberDataItem>();
        if (!origin.Any())
            return answer;
        
        var startOfCurrentInterval = origin.First().Distance;
        var currentCalculatedDistance = startOfCurrentInterval;
            
        var buffer = new LinkedList<NumberDataItem>();
        Distance? prepreviousDistance = null; 
        var previousItem = origin.First();
        foreach (var currentOrigin in origin.Skip(1))
        {
            //Нужно заполнить [current.Distance.. endOfCurrentInterval] интерполированными значениями
            while (currentOrigin.Distance >= currentCalculatedDistance)
            {
                var metersInLastKilometer = 1000;

                if (currentOrigin.Distance.Km > currentCalculatedDistance.Km && currentCalculatedDistance.M >= 1000)
                {
                    //Следующая рассчитанная точка вылезла за 1000 м. Это ожидаемо, ведь мы тупо прибавили интервал к ней
                    // В километре может быть больше 1000 метров
                    if (previousItem.Distance.M > 1000)
                    {
                        if (prepreviousDistance.HasValue && prepreviousDistance.Value.Km == previousItem.Distance.Km)
                            //Предполагаемая длина километра. Возьмем разницу между последними отсчетами
                            metersInLastKilometer = previousItem.Distance.M +
                                                    (previousItem.Distance.M - prepreviousDistance.Value.M);
                        else
                            metersInLastKilometer = previousItem.Distance.M;
                    }
                   
                    currentCalculatedDistance =
                        currentCalculatedDistance.ConvertToNextKm(currentOrigin.Distance.Km,
                            metersInLastKilometer);

                    if (currentOrigin.Distance < currentCalculatedDistance)
                        break;
                }

                var interpolated = 
                    Helper.Interpolate(previousItem, currentOrigin, currentCalculatedDistance, metersInLastKilometer);
                answer.AddLast(interpolated);
                currentCalculatedDistance = currentCalculatedDistance.AppendMeters(1);
                buffer = new LinkedList<NumberDataItem>();
            }
            
            if (currentOrigin.Distance < currentCalculatedDistance)
                buffer.AddLast(currentOrigin);
            prepreviousDistance = previousItem.Distance;
            previousItem = currentOrigin;
        }
        
        return answer;
    }
    
    public static string ConvertTableToFormatedText(IEnumerable<NumberDataItem> data)
    {
        var filtredText = new StringBuilder();
        foreach (var entry in data)
        {
            filtredText.AppendLine(
                $"{entry.Distance.Km}\t{entry.Distance.M}\t{string.Join("\t", entry.Data.Select(d => d.ToString("F1")))}");
        }

        return filtredText.ToString();
    }
    
    public static string ToFormattedString(this IEnumerable<NumberDataItem> entries, string separator = "\t")
    {
        var filtredText = new StringBuilder();
        foreach (var entry in entries)
        {
            filtredText.AppendLine($"{entry.Distance.Km}{separator}" +
                                   $"{entry.Distance.M}{separator}" +
                                   $"{string.Join(separator, entry.Data.Select(d => d.ToString("F2")))}");
        }

        return filtredText.ToString();
    }

}