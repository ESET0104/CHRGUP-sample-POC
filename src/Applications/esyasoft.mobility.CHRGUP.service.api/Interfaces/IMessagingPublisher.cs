namespace esyasoft.mobility.CHRGUP.service.api.Interfaces
{
    public interface IMessagingPublisher
    {
        Task PublishAsync<T>(T message);
    }

}
