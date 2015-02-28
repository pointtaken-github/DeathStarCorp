<%@ WebHandler Language="C#" Class="RbmBergenServerSearch.Webhandlers.RedButtonSearcher" %>

using System;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Verona.Lib.Common.Utility;

namespace RbmBergenServerSearch.Webhandlers
{
    public class RedButtonSearcher : IHttpHandler, IRequiresSessionState
    {
        private const string ApiKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        private const string SearchEngineId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";        
        
        public void ProcessRequest(HttpContext context)
        {
            var page = CnvUtility.ToInt(context.Request.QueryString["page"]);
            var buzzword = context.Request.QueryString["buzzword"];
            var weeks = CnvUtility.ToInt(context.Request.QueryString["weeks"]);
            if (page == 0 || string.IsNullOrEmpty(buzzword)) return;
            if (weeks == 0) weeks = 1;

            context.Response.ContentType = "text/html";
            context.Response.Write(CreateHtml(page, buzzword, weeks));
        }

        public bool IsReusable { get { return false; } }

        private static string CreateHtml(int page, string buzzword, int weeks)
        {
            var output = new StringBuilder();
            output.Append("<html>" + Environment.NewLine);
            output.Append("<head>" + Environment.NewLine);
            output.Append("<script src=\"https://code.jquery.com/jquery-2.1.3.min.js\"></script>" + Environment.NewLine);

            CreateHandlerMethod(ref output, buzzword);
            
            output.Append("</head>" + Environment.NewLine);
            output.Append("<body>" + Environment.NewLine);
            output.Append("<div id=\"content\"></div>" + Environment.NewLine);
            
            CreateGoogleApsisScript(page, buzzword, weeks, ref output);
            
            output.Append("</body>" + Environment.NewLine);            
            output.Append("</html>" + Environment.NewLine);
            return output.ToString();
        }

        private static void CreateHandlerMethod(ref StringBuilder output, string buzzword)
        {
            output.Append("<script>" + Environment.NewLine);
            output.Append("function hndlr(response) {" + Environment.NewLine);
            output.Append("for (var i = 0; i < response.items.length; i++) {" + Environment.NewLine);

            output.Append("var item = response.items[i];" + Environment.NewLine);
            
            output.Append("var query=");
            output.Append("\"i=\" + item.cacheId +");
            output.Append("\"&t=\" + item.title +");
            output.Append("\"&s=\" + item.snippet +");
            output.Append("\"&l=\" + item.link +");
            output.AppendFormat("\"&b={0}\";", buzzword);
            output.Append(Environment.NewLine);

            output.Append("document.getElementById(\"content\").innerHTML += \"<hr><b>Match:</b><br>\" + query;" + Environment.NewLine + Environment.NewLine);
            CreateAjax(ref output);
            
            output.Append("}" + Environment.NewLine);
            output.Append("}" + Environment.NewLine);
            output.Append("</script>" + Environment.NewLine);
        }

        private static void CreateAjax(ref StringBuilder output)
        {
            output.Append("$.ajax({" + Environment.NewLine);
            output.Append("url:\"RedButtonStorer.ashx\"," + Environment.NewLine);
            output.Append("type: \"GET\"," + Environment.NewLine);
            output.Append("data: query," + Environment.NewLine);
            output.Append("async: false," + Environment.NewLine);
            output.Append("success: function(){ " + Environment.NewLine);
            output.Append("document.getElementById(\"content\").innerHTML += \"<br><b>Ajaxcall:</b><br>\" + query;" + Environment.NewLine + Environment.NewLine);
            output.Append(" }" + Environment.NewLine);
            output.Append("});" + Environment.NewLine);
        }
        
        private static void CreateGoogleApsisScript(int page, string buzzword, int weeks, ref StringBuilder output)
        {
            output.AppendFormat("<script src=\"https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&callback=hndlr&dateRestrict=w{2}&start={3}&q={4}\"></script>", ApiKey, SearchEngineId, weeks, page, buzzword);
        }
    }
}