using BenchmarkDotNet.Running;

namespace Collections2.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DictionariesLookupComparison>();
        }
    }
}
