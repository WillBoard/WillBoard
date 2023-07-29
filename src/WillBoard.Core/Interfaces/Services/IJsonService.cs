namespace WillBoard.Core.Interfaces.Services
{
    public interface IJsonService
    {
        string SerializeData(object data);
        string SerializeError(object error);
    }
}