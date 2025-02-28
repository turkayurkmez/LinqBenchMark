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
        private TestDbContext _context;

        private const int readCount = 1_000_000;





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
            var customers = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Age > 35)
                .OrderBy(c => c.LastPurchasedDate)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name

                })
                .ToListAsync();

            foreach (var customer in customers)
            {
                _ = customer.Name.Length;
            }
        }


      
        //public async Task WithOptimizedYield()
        //{

        //    //await foreach (var customer in Optimized())
        //    //{
        //    //    _ = customer.Name.Length;
        //    //}
        //}



        [Benchmark]
        public async Task Optimized()
        {
            var customers = _context.Customers
                                    .AsNoTracking()
                                    .Where(c => c.Age > 35)
                                    .OrderBy(c => c.LastPurchasedDate)
                                    .Select(c => new CustomerDto
                                    {
                                        Id = c.Id,
                                        Name = c.Name
                                    })
                                    .AsAsyncEnumerable();

            await foreach (var customer in customers)
            {
               _ = customer.Name.Length;
              
            }


        }


        [Benchmark]
        public async Task WithBufferAsync()
        {
            const int bufferCount = 100;
            await foreach (var batch in _context.Customers
                                                .AsNoTracking()
                                                .Where(c => c.Age > 35)
                                                .OrderBy(c => c.LastPurchasedDate)
                                                .Select(c => new CustomerDto { Id = c.Id, Name = c.Name })
                                                .AsAsyncEnumerable()
                                                .Buffer(bufferCount))
            {
                foreach (var customer in batch)
                {
                    _ = customer.Name.Length;
                }
            }

        }

        //private async Task<IEnumerable<CustomerDto>> withBufferAsync()
        //{
        //    var result = new List<CustomerDto>();
        //    const int bufferCount = 100;

        //    await foreach (var batch in _context.Customers
        //          .AsNoTracking()
        //          .Where(c => c.Age > 35)
        //          .OrderBy(c => c.LastPurchasedDate)
        //          .Select(c => new CustomerDto { Id = c.Id, Name = c.Name })
        //          .AsAsyncEnumerable()
        //          .Buffer(bufferCount))
        //    {
        //        result.AddRange(batch);



        //    }

        //    return result;




        //}

        [Benchmark]
        public async Task WithKeySetPagination()
        {
            const int pageSize = 100;
            var result = new List<CustomerDto>();
            int lastId = 0;
            while (true)
            {
                var batch = await _context.Customers
                    .AsNoTracking()
                    .Where(c => c.Id > lastId && c.Age > 35)
                    .OrderBy(c => c.LastPurchasedDate)
                    .Take(pageSize)
                    .Select(c => new CustomerDto
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToListAsync();


                if (!batch.Any())
                    break;

                foreach (var customer in batch)
                {
                    _ = customer.Name.Length;
                    // Gerçek senaryoda burada veri işleme yapılırdı
                    // Örn: processCustomer(customer);
                }
                lastId = batch.Last().Id;
            }
        }


    }
}
