﻿using Bohntemps.Models;

namespace Bohntemps
{
    public class BohnContainer
    {
        public string? Talent { get; set; }
        public ChannelGroup ChannelGroup { get; set; } = new ChannelGroup();
        public List<ScheduleElement> Elements { get; set; }=new List<ScheduleElement>();
    }
}