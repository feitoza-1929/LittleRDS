public class ServerPub : IPublisher
{
    private static List<ISubscriber> _subscribers = new();
    
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