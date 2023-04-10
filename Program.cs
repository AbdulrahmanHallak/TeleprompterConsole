namespace TeleprompterConsole;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        RunTeleprompter().Wait();
    }


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
        var words = ReadFrom("C:\\Users\\Abdulrahman\\Desktop\\New Text Document.txt");
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
