using Microsoft.SharePoint.Client;

namespace Verona.Lib.o365.App.Abstract
{
    public abstract class ASpList<T> : ASpListAndSpLibrary<T>
    {
        protected ASpList(ClientContext clientContext, ListTemplateType listTemplateType, string listName)
            : base(clientContext, listTemplateType, listName)
        { }       
    }
}
