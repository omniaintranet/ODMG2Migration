using Models; 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ODMProcesses
    {
        public List<MappingColumnsName> MappingColumns { get; set; }
        public AdvanceMapping AdvanceMapping { get; set; }
        private CSOMServices csomsv;
        public ConnectionDetail SiteInfo { get; set; }
        private string MappingFields;
        public List<ListItemSimple> SelectedItems { get; set; }

        public ODMProcesses(ConnectionDetail siteInfo)
        {
            this.SiteInfo = siteInfo;
        }

        public string ViewSelectedItemwithReplacingField(string mappingFields, int listItemId)
        {
            csomsv = new CSOMServices(this.SiteInfo);
            MappingFields = mappingFields;
            var ret = generateMappingSourceFields(mappingFields);
            //mybg.ReportProgress((int)ReportProgress.DONE, "DONE!!!");
            csomsv = new CSOMServices(SiteInfo);
            return csomsv.GetItemWithAllFields(listItemId, ret);
        }

        public void UpdateMetaDatabyReplacement(BackgroundWorker mybg, string mappingFields)
        {
            mybg.ReportProgress((int)ReportProgress.REPORT, "Start updating data");
            csomsv = new CSOMServices(this.SiteInfo);
            MappingFields = mappingFields;
            var ret = generateMappingSourceFields(mappingFields);
            csomsv = new CSOMServices(SiteInfo);
            foreach (var odmFile in SelectedItems)
            {
                ReplaceData(mybg, odmFile, ret);
            }
            mybg.ReportProgress((int)ReportProgress.DONE, "Done");
        }

        public void ReplaceData(BackgroundWorker mybg, ListItemSimple odmFile, List<ReplaceField> replaceFields)
        {
            try
            {
                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">> >> >> >> >> >> >> >> >>");
                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>ODMFile:" + odmFile.FileRef);   
                csomsv.UpdateItemWithFields(mybg, odmFile.Id, replaceFields);

                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>Continue updating for UnProcess Library with ODM Doc-id:");
                var fields = csomsv.GetItemWithAllFields(odmFile.Id, this.SiteInfo.SourceFields.Where(x => x.InternalName == "ODMDocId").ToList());
                if (fields.Count > 0)
                {
                    mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>Doc-id:" + fields[0].Value);
                    csomsv.UpdateUnprocessFolderThreshold(mybg, fields[0].Value, replaceFields, null, null);
                }
                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>Success!");
            }
            catch ( Exception ex)
            {
                mybg.ReportProgress((int)ReportProgress.ERRORDETAIL, "ReplaceData:" + ex.Message);
            }
        }
        private List<ReplaceField> generateMappingSourceFields(string mappingFields)
        {
            List<ReplaceField> ret = new List<ReplaceField>(); 
            MappingColumns = new List<MappingColumnsName>();
            var arr = mappingFields.Split('\n');

            foreach (var row in arr)
            {
                var tmp = row.Trim().Replace("=>", "Æ");
                if (tmp != "")
                {
                    var columns = tmp.Split('Æ');
                    string sourceCol, destCol, defaulValue;
                    sourceCol = columns[0].Trim();
                    destCol = columns.Length > 1 ? columns[1].Trim() : columns[0].Trim();
                    defaulValue = "";
                    if (destCol.Contains("#"))
                    {
                        //default val
                        var defaultVal = destCol.Split('#');
                        sourceCol = sourceCol != destCol ? sourceCol : defaultVal[0].Trim();
                        destCol = defaultVal[0].Trim();
                        defaulValue = defaultVal[1];
                    }
                    MappingColumns.Add(
                        new MappingColumnsName()
                        {
                            SourceColumnName = sourceCol,//.Replace("*", ""),
                            DestinationColumnName = destCol,//.Replace("*", "")
                            DefaultValue = defaulValue
                        });
                }
            } 
           foreach(var item in MappingColumns)
            {
                ReplaceField replaceField = new ReplaceField();

                foreach (var field in SiteInfo.SourceFields)//AdvanceMapping.SourceFieldsCollection)
                {
                    if (field.InternalName.ToLower() == item.SourceColumnName.ToLower())
                    {
                        replaceField.Source = field;
                    }
                    if (field.InternalName.ToLower() == item.DestinationColumnName.ToLower())
                    {
                        replaceField.Dest = field;
                    }
                }
                if (replaceField.Source != null)
                {
                    if(item.DefaultValue != null)
                    {
                        replaceField.DefaultValue = item.DefaultValue;
                    }
                    ret.Add(replaceField);
                }
            }
            return ret;
        }
    }
}
