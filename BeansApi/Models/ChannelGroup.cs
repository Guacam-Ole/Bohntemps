
namespace Bohntemps.Models
{
    public class ChannelGroup
    {
        public string Type { get; set; } = "unknown";
        public string Name { get; set; } = string.Empty;
        public List<Channel> Channels { get; set; }=new List<Channel>();
    }
}