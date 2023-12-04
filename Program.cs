namespace TeleprompterConsole;
class Program
{
    static async Task Main(string[] args)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Quotes.txt");
        var config = new TeleprompterConfig();

        var partialApp = (TeleprompterConfig config) => (string line) => WriteToConsole(config, line);
        var task = ReadLines(path).Map(FormatLine).ForEachAwaitAsync(partialApp(config));

        // * this code is imperative.
        // Func<TeleprompterConfig, Task> task = async (config) =>
        // {
        //     await foreach (var line in ReadLines(path))
        //     {
        //         var formatted = FormatLine(line);
        //         foreach (var item in formatted)
        //             await WriteToConsole(config, item);
        //     }
        // };

        Task.WhenAny(Task.Run(() => GetInput(config)), task).Wait();
    }
    /// <summary>
    /// Reads through the specified file one line at a time.
    /// </summary>
    /// <param name="filePath">The full path for the file to read from.</param>
    /// <returns>Each call returns a single line in an <see cref="IAsyncEnumerable{string}"/> of type <see cref="string"/>.</returns>
    public static async IAsyncEnumerable<string> ReadLines(string filePath)
    {
        using var reader = File.OpenText(filePath);
        while (!reader.EndOfStream)
            yield return (await reader.ReadLineAsync())!;
    }

    /// <summary>
    /// Formats the provided line of text to not exceeds 70 characters per line and inserts line breaks in between.
    /// </summary>
    /// <param name="line">The input string to be formatted.</param>
    /// <returns>An <see cref="IEnumerable{string}"/> of strings representing the formatted lines.</returns>
    public static IEnumerable<string> FormatLine(string line)
    {
        var words = line.Split(' ');
        var lineLength = 0;
        foreach (var word in words)
        {
            yield return word + " ";
            lineLength += word.Length + 1;

            if (lineLength > 70)
            {
                yield return Environment.NewLine;
                lineLength = 0;
            }
        }
        yield return Environment.NewLine;
    }

    /// <summary>
    /// Writes a word to the console with an optional delay between characters.
    /// </summary>
    /// <param name="config">The configuration for controlling the writing process.</param>
    /// <param name="word">The word to be written to the console.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task WriteToConsole(TeleprompterConfig config, string word)
    {
        Console.Write(word);
        if (!string.IsNullOrEmpty(word))
            await Task.Delay(config.DelayInMilliseconds);
    }

    /// <summary>
    /// Gets input through the console to speed up, slow down or stop the <see cref="ShowTeleprompter(TeleprompterConfig)"/>
    /// </summary>
    /// <param name="config"> A configuration object to customize <see cref="ShowTeleprompter(TeleprompterConfig)"/> based on user input.</param>
    public static void GetInput(TeleprompterConfig config)
    {
        do
        {
            var key = Console.ReadKey(true);
            if (key.KeyChar == '<')
                config.UpdateDelay(10);

            else if (key.KeyChar == '>')
                config.UpdateDelay(-100);

            else if (key.KeyChar == 'X' || key.KeyChar == 'x')
                break;

        } while (!config.Done);
    }
}
