using System;
using Microsoft.SharePoint.Client;
using Verona.Lib.o365.App.Utility;

namespace Verona.Lib.o365.App.Abstract
{
    public abstract class ASpListAndSpLibrary<T>
    {
        public virtual string SpListName { get; private set; }
        public virtual ClientContext Clientcontext { get; private set; }
        public virtual ListTemplateType SpListTemplateType { get; private set; }
        public virtual List CurrentList { get { return SpListUtility.GetList(Clientcontext, SpListName); } }

        protected abstract object CurrentInstance { get; }
        public abstract string Description { get; }

        protected ASpListAndSpLibrary(ClientContext clientContext, ListTemplateType listTemplateType, string listName)
        {
            SpListTemplateType = listTemplateType;
            SpListName = listName;
            Clientcontext = clientContext;
        }

        public static T Instance(params object[] args)
        {
            return (T)Activator.CreateInstance(typeof(T), args);
        }

        public abstract List Create();
        public abstract ListItemCollection ReadItems();
        public abstract ListItem ReadItem(params object[] args);
        public abstract int Update();

        public virtual bool Delete()
        {
            if (!SpListUtility.ListExists(Clientcontext, SpListName)) return false;

            SpListUtility.DeleteSpList(Clientcontext, SpListName);

            return true;
        }
    }
}
