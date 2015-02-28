<%@ WebHandler Language="C#" Class="RbmBergenServerSearch.WebHandlers.RedButtonService" %>

using System.Data;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json;

namespace RbmBergenServerSearch.WebHandlers
{  
    public class RedButtonService : IHttpHandler
    {
        private static string ConnStr { get { return "Data Source=xxxxxxxxxxxxxx;Initial Catalog=RedButtonBase;User Id=xxxxxx;Password=xxxxxxxxxxxxxx"; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/json";
            
            var items = GetItems();

            if (items.Count > 0)
            {
                var jsonString = JsonConvert.SerializeObject(items[0]);
                context.Response.Write(jsonString);    
            }
        }

        public DataRowCollection GetItems()
        {
            var ds = new DataSet("currentdataSet");
            
            using (var conn = new SqlConnection(ConnStr))
            {
                var sqlComm = new SqlCommand("GetAllItems", conn) { CommandType = CommandType.StoredProcedure };                
                var sqlDataAdapter = new SqlDataAdapter { SelectCommand = sqlComm };
                sqlDataAdapter.Fill(ds);
                return ds.Tables[0].Rows;
            }
        }

        public bool IsReusable { get { return false; } }

    }
}