namespace ABS.Mapper;

public static partial class Mapper
{
    public static string? ToDollars(decimal? input)
    {
        if (input.HasValue)
        {
            return $"{input / 100:#,##0.00}";
        }

        return null;
    }
}