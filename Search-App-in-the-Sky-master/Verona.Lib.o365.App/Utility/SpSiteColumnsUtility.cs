using Microsoft.SharePoint.Client;
using Verona.Lib.o365.App.Object;

namespace Verona.Lib.o365.App.Utility
{
    public static class SpSiteColumnsUtility
    {
        public static bool AddTaxonomySpField(ClientContext clientContext, TaxonomyFieldSchema taxFieldSchema, bool addToDefaultView)
        {
            var noteSchema = taxFieldSchema.GetNoteFieldSchema();
            var txSchema = taxFieldSchema.Multi ? taxFieldSchema.GetTaxonomyFieldSchemaMulti() : taxFieldSchema.GetTaxonomyFieldSchema();

            clientContext.Web.Fields.AddFieldAsXml(noteSchema, false, AddFieldOptions.DefaultValue);
            clientContext.ExecuteQuery();

            clientContext.Web.Fields.AddFieldAsXml(txSchema, addToDefaultView, AddFieldOptions.DefaultValue);
            clientContext.ExecuteQuery();

            return false;
        }

        public static void DeleteSpField(ClientContext clientContext, string fieldName)
        {
            var field = clientContext.Web.Fields.GetByInternalNameOrTitle(fieldName);
            if (field != null)
                field.DeleteObject();

            clientContext.ExecuteQuery();
        }

        public static bool SpFieldExists(ClientContext clientContext, string fieldName)
        {
            clientContext.Load(clientContext.Web.Fields);
            clientContext.ExecuteQuery();

            foreach (var field in clientContext.Web.Fields)
            {
                if (field.InternalName.Equals(fieldName))
                    return true;
            }
            return false;
        }

        public static Field GetSpField(ClientContext clientContext, string fieldName)
        {
            clientContext.Load(clientContext.Web.Fields);
            clientContext.ExecuteQuery();

            foreach (var field in clientContext.Web.Fields)
            {
                if (field.InternalName.Equals(fieldName))
                    return field;
            }
            return null;
        }
    }
}
