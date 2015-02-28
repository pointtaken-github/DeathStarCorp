<%@ WebHandler Language="C#" Class="RbmBergenServerSearch.WebHandlers.RedButtonStorer" %>

using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;

namespace RbmBergenServerSearch.WebHandlers
{
    public class RedButtonStorer : IHttpHandler, IRequiresSessionState
    {
        private string CacheId { get; set; }
        private string Title { get; set; }
        private string Snippet { get; set; }
        private string Link { get; set; }
        private string Buzzword { get; set; }

        private static string ConnStr { get { return "Data Source=xxxxxxxxxxxx;Initial Catalog=RedButtonBase;User Id=xxxxxxxxxx;Password=xxxxxxxxxxxxxxxxxx"; } }

        public void ProcessRequest(HttpContext context)
        {
            if (!GetQueryParams(context)) return;
            if (ColumnEqValue("GetItemByCacheId", "cacheid", CacheId).Count > 0) return;

            StoreItem();

            context.Response.ContentType = "text/plain";
            context.Response.Write("Got here");
        }

        public bool IsReusable { get { return false; } }

        private bool GetQueryParams(HttpContext context)
        {
            CacheId = context.Request.QueryString["i"];
            Title = context.Request.QueryString["t"];
            Snippet = context.Request.QueryString["s"];
            Link = context.Request.QueryString["l"];
            Buzzword = context.Request.QueryString["b"];

            return (!(string.IsNullOrEmpty(CacheId) || string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Snippet) ||
                    string.IsNullOrEmpty(Link) || string.IsNullOrEmpty(Buzzword)));
        }

        private int StoreItem()
        {
            int rowsCreated;

            using (var conn = new SqlConnection(ConnStr))
            {
                var sqlComm = new SqlCommand("CreateItem", conn);

                sqlComm.Parameters.AddWithValue("@cacheid", CacheId);
                sqlComm.Parameters.AddWithValue("@title", Title);
                sqlComm.Parameters.AddWithValue("@snippet", Snippet);
                sqlComm.Parameters.AddWithValue("@link", Link);
                sqlComm.Parameters.AddWithValue("@buzzword", Buzzword);

                sqlComm.CommandType = CommandType.StoredProcedure;
                var sqlDataAdapter = new SqlDataAdapter { InsertCommand = sqlComm };

                conn.Open();
                rowsCreated = sqlDataAdapter.InsertCommand.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }

            return rowsCreated;
        }

        public static DataRowCollection ColumnEqValue(string storedProcedure, string fieldName, object fieldValue)
        {
            var ds = new DataSet("currentdataSet");
            using (var conn = new SqlConnection(ConnStr))
            {
                var sqlComm = new SqlCommand(storedProcedure, conn) { CommandType = CommandType.StoredProcedure };
                sqlComm.Parameters.AddWithValue(string.Format("@{0}", fieldName), fieldValue);
                var sqlDataAdapter = new SqlDataAdapter { SelectCommand = sqlComm };
                sqlDataAdapter.Fill(ds);
                return ds.Tables[0].Rows;
            }
        }
    }
}