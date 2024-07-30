using HackerNewsStory.Interface;

namespace HackerNewsStory.Repository
{
    public class HackerNewsRepository : IHackerNewsRepository
    {
        private static HttpClient client = new HttpClient();
        public HackerNewsRepository() { }

        public async Task<HttpResponseMessage> GetTopStoryList()
        {
            return await client.GetAsync("https://hacker-news.firebaseio.com/v0/newstories.json");
        }

        public async Task<HttpResponseMessage> GetStoryById(int id)
        {
            return await client.GetAsync(string.Format("https://hacker-news.firebaseio.com/v0/item/{0}.json", id));
        }
    }
}
