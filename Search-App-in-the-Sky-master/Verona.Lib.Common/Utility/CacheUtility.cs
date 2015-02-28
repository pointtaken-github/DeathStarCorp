//using Common.Setting;

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using Verona.Lib.Common.Setting;

namespace Verona.Lib.Common.Utility
{
    public static class CacheUtility
    {
        public static class Duration
        {
            public static int Default
            {
                get
                {
                    return 0;
                }
            }

            public static int Long
            {
                get
                {
                    return 10;
                }
            }
        }

        private static bool UpdateCacheGroupRegister(string cacheGroupName)
        {
            if (string.IsNullOrEmpty(cacheGroupName))
                return false;

            if (!HasKey("CacheGroupRegister"))
                Create("CacheGroupRegister", new List<string>(), 525600);

            var cacheGroupNames = (List<string>)Read("CacheGroupRegister");

            if (cacheGroupNames.Contains(cacheGroupName))
                return true;

            cacheGroupNames.Add(cacheGroupName);
            return true;
        }

        /// <summary>
        /// Updates the cache group keys.
        /// </summary>
        /// <param name="cacheGroupName">Name of the cache group.</param>
        /// <param name="cacheKeyName">Name of the cache key.</param>
        /// <returns></returns>
        private static bool UpdateCacheGroupKeys(string cacheGroupName, string cacheKeyName)
        {
            if (string.IsNullOrEmpty(cacheGroupName) || string.IsNullOrEmpty(cacheKeyName))
                return false;

            if (!HasKey(cacheGroupName))
            {
                Create(cacheGroupName, new List<string>(), (60 * 24));
                UpdateCacheGroupRegister(cacheGroupName);
            }

            var cacheGroupKeys = (List<string>)Read(cacheGroupName);

            if (cacheGroupKeys.Contains(cacheKeyName))
                return true;

            cacheGroupKeys.Add(cacheKeyName);
            return true;
        }

        public static void ClearCacheRegister()
        {
            if (!HasKey("CacheGroupRegister")) return;
            var cacheGroupRegister = (List<string>)Read("CacheGroupRegister");
            foreach (var cacheGroupName in cacheGroupRegister) ClearCacheGroup(cacheGroupName);
            Delete("CacheGroupRegister");
        }

        /// <summary>
        /// Clears the cache group and clears all its registered cache keys.
        /// </summary>
        /// <param name="cacheGroupName">Name of the cache group.</param>
        public static void ClearCacheGroup(string cacheGroupName)
        {
            if (!HasKey(cacheGroupName)) return;
            var cacheGroupKeys = (List<string>)Read(cacheGroupName);
            foreach (var cacheGroupKey in cacheGroupKeys) Delete(cacheGroupKey);
            Delete(cacheGroupName);
        }

        /// <summary>
        /// Inserts the specified cache key name.
        /// </summary>
        /// <param name="cacheKeyName">Name of the cache key.</param>
        /// <param name="dataElement">The data element.</param>
        /// <param name="durationInMinutes">The duration in minutes.</param>
        /// <returns></returns>
        private static bool Create(string cacheKeyName, object dataElement, int durationInMinutes)
        {
            var success = false;
            if (System.Threading.Monitor.TryEnter(LockName.CacheLockName.Lock))
            {
                try
                {
                    var duration = new TimeSpan(0, 0, durationInMinutes, 0);
                    HttpContext.Current.Cache.Add(cacheKeyName, dataElement, null, System.Web.Caching.Cache.NoAbsoluteExpiration, duration, CacheItemPriority.High, null);
                    success = true;
                }
                catch
                {
                    return success;
                }
                finally
                {
                    System.Threading.Monitor.Exit(LockName.CacheLockName.Lock);
                }
            }
            return success;
        }

        /// <summary>
        /// Inserts the specified cache key name in specifiled cache key group (group can be set to null).
        /// </summary>
        /// <param name="cacheGroupName">Name of the cache group.</param>
        /// <param name="cacheKeyName">Name of the cache key.</param>
        /// <param name="dataElement">The data element.</param>
        /// <param name="durationInMinutes">The duration in minutes.</param>
        /// <returns></returns>
        public static bool Create(string cacheGroupName, string cacheKeyName, object dataElement, int durationInMinutes)
        {
            return Create(cacheKeyName, dataElement, durationInMinutes) && UpdateCacheGroupKeys(cacheGroupName, cacheKeyName);
        }

        /// <summary>
        /// Removes the specified cache key name.
        /// </summary>
        /// <param name="cacheKeyName">Name of the cache key.</param>
        /// <returns></returns>
        public static bool Delete(string cacheKeyName)
        {
            var success = false;

            if (HasKey(cacheKeyName))
            {
                if (System.Threading.Monitor.TryEnter(LockName.CacheLockName.Lock))
                {
                    try
                    {
                        HttpContext.Current.Cache.Remove(cacheKeyName);
                        success = true;
                    }
                    catch
                    {
                        return success;
                    }
                    finally
                    {
                        System.Threading.Monitor.Exit(LockName.CacheLockName.Lock);
                    }
                }
            }
            return success;
        }
        /// <summary>
        /// Gets the specified cache key name.
        /// </summary>
        /// <param name="cacheKeyName">Name of the cache key.</param>
        /// <returns></returns>
        public static object Read(string cacheKeyName)
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache != null)
            {
                var item = HttpContext.Current.Cache[cacheKeyName];
                return item;
            }

            return null;
        }
        /// <summary>
        /// Determines whether the cache has key.
        /// </summary>
        /// <param name="cacheKeyName">Name of the cache key.</param>
        /// <returns>
        ///   <c>true</c> if the specified cache key name has key; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasKey(string cacheKeyName)
        {
            return Read(cacheKeyName) != null;
        }

        public static string GetHostSpecificKeyName(string label)
        {
            return string.Format("{0}_{1}", HttpContext.Current.Request.Url.Host, label);
        }
    }
}
