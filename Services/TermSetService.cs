using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Services
{
    public class TermSetService
    {
        public Term GetTermById(ClientContext clientContext, Guid termId, Guid termStoreId, Guid termSetId)
        {
            var termCollection = Cache.Get<TermCollection>(termSetId.ToString());
            if (termCollection == null)
            {
                var termSet = GetTermSetById(clientContext, termStoreId, termSetId);
                termCollection = LoadFlatTermAndChildren(clientContext, termSet);
                Cache.Set(termSetId.ToString(), termCollection);
            }
            return termCollection.FirstOrDefault(t => t.Id == termId);
        }

        public Term FindTermByName(ClientContext clientContext, string termName, Guid termStoreId, Guid termSetId)
        {
            var termCollection = Cache.Get<TermCollection>(termSetId.ToString());
            if (termCollection == null)
            {
                var termSet = GetTermSetById(clientContext, termStoreId, termSetId);
                termCollection = LoadFlatTermAndChildren(clientContext, termSet);
                Cache.Set(termSetId.ToString(), termCollection);
            }
            return termCollection.FirstOrDefault(t => t.Labels.FirstOrDefault(lbl => lbl.Value == termName) != null);
        }

        public TermSet GetTermSetById(ClientContext clientContext, Guid termStoreId, Guid termSetId)
        {
            try
            {
                TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
                var termStore = taxonomySession.TermStores.GetById(termStoreId);
                TermSet result = termStore.GetTermSet(termSetId);

                clientContext.Load(result, g => g.Id, g => g.Name, g => g.Description, g => g.IsOpenForTermCreation);
                clientContext.ExecuteQuery();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public void EnsureTermsInTermset(ClientContext clientContext, Guid termStoreId, Guid termSetId, List<SPTerm> terms)
        //{

        //    TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
        //    var termStore = taxonomySession.TermStores.GetById(termStoreId);
        //    TermSet termSet = termStore.GetTermSet(termSetId);

        //    clientContext.Load(termSet, g => g.Id, g => g.Name, g => g.Description, g => g.IsOpenForTermCreation);
        //    clientContext.ExecuteQuery();


        //    foreach (SPTerm term in terms)
        //    {
        //        var existedTermId = termSet.GetTerm(term.Id);
        //        clientContext.Load(existedTermId, g => g.Id, g => g.Name, g => g.Description, g => g.Labels);
        //        clientContext.ExecuteQuery();

        //        try
        //        {
        //            var name = existedTermId.Name;
        //        }
        //        catch (Exception ex)
        //        {
        //            Term newTerm = termSet.CreateTerm(term.Name, term.LCID, term.Id);
        //        }

        //    }

        //    termStore.CommitAll();
        //    clientContext.ExecuteQuery();

        //}

        private TermCollection LoadFlatTermAndChildren(ClientContext clientContext, TermSet termSet)
        {
            TermCollection termCollection = termSet.GetAllTerms();
            clientContext.Load(termCollection,
                      t => t.Include(
                          l => l.Name,
                          l => l.Description,
                          l => l.PathOfTerm,
                          l => l.Parent.Id,
                          l => l.TermsCount,
                          l => l.CustomSortOrder,
                          l => l.IsAvailableForTagging,
                          l => l.Id,
                          l => l.Labels.Include(
                                lbl => lbl.Value,
                                lbl => lbl.IsDefaultForLanguage,
                                lbl => lbl.Language
                                ),
                          l => l.Terms.Include(
                                k => k.Name,
                                k => k.Description,
                                k => k.PathOfTerm,
                                k => k.Parent.Id,
                                k => k.TermsCount,
                                k => k.CustomSortOrder,
                                k => k.IsAvailableForTagging,
                                k => k.Id,
                                k => k.Labels.Include(
                                    lbl => lbl.Value,
                                    lbl => lbl.IsDefaultForLanguage,
                                    lbl => lbl.Language)
                              )));
            clientContext.ExecuteQuery();
            return termCollection;
        }
    }
}
