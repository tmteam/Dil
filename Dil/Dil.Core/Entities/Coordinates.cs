using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dil.Core.Entities;

public class Coordinate
{
    public Coordinate(Distance distance, int latitudeMajor,  decimal latitudeMinor,int longitudeMajor, decimal longitudeMinor)
    {
            Distance = distance;
            LatitudeMajor = latitudeMajor;
            LongitudeMajor = longitudeMajor;
            LatitudeMinor = latitudeMinor;
            LongitudeMinor = longitudeMinor;
        }
        
    public Distance Distance { get; }
    public int LatitudeMajor { get; }
    public Decimal LatitudeMinor { get; }
    public int LongitudeMajor { get; }
    public Decimal LongitudeMinor { get; }

    public static Coordinate ParseOrNull(string line)
    {
            var dataItem = DataItem.ParseOrNull(line);
            if (dataItem.Data.Length < 4)
                return null;
            if (!int.TryParse(dataItem.Data[0], out var wideMajor))
                return null;
            if (!decimal.TryParse(dataItem.Data[1], out var wideMinor))
                return null;
            if (!int.TryParse(dataItem.Data[2], out var longMajor))
                return null;
            if (!decimal.TryParse(dataItem.Data[3], out var longMinor))
                return null;
            
            return new Coordinate(
                distance: dataItem.Distance, 
                latitudeMajor: wideMajor, 
                latitudeMinor: wideMinor, 
                longitudeMajor: longMajor, 
                longitudeMinor: longMinor);
        }
}
public class Coordinates
{
    public static Coordinates Parse(string text)
    {
            int strNumber = 0;
            var items = new List<Coordinate>();
            foreach (var line in text.Split(new[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries))
            {
                strNumber++;
                if(string.IsNullOrWhiteSpace(line))
                    continue;
                var item = Coordinate.ParseOrNull(line);
                if(item==null)    
                    throw new InvalidDataException($"В файл с координатами закралась ошибка на строке {strNumber}");
                items.Add(item);
            }

            return new Coordinates {Items = items.ToArray()};
        }

    public Coordinate GetOrNull(Distance distance)
    {
            return Items.FirstOrDefault(i => Equals(i.Distance, distance));
        }
    public Coordinate[] Items { get; private set; }

    public static Coordinates ReadFromFile(string filePath)
    {
            string text;
            try
            {
                text = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Файл с координатами не найден или поврежден");
            }

            return Parse(text);
        }
}