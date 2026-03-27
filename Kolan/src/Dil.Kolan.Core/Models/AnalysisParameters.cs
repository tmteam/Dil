namespace Dil.Core.Models;

/// <summary>
/// Настраиваемые параметры расчёта.
/// </summary>
public record AnalysisParameters(
    double Threshold = 20.0,
    int WindowSize = 10,
    int MinCardLength = 100,
    int MergeDistance = 100);
