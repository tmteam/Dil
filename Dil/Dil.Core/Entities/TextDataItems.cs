﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dil.Core.Entities;

public class DataItems
{
    public DataItem[] Items { get; private set; }

    public static DataItems Parse(string text)
    {
            int strNumber = 0;
            var items = new List<DataItem>();
            foreach (var line in text.Split(new[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                strNumber++;
                if(string.IsNullOrWhiteSpace(line))
                    continue;
                var item = DataItem.ParseOrNull(line);
                if(item==null)    
                    throw new InvalidDataException($"В файл с данными закралась ошибка на строке {strNumber}");
                items.Add(item);
            }

            return new DataItems {Items = items.ToArray()};
        }

    public static DataItems ReadFromFile(string filePath)
    {
            string text;
            try
            {
                text = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Файл с данными не найден или поврежден");
            }

            return Parse(text);
        }
}

public class DataItem
{
    public Distance Distance { get; }
    public readonly int Km;
    public readonly int M;
    public readonly string[] Data;

    public static DataItem ParseOrNull(string line)
    {
            var items = line.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if(items.Length<2)
                return null;
            if (!int.TryParse(items[0], out var km))
                return null;
            if (!int.TryParse(items[1], out var m))
                return null;
            return new DataItem(km, m, items.Skip(2).ToArray());
        }
        
    public DataItem(int km, int m, string[] data)
    {
            Distance = new Distance(km, m);
            Km = km;
            M = m;
            Data = data;
        }
        
    public DataItem(Distance distance, string[] data)
    {
            Distance = distance;
            Km = distance.Km;
            M = distance.M;
            Data = data;
        }

    public void AppendTo(StringBuilder sb) => sb.Append($"\r\n{Km}\t{M}\t{string.Join("\t", Data)}");
}