public class AOFSub : ISubscriber
{
    public AOFSub(ServerPub serverPub)
    {
        serverPub.Subscribe(this);
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