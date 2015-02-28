<%@ WebHandler Language="C#" Class="RbmBergenServerSearch.WebHandlers.RedButtonControl" %>

using System;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace RbmBergenServerSearch.WebHandlers
{
    public class RedButtonControl : IHttpHandler, IRequiresSessionState
    {
        public int Pages { get; set; }
        public string Buzzwords { get; set; }
        public int Weeks { get; set; }

        private static string ConnStr { get { return "Data Source=xxxxx;Initial Catalog=RedButtonBase;User Id=xxxxx;Password=xxxxx"; } }

        public void ProcessRequest(HttpContext context)
        {
            CreateHtml();
            RunPopulation();
            
            context.Response.ContentType = "text/html";            
            context.Response.Write(CreateHtml());
        }

        public bool IsReusable { get { return false; } }

        private string CreateHtml()
        {
            var output = new StringBuilder();
            output.Append("<html>" + Environment.NewLine);
            output.Append("<head>" + Environment.NewLine);
            output.Append("</head>" + Environment.NewLine);
            output.Append("<body>" + Environment.NewLine);
            output.Append("<div id=\"content\"></div>" + Environment.NewLine);            
            output.Append(RunPopulation());
            output.Append("</body>" + Environment.NewLine);
            output.Append("</html>" + Environment.NewLine);
            return output.ToString();
        }

        private string RunPopulation()
        {
            // Testdata - this would be automated in a production environment
            
            Pages = 1;
            var buzzword = "memory+leak";
            Weeks = 99;
            var page = 1;
            
            var output = new StringBuilder();
            
            //for (int page = 1; page < (Pages+1); page++)
            //{
                output.AppendFormat("<iframe style=\"height:200px;width:500px;\" src=\"xxxxxxxxxx/redbuttonsearcher.ashx?page={0}&buzzword={1}&weeks={2}\"></iframe>{3}", page, buzzword, Weeks, Environment.NewLine);    
            //}
            
            return output.ToString();
        }
    }
}