using Microsoft.Extensions.Caching.Memory;
using NextechExercise.Models;
using System.Collections.Concurrent;
using System.Text.Json;

namespace NextechExercise.Client
{
    public class NewsClient
    {
        private readonly IMemoryCache _cache;
        private readonly string _newsCacheKey = "newsList";
        private readonly HttpClient _client;

        public NewsClient(IMemoryCache cache, HttpClient client)
        {
            _cache = cache;
            _client = client;
        }

        public async Task<Response> GetNews(int page, int pageSize, string? filter)
        {
            News? news;

            Stream newStoriesJson = await _client.GetStreamAsync("https://hacker-news.firebaseio.com/v0/newstories.json");
            IEnumerable<int> newStories = await JsonSerializer.DeserializeAsync<IEnumerable<int>>(newStoriesJson);

            if (!_cache.TryGetValue(_newsCacheKey, out ConcurrentBag<News>? currentNews))
                currentNews = new ConcurrentBag<News>();

            if (newStories != null)
            {

                Parallel.ForEach(newStories.Where(x => !currentNews.Any(y => y.id == x)), async id =>
                {
                    Stream newsJson = await _client.GetStreamAsync("https://hacker-news.firebaseio.com/v0/item/" + id + ".json");
                    if (newsJson != null)
                    {
                        news = await JsonSerializer.DeserializeAsync<News>(newsJson);
                        if (!string.IsNullOrWhiteSpace(news.url))
                        {
                            currentNews.Add(news);
                        }
                    }
                });
            }

            _cache.Set(_newsCacheKey, currentNews);

            IEnumerable<News> responseNews = string.IsNullOrWhiteSpace(filter) ? currentNews : currentNews.Where(x => x.title.Contains(filter));
            Response response = new Response();
            response.totalNews = responseNews.Count();
            if (responseNews.Count() > (page - 1) * pageSize)
            {
                response.news = responseNews.OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize);
                response.pageNumber = page;
            }
            else
            {
                response.news = responseNews.OrderBy(x => x.id).TakeLast(pageSize);
                response.pageNumber = (responseNews.Count() / pageSize) + 1;
            }

            return response;
        }
    }
}
