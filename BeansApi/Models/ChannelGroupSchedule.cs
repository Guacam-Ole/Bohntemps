using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bohntemps.Models
{
    public class ChannelGroupSchedule
    {
        public ChannelGroup ChannelGroup { get; set; }
        public List<ScheduleItem> Schedule = new List<ScheduleItem>();

    }
}
