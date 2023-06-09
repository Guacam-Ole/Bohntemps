namespace Bohntemps.Models
{
    public class ChannelGroupSchedule
    {
        public ChannelGroup ChannelGroup { get; set; } = new ChannelGroup();
        public List<ScheduleItem> Schedule = new List<ScheduleItem>();
    }
}