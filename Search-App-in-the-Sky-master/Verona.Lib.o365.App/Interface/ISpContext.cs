using System;
using System.Collections.Specialized;
using System.Web.UI;
using Microsoft.SharePoint.Client;

namespace Verona.Lib.o365.App.Interface
{
    public interface ISpContext
    {
        string SpAppSessionId { get; }
        string SpAppContextToken { get; }
        string SpAppEntryUrl { get; }
        string SpAppHostUrl { get; }
        string SpAppSenderId { get; }
        NameValueCollection SpAppCwpProps { get; }
        ClientContext Clientcontext { get; }
        Uri SpAppEntryHostUrl { get; }

        string SessionId();
        bool GetClientContext();
    }
}