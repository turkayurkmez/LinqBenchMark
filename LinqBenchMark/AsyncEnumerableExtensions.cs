//using System.Linq.Async;

namespace LinqBenchMark
{
    
    public static class AsyncEnumerableExtensions
    {
        // IAsyncEnumerable<T> için bir tampon oluşturur ve belirli bir sayıda öğeyi gruplar
        public static async IAsyncEnumerable<IEnumerable<T>> BufferAsync<T>(this IAsyncEnumerable<T> source, int count)
        {
            var buffer = new List<T>(count); // Tamponu başlat

            // Kaynağı asenkron olarak döngüye al
            await foreach (var item in source)
            {
                buffer.Add(item); // Öğeyi tampona ekle
                // Tampon belirli bir boyuta ulaştığında
                if (buffer.Count == count)
                {
                    yield return buffer; // Tamponu döndür
                    buffer = new List<T>(count); // Yeni bir tampon oluştur
                }
            }

            // Eğer tamponda kalan öğeler varsa
            if (buffer.Count > 0)
            {
                yield return buffer; // Kalan öğeleri döndür
            }
        }
    }
}
