//using System.Linq.Async;

namespace LinqBenchMark
{
    
    //public static class AsyncEnumerableExtensions
    //{
    //    // IAsyncEnumerable<T> için bir tampon oluşturur ve belirli bir sayıda öğeyi gruplar
    //    public static async IAsyncEnumerable<T> BufferAsync<T>(this IAsyncEnumerable<T> source, int count)
    //    {
    //        var buffer = new List<T>(count);
    //        await foreach (var item in source)
    //        {
    //            buffer.Add(item);
    //            if (buffer.Count == count)
    //            {
    //                foreach (var i in buffer)
    //                {
    //                    yield return i;
    //                }
    //                buffer.Clear();
    //            }
    //        }
    //        foreach (var i in buffer)
    //        {
    //            yield return i;
    //        }

    //    }
    //}
}
