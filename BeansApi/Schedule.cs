using Bohntemps.Models;

namespace BohnTemps.BeansApi
{
    public class Schedule
    {
        public static async Task<BohnenResponse<ChannelGroupSchedule>> GetScheduleFor(DateTime start, DateTime end)
        {
            var startDay = start.GetTimestamp();
            var endDay = end.GetTimestamp();

            var schedule = await Communications.GetResponse<BohnenResponse<ChannelGroupSchedule>>("schedule",
                new Dictionary<string, object> { { "startDay", startDay }, { "endDay", endDay } });
            return schedule;
        }
    }
}