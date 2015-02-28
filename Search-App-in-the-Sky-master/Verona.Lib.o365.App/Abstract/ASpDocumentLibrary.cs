using Microsoft.SharePoint.Client;

namespace Verona.Lib.o365.App.Abstract
{
    public abstract class ASpDocumentLibrary<T> : ASpListAndSpLibrary<T>
    {
        protected ASpDocumentLibrary(ClientContext clientContext, string libraryName)
            : base(clientContext, ListTemplateType.DocumentLibrary, libraryName)
        { }
    }
}
