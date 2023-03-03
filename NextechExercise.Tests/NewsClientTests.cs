using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Moq;
using Moq.Protected;
using NextechExercise.Client;
using NextechExercise.Models;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NextechExercise.Tests
{
    public class NewsClientTests
    {
        [Fact]
        public void GetNews_InitialPageNoFilter()
        {
            // Setup
            var mockClient = new Mock<HttpClient>();

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    HttpResponseMessage response = new HttpResponseMessage();

                    HttpResponseMessage httpResponse;
                    string url = request.RequestUri.ToString();

                    if (url.Equals("https://hacker-news.firebaseio.com/v0/newstories.json"))
                    {
                        response = new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StreamContent(GetSampleNewsListStream())
                        };
                    }
                    else
                    {
                        int id = int.Parse(Regex.Match(url, "[0-9]+").Value);
                        response = new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StreamContent(GetSampleNewsStream(id))
                        };
                    }

                    return response;
                });

            NewsClient client = new NewsClient(new MemoryCache(new MemoryCacheOptions()), new HttpClient(httpMessageHandler.Object));

            // Act
            Response response = client.GetNews(1, 10, null).Result;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(12, response.totalNews);
            Assert.Equal(1, response.pageNumber);
            Assert.Equal(10, response.news.Count());
        }

        private Stream GetSampleNewsListStream()
        {
            var streamNewsList = new MemoryStream();
            var writer = new StreamWriter(streamNewsList);
            writer.Write("[ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 ]");
            writer.Flush();
            streamNewsList.Position = 0;
            return streamNewsList;
        }

        private Stream GetSampleNewsStream(int id)
        {
            var streamNews = new MemoryStream();
            var writer = new StreamWriter(streamNews);
            string json = @"{  ""by"" : ""aa"",  ""descendants"" : 71, ""id"" : " + id + @", ""kids"" : [ 9224 ], ""score"" : 104,  ""time"" : 1175714200, ""title"" : ""title id " + id + @""", ""type"" : ""story"", ""url"" : ""http://www.getdropbox.com/u/2/screencast.html"" }";
            writer.Write(json);
            writer.Flush();
            streamNews.Position = 0;
            return streamNews;
        }
    }
}