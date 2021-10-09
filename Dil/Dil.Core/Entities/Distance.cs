using System;

namespace Dil.Core.Entities
{
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
            if (left.Km < right.Km)
                return false;
            if (left.M > right.M)
                return true;
            if (left.M < right.M)
                return false;
            throw new InvalidOperationException();
        }

        public static bool operator <(Distance left, Distance right)
        {
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
            if (left.Km < right.Km)
                return false;
            if (left.Km > right.Km)
                return true;
            return !(left.M < right.M);
        }

        public static bool operator <=(Distance left, Distance right)
        {
            if (left.Km > right.Km)
                return false;
            if (left.Km < right.Km)
                return true;
            return !(left.M > right.M);
        }

        public override bool Equals(object obj)
            => obj is Distance e && Km == e.Km && M == e.M;

        public Distance AppendMeters(int meters)
            => new Distance(Km, M + meters);

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
}