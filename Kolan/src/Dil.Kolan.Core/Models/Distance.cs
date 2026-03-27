namespace Dil.Core.Models;

/// <summary>
/// Адрес точки на дороге: километровый столб + смещение в метрах.
/// Между километровыми столбами может быть любое количество метров (не обязательно 1000).
/// Сравнение и арифметика работают только в пределах одного километра
/// или при явном указании длины километра.
/// </summary>
public readonly struct Distance : IEquatable<Distance>
{
    public int Km { get; }
    public int M { get; }

    public Distance(int km, int m)
    {
        Km = km;
        M = m;
    }

    public override string ToString() => $"{Km}+{M}";

    public bool Equals(Distance other) => Km == other.Km && M == other.M;
    public override bool Equals(object? obj) => obj is Distance d && Equals(d);
    public override int GetHashCode() => HashCode.Combine(Km, M);

    public static bool operator ==(Distance left, Distance right) => left.Equals(right);
    public static bool operator !=(Distance left, Distance right) => !left.Equals(right);
}
