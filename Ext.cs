namespace TeleprompterConsole;
static class Ext
{
    public static async IAsyncEnumerable<R> Map<T, R>(this IAsyncEnumerable<T> source, Func<T, IEnumerable<R>> func)
    {
        await foreach (var line in source)
        {
            foreach (var result in func(line))
            {
                yield return result;
            }
        }
    }
}