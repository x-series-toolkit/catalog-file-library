namespace X4.CatalogFileLib.Extensions;

public static class EnumerableExtensions
{
    public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, CancellationToken, Task> body, CancellationToken token = default)
    {
        async Task AwaitPartition(IEnumerator<T> partition)
        {
            using (partition)
            {
                while (partition.MoveNext())
                {
                    await body(partition.Current, token);
                }
            }
        }
        
        return Task.WhenAll(
            Partitioner
                .Create(source)
                .GetPartitions(dop)
                .AsParallel()
                .Select(AwaitPartition));
    }
}