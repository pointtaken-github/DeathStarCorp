namespace Verona.Lib.o365.App.Utility
{
    public static class SpAppNavUtility
    {
        public static string InternalUrl(string url, string spAppSessionId )
        {
            return string.Format("{0}{1}sid={2}", url, url.Contains("?") ? "&" : "?", spAppSessionId);
        }
    }
}
