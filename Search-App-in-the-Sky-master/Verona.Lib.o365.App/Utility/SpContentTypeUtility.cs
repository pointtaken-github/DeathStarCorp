using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace Verona.Lib.o365.App.Utility
{
    public static class SpContentTypeUtility
    {
        public static ContentType CreateSpContentType(ClientContext clientContext, string contentTypeName, string description, string group, ContentType parentContentType)
        {
            var contentTypeColl = clientContext.Web.ContentTypes;

            var contentTypeCreation = new ContentTypeCreationInformation
            {
                Name = contentTypeName,
                Description = description,
                ParentContentType = parentContentType,
                Group = group
            };

            var contentType = contentTypeColl.Add(contentTypeCreation);
            clientContext.Load(contentType);
            clientContext.ExecuteQuery();

            return contentType;
        }

        private static ContentType GetContentTypeByName(ClientContext clientContext, string contentTypeName)
        {
            if (string.IsNullOrEmpty(contentTypeName) || clientContext.Web.ContentTypes == null)
                return null;

            var contentTypes = clientContext.Web.ContentTypes;
            clientContext.Load(contentTypes);
            clientContext.ExecuteQuery();

            foreach (var contentType in clientContext.Web.ContentTypes)
            {
                if (contentType.Name.ToLower().Equals(contentTypeName.ToLower()))
                    return contentType;
            }

            return null;
        }

        private static ContentType GetContentTypeById(ClientContext clientContext, string contentTypeId)
        {
            if (clientContext == null || clientContext.Web.ContentTypes == null)
                return null;

            try
            {
                return clientContext.Web.ContentTypes.GetById(contentTypeId); // Feiler her -godtar ikke "Govering Documents"
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ContentType GetContentType(ClientContext clientContext, string contentTypeNameOrId)
        {
            return GetContentTypeById(clientContext, contentTypeNameOrId) ?? GetContentTypeByName(clientContext, contentTypeNameOrId);
        }

        public static bool ContentTypeExists(ClientContext clientContext, string contentTypeNameOrId)
        {
            return GetContentType(clientContext, contentTypeNameOrId) != null;
        }

        public static void DeleteContentType(ClientContext clientContext, string contentTypeName)
        {
            if (clientContext == null || string.IsNullOrEmpty(contentTypeName) || !ContentTypeExists(clientContext, contentTypeName))
                return;

            var contentType = GetContentType(clientContext, contentTypeName);

            if (contentType == null)
                return;

            contentType.DeleteObject();
            clientContext.ExecuteQuery();
        }

        public static bool AddExistingSpField(ClientContext clientContext, ContentType contentType, string fieldName)
        {
            if (SpFieldInContentTypeExists(clientContext, contentType, fieldName))
                return false;

            var field = SpSiteColumnsUtility.GetSpField(clientContext, fieldName);

            if (field == null)
                return false;

            var fieldLink = new FieldLinkCreationInformation { Field = field };

            contentType.FieldLinks.Add(fieldLink);
            contentType.Update(true);
            clientContext.ExecuteQuery();

            return true;
        }

        public static bool SpFieldInContentTypeExists(ClientContext clientContext, ContentType contentType, string fieldName)
        {
            clientContext.Load(contentType.Fields);
            clientContext.ExecuteQuery();

            return Enumerable.Any(contentType.Fields, field => field.InternalName.Equals(fieldName));
        }

        public static void SetDefaultContentType(ClientContext clientContext, List list, ContentType contentType)
        {
            ContentTypeCollection currentCtOrder = list.ContentTypes;
            clientContext.Load(currentCtOrder);
            clientContext.ExecuteQuery();

            IList<ContentTypeId> reverceOrder = new List<ContentTypeId>();

            foreach (ContentType ct in currentCtOrder)
            {
                if (ct.Name.Equals(contentType.Name))
                {
                    reverceOrder.Add(ct.Id);
                }
            }
            list.RootFolder.UniqueContentTypeOrder = reverceOrder;
            list.RootFolder.Update();
            list.Update();
            clientContext.ExecuteQuery();
        }
    }
}
