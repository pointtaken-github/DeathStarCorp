using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Verona.Lib.o365.App.Object;

namespace Verona.Lib.o365.App.Utility
{
    public static class TaxonomyFieldSchemaUtility
    {
        public static TaxonomyFieldSchema GetTaxFieldSchema(ClientContext clientcontext, TermStore termStore, TermGroup termGroup, string name, string displayName, string termSetName, bool multiField, bool requiredField, bool enforceUniqueValues)
        {
            return new TaxonomyFieldSchema
            {
                DisplayName = displayName,
                Name = name,
                Multi = multiField,
                EnforceUniqueValues = enforceUniqueValues,
                Lcid = 1044,
                RequiredField = requiredField,
                TermSetGuid = TaxonomyUtility.GetTermSetByName(clientcontext, termGroup, termSetName).Id.ToString(),
                TermStoreGuid = termStore.Id.ToString(),
                Open = false
            };
        }
    }
}
