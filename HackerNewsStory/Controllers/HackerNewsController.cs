using HackerNewsStory.Interface;
using HackerNewsStory.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HackerNewsStory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsRepository _hackernewsrepo;
        private IMemoryCache _cache;
        public HackerNewsController(IMemoryCache cache, IHackerNewsRepository hackernewsrepo)
        {
           this._cache = cache;
           this._hackernewsrepo = hackernewsrepo;
        }

        // GET: api/<HackerNews>
        [HttpGet]
        public async Task<IActionResult> GetNewsStory(string? searchItem)
        {
            List<Story> allstories = new List<Story>();
            List<Story> filteredstories = new List<Story>();
            try
            {
                var response = await _hackernewsrepo.GetTopStoryList();
                if (response.IsSuccessStatusCode)
                {
                    var storiesResponse = response.Content.ReadAsStringAsync().Result;
                    var storyIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);

                    if (storyIds?.Count > 0)
                    {
                        var tasks = storyIds.Select(GetStorybyID);
                        allstories = (List<Story>)(await Task.WhenAll(tasks)).ToList();
                        filteredstories = allstories.Where(x => x != null).ToList();
                        if (!String.IsNullOrEmpty(searchItem))
                        {
                            var search = searchItem.ToLower();
                            filteredstories = filteredstories.Where(s => s.title.ToLower().IndexOf(search) > -1 || s.by.ToLower().IndexOf(search) > -1).ToList();
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "exception occured in API");
            }

            return Ok(filteredstories);
        }

        // GET api/<HackerNews>/5
        [HttpGet("{id}")]
        public async Task<Story> GetStorybyID(int id)
        {
            Story? story = new Story();
            var response = await _hackernewsrepo.GetStoryById(id);
            if (response.IsSuccessStatusCode)
            {
                var storyResponse = response.Content.ReadAsStringAsync().Result;
                story = JsonConvert.DeserializeObject<Story>(storyResponse);
            }

            if (story != null && !string.IsNullOrWhiteSpace(story.url))
            {
                return _cache.GetOrCreate(id, entry => story);
            }
            else { return null; }
        }

       
    }
}
