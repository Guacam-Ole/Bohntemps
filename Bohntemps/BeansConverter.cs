using Bohntemps.Models;

using BohnTemps.BeansApi;
using BohnTemps.Mastodon;

namespace Bohntemps
{
    public class BeansConverter
    {
        private readonly Schedule _schedule;
        private readonly Toot _toot;
        private readonly Communications _communications;

        public BeansConverter(Schedule schedule, Toot toot, Communications communications)
        {
            _schedule = schedule;
            _toot = toot;
            _communications = communications;
        }

        public async Task RetrieveAndSend()
        {
            var todaysShows = await _schedule.GetScheduleFor(Helpers.GetTodayUtc());
            await SendTootsWithinTime(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), todaysShows.Data);
        }

        private string GetLocalTimeString(DateTime dateTime)
        {
            var localTime = dateTime.ToLocalTime();
            return localTime.ToString("HH:mm");
        }

        public string CreateTootFromElement(ChannelGroup group, ScheduleElement element, string? talent)
        {
            string toot = "Nicht verpassen:\n";
            if (element.TimeEnd.HasValue)
            {
                toot += $"von {GetLocalTimeString(element.TimeStart)} bis {GetLocalTimeString(element.TimeEnd.Value)} Uhr ";
            }
            else
            {
                toot += $"um {GetLocalTimeString(element.TimeStart)} Uhr ";
            }

            toot += $"streamt {talent ?? "RBTV"} ";
            if (element.Bohnen.Count > 0)
            {
                toot += $"(mit {string.Join(',', element.Bohnen.Select(q => q.Name))}) ";
            }
            if (!string.IsNullOrWhiteSpace(element.Game))
            {
                toot += $" ('{element.Game}')";
            }
            toot += $":\n\n";
            if (talent == null)
            {
                toot += element.Title; // RB-Channel
            }
            else
            {
                toot += element.Topic;  // Bohne (Title="xxx streamt")
            }
            toot += "\n\n";

             foreach (var channel in group.Channels)
            {
                toot += $"{channel.ServiceType}:🎮 {channel.Url}\n";
            }
            return toot;
        }

        private async Task SendTootsWithinTime(DateTime from, DateTime until, List<ChannelGroupSchedule> schedule)
        {
            var elementsToShow = new List<BohnContainer>();

            foreach (var scheduleHeader in schedule)
            {
                var talent = scheduleHeader.ChannelGroup.Type != "talent" ? null : scheduleHeader.ChannelGroup.Name;
                foreach (var scheduleElement in scheduleHeader.Schedule)
                {
                    elementsToShow.Add(new BohnContainer { Talent = talent, ChannelGroup = scheduleHeader.ChannelGroup, Elements = new List<ScheduleElement>() });
                    foreach (var stream in scheduleElement.Elements)
                    {
                        if (stream.TimeStart > until || stream.TimeStart < from) continue;
                        elementsToShow.First(q => q.Talent == talent).Elements.Add(stream);
                    }
                }
            }

            foreach (var elementToShow in elementsToShow)
            {
                if (elementToShow.Elements.Count == 0) continue;
                foreach (var singleStream in elementToShow.Elements)
                {
                    var toot = CreateTootFromElement( elementToShow.ChannelGroup, singleStream, elementToShow.Talent);
                    Stream? imageStream = null;
                    if (!string.IsNullOrWhiteSpace(singleStream.EpisodeImage))
                    {
                        imageStream = await _communications.DownloadImage(singleStream.EpisodeImage);
                    }
                    await _toot.SendToot(toot, imageStream);
                }
            }
        }
    }
}