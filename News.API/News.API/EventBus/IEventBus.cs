using News.API.Model;

namespace News.API.EventBus
{
    public interface IEventBus
    {
        void Publish(NewsModel newsModel);
    }
}