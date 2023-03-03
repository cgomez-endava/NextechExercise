using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using NextechExercise.Model;

namespace NextechExercise.Test
{
    [TestClass]
    public class NewsClientTest
    {
        [TestMethod]
        public void GetNews_InitialPageNoFilter()
        {
            // Setup
            var mockClient = new Mock<HttpClient>();
                        
            mockClient.Setup(x => x.GetStreamAsync("https://hacker-news.firebaseio.com/v0/newstories.json")).Returns(Task.FromResult(GetSampleNewsListStream()));
            for (int i =1; i <= 12; i++)
            {
                mockClient.Setup(x => x.GetStreamAsync("https://hacker-news.firebaseio.com/v0/item/" + i + ".json")).Returns(Task.FromResult(GetSampleNewsStream(i)));
            }

            var mockCache = new Mock<IMemoryCache>();
            mockCache.Setup(x => x.TryGetValue(It.IsAny<string>, out It.IsAny<List<News>?>))
        }

        private Stream GetSampleNewsListStream()
        {
            var streamNewsList = new MemoryStream();
            var writer = new StreamWriter(streamNewsList);
            writer.Write("[ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]");
            writer.Flush();
            streamNewsList.Position = 0;
            return streamNewsList;
        }

        private Stream GetSampleNewsStream(int id)
        {
            var streamNewsList = new MemoryStream();
            var writer = new StreamWriter(streamNewsList);
            writer.Write(@"{  ""by"" : ""aa"",  ""descendants"" : 71, ""id"" : " + id + @", ""kids"" : [ 9224 ], ""score"" : 104,  ""time"" : 1175714200, ""title"" : ""title id " + id + @""", ""type"" : ""story"", ""url"" : ""http://www.getdropbox.com/u/2/screencast.html""");
            writer.Flush();
            streamNewsList.Position = 0;
            return streamNewsList;
        }
    }
}
