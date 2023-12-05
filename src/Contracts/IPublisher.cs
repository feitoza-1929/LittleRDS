public interface IPublisher
{
    void Emit(Event eventData);
    void Subscribe(ISubscriber subscriber);
    bool Remove(ISubscriber subscriber);
}