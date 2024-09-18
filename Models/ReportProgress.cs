using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ReplaceItem
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        //public string AddTerms { get; set; }
    }
    public enum ReportProgress
    {
        VERSIONHISTORY =10,
        COUNTER = 20,
        REPORTSTART = 40,
        REPORTREPUBLISHED = 45,
        REPORT = 50,
        REPORTDETAIL = 51,
        REPORTDETAILBOLD = 52,
        REPORTCSV = 53,
        REPORTDETAILNOTICE = 54,
        ERROR = 60,
        ERRORDETAIL = 61,
        DONE = 100,
        ITEM = 80,
        COLLECTION = 81,
        SOURCEFIELDS = 82,
        LOADPUBLISHEDDOCUMENTS = 200 ,
        REPORTITEMS= 300,
        REPORTUPDATEITEM = 301,
        SAFELOADING = 400,
        PUBLISHEDSITES = 500
    }
    public enum RunMode
    {
        MigrateData=1,
        MovingDocument=2,
        ArchiveData=3,
        ViewVersion=4,
        LoadingPublishedDocument=5,
        LoadingPublishedSites = 51,
        SystemUpdate = 6,
        DeleteData = 7,
        CorrectPublishedStatus = 8,
        TestPermission = 20,
        MigrateDummyData = 30,
        UpdateReviewDate=40,
        RePublish=50
    }
}
