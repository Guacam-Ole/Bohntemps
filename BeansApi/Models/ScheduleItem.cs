using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohntemps.Models
{
    public class ScheduleItem
    {
        public DateTime Date { get; set; }
        public List<ScheduleElement> Elements { get; set; }
    }
}
