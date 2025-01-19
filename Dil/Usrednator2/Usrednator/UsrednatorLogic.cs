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
        IEnumerable<NumberDataItem> origin,
        int averageMetters,
        bool useZeroPivots,
        bool useNextIntervalStart = false)
    {
        var answer = new LinkedList<NumberDataItem>();
        if (!origin.Any())
            return answer;
        var deduplicated = Deduplication(origin);
        //В начале интерполируем все по одному метру
        var interpolatedByMeter = OneMeterInterpolation(deduplicated.ToArray());
        //высчитываем дистанции 
        var distances = CalculateIntervals(origin.Select(o => o.Distance).ToArray(), averageMetters, useZeroPivots);
        var lastDistance = interpolatedByMeter.Last();
        if (distances.Last() <= lastDistance.Distance)
        {
            //Идея в том, что если последняя дистанция - последняя, то нужно использовать ее значение как финальное
            distances.Add(lastDistance.Distance);
        }
        var currentValue = interpolatedByMeter.First;
        for (int i = 1; i < distances.Count; i++)
        {
            var start = distances[i - 1];
            var end = distances[i];
            var startLeaf = currentValue.ThisOrFirstOrDefault((x) => x.Value.Distance.Equals(start));
            if(startLeaf==null)
                break;
            var leaf = startLeaf.Next;
            var acc = startLeaf.Value.Data;
            var count = 1;
            while (leaf!=null )
            {
                if(useNextIntervalStart || i == distances.Count -1)
                {
                    if (leaf.Value.Distance.Km>=end.Km && leaf.Value.Distance>end) 
                        break;
                }
                else
                {
                    if (leaf.Value.Distance.Km>=end.Km && leaf.Value.Distance>=end) 
                        break;
                }
                acc.ArrayInplaceSummOrThrow(leaf.Value.Data);
                count++;

                leaf = leaf.Next;
            }
            
            Console.WriteLine($"Handled: {start}-{end} for {count} items");
            answer.AddLast(new NumberDataItem(start, acc.ArrayDivide(count)));
        }

        return answer;
    }

    private static LinkedListNode<T> ThisOrFirstOrDefault<T>(this LinkedListNode<T> leaf,
        Func<LinkedListNode<T>, bool> predicate)
    {
        
        while (leaf!=null)
        {
            if (predicate(leaf))
                return leaf;
            leaf = leaf.Next;
        }

        return null;
    }

    /// <summary>
    /// Вычисляет отрезки для усреднятора
    /// </summary>
    public static List<Distance> CalculateIntervals(Distance[] distances, int step, bool useZeoPivots)
    {
        var answer = new List<Distance>();
        var current = distances.FirstOrDefault();
        var lastKm = distances.Max(d => d.Km);
        var currentKm = current.Km;
        var startMeter = current.M;
        for (; currentKm <= lastKm; currentKm++)
        {
            var lastMeters = distances
                .Where(d => d.Km == currentKm)
                .DistinctBy(d=>d.M)
                .TakeLast(2);
            
            var originLastMInCurrentKm = lastMeters.LastOrDefault().M;
            var preLastM = lastMeters.FirstOrDefault().M;
            if (lastMeters.Count() < 2)
                preLastM = 0;
            var calculatedLastMInCurrentKm = originLastMInCurrentKm;
            
            if (originLastMInCurrentKm <= 1000 && currentKm<lastKm)
            {
               //Если длина участка суммирования перед новым столбом < 0.66*averageMeters,
               if (999 - calculatedLastMInCurrentKm < 0.66 * step 
                   // Если длинна участка с предыдущим
                   || 999 - calculatedLastMInCurrentKm<= originLastMInCurrentKm - preLastM
                   // или все по нулям 
                   || ( currentKm<lastKm && calculatedLastMInCurrentKm ==0 && preLastM ==0) 
                   )
                   calculatedLastMInCurrentKm = 999;
            }

            var currentM = startMeter;
            if (useZeoPivots)
            {
                currentM = (Enumerable.Range(0, 1000).FirstOrDefault(x => x >= startMeter));
            }

            //Заполняем интервалами ответ до последнего метра
            do
            {
                var newDistance = new Distance(currentKm, currentM);
                answer.Add(newDistance);
                currentM += step;
            } while (currentM <= calculatedLastMInCurrentKm);
            
            if (useZeoPivots)
            {
                startMeter = 0;
            }
            else if (originLastMInCurrentKm < 1000 && currentM  > 1000)
            {
                startMeter = (currentM) % 1000;
            }
            else
            {
                startMeter = 0;
            }
        }

        return answer;
    }

    public static LinkedList<NumberDataItem> OneMeterInterpolation(NumberDataItem[] origin)
    {
        var answer = new LinkedList<NumberDataItem>();
        if (!origin.Any())
            return answer;
        answer.AddLast(origin[0]);
        for (int i = 1; i < origin.Length; i++)
        {
            var previousItem = origin[i-1];
            var currentItem = origin[i];
            //var nextItem = (i >= origin.Length - 1) ? null : origin[i + 1];
            if (previousItem.Distance.Km < currentItem.Distance.Km)
            {
                    //Дозаполняем километр
                    // От конца до 1000
                    for (var m = previousItem.Distance.M+1; m < 1000; m++)
                    {
                        var interpolated =
                            Helper.Interpolate(
                                previousItem, currentItem,
                                new Distance(previousItem.Distance.Km, m), 1000);
                        answer.AddLast(interpolated);
                    }
                    //от начала до currentItem
                    for (var m = 0; m < currentItem.Distance.M; m++)
                    {
                        var interpolated =
                            Helper.Interpolate(
                                previousItem, currentItem,
                                new Distance(currentItem.Distance.Km, m), Math.Max(1000, previousItem.Distance.M+1));
                        answer.AddLast(interpolated);
                    }
                    
            }
            else
            {
                //Считаем интерполяцию по метрам
                for (var m = previousItem.Distance.M+1; m < currentItem.Distance.M; m++)
                {
                    var interpolated =
                        Helper.Interpolate(
                            previousItem, currentItem,
                            new Distance(previousItem.Distance.Km, m), 1000);
                    answer.AddLast(interpolated);
                }
            }
            answer.AddLast(currentItem);
        }
        return answer;
    }
}