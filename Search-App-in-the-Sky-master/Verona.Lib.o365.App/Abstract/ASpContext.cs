using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Verona.Lib.Common.Utility;
using Verona.Lib.o365.App.Interface;
using Verona.Lib.o365.App.Utility;

namespace Verona.Lib.o365.App.Abstract
{
    public abstract class ASpContext : Page,ISpContext
    {
        public string SpAppSessionId { get; private set; }
        private string SpAppSessionIdControlId { get { return Page.Master == null ? "SpAppSid" : "ctl00$SpAppSid"; } }

        public string SpAppContextToken { get; private set; }
        public string SpAppEntryUrl { get; private set; }
        public string SpAppHostUrl { get; private set; }

        public ClientContext Clientcontext { get; private set; }

        private NameValueCollection SpAppEntryQuery { get { return CnvUtility.ToRequestQueryString(SpAppEntryUrl); } }
        public Uri SpAppEntryHostUrl { get { return CnvUtility.ToAbsoluteUri(HttpUtility.UrlDecode(SpAppEntryQuery["SPHostUrl"])); } }
        public string SpAppSenderId { get { return CnvUtility.ToStr(HttpUtility.UrlDecode(SpAppEntryQuery["SenderId"])); } }
        public NameValueCollection SpAppCwpProps { get; private set; }

        private const int CacheDuration = (60 * 24);
        protected string CacheGroupName { get; set; }
        private string SpAppContextTokenKeyname { get { return CreateCacheKeyName("SpContextToken"); } }
        private string SpAppEntryUrlKeyname { get { return CreateCacheKeyName("SpAppEntryUrl"); } }
        private string SpAppHostUrlKeyname { get { return CreateCacheKeyName("SpAppHostUrl"); } }
        private string SpAppCwpPropsKeyname { get { return CreateCacheKeyName("SpAppCwpProps"); } }

        public virtual bool GetClientContext()
        {
            if (!GetSpAppSessionId())
                return false;
            
            if (CacheUtility.HasKey(SpAppContextTokenKeyname))
            {
                SpAppEntryUrl = CacheUtility.Read(SpAppEntryUrlKeyname).ToString();
                SpAppContextToken = CacheUtility.Read(SpAppContextTokenKeyname).ToString();
                SpAppHostUrl = CacheUtility.Read(SpAppHostUrlKeyname).ToString();
                SpAppCwpProps = (NameValueCollection)CacheUtility.Read(SpAppCwpPropsKeyname);
            }
            else
            {
                SpAppEntryUrl = CnvUtility.ToStr(Request.Url);
                SpAppContextToken = TokenHelper.GetContextTokenFromRequest(HttpContext.Current.Request);
                SpAppHostUrl = CnvUtility.ToStr(HttpContext.Current.Request.Url.Authority);

                CacheUtility.Create(CacheGroupName, SpAppContextTokenKeyname, SpAppContextToken, CacheDuration);
                CacheUtility.Create(CacheGroupName, SpAppEntryUrlKeyname, SpAppEntryUrl, CacheDuration);
                CacheUtility.Create(CacheGroupName, SpAppHostUrlKeyname, SpAppHostUrl, CacheDuration);

                GetClientWebpartProperties();
                CacheUtility.Create(CacheGroupName, SpAppCwpPropsKeyname, SpAppCwpProps, CacheDuration);
            }

            Clientcontext = TokenHelper.GetClientContextWithContextToken(SpAppEntryHostUrl.ToString(), SpAppContextToken, SpAppHostUrl);

            return true;
        }

        public string SessionId()
        {
            return string.Format("sid={0}", SpAppSessionId);
        }

        private bool GetSpAppSessionId()
        {
            SpAppSessionId = HttpContext.Current.Request.QueryString["sid"];

            if (SpAppSessionId != null)
                return SetSpAppHidden();

            if (!Page.IsPostBack)
            {
                SpAppSessionId = Guid.NewGuid().ToString().Replace("-", string.Empty);
                return SetSpAppHidden();
            }

            // SpAppSessionId == null og vi har postback ==> Vi har button click

            var requestValue = Page.Request.Form[SpAppSessionIdControlId];

            if (requestValue == null) return false;
            
            SpAppSessionId = requestValue;
            return SetSpAppHidden();
        }

        private bool SetSpAppHidden()
        {
            var obj = Page.FindControl(SpAppSessionIdControlId);
            if (obj == null) return false;
            ((HiddenField) obj).Value = SpAppSessionId;
            return true;
        }
        
        private void GetClientWebpartProperties()
        {
            var qs = HttpContext.Current.Request.QueryString;
            SpAppCwpProps = new NameValueCollection();
            foreach (var key in HttpContext.Current.Request.QueryString.AllKeys)
            {
                if (key.Equals("SPHostUrl") || key.Equals("SPHostTitle") || key.Equals("SPAppWebUrl") || key.Equals("SPLanguage") || key.Equals("SPProductNumber"))
                    continue;

                SpAppCwpProps.Add(key, CnvUtility.ToStr(qs[key]));
            }
        }

        private string CreateCacheKeyName(string key)
        {
            return string.Format("{0}_{1}", SpAppSessionId, key);
        }
    }
}