namespace HackerNewsStory.Interface
{
    public interface IHackerNewsRepository
    {
        Task<HttpResponseMessage> GetTopStoryList();
        Task<HttpResponseMessage> GetStoryById(int id);
    }
}
