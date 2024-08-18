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
         *  Если длина участка суммирования перед новым столбом будет меньше, примерно,
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
    
    public static LinkedList<NumberDataItem> AverageFilter(
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

    // public static LinkedList<NumberDataItem> AverageFilter2(
    //     IEnumerable<NumberDataItem> origin, int averageMetters, bool useZeroPivots)
    // {
    //     var answer = new LinkedList<NumberDataItem>();
    //     if (!origin.Any())
    //         return answer;
    //     
    //     var startOfCurrentInterval = origin.First().Distance;
    //     var endOfCurrentInterval = startOfCurrentInterval.AppendMeters(averageMetters);
    //         
    //     var buffer = new LinkedList<NumberDataItem>();
    //     var previousBuffer = new LinkedList<NumberDataItem>();
    //     int currentKm = startOfCurrentInterval.Km;
    //     int lastMeterInterval = averageMetters;
    //     
    //     var previousItem = origin.First();
    //     
    //     foreach(var nextItem in origin.Skip(1))
    //     {
    //         var differenceInMetters = nextItem.Distance.DifferenceInMetters(previousItem.Distance);
    //         var differenceInData = nextItem.Data.ArraySubstrOrThrow(previousItem.Data);
    //         var stepDelta = differenceInData.ArrayDivide(differenceInMetters);
    //         
    //         for (var i = 0; i < differenceInMetters; i++)
    //         {
    //             var stepData = previousItem.Data.ArraySummOrThrow(differenceInData.ArrayMultiply(i));
    //             
    //         }
    //         Dictionary<int, DataItem> approximations
    //         
    //     }
    //     
    //     int bufferStartIndex = 0;
    //     
    //     int i = -1;
    //     
    //     var previousDistance = new Distance();
    //     var prePreviousDistance = new Distance();
    //         
    //     foreach (var current in origin)
    //     {
    //         i++;
    //         if (current.Distance.Km > currentKm)
    //         {
    //             #region Обработка перехода на другой киллометр
    //
    //             currentKm = current.Distance.Km;
    //
    //             if (i > 1)
    //             {
    //                 //Если две последних записи в одном километре - то можем скорректироваться по ним
    //                 if (prePreviousDistance.Km == previousDistance.Km)
    //                 {
    //                     lastMeterInterval = previousDistance.M - prePreviousDistance.M;
    //                 }
    //
    //                 //Если нужна привязка к нулевому метру киллометра
    //                 if (useZeroPivots)
    //                 {
    //                     endOfCurrentInterval = new Distance(current.Distance.Km, 0);
    //                         
    //                     /*
    //                      *  Если длина участка суммирования перед новым столбом < 0.66*averageMeters,
    //                      *  то этот кусок приобщается к предыдущему участку,
    //                      *  который усредняется с большим количеством метров,
    //                      *  чем установлено в программе обработки.
    //                      *
    //                      * Если участок суммирования, примыкающий к километровому столбу, будет больше 2 / 3 { averageMeters},
    //                      * то он усредняется по фактической длине.
    //                      */
    //                     if (lastMeterInterval < averageMetters * 0.66 && answer.Any())
    //                     {
    //                         //Сливаем два буфера
    //                         foreach (var entry in previousBuffer)
    //                             buffer.AddFirst(entry);
    //                         previousBuffer.Clear();
    //
    //                         //Отменяем последнее значение. Его нужно посчитать заново
    //                         startOfCurrentInterval = answer.Last().Distance;
    //                         answer.RemoveLast();
    //                     }
    //
    //                 }
    //                 else
    //                 {
    //                     //Предполагаемая длина километра
    //                     var lastMeterOfKm = previousDistance.M + lastMeterInterval;
    //                     //Корректируем концовку интервала
    //                     endOfCurrentInterval = endOfCurrentInterval.ConvertToNextKm(current.Distance.Km, lastMeterOfKm);
    //                 }
    //             }
    //             #endregion
    //         }
    //         
    //         if (current.Distance >= endOfCurrentInterval)
    //         {
    //             //Нужно заполнить [current.Distance.. endOfCurrentInterval] интерполированными значениями
    //             #region Обработка полученного интервала
    //             if (buffer.Any())
    //             {
    //                 var resultData = new double[buffer.First().Data.Length];
    //                 int j = -1;
    //                 foreach (var entry in buffer)
    //                 {
    //                     j++;
    //                     resultData.ArrayInplaceSummOrThrow(entry.Data, bufferStartIndex + j);
    //                 }
    //
    //                 answer.AddLast(new NumberDataItem(startOfCurrentInterval, resultData.Select(r => r / buffer.Count).ToArray()));
    //                 previousBuffer = buffer;
    //                 buffer = new LinkedList<NumberDataItem>();
    //             }
    //             bufferStartIndex = i;
    //             startOfCurrentInterval = endOfCurrentInterval;
    //             endOfCurrentInterval = endOfCurrentInterval.AppendMeters(averageMetters);
    //             #endregion
    //         }
    //
    //         if (current.Distance < endOfCurrentInterval)
    //             buffer.AddLast(current);
    //
    //         prePreviousDistance = previousDistance;
    //         previousDistance = current.Distance;
    //
    //     }
    //     return answer;
    // }
    
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