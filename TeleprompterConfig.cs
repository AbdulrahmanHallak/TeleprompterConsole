namespace TeleprompterConsole;
using static Math;
internal class TeleprompterConfig
{
    public int DelayInMilliseconds { get; private set; } = 200;
    public bool Done { get; private set; }

    public void SetDone() => Done = true;

    public void UpdateDelay(int increment) // Negative to speed up.
    {
        var newDelay = Min(DelayInMilliseconds + increment, 1000);
        newDelay = Max(newDelay, 20);
        DelayInMilliseconds = newDelay;
    }

}

