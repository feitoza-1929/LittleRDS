using LittleRDS.Persistence;
using LittleRDS.PubSub.Publishers;

namespace LittleRDS.PubSub.Subscribers;

public class AOFSub : ISubscriber
{
    private static AOFSub _instance;

    private AOFSub()
    {
        ServerPub.GetInstance().Subscribe(this);
    }

    public static AOFSub GetInstance()
    {
        if (_instance != null)
            return _instance;

        _instance = new AOFSub();
        return _instance;
    }

    public void Update(Event eventData)
    {

        switch (eventData.Name)
        {
            case ServerEvents.SERVER_STARTED:
                AOF.Read((Func<string, object>)eventData.Data);
                break;
            case ServerEvents.SERVER_RECEIVED_COMMAND:
                var arguments = (List<string>)eventData.Data;
                AOF.Write(arguments[0], arguments[1]);
                break;
            default:
                break;
        }   
    }

}