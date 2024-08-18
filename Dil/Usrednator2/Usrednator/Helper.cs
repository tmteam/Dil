﻿using System.IO;
using Dil.Core.Entities;

namespace Usrednator;

public static class Helper
{



    public static string[] SplitLines(string originalText)
    {
        var lines = originalText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
    }


    public static void ArrayInplaceSummOrThrow(this double[] origin, double[] addition, int? displayRowNumber = null)
    {
        if (origin.Length != addition.Length)
        {
            if(displayRowNumber!=null)
                throw new InvalidDataException(
                    $"Количество данных в строке {displayRowNumber} отличается от строки {displayRowNumber - 1}");
            else
                throw new InvalidDataException(
                    "Количество данных в строках отличается");
        }

        for (int j = 0; j < origin.Length; j++)
            origin[j] += addition[j];
    }
    
    private static double[] ArraySubstrOrThrow(this double[] origin, double[] substr, int? displayRowNumber = null)
    {
        if (origin.Length != substr.Length)
        {
            if(displayRowNumber!=null)
                throw new InvalidDataException(
                    $"Количество данных в строке {displayRowNumber} отличается от строки {displayRowNumber - 1}");
            else
                throw new InvalidDataException(
                    "Количество данных в строках отличается");
        }

        var result = new double[origin.Length];
        
        for (int j = 0; j < origin.Length; j++)
            result[j] = origin[j] - substr[j];
        return result;
    }
    
    private static double[] ArraySummOrThrow(this double[] origin, double[] b, int? displayRowNumber = null)
    {
        if (origin.Length != b.Length)
        {
            if(displayRowNumber!=null)
                throw new InvalidDataException(
                    $"Количество данных в строке {displayRowNumber} отличается от строки {displayRowNumber - 1}");
            else
                throw new InvalidDataException(
                    "Количество данных в строках отличается");
        }

        var result = new double[origin.Length];
        
        for (int j = 0; j < origin.Length; j++)
            result[j] = origin[j] + b[j];
        return result;
    }
    
    private static double[] ArrayDivide(this double[] origin, double divider)
    {
        var result = new double[origin.Length];
        for (int j = 0; j < origin.Length; j++)
            result[j] = origin[j] /divider;
        return result;
    }
    
    private static double[] ArrayMultiply(this double[] origin, double multiplier)
    {
        var result = new double[origin.Length];
        for (int j = 0; j < origin.Length; j++)
            result[j] = origin[j] * multiplier;
        return result;
    }
    
    public static NumberDataItem Interpolate(NumberDataItem left, NumberDataItem right, Distance target)
    {
        var intervalDistance = right.Distance.DifferenceInMetters(left.Distance);
        var distanceToTarget = target.DifferenceInMetters(left.Distance);

        var differenceInData = right.Data.ArraySubstrOrThrow(left.Data);
        var stepDelta = differenceInData.ArrayDivide(intervalDistance);

        var interpolatedData = left.Data.ArraySummOrThrow(stepDelta.ArrayMultiply(distanceToTarget));
        return new NumberDataItem(target, interpolatedData);
    }

    }