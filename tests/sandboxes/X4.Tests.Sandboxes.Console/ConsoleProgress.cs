namespace X4.Tests.Sandboxes.Console;

public class ConsoleProgress<T> : IProgress<T>
{
    private readonly Action<T> _action;

    public ConsoleProgress(Action<T> action)
    {
        _action = action;
    }
    
    public void Report(T value)
    {
        _action(value);
    }
}