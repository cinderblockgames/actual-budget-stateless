using System.Collections;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static string ToDollars(decimal input)
    {
        return $"{input / 100:#,##0.00}";
    }
}