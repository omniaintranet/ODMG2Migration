using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DefaultSettings
    {
        public List<MappingAccount> MappingAccounts { get; set; }
        public string DefaultUser { get; set; }

        public string SourceUrl { get; set; }
        public string TermStoreId { get; set; }
        public string OmniaCookies { get; set; }
        public string OmniaODMSiteUrl { get; set; }
        public string OmniaAPI { get; set; }
        public string OmniaODMAPI { get; set; }

        public bool convertToPDF { get; set; }
        public string convertToPDFwithFileTypes { get; set; }
        public bool isRevisionPublishing { get; set; } 
        public bool isLimitedAccess { get; set; }
        public List<LimitedUser> limitedUsers { get; set; }
        public List<NotifiedUser> notifiedUsers { get; set; }
        public bool isReadReceiptRequired { get; set; }

        public bool attachDocumentTemplate { get; set; }
        public bool keepingVersionHistory { get; set; }
        public bool mergeTemplate { get; set; }
        public bool onlyMigrateLatestEdition { get; set; }
        public bool onlyMigrateLatestAndDraft { get; set; }
        public bool onlyDraft { get; set; }

          
        public string MappingColumnsFilter { get; set; }
        public string comment { get; set; }

        public string DynamicDocumentTypeFieldName { get; set; }
        public string ODMContentLanguage { get; set; }
        public string FixedDocumentTypeName { get; set; }
        public bool UseFixedDocumentType { get; set; } 
        public bool SameUser { get; set; }




        public string MovingDocSource { get; set; }
        public string MovingDocTarget { get; set; }
        public string MovingDocReplaceUser { get; set; }
        public string MovingDocFilter { get; set; }
        public string SystemUpdateReplaceFields { get; set; }
        public string MovingViewFields { get; set; }

    }
}
