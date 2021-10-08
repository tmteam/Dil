using System;
using System.Collections.Generic;
using System.IO;

namespace Pereverter
{
    public class Lengths
    {
        public static Lengths Create(Dictionary<int, int> lengths)
        {
            var res = new Lengths();
            foreach (var length in lengths)
            {
                res._lengths   .Add(length.Key, length.Value);
                res._kmsOrdered.Add(length.Key);
            }
            res._kmsOrdered.Sort();
            return res;
        }
        
        /// <exception cref="InvalidDataException"></exception>
        public static Lengths Parse(string text)
        {
            var lines = text.Split(new[]{"\r","\n"}, StringSplitOptions.RemoveEmptyEntries);
            var results = new Lengths();
            int strNumber = 0;
            foreach (var line in lines)
            {
                strNumber++;
                var items = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 0) 
                    continue;
                if(items.Length<2)
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}");
                
                if(!int.TryParse(items[0], out var km)) 
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}. Неверный формат километража");
                
                if(!int.TryParse(items[1], out var length)) 
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}. Неверный формат протяженности");
                
                if(results._lengths.ContainsKey(km))
                    throw new InvalidDataException($"В файл протяженностей закралась ошибка на строке {strNumber}. Километраж повторяетя");
                results._lengths.Add(km, length);
                results._kmsOrdered.Add(km);
            }
            results._kmsOrdered.Sort();

            return results;
        }
        /// <exception cref="InvalidDataException"></exception>
        public static Lengths ReadFromFile(string filePath)
        {
            string text;
            try
            {
                text = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Файл протяженностей не найден или поврежден");
            }

            return Parse(text);
        }

        readonly List<int> _kmsOrdered = new List<int>();
        
        readonly Dictionary<int, int> _lengths  = new Dictionary<int, int>();

        public int Count => _lengths.Count;
        public (int resKm, int resM) GetReversed(int itemKm, int itemM)
        {
            if (itemM == 0)
                return (itemKm, itemM);
            
            //Если протяженность 2-3: 900, задано:3.100, значит результат: 2.800
            //var lengths = Lengths.Create(new Dictionary<int, int> {{2, 800}, {3, 900}, {4, 1000}});
            //var (km,m) = lengths.GetReversed(3, 100);
            //Assert.AreEqual(800,m);
            //Assert.AreEqual(2,km);

            var nextKm = 0;
            var i = _kmsOrdered.IndexOf(itemKm);
            if (i < 1) nextKm = itemKm - 1;
            else       nextKm = _kmsOrdered[i - 1];
            int length = 1000;
            if (_lengths.TryGetValue(nextKm, out var m))
            {
                length = m;
            }
            return (nextKm, Math.Max(0, length - itemM));

        }
    }
}