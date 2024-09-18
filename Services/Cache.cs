using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public static class Cache
    {
        private static Timer cleanupTimer = new Timer() { AutoReset = true, Enabled = true, Interval = 60000 };
        private static readonly Dictionary<string, CacheItem> internalCache = new Dictionary<string, CacheItem>();

        static Cache()
        {
            cleanupTimer.Elapsed += Clean;
            cleanupTimer.Start();
        }

        private static void Clean(object sender, ElapsedEventArgs e)
        {
            internalCache.Keys.ToList().ForEach(x => { try { if (internalCache[x].ExpireTime <= e.SignalTime) { Remove(x); } } catch (Exception) { /*swallow it*/ } });
        }

        public static void ClearCache()
        {
            internalCache.Keys.ToList().ForEach(x => { try { Remove(x); } catch (Exception) { /*swallow it*/ } });
        }

        public static T Get<T>(string key)
        {
            if (internalCache.ContainsKey(key) && internalCache[key].ExpireTime > DateTime.Now)
            {
                return (T)internalCache[key].Item;
            }
            return default(T);
        }

        public static void Set(string key, object item, int expiresMinutes = 60)
        {
            Remove(key);

            internalCache.Add(key, new CacheItem(item, expiresMinutes));
        }

        public static void Remove(string key)
        {
            if (internalCache.ContainsKey(key))
            {
                internalCache.Remove(key);
            }
        }

        private struct CacheItem
        {
            public CacheItem(object item, int expiresMinutes)
                : this()
            {
                Item = item;
                ExpireTime = DateTime.Now.AddMinutes(expiresMinutes);
            }

            public object Item { get; private set; }
            public DateTime ExpireTime { get; private set; }
        }
    }
}
