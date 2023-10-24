namespace Task_1.Services
{
    public interface IRedisCaching
    {
        T GetData<T>(string key);

        bool SetData<T>(string key, T vaue, DateTimeOffset expirationTime);

        object RemoveData(string key);
    }
}
