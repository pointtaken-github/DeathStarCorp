using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Verona.Lib.o365.App.Abstract
{
    public abstract class AppPage : ASpContext
    {
        public bool InitApp(string cacheGroupName)
        {
            CacheGroupName = cacheGroupName;

            if (!GetClientContext())
                return false;

            AddRequiredJavascripts();
            AddAppResizeJavascript();

            return true;
        }

        private void AddRequiredJavascripts()
        {
            var appjs = new HtmlGenericControl("script");
            appjs.Attributes["type"] = "text/javascript";
            appjs.Attributes["src"] = "/Scripts/app.js";

            var jqueryjs = new HtmlGenericControl("script");
            jqueryjs.Attributes["type"] = "text/javascript";
            jqueryjs.Attributes["src"] = "/Scripts/jquery-1.9.1.js";

            Page.Header.Controls.Add(appjs);
            Page.Header.Controls.Add(jqueryjs);
        }

        private void AddAppResizeJavascript()
        {
            var js = new StringBuilder();
            js.Append("<script type=\"text/javascript\">");
            js.Append("$(window).ready(function() {");
            js.Append("setInterval(function() { ");
            js.AppendFormat("VresizeAppPart('{0}','{1}');", SpAppEntryHostUrl, SpAppSenderId);
            js.Append(" }, 10000);");
            js.AppendFormat("VresizeAppPart('{0}','{1}');", SpAppEntryHostUrl, SpAppSenderId);
            js.Append("});");
            js.Append("</script>");

            Page.Header.Controls.Add(new Literal { Text = js.ToString() });
        }
    }
}
