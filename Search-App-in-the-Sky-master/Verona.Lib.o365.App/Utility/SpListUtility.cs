using System;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Verona.Lib.o365.App.Object;

namespace Verona.Lib.o365.App.Utility
{
    public static class SpListUtility
    {
        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="listName">Name of the list.</param>
        /// <returns></returns>
        public static List GetList(ClientContext clientContext, string listName)
        {
            if (string.IsNullOrEmpty(listName) || clientContext.Web.Lists == null)
                return null;

            return clientContext.Web.Lists.GetByTitle(listName);
        }

        /// <summary>
        /// Gets the field collection.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static FieldCollection GetFieldCollection(ClientContext clientContext, List list)
        {
            if (list == null)
                return null;

            var fields = list.Fields;

            if (fields == null)
                return null;

            clientContext.Load(fields);
            return fields;
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static Field GetField(ClientContext clientContext, FieldCollection fields, string fieldName)
        {
            var field = fields.GetByInternalNameOrTitle(fieldName);

            if (field == null) return null;

            clientContext.Load(field);
            return field;
        }



        /// <summary>
        /// Gets the list item by identifier.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="listName">Name of the list.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static ListItem GetListItemById(ClientContext clientContext, string listName, int id)
        {
            if (clientContext == null || string.IsNullOrEmpty(listName))
                return null;

            var spList = clientContext.Web.Lists.GetByTitle(listName);
            clientContext.Load(spList);
            clientContext.ExecuteQuery();

            if (spList == null || spList.ItemCount <= 0)
                return null;

            var viewFields = new StringBuilder();
            viewFields.Append("<FieldRef Name='ID' />");
            const string orderByFieldName = "ID";

            var camlQuery = new CamlQuery
            {
                ViewXml = string.Format(@"
                    <View>
                        <Query> 
                            <Where>
                                <Eq><FieldRef Name='{0}' /><Value Type='Counter'>{1}</Value></Eq>
                            </Where>
                            <OrderBy>
                                <FieldRef Name='{2}' />
                            </OrderBy>  
                        </Query> 
                        <ViewFields>{3}</ViewFields> 
                        <RowLimit>{4}</RowLimit> 
                    </View>",
                    "ID",
                    id,
                    orderByFieldName,
                    viewFields,
                    1)
            };



            var listItems = spList.GetItems(camlQuery);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            return listItems.FirstOrDefault();
        }



        /// <summary>
        /// Determines if the list exists.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="spListName">Name of the sp list.</param>
        /// <returns></returns>
        public static bool ListExists(ClientContext clientContext, string spListName)
        {
            if (clientContext == null || string.IsNullOrEmpty(spListName))
                return false;

            var lists = clientContext.Web.Lists;

            if (lists == null) return false;

            var existingLists = clientContext.LoadQuery(lists.Where(list => list.Title == spListName));
            clientContext.ExecuteQuery();
            var existingList = existingLists.FirstOrDefault();
            return existingList != null;
        }

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="spListTemplateType">Type of the list template.</param>
        /// <param name="spListName">Name of the list.</param>
        /// <param name="spListDescription"></param>
        /// <returns></returns>
        public static List CreateSpList(ClientContext clientContext, ListTemplateType spListTemplateType, string spListName, string spListDescription)
        {
            if (clientContext == null || string.IsNullOrEmpty(spListName))
                return null;

            var listCreationInfo = new ListCreationInformation
            {
                Title = spListName,
                Description = spListDescription,
                TemplateType = (int)spListTemplateType
            };

            var list = clientContext.Web.Lists.Add(listCreationInfo);

            clientContext.ExecuteQuery();

            return list;
        }

        /// <summary>
        /// Adds the sp field.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="name"></param>
        /// <param name="displayName">The display name.</param>
        /// <param name="addToDefaultView">if set to <c>true</c> [add to default view].</param>
        /// <returns></returns>
        private static void AddSpField(this FieldCollection fields, FieldType fieldType, string name, string displayName, bool addToDefaultView)
        {
            fields.AddFieldAsXml(string.Format("<Field DisplayName='{0}' Type='{1}' Name='{2}' />", displayName, fieldType, name), addToDefaultView, AddFieldOptions.DefaultValue);
        }

        /// <summary>
        /// Adds the sp field.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="choices">The choices.</param>
        /// <param name="addToDefaultView">if set to <c>true</c> [add to default view].</param>
        /// <returns></returns>
        private static void AddSpField(this FieldCollection fields, FieldType fieldType, string displayName, string[] choices, bool addToDefaultView)
        {
            fields.AddFieldAsXml(
                string.Format("<Field DisplayName='{0}' Type='{1}'><CHOICES>{2}</CHOICES></Field>",
                    displayName,
                    fieldType,
                    string.Concat(Array.ConvertAll(choices, choice => string.Format("<CHOICE>{0}</CHOICE>", choice)))),
                addToDefaultView, AddFieldOptions.DefaultValue);
        }

        /// <summary>
        /// Adds the sp field to sp list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="spListName">Name of the sp list.</param>
        /// <param name="name"></param>
        /// <param name="displayName">The display name.</param>
        /// <param name="fieldType"></param>
        /// <param name="choices"></param>
        /// <param name="addToDefaultView"></param>
        public static bool AddSpFieldToSpList(ClientContext clientContext, string spListName, string name, string displayName, FieldType fieldType, string[] choices, bool addToDefaultView)
        {
            if (clientContext == null || string.IsNullOrEmpty(spListName) || string.IsNullOrEmpty(displayName))
                return false;

            var list = GetList(clientContext, spListName);

            if (list == null)
                return false;

            var fields = GetFieldCollection(clientContext, list);

            if (fields == null)
                return false;

            if (choices != null && choices.Length > 0)
                AddSpField(fields, fieldType, displayName, choices, true);
            else
                AddSpField(fields, fieldType, name, displayName, true);

            clientContext.ExecuteQuery();

            return true;
        }

        /// <summary>
        /// Adds the sp field to sp list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="spListName">Name of the sp list.</param>
        /// <param name="name"></param>
        /// <param name="displayName">The display name.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="addToDefaultView">if set to <c>true</c> [add to default view].</param>
        /// <returns></returns>
        public static bool AddSpFieldToSpList(ClientContext clientContext, string spListName, string name, string displayName, FieldType fieldType, bool addToDefaultView)
        {
            if (clientContext == null || string.IsNullOrEmpty(spListName) || string.IsNullOrEmpty(displayName))
                return false;

            var list = GetList(clientContext, spListName);

            if (list == null)
                return false;

            var fields = GetFieldCollection(clientContext, list);

            if (fields == null)
                return false;

            AddSpField(fields, fieldType, name, displayName, true);

            clientContext.ExecuteQuery();

            return true;
        }

        /// <summary>
        /// Adds the taxonomy sp field to sp list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="spListName">Name of the sp list.</param>
        /// <param name="taxFieldSchema">The schema.</param>
        /// <param name="addToDefaultView">if set to <c>true</c> [add to default view].</param>
        /// <returns></returns>
        public static bool AddTaxonomySpFieldToSpList(ClientContext clientContext, string spListName, TaxonomyFieldSchema taxFieldSchema, bool addToDefaultView)
        {
            if (clientContext == null || string.IsNullOrEmpty(spListName) || taxFieldSchema == null)
                return false;

            var list = GetList(clientContext, spListName);
            if (list == null) return false;

            var fields = GetFieldCollection(clientContext, list);
            if (fields == null) return false;

            var noteSchema = taxFieldSchema.GetNoteFieldSchema();
            var taxFieldSchemaXml = taxFieldSchema.Multi ? taxFieldSchema.GetTaxonomyFieldSchemaMulti() : taxFieldSchema.GetTaxonomyFieldSchema();

            fields.AddFieldAsXml(noteSchema, false, AddFieldOptions.DefaultValue);
            clientContext.ExecuteQuery();

            list = GetList(clientContext, spListName);
            fields = GetFieldCollection(clientContext, list);

            fields.AddFieldAsXml(taxFieldSchemaXml, addToDefaultView, AddFieldOptions.DefaultValue);

            return true;
        }

        /// <summary>
        /// Deletes the sp list.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="spListName">Name of the list.</param>
        public static void DeleteSpList(ClientContext clientContext, string spListName)
        {
            if (clientContext == null || string.IsNullOrEmpty(spListName) || !ListExists(clientContext, spListName))
                return;

            var oList = clientContext.Web.Lists.GetByTitle(spListName);

            if (oList == null)
                return;

            oList.DeleteObject();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Determines whether the specified list is a document library
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static bool IsDocumentLibrary(List list)
        {
            if (list.Equals(null))
                return false;

            return list.BaseType == BaseType.DocumentLibrary;
        }

        public static bool IsDiscussionBoard(List list)
        {
            if (list.Equals(null))
                return false;

            return list.BaseType == BaseType.DiscussionBoard;
        }

        public static bool IsGenericList(List list)
        {
            if (list.Equals(null))
                return false;

            return list.BaseType == BaseType.GenericList;
        }

        public static bool IsIssue(List list)
        {
            if (list.Equals(null))
                return false;

            return list.BaseType == BaseType.Issue;
        }

        public static bool IsNone(List list)
        {
            if (list.Equals(null))
                return false;

            return list.BaseType == BaseType.None;
        }

        public static bool IsSurvey(List list)
        {
            if (list.Equals(null))
                return false;

            return list.BaseType == BaseType.Survey;
        }

        public static bool IsUnused(List list)
        {
            if (list.Equals(null))
                return false;

            return list.BaseType == BaseType.Unused;
        }
    }
}
