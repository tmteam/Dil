using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dil.Core.Entities;

namespace Dil.Core;

public static class DilHelper
{
    public static string[] SplitLines(string originalText)
    {
        var lines = originalText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        return lines;
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
}