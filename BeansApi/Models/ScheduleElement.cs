using Bohntemps.BeansApi.Models;

namespace Bohntemps.Models
{
    public class ScheduleElement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Topic { get; set; }
        public string Game { get; set; }
        public int ShowId { get; set; }
        public int EpisodeId { get; set; }
        public string EpisodeImage { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public List<Bohne> Bohnen { get; set; }
        public string Type { get; set; }
    }
}