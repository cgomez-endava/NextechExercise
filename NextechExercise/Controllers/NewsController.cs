using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NextechExercise.Client;
using NextechExercise.Models;

namespace NextechExercise.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly ILogger<NewsController> _logger;
        private readonly IMemoryCache _cache;

        public NewsController(ILogger<NewsController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public Response Get(int page, int pageSize, string? filter)
        {
            try
            {
                NewsClient client = new NewsClient(_cache, new HttpClient());
                return client.GetNews(page, pageSize, filter).Result;
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }
    }
}