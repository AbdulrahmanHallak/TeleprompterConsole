namespace TeleprompterConsole;
class Program
{
    static void Main(string[] args)
    {
        RunTeleprompter().Wait();
    }
    /// <summary>
    /// Reads through the specified file one line at a time and splits each line into words.
    /// </summary>
    /// <param name="filePath">The full path for the file to read from.</param>
    /// <returns>Each call returns the words of a single line in an <see cref="IEnumerable{T}"/> of type <see cref="String"/>.</returns>
    public static IEnumerable<string> ReadFrom(string filePath)
    {
        string? line;
        using StreamReader reader = new StreamReader(filePath);

        while ((line = reader.ReadLine()) != null)
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
    }

    public static async Task ShowTeleprompter(TeleprompterConfig config)
    {
        var path = "<Quotes.txt path>";
        var words = ReadFrom(path);
        foreach (var word in words)
        {
            Console.Write(word);
            if (!string.IsNullOrEmpty(word))
            {
                await Task.Delay(config.DelayInMilliseconds);
            }
        }
        config.SetDone();
    }
    /// <summary>
    /// Gets input through the console to speed up, slow down or stop the <see cref="ShowTeleprompter(TeleprompterConfig)"/>
    /// </summary>
    /// <param name="config"> A configuration object to customize <see cref="ShowTeleprompter(TeleprompterConfig)"/> based on user input.</param>
    private static async Task GetInput(TeleprompterConfig config)
    {
        Action work = () =>
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

        };
        await Task.Run(work);
    }
    private static async Task RunTeleprompter()
    {
        var config = new TeleprompterConfig();
        var displayTask = ShowTeleprompter(config);

        var speedTask = GetInput(config);
        await Task.WhenAny(displayTask, speedTask);
    }
}
