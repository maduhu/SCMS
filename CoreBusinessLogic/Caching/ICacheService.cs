using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.CoreBusinessLogic.Caching
{
    public enum CacheTimeSpan
    {
        FiveMinutes = 0,
        Infinite = 1,
        ThirtySeconds = 2,
        OneMinute = 3,
        TwoMinutes = 4,
        Hour = 5,
        TenMinutes = 6,
        FifteenMinutes = 7,
        ThirtyMinutes = 8
    }

    public interface ICacheService
    {
        void Put(String key, CacheTimeSpan timeSpan, object @object);
        void Remove(string key);
        T Get<T>(string key);
        T Get<T>(string key, CacheTimeSpan timeSpan, Func<T> getItem);
        void Clear();
        void RemoveByPattern(string pattern);
    }
}
