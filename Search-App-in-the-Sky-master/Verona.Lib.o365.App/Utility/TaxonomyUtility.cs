using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Verona.Lib.Common.Utility;

namespace Verona.Lib.o365.App.Utility
{
    public static class TaxonomyUtility
    {
        /// <summary>
        /// Gets the term store identified by its Guid.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termStoreGuid">The term store unique identifier.</param>
        /// <returns></returns>
        public static TermStore GetTermStoreByGuid(ClientContext clientContext, Guid termStoreGuid)
        {
            var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            var termStore = taxonomySession.TermStores.GetById(termStoreGuid);
            clientContext.Load(termStore);
            clientContext.ExecuteQuery();
            return termStore;
        }

        /// <summary>
        /// Gets the first or default term store.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <returns></returns>
        public static TermStore GetFirstOrDefaultTermStore(ClientContext clientContext)
        {
            var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            clientContext.Load(taxonomySession.TermStores);
            clientContext.ExecuteQuery();

            var termStore = taxonomySession.TermStores.FirstOrDefault();
            if (termStore != null)
            {
                clientContext.Load(termStore);
                clientContext.ExecuteQuery();
                return termStore;
            }
            return null;
        }

        /// <summary>
        /// Gets all the term stores with the given name.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termStoreName">Name of the term store.</param>
        /// <returns></returns>
        public static List<TermStore> GetTermStoresByName(ClientContext clientContext, string termStoreName)
        {
            var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            var termStores = taxonomySession.TermStores;
            return termStores.Where(termStore => termStoreName.Equals(termStore.Name)).ToList();
        }

        /// <summary>
        /// Gets the termstore group by name.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termStore">The term store.</param>
        /// <param name="termStoreGroupName">Name of the term store group.</param>
        /// <returns></returns>
        public static TermGroup GetTermGroupByName(ClientContext clientContext, TermStore termStore, string termStoreGroupName)
        {
            if (!Contains(clientContext, termStore, termStoreGroupName))
                return null;

            var termGroup = termStore.Groups.GetByName(termStoreGroupName);
            clientContext.Load(termGroup);
            clientContext.ExecuteQuery();
            return termGroup;
        }

        /// <summary>
        /// Gets the term set.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termGroup">The term group.</param>
        /// <param name="termSetName">Name of the term set.</param>
        /// <returns></returns>
        public static TermSet GetTermSetByName(ClientContext clientContext, TermGroup termGroup, string termSetName)
        {
            var termSet = termGroup.TermSets.GetByName(termSetName);
            clientContext.Load(termSet);
            clientContext.ExecuteQuery();
            return termSet;
        }

        /// <summary>
        /// Gets the term by name.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termSet">The term set.</param>
        /// <param name="termName">Name of the term.</param>
        /// <returns></returns>
        public static Term GetTermByName(ClientContext clientContext, TermSet termSet, string termName)
        {
            var term = termSet.Terms.GetByName(termName);
            clientContext.Load(term);
            clientContext.ExecuteQuery();
            return term;
        }

        /// <summary>
        /// Gets the term by identifier.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termSet">The term set.</param>
        /// <param name="termGuid">The term unique identifier.</param>
        /// <returns></returns>
        public static Term GetTermById(ClientContext clientContext, TermSet termSet, Guid termGuid)
        {
            var term = termSet.Terms.GetById(termGuid);
            clientContext.Load(term);
            clientContext.ExecuteQuery();
            return term;
        }

