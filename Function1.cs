using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace GetNews
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 3 * * *")]TimerInfo myTimer, ILogger log)
        {
            var url = "https://www.autonomosyemprendedor.es/rss";
            using var reader = XmlReader.Create(url);
            var feed = SyndicationFeed.Load(reader);
            var post = feed.Items.FirstOrDefault();

            PostRequest(post.Id, post.Title.Text, post.Summary.Text);
        }

        async static void PostRequest(string url, string title, string description)
        {
            NewsVM news = new NewsVM()
            {
                Description = description,
                Url = url,
                Title = title
            };
            var client = new HttpClient();
            await client.PostAsync("http://myadvicewebapi.azurewebsites.net/api/News", new StringContent(JsonConvert.SerializeObject(news), Encoding.UTF8, "application/json"));


        }
    }
}
