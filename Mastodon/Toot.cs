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
            _secrets = JsonConvert.DeserializeObject<Secrets>(secrets)!;
            _logger = logger;
        }

        private async Task<string?> UploadMeda(MastodonClient client, Stream fileStream, string filename, string description)
        {
            if (fileStream == null) return null;
            _logger.LogDebug("Uploading file '{filename}' with {size} bytes '{description}'", filename, fileStream.Length, description);
            try
            {
                var attachment = await client.UploadMedia(fileStream, filename, description);
                return attachment.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error uploading file");
                return null;
            }
        }

        public async Task<Status> SendToot(string content, string? replyTo, Stream? media)
        {
            _logger.LogDebug("Sending Toot");
            var client = GetServiceClient();
            string? attachmentId = null;
            if (media != null) attachmentId = await UploadMeda(client, media, "preview.png", "Vorschaubild zum Kanal");
            if (attachmentId != null)
            {
                return await client.PublishStatus(content, Visibility.Public, replyTo, mediaIds: new List<string> { attachmentId });
            }
            else
            {
                return await client.PublishStatus(content, Visibility.Public, replyTo);
            }
        }

        private MastodonClient GetServiceClient()
        {
            return new MastodonClient(_secrets.Instance, _secrets.AccessToken);
        }
    }
}