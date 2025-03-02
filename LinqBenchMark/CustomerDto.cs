using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqBenchMark
{
    public record CustomerDto(int Id, string Name,  DateTime LastPurchasedDate);
   
}
