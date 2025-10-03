using BenchmarkDotNet.Attributes;

namespace SpanExtensions.Tests.Performance
{
    [MemoryDiagnoser(false)]
    public class ReadOnlySpan_Min_Benchmark
    {
        [Benchmark]
        [ArgumentsSource(nameof(GetArgs))]
        public int Min(int[] value)
        {
            return value.AsSpan().Min();
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetArgs))]
        public int Min_Array(int[] value)
        {
            return value.Min();
        }

        [Benchmark]
        [ArgumentsSource(nameof(GetArgs))]
        public int Min_StraightForward(int[] value)
        {
            ReadOnlySpan<int> source = value;
            int min = source[0];
            for(int i = 1; i < source.Length; i++)
            {
                int current = source[i];
                if(current < min)
                {
                    min = current;
                }
            }
            return min;
        }

        public IEnumerable<int[]> GetArgs()
        {
            Random random = new Random(3);

            int[] choices = Enumerable.Range(0, 10000).ToArray();

            int[] data = random.GetItems(choices, 10);

            yield return data;

            data = random.GetItems(choices, 100);

            yield return data;

            data = random.GetItems(choices, 1000);

            yield return data;
        }
    }
}