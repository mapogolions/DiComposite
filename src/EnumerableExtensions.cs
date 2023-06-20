namespace DiComposite;

public static class EnumerableExtensions
{
    public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        int index = 0;
        foreach (var value in source) yield return (index++, value);
    }
}
