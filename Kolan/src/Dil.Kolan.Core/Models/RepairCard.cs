namespace Dil.Core.Models;

/// <summary>
/// Результат — одна карта ремонта.
/// </summary>
public class RepairCard
{
    public Distance Start { get; init; }
    public Distance End { get; init; }
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
    public bool WasExtended { get; init; }
    public double AvgLeftRut { get; init; }
    public double AvgRightRut { get; init; }
    public double MaxLeftRut { get; init; }
    public double MaxRightRut { get; init; }

    /// <summary>
    /// Длина карты в метрах = количество метровых отметок, включая обе границы.
    /// </summary>
    public int LengthMeters => EndIndex - StartIndex + 1;
}
