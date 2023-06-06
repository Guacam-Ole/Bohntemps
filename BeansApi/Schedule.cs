using Bohntemps.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BohnTemps.BeansApi
{
    public class Schedule
    {
        private readonly Communications _communications;

        public Schedule(Communications communications)
        {
            _communications = communications;
        }
        public async Task<BohnenResponse<ChannelGroupSchedule>> GetScheduleFor(DateTime day)
        {
            var startDay = day.GetTimestamp();
            var endDay = day.AddDays(1).AddSeconds(-1).GetTimestamp();

            var schedule = await _communications.GetResponse<BohnenResponse<ChannelGroupSchedule>>("schedule", new Dictionary<string, object> { { "startDay", startDay }, { "endDay", endDay } });
            return schedule;
        }
    }
}
