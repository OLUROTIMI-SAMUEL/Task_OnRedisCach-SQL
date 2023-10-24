
using StackExchange.Redis;
using System.Text.Json;

namespace Task_1.Services
{
    public class RedisCaching : IRedisCaching
    {
       private IDatabase _cacheDb;
        
        public RedisCaching()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");   //For development enviroment

            _cacheDb = redis.GetDatabase();
        }
        
        public T GetData<T>(string key)
        {
           var value = _cacheDb.StringGet(key);
            if(!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T> (value);

            return default;
        }

        public object RemoveData(string key)
        {
           var _exist = _cacheDb.KeyExists(key);

            if (_exist) 
                return _cacheDb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T vaue, DateTimeOffset expirationTime)
        {
            var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(vaue), expiryTime);
        }
    }
}
