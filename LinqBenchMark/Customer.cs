using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqBenchMark
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastPurchasedDate { get; set; }

    }
}
