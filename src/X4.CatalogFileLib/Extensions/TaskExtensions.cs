namespace X4.CatalogFileLib.Extensions;

internal static class TaskExtensions
{
#if NETSTANDARD2_0 || NET461
    public static Task<T> WaitAsync<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        return task.IsCompleted // fast-path optimization
            ? task
            : task.ContinueWith(
                completedTask => completedTask.GetAwaiter().GetResult(),
                cancellationToken,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
    }
#endif
    
    public static async Task<IEnumerable<TModel>> WhenAllWithProgress<TModel>(this ICollection<Task<TModel>> tasks, Action<ICollection<Task<TModel>>> reportProgressAction)
    {
        // get Task which completes when all 'tasks' have completed
        var whenAllTask = Task.WhenAll(tasks);
        while (true)
        {
            // get Task which completes after 100ms
            Task timer = Task.Delay(100); // you might want to make this configurable
            // Wait until either all tasks have completed OR 100ms passed
            await Task.WhenAny(whenAllTask, timer);

            // if all tasks have completed, complete the returned task
            if (whenAllTask.IsCompleted)
            {
                return whenAllTask.Result;
            }

            // Otherwise call progress report lambda and do another round
            reportProgressAction(tasks);
        }
    }
}