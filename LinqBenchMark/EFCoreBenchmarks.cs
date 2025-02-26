using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;
//using System.Linq.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LinqBenchMark
{
    [MemoryDiagnoser]
    public class EFCoreBenchmarks
    {
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
                    LastPurchasedDate = DateTime.Now.AddDays(-i)
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


        [Benchmark]
        public void WithOptimizedYield()
        {
            
            var result = Optimized();
            foreach (var customer in result)
            {
                _ = customer.Name.Length;
            }
        }



      
        public IEnumerable<CustomerDto> Optimized()
        {
            foreach (var customer in _context.Customers
                                                    .AsNoTracking()
                                                    .Where(c => c.Age > 35)
                                                    .OrderBy(c => c.LastPurchasedDate)
                                                    .Select(c => new CustomerDto
                                                    {
                                                        Id = c.Id,
                                                        Name = c.Name
                                                    })
                                                    )
            {
                yield return customer;
            }
        }


        [Benchmark]
        public async Task withBufferAsync()
        {
            const int pageSize = 100;
            var result = new List<CustomerDto>();
            await foreach (var batch in _context.Customers
                .AsNoTracking()
                .Where(c => c.Age>35)
                .OrderBy(c => c.LastPurchasedDate)
                .Select(c => new CustomerDto { Id = c.Id, Name = c.Name })
                .AsAsyncEnumerable()                
                .BufferAsync(pageSize))
            {
                result.AddRange(batch);
            }


        }

        [Benchmark]
        public async Task WithSkipTake()
        {
            const int pageSize = 100;
            var result = new List<CustomerDto>();
            //int skip = 0;
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

                result.AddRange(batch);
                lastId = batch.Last().Id;
            }
        }


    }
}