        /// <summary>
        /// Gets the WSS identifier.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termGuid">The term unique identifier.</param>
        /// <returns></returns>
        public static int GetWssId(ClientContext clientContext, Guid termGuid)
        {
            var spList = clientContext.Site.RootWeb.Lists.GetByTitle("TaxonomyHiddenList");
            clientContext.Load(spList);
            clientContext.ExecuteQuery();

            if (spList == null || spList.ItemCount <= 0) return int.MinValue;

            var viewFields = new StringBuilder();

            viewFields.AppendFormat("<FieldRef Name='{0}' />", "ID");
            viewFields.AppendFormat("<FieldRef Name='{0}' />", "Title");
            viewFields.AppendFormat("<FieldRef Name='{0}' />", "IdForTerm");
            viewFields.AppendFormat("<FieldRef Name='{0}' />", "IdForTermStore");
            viewFields.AppendFormat("<FieldRef Name='{0}' />", "IdForTermSet");
            viewFields.AppendFormat("<FieldRef Name='{0}' />", "Term");

            var camlQuery = new CamlQuery
            {
                ViewXml = string.Format(@"
                    <View>  
                        <Query> 
                            <Where>
                                <Eq>
                                    <FieldRef Name='IdForTerm' /><Value Type='Text'>{0}</Value>
                                </Eq>
                            </Where>
                            <OrderBy>
                                <FieldRef Name='IdForTerm' />
                            </OrderBy> 
                        </Query> 
                        <ViewFields>{1}</ViewFields> 
                        <RowLimit>1</RowLimit>
                    </View>",

                    termGuid,
                    viewFields)
            };

            var listItems = spList.GetItems(camlQuery);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            var listItem = listItems[0];
            return CnvUtility.ToInt(listItem["ID"]);

        }

        /// <summary>
        /// Determines whether the specified term store contains a termgroup named groupname.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="termStore">The term store.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns></returns>
        public static bool Contains(ClientContext clientContext, TermStore termStore, string groupName)
        {
            if (clientContext == null || termStore == null || termStore.Groups == null || string.IsNullOrEmpty(groupName))
                return false;

            clientContext.Load(termStore.Groups);
            clientContext.ExecuteQuery();

            return termStore.Groups.Any() && Enumerable.Any(termStore.Groups, termStoreGroup => termStoreGroup.Name.Equals(groupName));
        }

        /// <summary>
        /// Determines whether the specified term group contains termsetname.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="termGroup">The term group.</param>
        /// <param name="termSetName">Name of the term set.</param>
        /// <returns></returns>
        public static bool Contains(ClientContext clientContext, TermGroup termGroup, string termSetName)
        {
            if (clientContext == null || termGroup == null || termGroup.TermSets == null || string.IsNullOrEmpty(termSetName))
                return false;

            clientContext.Load(termGroup.TermSets);
            clientContext.ExecuteQuery();

            return termGroup.TermSets.Any() && Enumerable.Any(termGroup.TermSets, termSet => termSet.Name.Equals(termSetName));
        }

        /// <summary>
        /// Determines whether the specified term set contains a term named termname.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="termSet">The term set.</param>
        /// <param name="termName">Name of the term.</param>
        /// <returns></returns>
        public static bool Contains(ClientContext clientContext, TermSet termSet, string termName)
        {
            if (termSet == null || termSet.Terms == null || string.IsNullOrEmpty(termName))
                return false;

            clientContext.Load(termSet.Terms);
            clientContext.ExecuteQuery();

            return termSet.Terms.Any() && Enumerable.Any(termSet.Terms, term => term.Name.Equals(termName));
        }

        /// <summary>
        /// Creates the termgroup.
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="termStore">The term store.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns></returns>
        public static TermGroup CreateTermGroup(ClientContext clientContext, TermStore termStore, string groupName)
        {
            if (Contains(clientContext, termStore, groupName))
                return null;

            var termGroup = termStore.CreateGroup(groupName, Guid.NewGuid());
            clientContext.ExecuteQuery();
            return termGroup;
        }

        /// <summary>
        /// Creates the term set.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termGroup">The term group.</param>
        /// <param name="termSetName">Name of the term set.</param>
        /// <param name="lcid">The lcid.</param>
        /// <returns></returns>
        public static TermSet CreateTermSet(ClientContext clientContext, TermGroup termGroup, string termSetName, int lcid)
        {
            if (Contains(clientContext, termGroup, termSetName))
                return null;

            var termSet = termGroup.CreateTermSet(termSetName, Guid.NewGuid(), lcid);
            clientContext.ExecuteQuery();
            return termSet;
        }

