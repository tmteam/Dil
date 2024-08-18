using System;

namespace Dil.Core.Entities;

public readonly struct Distance
{
    public Distance(int km, int m)
    {
        Km = km;
        M = m;
    }

    public static bool operator >(Distance left, Distance right)
    {
        if (left.Km > right.Km)
            return true;
        return left.Km * 1000 + left.M > right.Km * 1000 + right.M;

    }

    public static bool operator <(Distance left, Distance right)
    {
        if (left.Km > right.Km )
            return false;
        return left.Km * 1000 + left.M < right.Km * 1000 + right.M;

        if (left.Km < right.Km)
            return true;
        if (left.Km > right.Km)
            return false;
        if (left.M < right.M)
            return true;
        return false;
    }

    public static bool operator >=(Distance left, Distance right)
    {
        if (left.Km > right.Km)
            return true;
        return left.Km * 1000 + left.M >= right.Km * 1000 + right.M;
    }

    public static bool operator <=(Distance left, Distance right)
    {
        if (left.Km > right.Km )
            return false;
        return left.Km * 1000 + left.M <= right.Km * 1000 + right.M;
    }

    public int DifferenceInMetters(Distance r, int metersInKm = 1000) =>
        (Km * metersInKm + M) - (r.Km * metersInKm + r.M);

    public override bool Equals(object obj)
        => obj is Distance e && Km == e.Km && M == e.M;

    public Distance AppendMeters(int meters)
        => new Distance(Km, M + meters);

    public Distance Simplify()
    {
        if (M < 1000)
            return this;
        var addKm = M / 1000;
        return new Distance(Km + addKm, M % 1000);
    }

    public int Km { get; }
    public int M { get; }

    public override string ToString()
        => $"{Km}+{M}";

    public Distance ConvertToNextKm(int nextKm, int lastMeterOfKm)
    {
        if (nextKm <= Km)
            throw new InvalidOperationException();
        if (lastMeterOfKm > M)
            return this;
        return new Distance(nextKm, M - lastMeterOfKm);
    }
}