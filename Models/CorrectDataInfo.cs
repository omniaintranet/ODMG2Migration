using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client; 
namespace Models
{
    public enum RecordManagement
    {
        NULL=-1,
        Lock = 1,
        UnLock = 0,
        Refresh = 2
    } 

    public enum ShowDataMode
    {
        ViewProperties=1,
        MappingProperties=2

    }
    public class CorrectDataInfo
    {
        public object SelectedNode { get; set; }
        public bool Update { get; set; }
        public bool AddNewTerms { get; set; }
        public string[] ExcludeFormat { get; set; }
        public string TestIds { get; set; }
        public string SiteUrl { get; set; }
        public string ReplacementJsonString { get; set; }
        public string ColumnName { get; set; }

        public SP.ClientContext ClientContext { get; set; }
        public SP.List List { get; set; }

        public RecordManagement Record { get; set; }
        public string[] Fields { get; set; }

        //public object PSService { get; set; }

        public List<ReplaceItem> ReplaceItems { get; set; } 

        public List<Field> LoadUpdatedFields()
        {
            var ret = new List<Field>();
            SP.FieldCollection columns = List.Fields;
            ClientContext.Load(columns);            
            ClientContext.ExecuteQuery();
            foreach (var field in Fields)
            {
                SP.Field taxField = columns.GetByInternalNameOrTitle(field);
                ClientContext.Load(taxField);
                ret.Add(taxField);
            }
            ClientContext.ExecuteQuery();
            return ret;
        }
    }
}
