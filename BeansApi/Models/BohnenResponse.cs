using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohntemps.Models
{
    public class BohnenResponse<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; }
    }
}
