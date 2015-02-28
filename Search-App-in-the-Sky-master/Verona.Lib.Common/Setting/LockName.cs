using System;

namespace Verona.Lib.Common.Setting
{
    public static class LockName
    {
        /// <summary>
        /// Metode som hindrer flere å skrive til samme cachekey på en gang
        /// </summary>
        public static class CacheLockName
        {
            private static readonly string LockString = Cache.CacheLockName;

            public static String Lock
            {
                get { return LockString; }
            }
        }

        /// <summary>
        /// Metode som hindrer flere å skrive til samme sessionkey på en gang (hvilket sjeldent eller egentlig aldri skjer ...)
        /// </summary>
        public static class SessionLockName
        {
            private static readonly string LockString = Session.SessionLockName;

            public static String Lock
            {
                get { return LockString; }
            }
        }
    }
}
