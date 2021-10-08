using System;
using System.Collections.Generic;
using System.IO;

namespace Pereverter
{
    public class Lengths
    {
        /// <exception cref="InvalidDataException"></exception>
        public static Lengths ReadFromFile(string filePath)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Файл протяженностей не найден или поврежден");
            }

            var results = new Lengths();
            int strNumber = 0;
            foreach (var line in lines)
            {
                strNumber++;
                var items = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 0) 
                    continue;
                if(items.Length<3)
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}");
                if(!int.TryParse(items[0], out var km)) 
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}. Неверный формат километража");
                if(!int.TryParse(items[2], out var length)) 
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}. Неверный формат протяженности");
                
                if(results._lengths.ContainsKey(km))
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}. Километраж повторяетя");
                results._lengths.Add(km, length);
                results._kmsOrdered.Add(km);
            }

            return results;
        }

        readonly List<int> _kmsOrdered = new List<int>();
        
        readonly Dictionary<int, int> _lengths  = new Dictionary<int, int>();

        public (int resKm, int resM) GetReversed(int itemKm, int itemM)
        {
            //Если было 1.100, при этом протяженность 1: 1300, а следующий км это 2 то 
            //перевернутый километраж это  2.1200
            int nextKm = itemKm + 1;
            int length = 1000;
            if (_lengths.TryGetValue(itemKm, out var m))
            {
                length = m;
                var i = _kmsOrdered.IndexOf(itemKm);
                if (_kmsOrdered.Count < i + 1) 
                    nextKm = _kmsOrdered[i + 1];
            }
            return (nextKm, Math.Max(0, length - itemM));
        }
    }
}