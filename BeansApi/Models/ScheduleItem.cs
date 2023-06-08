namespace Bohntemps.Models
{
    public class ScheduleItem
    {
        public DateTime Date { get; set; }
        public List<ScheduleElement> Elements { get; set; } = new List<ScheduleElement>();
    }
}