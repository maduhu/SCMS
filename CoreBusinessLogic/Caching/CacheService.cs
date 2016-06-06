using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;

namespace SCMS.CoreBusinessLogic.Caching
{
    public class CacheService : ICacheService
    {
        protected ObjectCache CacheStore
        {
            get
            {
                return MemoryCache.Default;
            }
        }


        public void Put(String key, CacheTimeSpan timeSpan, object @object)
        {
            Remove(key);
            if (@object.IsNotNull())
                CacheStore.Add(
                    new CacheItem(key, @object),
                    new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + GetTimeSpan(timeSpan) });
        }

        public void Remove(string key)
        {
            CacheStore.Remove(key);
        }

        public T Get<T>(string key)
        {
            return Get<T>(key, CacheTimeSpan.FiveMinutes, null);
        }

        public T Get<T>(string key, CacheTimeSpan timeSpan, Func<T> getItem)
        {
            var cacheItem = CacheStore.Get(key);
            if (cacheItem.IsNull() && getItem.IsNotNull())
            {
                cacheItem = getItem();
                if (cacheItem != null)
                    Put(key, timeSpan, cacheItem);
            }

            return (T)cacheItem;
        }


        private TimeSpan GetTimeSpan(CacheTimeSpan cacheTimeSpan)
        {
            int minutes = 10;
            switch (cacheTimeSpan)
            {
                case CacheTimeSpan.Infinite:
                    minutes = 2 * 24 * 60;
                    break;
                case CacheTimeSpan.FiveMinutes:
                    minutes = 5;
                    break;
                case CacheTimeSpan.ThirtySeconds:
                    return TimeSpan.FromSeconds(30);
                case CacheTimeSpan.OneMinute:
                    minutes = 1;
                    break;
                case CacheTimeSpan.TwoMinutes:
                    minutes = 2;
                    break;
                case CacheTimeSpan.Hour:
                    minutes = 60;
                    break;
                case CacheTimeSpan.TenMinutes:
                    minutes = 10;
                    break;
                case CacheTimeSpan.FifteenMinutes:
                    minutes = 15;
                    break;
                case CacheTimeSpan.ThirtyMinutes:
                    minutes = 30;
                    break;
            }
            return TimeSpan.FromMinutes(minutes);
        }

        public void Clear()
        {
            foreach (var item in CacheStore)
                Remove(item.Key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            (from item in CacheStore where regex.IsMatch(item.Key) select item.Key)
                .ForEach(Remove);
        }
    }
}
