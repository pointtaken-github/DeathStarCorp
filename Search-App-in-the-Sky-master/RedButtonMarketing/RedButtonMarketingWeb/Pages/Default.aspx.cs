using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Verona.Lib.o365.App.Abstract;
using Verona.Lib.o365.App.Utility;
using Yammer.api;

namespace RedButtonMarketingWeb.Pages
{
    public partial class Default : AppPage //System.Web.UI.Page
    {
        const string Url = "http://www.rodent-phalanx.com/webhandlers/redbuttonservice.ashx";

        protected void Page_Load(object sender, EventArgs e)
        {
            InitApp("RedButtonMarketing_CacheKey");

            if (!Page.IsPostBack)
                MultiView1.SetActiveView(View1);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(View2);

            var response = GetJsonResponse();

            var jObj = (JObject)JsonConvert.DeserializeObject(response);
            var jArr = jObj["Table"].Value<JArray>();

            var suggestionList = SpListUtility.GetList(Clientcontext, "Suggestions");

            var i = 0;

            foreach (var jToken in jArr)
            {
                if (HasItem(Clientcontext, suggestionList, jToken["cacheid"].ToString()))
                    continue;

                InsertItem(Clientcontext, suggestionList, jToken);
                i++;
            }

            Literal1.Text = string.Format("Mr. Vader is done.</br>{0} items added.", i);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(View2);

            //var response = GetYammerResponse();

            //Literal1.Text = response;
        }

        //protected string GetYammerResponse()
        //{
        //    //QHMTRGYc2VznOsEr0AuRyQ
        //    //M0uRxZJzCqe3DMXXRhw7gDX9eUNZ3ISj0G5qq6JG0
        //    //https://redbuttonmarketing.azurewebsites.net/Pages/Default.aspx

        //    var stringResponse = string.Empty;

        //    var request = WebRequest.Create("https://www.yammer.com/dialog/oauth?client_id=QHMTRGYc2VznOsEr0AuRyQ&redirect_uri=https://redbuttonmarketing.azurewebsites.net/");
        //    var response = request.GetResponse();
        //    using (var responseStream = response.GetResponseStream())
        //    {
        //        if (responseStream == null) return stringResponse;
        //        var reader = new StreamReader(responseStream);
        //        stringResponse = reader.ReadToEnd();
        //    }

        //    return stringResponse;
        //}

        protected string GetJsonResponse()
        {
            var stringResponse = string.Empty;

            var request = WebRequest.Create(Url);

            var response = request.GetResponse();
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null) return stringResponse;
                var reader = new StreamReader(responseStream);
                stringResponse = reader.ReadToEnd();
            }

            return stringResponse;
        }

        protected void InsertItem(ClientContext clientContext, List suggestionList, JToken jToken)
        {
            //List suggestionList = SpListUtility.GetList(clientContext, "Suggestions");

            var itemCreateInfo = new ListItemCreationInformation();
            var newItem = suggestionList.AddItem(itemCreateInfo);

            newItem["Title"] = jToken["title"].ToString();
            newItem["SourceID"] = jToken["cacheid"].ToString();
            newItem["URL"] = new FieldUrlValue
            {
                Url = jToken["link"].ToString(),
                Description = jToken["snippet"].ToString()
            };
            newItem["Buzzword"] = jToken["buzzword"].ToString();
            
            newItem.Update();
            clientContext.ExecuteQuery();
        }

        protected bool HasItem(ClientContext clientContext, List spList, string cacheid)
        {
            clientContext.Load(spList);
            clientContext.ExecuteQuery();

            if (spList.ItemCount <= 0)
                return false;

            var viewFields = new StringBuilder();
            viewFields.Append("<FieldRef Name='SourceID' />");
            const string orderByFieldName = "SourceID";

            var camlQuery = new CamlQuery
            {
                ViewXml = string.Format(@"
                    <View>  
                        <Query> 
                            <Where><Contains><FieldRef Name='{0}' /><Value Type='Text'>{1}</Value></Contains></Where> 
                            <OrderBy><FieldRef Name='{2}' /></OrderBy>  
                        </Query> 
                        <ViewFields>{3}</ViewFields>
                        <RowLimit>{4}</RowLimit> 
                    </View>",
                    "SourceID", cacheid, orderByFieldName, viewFields, 1)
            };


            var listItems = spList.GetItems(camlQuery);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            return listItems.Count > 0;
        }

        

        //public void ActionResult()
        //{
        //    var myConfig = new ClientConfigurationContainer
        //    {
        //        ClientCode = null,
        //        ClientId = "AW9NGn7DJxLIEZ5gFbhnjw",
        //        ClientSecret = "L905g5aMKBxMvYjDupMg4UiJYTZG4QrOJv8J1sHOOnU",
        //        RedirectUri = Request.Url.AbsoluteUri + Url.Action("AuthCode")
        //    };

        //    var myYammer = new YammerClient(myConfig);
        //    var url = myYammer.GetLoginLinkUri(); // <== Obtain the URL of Yammer Authorisation Page
        //    this.TempData["YammerConfig"] = myConfig;
        //    return Redirect(url); // <= Jump to this page
           
        //    //return View(model);
        //}
    }
}
