using System;
using System.Globalization;
using System.IO;

namespace Usrednator
{
    public class Entry
    {
        public Entry(Distance distance, double[] data)
        {
            Distance = distance;
            Data = data;
        }

        public Distance Distance { get; }
        public double[] Data { get; }

        public static Entry Parse(string s)
        {
            var splitted = s.Split( new []{'\t'}, StringSplitOptions.RemoveEmptyEntries);
            if(splitted.Length<3)
                throw new InvalidDataException("Количество элементов в строке недостаточно");
            if(!int.TryParse(splitted[0], out int km))
                throw new InvalidDataException("Первый столбец должен содержать километры. А тут - не пойми что!");
            if (!int.TryParse(splitted[1], out int m))
                throw new InvalidDataException("Второй столбец должен содержать метры. А тут - не пойми что!");
            var data = new double[splitted.Length - 2];
            for (int i = 2; i < splitted.Length; i++)
            {
                if (!double.TryParse(splitted[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                    throw new InvalidDataException($"В столбце номер {i} должно быть число - а тут непойми что!");
                data[i - 2] = val;
            }
            return new Entry(new Distance(km,m),data);
        }

        public override string ToString() => $"{Distance} ->{Data.Length}";
    }
}