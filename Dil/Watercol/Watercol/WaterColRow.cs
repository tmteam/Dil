using Dil.Core.Entities;

namespace Watercol;

public class WaterColRow(double incline, Track right, Track left, Distance distance)
{
    public static WaterColRow FromRow(NumberDataItem row)
    {
        var incline = row.Data[0];
        var leftTrack = new Track(row.Data[1], row.Data[2], row.Data[3]);
        var rightTrack = new Track(row.Data[4], row.Data[5], row.Data[6]);
        return new WaterColRow(incline, leftTrack, rightTrack, row.Distance);
    }

    public NumberDataItem ToNumberDataItem()
    {
        var ll = Left.LeftRelative + Incline;
        var rl = Right.LeftRelative + Incline;
        var lr = Left.RightRelative + Incline;
        var rr = Right.RightRelative + Incline;

        var hll = ll * Left.LeftSize / 1000.0;
        var hrl = rl * Right.LeftSize / 1000.0;
        var hlr = lr * Left.RightSize / 1000.0;
        var hrr = rr * Right.RightSize / 1000.0;

        var hLeft = 0.0;
        var hRight = 0.0;
        if (Incline < 0)
        {
            if (hll < 0)
                hLeft = -hll;
            if (hrl < 0)
                hRight = -hrl;
        }
        else if (Incline > 0)
        {
            if (hlr > 0)
                hLeft = hlr;
            if (hrr > 0)
                hRight = hrr;
        }
        else {
            hLeft = Left.Depth;
            hRight = Right.Depth;
        }

        var data = new[] {
             incline,
             Left.Depth,
             Left.LeftRelative,
             Left.RightRelative,
            
             Right.Depth,
             Right.LeftRelative,
             Right.RightRelative,
            
             Left.LeftRelative,
             Right.LeftRelative,
             Left.RightRelative,
             Right.RightRelative,
            
             ll,
             rl,
             lr,
             rr,
            
             hll,
             hrl,
             hlr,
             hrr,
            
            hLeft, hRight
        };
        return new NumberDataItem(Distance, data);
    }

    public Distance Distance { get; } = distance;
    public double Incline { get; } = incline;
    public Track Right { get; } = right;
    public Track Left { get; } = left;

    //Далее определяем фактические уклоны стенок, с этой целью все уклоны,
    //содержащиеся в соответствующих столбцах, складываем с уклоном полосы (столбец 3).
    public double RightRightIncline => Right.RightRelative + Incline;
    public double RightLeftIncline => Right.LeftRelative + Incline;
    public double LeftRightIncline => Left.RightRelative + Incline;
    public double LeftLeftIncline => Left.LeftRelative + Incline;

    public double CalcWaterHeight()
    {
        /*
         * Теперь определяем возвышение водораздельных точек над дном колеи. Полученные возвышения и есть глубина колеи.
            С этой целью фактический уклон колеи умножаем на расстояние от точки на дне до водораздельной точки и домножаем на 1000, чтобы перевести в мм.
            При левом уклоне, вода уходит через левые стенки колеи и. если произведение для левых стенок положительное - водораздельная точка выше дна - это и является толщиной слоя воды.
            Если произведение отрицательное водораздельная точка расположена ниже дна колеи, воды в колее не будет. Полученные произведения для левых стенок приведены в столбцах 18, 19, для правых - 20, 21.
            Таким образом, вода с полосы удаляется либо налево, либо направо. При отрицательном поперечном водоотвод происходит через левые  стенки кольей и
            произведения фактического уклона стенок на расстояния до водораздельной точки отрицательные при обеспеченном водоотводе и положительные при необеспеченном.
            При общем наклоне полосы направо - все наоборот: положительные значения произведений при обеспеченном водоотводе, отрицательные при необеспеченном. Если уклон полосы равен нулю, глубина воды равно глубине колеи.

         */
        return 0;
    }
}

public class Track(double depth, double leftSize, double rightSize)
{
    public double Depth { get; } = depth;
    public double LeftSize { get; } = leftSize;
    public double RightSize { get; } = rightSize;

    //Для этого глубину колеи нужно разделить на расстояние до левой водораздельной точки и умножить на 1000,
    //получим уклон в промилле. 
    public double LeftRelative => (Depth * 1000) / LeftSize;
    public double RightRelative => (Depth * 1000) / RightSize;
}