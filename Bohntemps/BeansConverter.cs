﻿using Bohntemps.Models;

using BohnTemps.BeansApi;
using BohnTemps.Mastodon;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Bohntemps
{
    public class BeansConverter
    {
        private readonly Schedule _schedule;
        private readonly Toot _toot;
        private readonly Communications _communications;
        private readonly ILogger<BeansConverter> _logger;
        private readonly Config _config;
        private const int _maxLength = 490;

        public BeansConverter(Schedule schedule, Toot toot, Communications communications, ILogger<BeansConverter> logger)
        {
            _schedule = schedule;
            _toot = toot;
            _communications = communications;
            _logger = logger;
            var config = File.ReadAllText("./config.json");
            _config = JsonConvert.DeserializeObject<Config>(config)!;
        }

        public async Task RetrieveAndSend()
        {
            try
            {
                _logger.LogDebug("Retrieving Data");
                var today = Helpers.GetTodayUtc();
                var todaysShows = await _schedule.GetScheduleFor(today, today.Add(_config.ScheduleTimeSpan));
                _logger.LogDebug($"Retrieved {todaysShows.Data.Count()} items");
                await SendTootsWithinTime(DateTime.UtcNow, DateTime.UtcNow.Add(_config.PostTimeSpan), todaysShows.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrent wrong");
                throw;
            }
        }

        private string GetLocalTimeString(DateTime dateTime)
        {
            var localTime = dateTime.ToLocalTime();
            return localTime.ToString("HH:mm");
        }

        public string CreateTootFromElement(ChannelGroup group, ScheduleElement element, string? talent)
        {
            string toot = string.Empty;
            if (element.TimeEnd.HasValue && !element.OpenEnd)
            {
                toot += $"von {GetLocalTimeString(element.TimeStart)} bis {GetLocalTimeString(element.TimeEnd.Value)} Uhr ";
            }
            else
            {
                toot += $"ab {GetLocalTimeString(element.TimeStart)} Uhr ";
            }

            toot += $"streamt {talent ?? "RBTV"}: \n\n";

            string title = talent == null ? element.Title : element.Topic;

            toot += title;
            if (!string.IsNullOrWhiteSpace(element.Game) && string.Compare(element.Game, title, true) != 0)
            {
                toot += $" ({element.Game})";
            }

            toot += "\n\n";

            foreach (var channel in group.Channels)
            {
                toot += $"📺 {channel.ServiceType}: {channel.Url}\n";
            }

            toot += "\n\n\n#RBTV #RocketBeans #RocketBeansTV ";
            if (!string.IsNullOrWhiteSpace(element.Game))
            {
                toot += element.Game.HashTagFromGame();
            }

            if (talent != null)
            {
                toot += "#RBTV_";
                toot += string.Concat(talent.Where(c => !char.IsWhiteSpace(c)));
                toot += " ";
            }

            if (element.Bohnen.Count > 0)
            {
                var allBohnen = element.Bohnen.Select(q => q.Name).ToArray();

                if (element.Bohnen.Count == 1)
                {
                    toot += $"\n\n(mit dabei ist {allBohnen.First()}) ";
                }
                else
                {
                    var allButOne = allBohnen[..^1];
                    toot += $"\n\n(mit dabei sind {string.Join(", ", allButOne)} und {allBohnen.Last()}) ";
                }
            }
            toot += "🫘⌛";
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
                        _logger.LogDebug($"Added {stream.Game} for {talent} ");
                        elementsToShow.First(q => q.Talent == talent).Elements.Add(stream);
                    }
                }
            }

            foreach (var elementToShow in elementsToShow)
            {
                if (elementToShow.Elements.Count == 0) continue;
                foreach (var singleStream in elementToShow.Elements)
                {
                    try
                    {
                        var toot = CreateTootFromElement(elementToShow.ChannelGroup, singleStream, elementToShow.Talent);
                        Stream? imageStream = null;
                        if (!string.IsNullOrWhiteSpace(singleStream.EpisodeImage))
                        {
                            imageStream = await Communications.DownloadImage(singleStream.EpisodeImage);
                        }

                        string? replyTo = null;
                        if (_config.Dummy) return;
                        while (toot.Length > _maxLength)
                        {
                            replyTo = (await _toot.SendToot(toot[.._maxLength], replyTo, imageStream)).Id;
                            toot = toot[_maxLength..];
                            imageStream = null; // image just in first toot
                        }
                        await _toot.SendToot(toot, replyTo, imageStream);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "error sending toot");
                        throw;
                    }
                }
            }
        }
    }
}