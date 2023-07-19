// Taken from https://github.com/sourcechord/FluentWPF/blob/master/FluentWPF/Utility/VersionInfo.cs

using System;

namespace Dashboard.Utilities;

internal readonly struct VersionInfo : IEquatable<VersionInfo>, IComparable<VersionInfo>, IComparable
{
    public readonly int Major;
    public readonly int Minor;
    public readonly int Build;

    public VersionInfo(int major, int minor, int build)
    {
        Major = major;
        Minor = minor;
        Build = build;
    }

    public bool Equals(VersionInfo other)
    {
        return Major == other.Major && Minor == other.Minor && Build == other.Build;
    }

    public override bool Equals(object obj)
    {
        return (obj is VersionInfo other) && Equals(other);
    }

    public override int GetHashCode()
    {
        return Major.GetHashCode() ^ Minor.GetHashCode() ^ Build.GetHashCode();
    }

    public static bool operator ==(VersionInfo left, VersionInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VersionInfo left, VersionInfo right)
    {
        return !(left == right);
    }


    public int CompareTo(VersionInfo other)
    {
        if (Major != other.Major)
        {
            return Major.CompareTo(other.Major);
        }

        if (Minor != other.Minor)
        {
            return Minor.CompareTo(other.Minor);
        }

        if (Build != other.Build)
        {
            return Build.CompareTo(other.Build);
        }

        return 0;
    }

    public int CompareTo(object obj)
    {
        if (obj is not VersionInfo other)
        {
            throw new ArgumentException($"{nameof(obj)} is not a {nameof(VersionInfo)}");
        }

        return CompareTo(other);
    }

    public static bool operator <(VersionInfo left, VersionInfo right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(VersionInfo left, VersionInfo right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(VersionInfo left, VersionInfo right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(VersionInfo left, VersionInfo right)
    {
        return left.CompareTo(right) >= 0;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Build}";
    }
}