        /// <summary>
        /// Creates the term.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termSet">The term set.</param>
        /// <param name="termName">Name of the term.</param>
        /// <param name="lcid">The lcid.</param>
        /// <returns></returns>
        public static Term CreateTerm(ClientContext clientContext, TermSet termSet, string termName, int lcid)
        {
            if (Contains(clientContext, termSet, termName))
                return null;

            var term = termSet.CreateTerm(termName, lcid, Guid.NewGuid());
            clientContext.ExecuteQuery();
            return term;
        }

        /// <summary>
        /// Deletes the specified termgroup and all its termsets and terms.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termGroup">The term group.</param>
        public static void Delete(ClientContext clientContext, TermGroup termGroup)
        {
            if (clientContext == null || termGroup == null || termGroup.TermSets == null)
                return;

            clientContext.Load(termGroup.TermSets);
            clientContext.ExecuteQuery();

            if (termGroup.TermSets.Any())
            {
                foreach (var termSet in termGroup.TermSets)
                    Delete(clientContext, termSet);
            }

            termGroup.DeleteObject();
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Deletes the specified termset and all its terms.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="termSet">The term set.</param>
        public static void Delete(ClientContext clientContext, TermSet termSet)
        {
            if (clientContext == null || termSet == null)
                return;

            termSet.DeleteObject();

            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Deletes the specified term.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="term">The term.</param>
        public static void Delete(ClientContext clientContext, Term term)
        {
            if (clientContext == null || term == null)
                return;

            term.DeleteObject();

            clientContext.ExecuteQuery();
        }

        public static TermCollection GetTermsFromTermSet(ClientContext context, string termGroupName, string termsetName,Guid termStoreId)
        {
            // Get the TaxonomySession
            var taxonomySession = TaxonomySession.GetTaxonomySession(context);

            // Get the term store by name
            var termStore = taxonomySession.TermStores.GetById(termStoreId);
            var termGroup = termStore.Groups.GetByName(termGroupName);
            var termSet = termGroup.TermSets.GetByName(termsetName);
            var termColl = termSet.GetAllTerms();

            if (termColl == null) return null;

            // Get the termset by guid
            context.Load(termColl);

            // Execute the query to the server
            context.ExecuteQuery();

            return termColl;
        }

        public static List<string> GetTermNamesFromTermSet(ClientContext context, string termGroupName, string termsetName, Guid termStoreId)
        {
            var terms = new List<string>();

            // Get the TaxonomySession
            var taxonomySession = TaxonomySession.GetTaxonomySession(context);

            // Get the term store by name
            var termStore = taxonomySession.TermStores.GetById(termStoreId);
            var termGroup = termStore.Groups.GetByName(termGroupName);
            var termSet = termGroup.TermSets.GetByName(termsetName);
            var termColl = termSet.GetAllTerms();

            if (termColl == null) return null;

            // Get the termset by guid
            context.Load(termColl);

            // Execute the query to the server
            context.ExecuteQuery();

            foreach (var term in termColl)
                terms.Add(term.Name);

            return terms;
        }

        public static Term GetTermByName(ClientContext clientContext, string termgroupName, string termsetName, string termName, Guid termStoreId)
        {
            // Get the TaxonomySession
            var taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);

            // Get the term store by name
            var termStore = taxonomySession.TermStores.GetById(termStoreId);
            var termGroup = termStore.Groups.GetByName(termgroupName);
            var termSet = termGroup.TermSets.GetByName(termsetName);
            var termColl = termSet.GetAllTerms();

            if (termColl == null) return null;

            // Get the termset by guid
            clientContext.Load(termColl);

            // Execute the query to the server
            clientContext.ExecuteQuery();

            foreach (var term in termColl)
                if (term.Name == termName)
                    return term;

            return null;
        }
    }
}
