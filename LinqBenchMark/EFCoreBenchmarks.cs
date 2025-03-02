using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LinqBenchMark
{
    [MemoryDiagnoser]
    public class EFCoreBenchmarks
    {
        private int _normalCount;
        private int _optimizedCount;
        private int _bufferCount;
        private int _keySetPaginationCount;

        private TestDbContext _context;

        private const int readCount = 100_000;





        private readonly Consumer consumer = new Consumer();

        [GlobalSetup]
        public void Setup()
        {
            _context = new TestDbContext();
            SeedData();
        }
        public void SeedData()
        {
            if (!_context.Customers.Any())
            {
                var customers = Enumerable.Range(1, readCount).Select(i => new Customer
                {

                    Name = $"Customer {i}",
                    Email = $"customer{i}@test.com",
                    IsActive = new Random().Next(5, 10) > 6,
                    Age = new Random().Next(10, 75),

                    LastPurchasedDate = DateTime.Now.AddDays(-new Random().Next(1, 3650))
                });

                _context.Customers.AddRange(customers);
                _context.SaveChanges();
            }
        }





        [Benchmark]
        public async Task Normal()
        {
            _normalCount = 0;
            var customers = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Age > 35)
                .OrderBy(c => c.LastPurchasedDate)
                .Select(c => new CustomerDto
                (
                     c.Id,
                    c.Name,
                     c.LastPurchasedDate
                ))
                .ToListAsync();

            foreach (var customer in customers)
            {
                _ = customer.Name.Length;
                _normalCount++;
            }

            Console.WriteLine($"Normal method processed {_normalCount} records");
        }







        [Benchmark]
        public async Task Optimized()
        {
            _optimizedCount = 0;
            var customers = _context.Customers
                                    .AsNoTracking()
                                    .Where(c => c.Age > 35)
                                    .OrderBy(c => c.LastPurchasedDate)
                                    .Select(c => new CustomerDto (c.Id, c.Name,c.LastPurchasedDate))
                                    .AsAsyncEnumerable();

            await foreach (var customer in customers)
            {
                _ = customer.Name.Length;
                _optimizedCount++;

            }

            Console.WriteLine($"Optimized method processed {_optimizedCount} records");


        }


        [Benchmark]
        public async Task WithBufferAsync()
        {
            _bufferCount = 0;
            const int bufferCount = 1000;
            await foreach (var batch in _context.Customers
                                                .AsNoTracking()
                                                .Where(c => c.Age > 35)
                                                .OrderBy(c => c.LastPurchasedDate)
                                                .Select(c => new CustomerDto (c.Id, c.Name, c.LastPurchasedDate))
                                                .AsAsyncEnumerable()
                                                .Buffer(bufferCount))
            {
                foreach (var customer in batch)
                {
                    _ = customer.Name.Length;
                    _bufferCount++;
                }

            }

            Console.WriteLine($"Buffer method processed {_bufferCount} records");

        }




        [Benchmark]
        public async Task WithBulkLoadAndLocalPagination()
        {
            // Tüm verileri tek sorguda getir
            var allCustomers = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Age > 35)
                .OrderBy(c => c.LastPurchasedDate)
                .Select(c => new CustomerDto
                (
                     c.Id,
                    c.Name,
                     c.LastPurchasedDate
                ))
                .ToListAsync();

            // Bellek üzerinde sayfalama yap
            const int chunkSize = 1000;
            int totalProcessed = 0;

            foreach (var chunk in allCustomers.Chunk(chunkSize))
            {
                // Her 10000'lik grup için işlem yap
                foreach (var customer in chunk)
                {
                    // İşlem yap
                    totalProcessed++;
                }

                // İsteğe bağlı: İlerleme durumunu göster
                Console.WriteLine($"Processed {totalProcessed} of {allCustomers.Count}");
            }
        }

        private static readonly Func<TestDbContext, int, IAsyncEnumerable<CustomerDto>> GetCustomersQuery =
    EF.CompileAsyncQuery((TestDbContext context, int ageThreshold) =>
        context.Customers
            .AsNoTracking()
            .Where(c => c.Age > ageThreshold)
            .OrderBy(c => c.LastPurchasedDate)
            .Select(c => new CustomerDto
            (
                c.Id,
                c.Name,
                c.LastPurchasedDate
            )));

        [Benchmark]
        public async Task WithCompiledQuery()
        {
            _optimizedCount = 0;
            var customers = GetCustomersQuery(_context, 35);
            await foreach (var customer in customers)
            {
                _ = customer.Name.Length;
                _optimizedCount++;
            }
            Console.WriteLine($"Optimized method processed {_optimizedCount} records");
        }

        //[Benchmark]
        public async Task WithKeySetPagination()
        {
           // Console.WriteLine("KeySetPagination working...");
            _keySetPaginationCount = 0;
            const int pageSize = 10000;
            var result = new List<CustomerDto>();
            int? lastId = null;
            DateTime? lastPurchasedDate = null;
            bool hasMore = true;
            
            while (hasMore)
            {         
              
            
                var query = _context.Customers
                    .AsNoTracking()
                    .Where(c => c.Age > 35);

                if (lastId != null && lastPurchasedDate != null)
                {
                    query = query.Where(c => c.LastPurchasedDate > lastPurchasedDate || c.LastPurchasedDate == lastPurchasedDate && c.Id > lastId);
                }

                var batch = await query
                .OrderBy(c => c.LastPurchasedDate)
                .ThenBy(c => c.Id)
                .Take(pageSize)
                .Select(c => new CustomerDto(c.Id, c.Name, c.LastPurchasedDate))
                .ToListAsync();


                if (!batch.Any())
                {
                    hasMore = false;
                    break;
                }


                foreach (var customer in batch)
                {
                  
                    _ = customer.Name.Length;
                    _keySetPaginationCount++;

                 

                    // Gerçek senaryoda burada veri işleme yapılırdı
                    // Örn: processCustomer(customer);
                }

             
                var lastCustomer = batch.Last();
                lastId = lastCustomer.Id;
                lastPurchasedDate = lastCustomer.LastPurchasedDate;
            }

            Console.WriteLine($"KeySetPagination method processed {_keySetPaginationCount} records");
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            Console.WriteLine("\n -- Total Records Processed --");
            Console.WriteLine($"Normal method processed {_normalCount} records");
            Console.WriteLine($"Optimized method processed {_optimizedCount} records");
            Console.WriteLine($"Buffer method processed {_bufferCount} records");
            Console.WriteLine($"KeySetPagination method processed {_keySetPaginationCount} records");

            bool allEqual = (_normalCount == _optimizedCount) && (_optimizedCount == _bufferCount) && (_bufferCount == _keySetPaginationCount);

            if (allEqual) Console.WriteLine("\n -- All methods processed equal records --");
        }


    }
}
