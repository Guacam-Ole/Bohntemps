using BohnTemps.BeansApi;

using Mastodon;

using Mastonet;
using Mastonet.Entities;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace BohnTemps.Mastodon
{
    public class Toot
    {
        private readonly Secrets _secrets;
        private readonly ILogger<Toot> _logger;

        public Toot(ILogger<Toot> logger)
        {
            var secrets = File.ReadAllText("./secrets.json");
            _secrets = JsonConvert.DeserializeObject<Secrets>(secrets);
            _logger = logger;
        }

        private async Task<string> UploadMeda(MastodonClient client, Stream fileStream, string filename, string description)
        {
            _logger.LogDebug("Uploading Image");
            if (fileStream == null) return null;
            var attachment=await client.UploadMedia(fileStream, filename, description);
            return attachment.Id;
        }


        public async Task SendToot(string content, Stream? media)
        {

            _logger.LogDebug("Sending Toot");
            var client=GetServiceClient();
            string? attachmentId = null;
            if (media != null) attachmentId = await UploadMeda(client, media, "preview.png","Vorschaubild zum Kanal");
            if (attachmentId!=null)
            {
                await client.PublishStatus(content, Visibility.Public,  mediaIds: new List<string> { attachmentId });


            } else
            {
                await client.PublishStatus(content, Visibility.Public);
            }
            _logger.LogDebug("Toot sent");

        }

        private MastodonClient GetServiceClient()
        {
            return new MastodonClient(_secrets.Instance, _secrets.AccessToken);
        }
    }
}