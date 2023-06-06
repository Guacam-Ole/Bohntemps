
namespace Bohntemps.Models
{
    public class ChannelGroup
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public List<Channel> Channels { get; set; }
    }
}