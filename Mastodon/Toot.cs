using BohnTemps.BeansApi;

using Mastodon;

using Mastonet;
using Mastonet.Entities;

using Newtonsoft.Json;

namespace BohnTemps.Mastodon
{
    public class Toot
    {
        private readonly Secrets _secrets;

        public Toot()
        {
            var secrets = File.ReadAllText("./secrets.json");
            _secrets = JsonConvert.DeserializeObject<Secrets>(secrets);
        }

        private async Task<string> UploadMeda(MastodonClient client, Stream fileStream, string filename, string description)
        {
            if (fileStream == null) return null;
            var attachment=await client.UploadMedia(fileStream, filename, description);
            return attachment.Id;
        }


        public async Task SendToot(string content, Stream? media)
        {
            content = "@guacamole@chaos.social \n\n\n" + content;
            //return;
            var client=GetServiceClient();
            string? attachmentId = null;
            if (media != null) attachmentId = await UploadMeda(client, media, "preview.png","Vorschaubild zum Kanal");
            if (attachmentId!=null)
            {
                await client.PublishStatus(content, Visibility.Direct,  mediaIds: new List<string> { attachmentId });


            } else
            {
                await client.PublishStatus(content, Visibility.Direct);
            }
        }

        private MastodonClient GetServiceClient()
        {
            return new MastodonClient(_secrets.Instance, _secrets.AccessToken);
        }
    }
}