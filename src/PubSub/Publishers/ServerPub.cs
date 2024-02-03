namespace LittleRDS.PubSub.Publishers;

public class ServerPub : IPublisher
{
    private static ServerPub _instance;
    private List<ISubscriber> _subscribers = new();

    private ServerPub(){}

    public static ServerPub GetInstance()
    {
        if (_instance != null)
            return _instance;

        _instance = new ServerPub();
        return _instance;
    }

    public void Emit(Event eventData)
    {
        foreach (var sub in _subscribers)
        {
            sub.Update(eventData);
        }
    }
 
    public bool Remove(ISubscriber subscriber)
    {
        return _subscribers.Remove(subscriber);
    }

    public void Subscribe(ISubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }
}