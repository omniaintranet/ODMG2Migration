using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.UserProfiles;
using Models;
using Models.Moving;
using Newtonsoft.Json; 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using File = Microsoft.SharePoint.Client.File;

namespace Services
{
    public class CSOMServices
    {
        public ConnectionDetail SiteInfo { get; set; }
        //ClientContext ctxODM;
        private const string FIELD = "[{0}] {1} [{2}]";
        private const string FIELDVALUE = "{0}: {1}\n";
        //public AdvanceMapping AdvanceMapping { get; set; }
        public string DocumentTypeValue { get; set; }
        public string DocumentTypeName { get; set; }

        //public List<FieldInfo> DraftFields { get; set; }
        public CSOMServices(ConnectionDetail siteInfo)
        {
            this.SiteInfo = siteInfo;
        }

        public void LoadListInfo()
        {
            var ctx = this.SiteInfo.ODMSiteCtx;

            var web = ctx.Web;
            var listCollection = web.Lists;
            ctx.Load(listCollection);
            ctx.ExecuteQuery();
        }
        public ListItemSimple LoadDraftByDocId(object sender, string docId)
        {
            BackgroundWorker mybg = (BackgroundWorker)sender;
            var ctx =  this.SiteInfo.ODMSiteCtx;
            var libName  = this.SiteInfo.DestLibrary; 
            try
            { 
                var web = ctx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(libName);

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml =
                   @"<View Scope='RecursiveAll'>  
                        <Query> 
                           <Where><Eq><FieldRef Name='ODMDocId' /><Value Type='Text'>" + docId + @"</Value></Eq></Where> 
                        </Query> 
                         <ViewFields><FieldRef Name='FileLeafRef' /></ViewFields> 
                    </View>";

                var items = list.GetItems(camlQuery);
                ctx.Load(items, includes => includes.Include(i => i["Id"], i => i["FileRef"]));

                ctx.ExecuteQuery();

                return items.Select(i => new ListItemSimple()
                {
                    Id = i.Id,
                    FileRef = i.FieldValues["FileRef"].ToString(),
                    IsSelected = false
                }).FirstOrDefault(); 
            }
            catch (Exception ex)
            { 
                throw ex;
            }
        }

        public List<UserSimple> LoadAllODMUsers()
        {
            List<UserSimple> ret = new List<UserSimple>();
            try
            {
                 
                var context = this.SiteInfo.ODMSiteCtx;
                using (context)
                {
                    var web = context.Web;
                    var peopleManager = new PeopleManager(context);

                    context.Load(web, w => w.Title, w => w.Description, w => w.SiteUsers);
                    var siteUsers = web.SiteUsers;
                    context.ExecuteQuery();

                    foreach (var user in siteUsers)
                        if (user.PrincipalType == Microsoft.SharePoint.Client.Utilities.PrincipalType.User && user.Email != null && user.Email != "")
                        { 
                            ret.Add(new UserSimple() { 
                                UserName=user.Title,
                                UserEmail = user.Email
                            });
                        } 
                }
            }
            catch(Exception ex)
            {

            }
            return ret;
        }
        public string CheckFileVersions(BackgroundWorker bg, int id, List<FieldInfo> queryFields = null)
        {

            bg.ReportProgress((int)ReportProgress.REPORT, ">>Loading version history of list item: " + id);
            this.SiteInfo.SourceCtx.RequestTimeout = -1;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.RequestKeepAlive = false;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.KeepAlive = false;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.Timeout = -1;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.ReadWriteTimeout = -1;

            var web = this.SiteInfo.SourceCtx.Web;
            var listCollection = web.Lists;
            var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);
            var item = list.GetItemById(id);
            var file = item.File;
            this.SiteInfo.SourceCtx.Load(file, f => f.ListItemAllFields, f => f.CheckOutType, f => f.Title, f => f.Name);
            //this.SiteInfo.SourceCtx.Load(item);
            this.SiteInfo.SourceCtx.ExecuteQuery();
            bg.ReportProgress((int)ReportProgress.REPORT, ">>Loading list Item");
            var versionList = item.Versions;// item.File.Versions;
            this.SiteInfo.SourceCtx.Load(web);
            this.SiteInfo.SourceCtx.Load(versionList);//, vs => vs.Include(v => v.Url));

            try
            {
                bg.ReportProgress((int)ReportProgress.REPORT, ">>Loading version history");
                this.SiteInfo.SourceCtx.ExecuteQuery();
                bg.ReportProgress((int)ReportProgress.REPORT, ">>File Name: " + file.Name);
                string ret = file.Name.ToUpper() + Environment.NewLine;
                if (versionList != null)
                {
                    bg.ReportProgress((int)ReportProgress.REPORT, ">>Number of versions: " + versionList.Count);
                    for (int i = (versionList.Count - 1); i > -1; i--)
                    {
                        try
                        {
                            var version = versionList[i];
                            var x = version.FieldValues;
                            bg.ReportProgress((int)ReportProgress.REPORT, ">>Loading version: " + version.VersionLabel);
                            ret += "----------------------------" + Environment.NewLine;
                            ret += "- Version: " + version.VersionLabel + Environment.NewLine;
                            try
                            {

                                var fileVersion = version.FileVersion;
                                this.SiteInfo.SourceCtx.Load(fileVersion, vs => vs.Url);
                                this.SiteInfo.SourceCtx.ExecuteQuery();
                                bg.ReportProgress((int)ReportProgress.REPORT, ">>File version: " + fileVersion.Url);
                                //load file binary
                                string fileURL = fileVersion.Url;
                                ret += "- File URL:" + fileURL + Environment.NewLine;
                            }
                            catch (Exception ex)
                            {
                                bg.ReportProgress((int)ReportProgress.REPORT, ">>File version not found: ");
                            }
                            ret += "- Field values: " + version.FieldValues.Count() + Environment.NewLine;

                            foreach (var fieldValue in version.FieldValues)
                            {
                                if ((queryFields == null || queryFields.Count==0) && fieldValue.Value != null)
                                {
                                    ret += "\t" + fieldValue.Key + ":" + fieldValue.Value + Environment.NewLine;
                                }
                                else if (queryFields != null)
                                {
                                    foreach (var field in queryFields)
                                    {
                                        if (field.InternalName == fieldValue.Key || field.SpecialInternalSourceColumnName== fieldValue.Key)
                                        {
                                            ret += "\t" + fieldValue.Key + ":" + getFieldFromVersionValues(fieldValue.Value) + Environment.NewLine;
                                            break;
                                            //if(field.SpecialInternalSourceColumnName!="" && field.SpecialInternalSourceColumnName.Length > 1)
                                            //{
                                            //    DateTime dt = Convert.ToDateTime(fieldValue.Value);
                                            //    dt = dt.AddHours(2);
                                            //}
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            bg.ReportProgress((int)ReportProgress.ERROR, "Inner: " + ex.Message);
                            //file version belong to the major version
                        }
                    }
                }
                bg.ReportProgress((int)ReportProgress.DONE, ">>Done ");
                return ret;

            }
            catch (Exception ex)
            {
                bg.ReportProgress((int)ReportProgress.ERROR, "Inner: " + ex.Message);
            }
            return "";
        }
         
        private byte[] ToByteArray(Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        }

        public string getDocumentTypeValueByVersion(ListItemVersion version, FieldInfo dtFieldInfo )
        {
            foreach (var fieldValue in version.FieldValues)
            { 
                if (fieldValue.Key.ToLower() == dtFieldInfo.InternalName.ToLower())
                {
                    //documentTypeName = fieldInfo.Value;
                    var arrtmp = getFieldFromVersionValues(fieldValue.Value).Split('|');
                    return  arrtmp[0];
                }
            }
            return "";
        }

        public VersionData MigrateODMFileByVersion(BackgroundWorker bg, ListItemVersion version, List<FieldInfo> queryFields)
        {
            try
            { 
                var fileVersion = version.FileVersion;
                this.SiteInfo.SourceCtx.Load(fileVersion, vs => vs.Url, vs => vs.CheckInComment);
                this.SiteInfo.SourceCtx.ExecuteQuery();
                //bg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>Loading version: " + version.VersionLabel);
                VersionData 
                    ret = new VersionData()
                    {
                        FieldsValues = new List<FieldInfoValue>(),
                        VersionLabel = version.VersionLabel
                    };
                try
                {
                    //load file binary
                    string fileURL = "";
                    fileURL = fileVersion.Url;
                    bg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>File version[" + version.VersionLabel + "]: " + fileVersion.Url);
                    //download file 

                    this.SiteInfo.ODMSiteCtx.RequestTimeout = -1;
                    this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.RequestKeepAlive = false;
                    this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.WebRequest.KeepAlive = false;
                    this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.WebRequest.Timeout = -1;
                    this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.WebRequest.ReadWriteTimeout = -1;

                    var str = fileVersion.OpenBinaryStream();
                    this.SiteInfo.SourceCtx.ExecuteQuery();
                    byte[] binary = ToByteArray(str.Value);

                    ret.Binary = binary;
                    ret.FileRef = fileURL;
                    ret.FileStream = str.Value;
                    ret.CheckinComment = fileVersion.CheckInComment;

                }
                catch (Exception ex)
                {
                    //no file url
                    bg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>Not found file of this version");
                }
                bg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>Generate the values from mapping fields");
                foreach (var fieldValue in version.FieldValues)
                {

                    foreach (var field in queryFields)
                    {
                        if (field.InternalName == fieldValue.Key || field.SpecialInternalSourceColumnName == fieldValue.Key)
                        {
                            bg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>" + field.InternalName + "-" + field.SpecialInternalSourceColumnName + ":" + fieldValue.Value);
                            ret.FieldsValues.Add(new FieldInfoValue()
                            {
                                Value = getFieldFromVersionValues(fieldValue.Value),
                                Field = field,
                                OriginalValue = fieldValue.Value
                            });
                            //break;
                        }
                    }
                }
                return ret;
            }
            catch(Exception ex)
            {
               bg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>MigrateODMFileByVersion: version" + version.VersionLabel);
                bg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>MigrateODMFileByVersion: " + ex.Message);
                //throw ex;
            }

            return new VersionData()
            {
                FieldsValues = new List<FieldInfoValue>(),
                VersionLabel = version.VersionLabel 
            };
        }

        private string getFieldFromVersionValues(object value)
        {
            string ret = "";
            string type = value + "";//.ToString();
            if (type == "Microsoft.SharePoint.Client.FieldUserValue[]")
            {
                Microsoft.SharePoint.Client.FieldUserValue[] users = (Microsoft.SharePoint.Client.FieldUserValue[])value;
                foreach(var user in users)
                {
                    ret += user.Email + ";";
                }
            }
            else if (type == "Microsoft.SharePoint.Client.FieldUserValue")
            {
                Microsoft.SharePoint.Client.FieldUserValue user = (Microsoft.SharePoint.Client.FieldUserValue)value;
                ret =  user.Email;
            }
            else if (type == "Microsoft.SharePoint.Client.FieldLookupValue")
            {
                Microsoft.SharePoint.Client.FieldLookupValue term = (Microsoft.SharePoint.Client.FieldLookupValue)value;
                ret = term.LookupValue + "|00-0000;";
            }
            else if (type == "Microsoft.SharePoint.Client.FieldLookupValue[]")
            {
                Microsoft.SharePoint.Client.FieldLookupValue[] terms = (Microsoft.SharePoint.Client.FieldLookupValue[])value;
                foreach(var term in terms)
                {
                    ret += term.LookupValue + "|00-0000;";
                }
            }
            else if (type == "Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue[]")
            {
                Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue[] terms = (Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue[])value;
                foreach(var term in terms)
                {
                    ret += term.Label  + "|00-0000;";
                }
            }
            else if (type == "Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue")
            {
                Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue term = (Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValue)value;
                ret = term.Label + "|" + term.TermGuid;
            }
            else if (type == "Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValueCollection")
            {
                Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValueCollection data = (Microsoft.SharePoint.Client.Taxonomy.TaxonomyFieldValueCollection)value;
                foreach(var term in data)
                {
                    ret += term.Label  + "|00-0000;";
                }
            }
            else if (type == "System.String[]")
            {
                System.String[] arrStr = (System.String[])value;
                foreach(var str in arrStr)
                {
                    ret += str + ";";
                }
            }
            else if (type == "System.Collections.Generic.Dictionary`2[System.String,System.Object]")
            {
                Dictionary<string, object> data = (Dictionary<string, object>)value;
                try
                {
                    ret = data["Label"] + "|" + data["TermGuid"];
                }
                catch { 
                
                }
            }
            else
            {
                ret = type.ToString();
            }
            return ret;
        }

        public ListItemVersionCollection LoadFileVersions(int id)
        {
            try
            {
                this.SiteInfo.SourceCtx.RequestTimeout = -1;
                this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.RequestKeepAlive = false;
                this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.KeepAlive = false;
                this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.Timeout = -1;
                this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.ReadWriteTimeout = -1;

                var web = this.SiteInfo.SourceCtx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);
                var item = list.GetItemById(id);
                var file = item.File;
                this.SiteInfo.SourceCtx.Load(file, f => f.ListItemAllFields, f => f.CheckOutType, f => f.UniqueId, f => f.Title, f => f.Name);
                this.SiteInfo.SourceCtx.ExecuteQuery();
                var versionList = item.Versions;// item.File.Versions;
                this.SiteInfo.SourceCtx.Load(web);
                this.SiteInfo.SourceCtx.Load(versionList);//, vs => vs.Include(v => v.Url));

                this.SiteInfo.SourceCtx.ExecuteQuery();
                return versionList;
            }     
            catch(Exception ex)
            {
                return null;
            }
        }

        public string LoadFileFromDocId(string docId)
        {
            try
            {
                var web = this.SiteInfo.ODMSiteCtx.Web;
                var listCollection = web.Lists;
                ListInfo publishedLibrary = SiteInfo.DestLists.Where(x => x.EntityTypeName == "ODMPublished").FirstOrDefault();
                var list = listCollection.GetByTitle(publishedLibrary.Title);

                this.SiteInfo.ODMSiteCtx.Load(list.RootFolder);
                this.SiteInfo.ODMSiteCtx.Load(list.RootFolder.Folders);
                this.SiteInfo.ODMSiteCtx.ExecuteQuery();

                // This sentence returns the folder searching by its name
                var folder = list.RootFolder.Folders.FirstOrDefault(f => f.Name == docId);
                this.SiteInfo.ODMSiteCtx.Load(folder);
                this.SiteInfo.ODMSiteCtx.ExecuteQuery();
                this.SiteInfo.ODMSiteCtx.Load(folder.Files);
                this.SiteInfo.ODMSiteCtx.ExecuteQuery();

                var file = folder.Files.FirstOrDefault();
                //"/sites/dk-sls-kon-dok/ODMPublished/Doc-1016/3_05_049_Beredskabsplan_Terror & Terrortrussel.docx"
                //https://arrivagroup.sharepoint.com/sites/dk-sls-kon-dok
                var serverRelativeUrl = file.ServerRelativeUrl;
                var arrStr = serverRelativeUrl.Split('/');

                string ret = this.SiteInfo.OmniaODMSiteUrl;
                foreach (var str in arrStr)
                {
                    if (!this.SiteInfo.OmniaODMSiteUrl.Contains(str))
                    {
                        ret += '/' + str;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public ListItem UploadDocument(byte[] contents, string fileName)
        //{
        //    try
        //    {
        //        if(fileName.LastIndexOf('/')>0)
        //            fileName = fileName.Substring(fileName.LastIndexOf('/')+1);
                 
        //        Web web = this.SiteInfo.ODMSiteCtx.Web;
        //        List list = web.GetListByTitle(this.SiteInfo.DestLibrary);
        //        this.SiteInfo.ODMSiteCtx.Load(list);
        //        this.SiteInfo.ODMSiteCtx.ExecuteQuery();

        //        this.SiteInfo.ODMSiteCtx.RequestTimeout = -1;
        //        this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.RequestKeepAlive = false;
        //        this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.WebRequest.KeepAlive = false;
        //        this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.WebRequest.Timeout = -1;
        //        this.SiteInfo.ODMSiteCtx.PendingRequest.RequestExecutor.WebRequest.ReadWriteTimeout = -1;


        //        FileCreationInformation newFile = new FileCreationInformation();
        //        //newFile.Content = contents;  //bytes here
        //        newFile.ContentStream = new System.IO.MemoryStream(contents);
        //        newFile.Url = fileName;
        //        newFile.Overwrite = true;

        //        //To create the folder
        //        var folderName = Guid.NewGuid().ToString();
        //        Folder newFolder = list.RootFolder.Folders.Add(folderName);
        //        this.SiteInfo.ODMSiteCtx.Load(newFolder);
        //        this.SiteInfo.ODMSiteCtx.ExecuteQuery();
        //        File uploadingFile = newFolder.Files.Add(newFile);
                 

        //        this.SiteInfo.ODMSiteCtx.Load(uploadingFile);
        //        ListItem listItem = uploadingFile.ListItemAllFields;
        //        this.SiteInfo.ODMSiteCtx.Load(listItem);
        //        this.SiteInfo.ODMSiteCtx.ExecuteQuery();
        //        return listItem;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public byte[] GetFileByID(int id)
        {
            this.SiteInfo.SourceCtx.RequestTimeout = -1;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.RequestKeepAlive = false;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.KeepAlive = false;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.Timeout = -1;
            this.SiteInfo.SourceCtx.PendingRequest.RequestExecutor.WebRequest.ReadWriteTimeout = -1; 

            var web = this.SiteInfo.SourceCtx.Web;
            var listCollection = web.Lists;
            var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);
            var item = list.GetItemById(id);
            // Access the file 
            var file = item.File;
            this.SiteInfo.SourceCtx.Load(file);
            this.SiteInfo.SourceCtx.ExecuteQuery();
            if (file != null)
            {
                ClientResult<System.IO.Stream> data = file.OpenBinaryStream();
                SiteInfo.SourceCtx.Load(file);
                SiteInfo.SourceCtx.ExecuteQuery();
                using (System.IO.MemoryStream mStream = new System.IO.MemoryStream())
                {
                    if (data != null)
                    {
                        data.Value.CopyTo(mStream);
                        byte[] imageArray = mStream.ToArray();
                        return imageArray;
                    }
                }
            }
            return null;
        } 

        private List<ListItem> LoopInSubFolder(BackgroundWorker mybg, ClientContext clientContext, Folder folder)
        {
            List<ListItem> ret = new List<ListItem>();
            clientContext.Load(folder.Folders);
            clientContext.Load(folder.Files);
            clientContext.ExecuteQuery();
            foreach (var file in folder.Files)
            {
                clientContext.Load(file.ListItemAllFields);
            } 
            FolderCollection fcol = folder.Folders;
            List<string> lstFile = new List<string>();
            mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, string.Format("Folder {0} has {1} files and {2} subfolders: ", folder.Name, folder.Files.Count, folder.Folders.Count));

            bool isloaded = false;
            foreach (Folder subFolder in fcol)
            {
                ret.AddRange(LoopInSubFolder(mybg,clientContext, subFolder));
                isloaded = true;
            }
            if(!isloaded)
                clientContext.ExecuteQuery();
            ret.AddRange(folder.Files.Select(x => x.ListItemAllFields).ToList());
            return ret;
        }

        private List<ListItem> ThresholdLoadDatainSubFolder(BackgroundWorker mybg, ClientContext ctx, List list, Folder folder)
        { 
            ListItemCollectionPosition itemPosition = null;
            List<ListItem> retttt = new List<ListItem>();
            while (true)
            {
                CamlQuery camlQuery1 = new CamlQuery();

                camlQuery1.ListItemCollectionPosition = itemPosition;

                camlQuery1.ViewXml = @"<View> <Query><OrderBy Override=""TRUE""><FieldRef Name=""FileDirRef"" /><FieldRef Name=""FileLeafRef"" /></OrderBy></Query>
                                <ViewFields><FieldRef Name='FileLeafRef' /><FieldRef Name='Title'/><FieldRef Name='FSObjType' /><FieldRef Name='Id' /><FieldRef Name='FileRef' />
                                </ViewFields><RowLimit Paged='TRUE'>20</RowLimit></View>";
                
                camlQuery1.FolderServerRelativeUrl = folder.ServerRelativeUrl;
                ListItemCollection collListItem = list.GetItems(camlQuery1);

                ctx.Load(collListItem);

                ctx.ExecuteQuery();

                itemPosition = collListItem.ListItemCollectionPosition;

                foreach (ListItem oListItem in collListItem)
                {
                    Console.WriteLine("Title: {0}:", oListItem["Title"]);
                    if (oListItem["FSObjType"].ToString() == "1")
                    {
                        //folder
                        var subfolder = oListItem.Folder;
                        ctx.Load(subfolder);
                        ctx.ExecuteQuery();
                        mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, string.Format("Load data from folder:{0} - path: {1}", subfolder.Name, subfolder.ServerRelativeUrl));


                        retttt.AddRange(ThresholdLoadDatainSubFolder(mybg, ctx, list, subfolder));
                    }
                    else
                    {
                        retttt.Add(oListItem);
                        //list itme
                    }
                }

                if (itemPosition == null)
                {
                    break;
                }

                Console.WriteLine("\n" + itemPosition.PagingInfo + "\n");
            }
            return retttt;
        }

        public SourceData ThresholdLoadData(object sender, bool isSource = true)
        {
            BackgroundWorker mybg = (BackgroundWorker)sender;
            mybg.ReportProgress((int)ReportProgress.REPORT, "Loading data in safe mode");
            var ctx = isSource ? this.SiteInfo.SourceCtx : this.SiteInfo.ODMSiteCtx;
            var libName = isSource ? this.SiteInfo.SourceLibrary : this.SiteInfo.DestLibrary;
            SourceData ret = new SourceData();
            try
            {
                var web = ctx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(libName); 
                 
                ret.FieldsCollection = this.allSourceListFields(ctx, list);
                mybg.ReportProgress((int)ReportProgress.DONE, "Done!!!");
                return ret;

            }
            catch (Exception ex)
            {
                mybg.ReportProgress((int)ReportProgress.ERROR, ex.Message);
                throw ex;
            }
        }

        private List geerateODMUnProcessedList(ClientContext ctx)
        {
            var web = ctx.Web;
            var listCollection = web.Lists;
            ctx.Load(listCollection);
            ctx.ExecuteQuery();
            List ODMUnprocessed = null;
            foreach (var list in listCollection)
            {
                if (list.EntityTypeName == "ODMUnprocessed")
                {
                    ODMUnprocessed = list;
                    break;
                }
            }
            return ODMUnprocessed;
        }

        public void UpdateUnprocessFolderThreshold(BackgroundWorker mybg, string documentId, List<ReplaceField> replaceFields, Folder folder, List odmunprocessed)
        {
            var ctx = this.SiteInfo.SourceCtx;
            List ODMUnprocessed = odmunprocessed==null? geerateODMUnProcessedList(ctx): odmunprocessed;
            if (ODMUnprocessed == null) return;
            string ServerRelativeUrl = folder==null?(ODMUnprocessed.ParentWebUrl + "/ODMUnprocessed/" + documentId):folder.ServerRelativeUrl;
              
            ListItemCollectionPosition itemPosition = null;
            List<ListItem> retttt = new List<ListItem>();
            mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, string.Format("System update in path: {0}", ServerRelativeUrl));
            while (true)
            {
                CamlQuery camlQuery1 = new CamlQuery();

                camlQuery1.ListItemCollectionPosition = itemPosition;

                camlQuery1.ViewXml = @"<View> <Query><OrderBy Override=""TRUE""><FieldRef Name=""FileDirRef"" /><FieldRef Name=""FileLeafRef"" /></OrderBy></Query>
                                <ViewFields><FieldRef Name='FileLeafRef' /><FieldRef Name='Title'/><FieldRef Name='FSObjType' /><FieldRef Name='Id' /><FieldRef Name='FileRef' />
                                </ViewFields><RowLimit Paged='TRUE'>20</RowLimit></View>";

                camlQuery1.FolderServerRelativeUrl = ServerRelativeUrl;
                ListItemCollection collListItem = ODMUnprocessed.GetItems(camlQuery1);

                ctx.Load(collListItem);

                ctx.ExecuteQuery();

                itemPosition = collListItem.ListItemCollectionPosition;

                foreach (ListItem oListItem in collListItem)
                {
                    Console.WriteLine("Title: {0}:", oListItem["Title"]);
                    if (oListItem["FSObjType"].ToString() == "1")
                    {
                        //folder
                        var subfolder = oListItem.Folder;
                        ctx.Load(subfolder);
                        ctx.ExecuteQuery();
                        UpdateUnprocessFolderThreshold(mybg, documentId, replaceFields, subfolder, ODMUnprocessed);
                    }
                    else
                    {

                        //list item
                        UpdateItemWithFields(mybg, oListItem.Id, replaceFields, oListItem);                         
                    }
                }

                if (itemPosition == null)
                {
                    mybg.ReportProgress((int)ReportProgress.COLLECTION, retttt.Where(c => c.FieldValues.Count > 0).Select(i => new ListItemSimple()
                    {
                        Id = i.Id,
                        FileRef = i.FieldValues["FileRef"].ToString(),
                        IsSelected = false
                    }).ToList());
                    break;
                }

                Console.WriteLine("\n" + itemPosition.PagingInfo + "\n");
            }
            //return retttt;
        }

        private void ThresholdLoadDatainSubFolderwithThreading(BackgroundWorker mybg, ClientContext ctx, List list, Folder folder)
        {
            ListItemCollectionPosition itemPosition = null;
            List<ListItem> retttt = new List<ListItem>();
            while (true)
            {
                CamlQuery camlQuery1 = new CamlQuery();

                camlQuery1.ListItemCollectionPosition = itemPosition;

                camlQuery1.ViewXml = @"<View> <Query><OrderBy Override=""TRUE""><FieldRef Name=""FileDirRef"" /><FieldRef Name=""FileLeafRef"" /></OrderBy></Query>
                                <ViewFields><FieldRef Name='FileLeafRef' /><FieldRef Name='Title'/><FieldRef Name='FSObjType' /><FieldRef Name='Id' /><FieldRef Name='FileRef' />
                                </ViewFields><RowLimit Paged='TRUE'>20</RowLimit></View>";

                camlQuery1.FolderServerRelativeUrl = folder.ServerRelativeUrl;
                ListItemCollection collListItem = list.GetItems(camlQuery1);

                ctx.Load(collListItem);

                ctx.ExecuteQuery();

                itemPosition = collListItem.ListItemCollectionPosition;

                foreach (ListItem oListItem in collListItem)
                {
                    Console.WriteLine("Title: {0}:", oListItem["Title"]);
                    if (oListItem["FSObjType"].ToString() == "1")
                    {
                        //folder
                        var subfolder = oListItem.Folder;
                        ctx.Load(subfolder);
                        ctx.ExecuteQuery();
                        //mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, string.Format("Load data from folder:{0} - path: {1}", subfolder.Name, subfolder.ServerRelativeUrl));

                        ThresholdLoadDatainSubFolderwithThreading(mybg, ctx, list, subfolder);
                    }
                    else
                    {

                        //list itme

                        if (oListItem.FieldValues.Count > 0)
                        {
                            retttt.Add(oListItem);
                            if (retttt.Count > 100)
                            {
                                mybg.ReportProgress((int)ReportProgress.COLLECTION, retttt.Select(i => new ListItemSimple()
                                {
                                    Id = i.Id,
                                    FileRef = i.FieldValues["FileRef"].ToString(),
                                    IsSelected = false
                                }).ToList());
                                retttt = new List<ListItem>();
                            }
                        }
                    }
                }

                if (itemPosition == null)
                {
                    mybg.ReportProgress((int)ReportProgress.COLLECTION, retttt.Where(c => c.FieldValues.Count > 0).Select(i => new ListItemSimple()
                    {
                        Id = i.Id,
                        FileRef = i.FieldValues["FileRef"].ToString(),
                        IsSelected = false
                    }).ToList());
                    break;
                }

                Console.WriteLine("\n" + itemPosition.PagingInfo + "\n");
            }
            //return retttt;
        }
        public void LoadingPublishedDocuments(BackgroundWorker bg, string sourceSiteURL, string titleFilter = "Title")
        {
            try
            {
                var viewFields = titleFilter.Split(',').ToList();
                var odmsv = new ODMAPIServices(this.SiteInfo);
                PublishedQuery query = new PublishedQuery()
                {
                    pagingInfo = "",
                    webUrl = sourceSiteURL,
                    rowPerPage = "5000",
                    sortAsc = false,
                    sortBy = "ODMPublished",
                    viewFields =  new List<string>() { "Title", "FileRef" },
                    sqlIds = new List<object>()
                };
                bg.ReportProgress((int)ReportProgress.REPORT, "Loading published document from: " + sourceSiteURL);

                Root root;
                root = this.SiteInfo.ThresholdMode ? odmsv.FilterPublished(query) : odmsv.GetPublishedbyGraph(query);

                //var root =  odmsv.GetPublishedbyGraph(query);
                //var root = odmsv.FilterPublished(query);
                if (root.success)
                {
                    var data = JsonConvert.DeserializeObject<AllPublishedThreshold>(root.data.ToString());
                    bg.ReportProgress((int)ReportProgress.REPORT, "Total: " + data.documents.Count);
                    //bg.ReportProgress((int)ReportProgress.LOADPUBLISHEDDOCUMENTS, data.documents);
                    bg.ReportProgress((int)ReportProgress.COLLECTION, data.documents.Select(x => new ListItemSimple()
                    {
                        Id = x.id,
                        FileRef = x.fileUrl,
                        IsSelected = false
                    }).ToList());
                    bg.ReportProgress((int)ReportProgress.DONE, "Done");
                    

                }
                else
                {
                    bg.ReportProgress((int)ReportProgress.ERROR, "LoadingPublishedDocuments: " + root.errorMessage);
                }
            }
            catch (Exception ex)
            {
                bg.ReportProgress((int)ReportProgress.ERROR, "LoadingPublishedDocuments: " + ex.Message);
            }
        }
        public void ThresholdLoadDatawithThreading(object sender, bool isSource = true)
        {
            BackgroundWorker mybg = (BackgroundWorker)sender;
            mybg.ReportProgress((int)ReportProgress.REPORT, "Loading data in safe mode");
            var ctx = isSource ? this.SiteInfo.SourceCtx : this.SiteInfo.ODMSiteCtx;
            var libName = isSource ? this.SiteInfo.SourceLibrary : this.SiteInfo.DestLibrary;
            try
            {
                ctx.RequestTimeout = -1;
                var web = ctx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(libName);

                //EntityTypeName == "ODMPublished"

                //Generate source field
                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, "Generate source field2");
                ctx.Load(list);
                var FieldsCollection = this.allSourceListFields(ctx, list);
                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, "Generate source field1");
                mybg.ReportProgress((int)ReportProgress.SOURCEFIELDS, FieldsCollection);

                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, "Generate source field2");
                if (list.EntityTypeName== "ODMPublished")
                {
                    LoadingPublishedDocuments(mybg, this.SiteInfo.SourceCtx.Url);
                    return;
                }

                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, "Load data");
                ListItemCollectionPosition itemPosition = null;

                //List<ListItem> retttt = new List<ListItem>();
                while (true)
                {
                    CamlQuery camlQuery1 = new CamlQuery();

                    camlQuery1.ListItemCollectionPosition = itemPosition;

                    camlQuery1.ViewXml = @"<View><Query><OrderBy Override=""TRUE""><FieldRef Name=""FileDirRef"" /><FieldRef Name=""FileLeafRef"" /></OrderBy></Query>
                                <ViewFields><FieldRef Name='FileLeafRef' /><FieldRef Name='Title'/><FieldRef Name='FSObjType' /><FieldRef Name='Id' /><FieldRef Name='FileRef' />
                                </ViewFields><RowLimit>20</RowLimit></View>";

                    ListItemCollection collListItem = list.GetItems(camlQuery1);

                    ctx.Load(collListItem);

                    ctx.ExecuteQuery();

                    itemPosition = collListItem.ListItemCollectionPosition;

                    foreach (ListItem oListItem in collListItem)
                    {
                        //Console.WriteLine("Title: {0}:", oListItem["Title"]);
                        if (oListItem["FSObjType"].ToString() == "1")
                        {
                            //folder
                            var folder = oListItem.Folder;
                            ctx.Load(folder);
                            ctx.ExecuteQuery();
                            ThresholdLoadDatainSubFolderwithThreading(mybg, ctx, list, folder);

                            //retttt.AddRange(LoopInSubFolder(mybg, ctx, folder));
                        }
                        else
                        {
                            //retttt.Add(oListItem);
                            //list itme
                            if (oListItem.FieldValues.Count > 0)
                            {
                                mybg.ReportProgress((int)ReportProgress.ITEM, new ListItemSimple()
                                {
                                    Id = oListItem.Id,
                                    FileRef = oListItem.FieldValues["FileRef"].ToString(),
                                    IsSelected = false
                                });
                            }
                        }
                    }

                    if (itemPosition == null)
                    {
                        break;
                    }

                    Console.WriteLine("\n" + itemPosition.PagingInfo + "\n");
                }
                /*ret.ItemsCollection.AddRange(retttt.Where(c => c.FieldValues.Count > 0).Select(i => new ListItemSimple()
                {
                    Id = i.Id,
                    FileRef = i.FieldValues["FileRef"].ToString(),
                    IsSelected = false
                }));*/
                //ret.FieldsCollection = this.allSourceListFields(ctx, list);
                mybg.ReportProgress((int)ReportProgress.DONE, "Done!!!");
                //return ret;

            }
            catch (Exception ex)
            {
                mybg.ReportProgress((int)ReportProgress.ERROR, ex.Message);
                throw ex;
            }
        }

        public SourceData LoadSourceItems(object sender, bool isSource=true)
        {
            BackgroundWorker mybg = (BackgroundWorker)sender;
            mybg.ReportProgress((int)ReportProgress.REPORT, "Loading data in normal mode");
            var ctx = isSource ? this.SiteInfo.SourceCtx : this.SiteInfo.ODMSiteCtx;
            var libName = isSource ? this.SiteInfo.SourceLibrary : this.SiteInfo.DestLibrary;
            SourceData ret = new SourceData();
            try
            { 
                var web = ctx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(libName); 

                /*if (isSource)
                { 
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml =
                       @"<View Scope='RecursiveAll'>  
                        <Query> 
                           <Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>0</Value></Eq></Where> 
                        </Query> 
                         <ViewFields><FieldRef Name='FileLeafRef' /></ViewFields> 
                    </View>";

                    var items = list.GetItems(camlQuery);
                    ctx.Load(items, includes => includes.Include(i => i["Id"], i => i["FileRef"])); 
                    ctx.ExecuteQuery();

                    ret.ItemsCollection.AddRange(items.Select(i => new ListItemSimple()
                    {
                        Id = i.Id,
                        FileRef = i.FieldValues["FileRef"].ToString(),
                        IsSelected = false
                    })); 
                }*/
                ret.FieldsCollection = this.allSourceListFields(ctx, list);
                mybg.ReportProgress((int)ReportProgress.DONE, "Done!!!");

                return ret;

            }
            catch (Exception ex)
            {
                mybg.ReportProgress((int)ReportProgress.ERROR, ex.Message);
                throw ex;
            }
        }

        public void LoadSourceItemswithThreading(object sender, bool isSource = true)
        {
            BackgroundWorker mybg = (BackgroundWorker)sender;
            mybg.ReportProgress((int)ReportProgress.REPORT, "Loading data in normal mode");
            var ctx = isSource ? this.SiteInfo.SourceCtx : this.SiteInfo.ODMSiteCtx;
            var libName = isSource ? this.SiteInfo.SourceLibrary : this.SiteInfo.DestLibrary;
            SourceData ret = new SourceData();
            try
            {
                var web = ctx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(libName);
                //Generate source field
                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, "Generate source field1");
                var FieldsCollection = this.allSourceListFields(ctx, list);
                mybg.ReportProgress((int)ReportProgress.SOURCEFIELDS, FieldsCollection);

                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, "Load data");



                if (isSource)
                {
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml =
                       @"<View Scope='RecursiveAll'>  
                        <Query> 
                           <Where><Eq><FieldRef Name='FSObjType' /><Value Type='Integer'>0</Value></Eq></Where> 
                        </Query> 
                         <ViewFields><FieldRef Name='FileLeafRef' /></ViewFields> 
                    </View>";

                    var items = list.GetItems(camlQuery);
                    ctx.Load(items, includes => includes.Include(i => i["Id"], i => i["FileRef"]));
                    ctx.ExecuteQuery();

                    ret.ItemsCollection.AddRange(items.Select(i => new ListItemSimple()
                    {
                        Id = i.Id,
                        FileRef = i.FieldValues["FileRef"].ToString(),
                        IsSelected = false
                    }));
                    mybg.ReportProgress((int)ReportProgress.COLLECTION, ret.ItemsCollection);
                }
                //ret.FieldsCollection = this.allSourceListFields(ctx, list);
                mybg.ReportProgress((int)ReportProgress.DONE, "Done!!!");
                 

            }
            catch (Exception ex)
            {
                mybg.ReportProgress((int)ReportProgress.ERROR, ex.Message);
                throw ex;
            }
        }
        public List<FieldInfoValue> GetItemWithAllFields(int id, List<FieldInfo> queryFields)
        {
            var ret = new List<FieldInfoValue>();
            var web = this.SiteInfo.SourceCtx.Web;
            var listCollection = web.Lists;
            var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);
            var item = list.GetItemById(id);

            //List<FieldInfo> queryFields = (mappingFields == null || mappingFields.Count == 0) ?
            //    SiteInfo.SourceFields : mappingFields.Select(x => x.SourceObj).ToList();

            this.SiteInfo.SourceCtx.Load(item, i => i.DisplayName);
            foreach (var field in queryFields)
            {
                if (field.Type != FieldType.Computed && field.Type != FieldType.WorkflowEventType
                    && field.Type != FieldType.WorkflowStatus
                    && field.Type != FieldType.Calculated)
                    this.SiteInfo.SourceCtx.Load(item, i => i[field.InternalName]);
            }
            this.SiteInfo.SourceCtx.ExecuteQuery();

            foreach (var field in queryFields)
            {
                if (field.Type != FieldType.Computed && field.Type != FieldType.WorkflowEventType
                    && field.Type != FieldType.WorkflowStatus
                    && field.Type != FieldType.Calculated)
                {
                    if (field.Type == FieldType.Invalid)
                    {
                        try
                        {
                            var taxonomyFieldValue = item[field.InternalName] as TaxonomyFieldValue;
                            if (taxonomyFieldValue != null)
                            {
                                ret.Add(new FieldInfoValue()
                                {
                                    Value = taxonomyFieldValue != null ? getFieldValues(field, taxonomyFieldValue.Label + "|" + taxonomyFieldValue.TermGuid, true) : "",
                                    Field = field,
                                    OriginalValue = item[field.InternalName] 
                                });

                            }
                            else
                            {
                                var taxonomyFieldValues = item[field.InternalName] as TaxonomyFieldValueCollection;
                                if (taxonomyFieldValues != null)
                                {
                                    string retValue = "";
                                    foreach (var taxItem in taxonomyFieldValues)
                                    {
                                        retValue += getFieldValues(field, taxItem.Label + "|" + taxItem.TermGuid, true) + ";";
                                    }

                                    ret.Add(new FieldInfoValue()
                                    {
                                        Value = retValue,
                                        Field = field,
                                        OriginalValue = item[field.InternalName]
                                    });
                                }
                            }
                        }
                        catch
                        { 
                        }
                    }
                    else if (field.Type != FieldType.User && field.Type != FieldType.Lookup && field.Type != FieldType.MultiChoice)
                    {
                        ret.Add(new FieldInfoValue()
                        {
                            Value = getFieldValues(field, item[field.InternalName],true),
                            Field = field,
                            OriginalValue = item[field.InternalName]
                        });
                    }
                    else if (item[field.InternalName] != null)
                    {
                        ret.Add(new FieldInfoValue()
                        {
                            Value = getLookupFieldValues(item, field,true),
                            Field = field,
                            OriginalValue = item[field.InternalName]
                        }); 
                        //ret += getLookupFieldValues(item, field); ;// getUserFieldValues(item, field);
                    }
                    else
                    {
                        ret.Add(new FieldInfoValue()
                        {
                            Value = getFieldValues(field, "",true),
                            Field = field,
                            OriginalValue = item[field.InternalName]
                        }); 
                    }
                }
            }
            return ret;
        }
        public string ShowItemWithAllFields(int id, List<FieldInfo> queryFields)
        {
            string ret = "";
            var web = this.SiteInfo.SourceCtx.Web;
            var listCollection = web.Lists;
            var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);
            var item = list.GetItemById(id);

            if(queryFields==null || queryFields.Count == 0)
            {
                queryFields = SiteInfo.SourceFields;
            }
            //List<FieldInfo> queryFields = (mappingFields == null || mappingFields.Count == 0) ?
            //    SiteInfo.SourceFields : mappingFields.Select(x => x.SourceObj).ToList();

            this.SiteInfo.SourceCtx.Load(item, i => i.DisplayName);
            foreach (var field in queryFields)
            {
                if (field.Type != FieldType.Computed && field.Type != FieldType.WorkflowEventType
                    && field.Type != FieldType.WorkflowStatus
                    && field.Type != FieldType.Calculated)
                    this.SiteInfo.SourceCtx.Load(item, i => i[field.InternalName]);
            }
            this.SiteInfo.SourceCtx.ExecuteQuery();

            foreach (var field in queryFields)
            {
                if (field.Type != FieldType.Computed && field.Type != FieldType.WorkflowEventType
                    && field.Type != FieldType.WorkflowStatus
                    && field.Type != FieldType.Calculated)
                {                     
                    if (field.Type != FieldType.User && field.Type != FieldType.Lookup && field.Type != FieldType.MultiChoice)
                    {
                        try
                        {

                            ret += getFieldValues(field, item[field.InternalName]);
                        }
                        catch
                        {

                        }
                    }
                    else if(item[field.InternalName]!=null)
                    { 
                        ret += getLookupFieldValues(item, field); ;// getUserFieldValues(item, field);
                    }
                    else
                    {
                        ret += getFieldValues(field, "");
                    }
                }
            }
            return ret;
        }
        private string getFieldValues(FieldInfo field, object value, bool valueOnly=false)
        {
            if (valueOnly) return getFieldFromVersionValues(value) + "";
            return string.Format(FIELDVALUE, field.FieldTitle+" [" +field.InternalName+ "]", getFieldFromVersionValues(value));
        }
        private string getLookupFieldValues(ListItem item, FieldInfo field, bool valueOnly = false)
        {
            try
            {
                if(item[field.InternalName]==null)
                    return getFieldValues(field, null, valueOnly);

                if (item[field.InternalName].GetType() == typeof(string[]))
                {
                    var tmp = "";
                    string[] items = (string[])item[field.InternalName];
                    foreach(var str in items)
                    {
                        tmp += str + "|00-0000;";
                    }
                    return getFieldValues(field, tmp, valueOnly);
                }
                else if(item[field.InternalName].GetType()== typeof(string))
                {
                    return item[field.InternalName].ToString();
                }
                else if (item[field.InternalName].ToString().Contains("Microsoft.SharePoint.Client.FieldLookupValue[]"))
                {
                    Microsoft.SharePoint.Client.FieldLookupValue[] items = (Microsoft.SharePoint.Client.FieldLookupValue[])item[field.InternalName];
                        var tmp = "";
                    foreach(var val in items)
                    {
                        tmp += val.LookupValue + "|00-0000;";
                    }
                    return getFieldValues(field, tmp, valueOnly);
                } 
                FieldLookupValue user = (FieldLookupValue)item[field.InternalName];
                return user != null ? getFieldValues(field, user.LookupValue, valueOnly) : "";
            }
            catch (Exception ex)
            {
                return getUserFieldValues(item, field, valueOnly);
                /*FieldLookupValue[] users = (FieldLookupValue[])item[field.InternalName];
                var tmp = "";
                foreach (var user in users)
                {
                    tmp += user != null ? string.Format(FIELDVALUE, field.FieldTitle, user.LookupValue) : "";
                }
                return tmp;*/
            }
        }

        private string getUserFieldValues(ListItem item, FieldInfo field, bool valueOnly = false)
        {
            try
            { 
                FieldUserValue user = (FieldUserValue)item[field.InternalName];
                return user != null ? getFieldValues(field, user.Email, valueOnly) : "";
            }
            catch(Exception ex)
            {
                FieldUserValue[] users = (FieldUserValue[])item[field.InternalName];
                var tmp = "";
                foreach(var user in users)
                {
                    tmp+= user != null ? getFieldValues(field, user.Email!=""?user.Email: user.LookupValue, valueOnly) : "";
                    tmp += ";";
                }
                return tmp;
            }
        }

        //public ListItem GetItemDetail(int id, List<MappingField> mappingFields = null)
        //{
        //    var web = this.SiteInfo.SourceCtx.Web;
        //    var listCollection = web.Lists;
        //    var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);
        //    var item = list.GetItemById(id);
        //    List<FieldInfo> queryFields = (mappingFields == null || mappingFields.Count == 0) ?
        //        this.SiteInfo.SourceFields : mappingFields.Select(x => x.SourceObj).ToList();
        //    this.SiteInfo.SourceCtx.Load(item, i => i.DisplayName);

        //    foreach (var field in queryFields)
        //    {
        //        if (field.Type != FieldType.Computed && field.Type != FieldType.WorkflowEventType
        //            && field.Type != FieldType.WorkflowStatus
        //            && field.Type != FieldType.Calculated)
        //            this.SiteInfo.SourceCtx.Load(item, i => i[field.InternalName]);
        //    }
        //    this.SiteInfo.SourceCtx.ExecuteQuery();
        //    return item;
        //}
        private List<FieldInfo> allSourceListFields(ClientContext ctx, List list)
        {
            List<FieldInfo> ret = new List<FieldInfo>(); 
            var contentTypeCollection = list.ContentTypes;
            ctx.Load(contentTypeCollection);
            ctx.ExecuteQuery();// Async(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));

            foreach (var item in contentTypeCollection)
            {
                var fields = item.Fields;
                ctx.Load(fields);
                ctx.ExecuteQuery();
                ret.AddRange(fields.Select(x => new FieldInfo()
                {
                    ContentType = item.Name,
                    InternalName = x.InternalName,
                    FieldTitle = x.Title,
                    Title = string.Format(FIELD, item.Name, x.Title, x.InternalName),
                    Type = x.FieldTypeKind,
                    Id = x.Id.ToString()
                }));
            }
            return ret;
        }  
        public string GetItemWithAllFields(int id, List<ReplaceField> replaceFields)
        {
            string ret = "";
            try
            {

                var web = this.SiteInfo.SourceCtx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);
                var item = list.GetItemById(id);

                List<FieldInfo> queryFields = replaceFields.Select(x => x.Source).ToList();

                foreach (var itemRp in replaceFields)
                {
                    if (queryFields.Where(x => x.InternalName == itemRp.Dest.InternalName).ToList().Count == 0)
                    {
                        queryFields.Add(itemRp.Dest);
                    }
                }
                //queryFields.AddRange(replaceFields.Select(x => x.Dest).ToList());

                this.SiteInfo.SourceCtx.Load(item, i => i.DisplayName);
                foreach (var field in queryFields)
                {
                    if (field.Type != FieldType.Computed && field.Type != FieldType.WorkflowEventType
                        && field.Type != FieldType.WorkflowStatus
                        && field.Type != FieldType.Calculated)
                        this.SiteInfo.SourceCtx.Load(item, i => i[field.InternalName]);
                }
                this.SiteInfo.SourceCtx.ExecuteQuery();

                for (int idx = 0; idx < replaceFields.Count; idx++)
                {
                    var replaceItem = replaceFields[idx];
                    foreach (var field in queryFields)
                    {
                        if (field.Type != FieldType.Computed
                            && field.Type != FieldType.WorkflowEventType
                            && field.Type != FieldType.WorkflowStatus
                            && field.Type != FieldType.Calculated)
                        {
                            if (field.InternalName == replaceItem.Source.InternalName || field.InternalName == replaceItem.Dest.InternalName)
                            {
                                if (field.Type != FieldType.User && field.Type != FieldType.Lookup && field.Type != FieldType.MultiChoice)
                                {
                                    try
                                    {
                                        ret += (idx + 1) + ". " + getFieldValues(field, item[field.InternalName]);
                                    }
                                    catch
                                    {

                                    }
                                }
                                else if (item[field.InternalName] != null)
                                {
                                    ret += (idx + 1) + ". " + getLookupFieldValues(item, field); ;// getUserFieldValues(item, field);
                                }
                                else
                                {
                                    ret += (idx + 1) + ". " + getFieldValues(field, "");
                                }
                            }
                        }
                    }
                    ret += "\n";
                }
            }
            catch(Exception ex)
            {
                ret = ex.Message;
            }
                
            return ret;
        }
        public bool UpdateItemWithFields(BackgroundWorker mybg, int id, List<ReplaceField> replaceFields, ListItem item=null)
        {
            try
            {
                mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, string.Format("List item id: {0}", id));
                bool ret = true;
                
                var web = this.SiteInfo.SourceCtx.Web;
                var listCollection = web.Lists;
                var list = listCollection.GetByTitle(this.SiteInfo.SourceLibrary);

                item = item==null? list.GetItemById(id): item;
                //var file = item.File;
                //this.SiteInfo.SourceCtx.Load(file, f => f.ListItemAllFields, f => f.CheckOutType, f => f.Title, f => f.Name);

                List<FieldInfo> queryFields = replaceFields.Select(x => x.Source).ToList();

                foreach (var itemRp in replaceFields)
                {
                    if (queryFields.Where(x => x.InternalName == itemRp.Dest.InternalName).ToList().Count == 0)
                    {
                        queryFields.Add(itemRp.Dest);
                    }
                }

                //this.SiteInfo.SourceCtx.Load(item, i => i.DisplayName);
                foreach (var field in queryFields)
                {
                    if (field.Type != FieldType.Computed && field.Type != FieldType.WorkflowEventType
                        && field.Type != FieldType.WorkflowStatus
                        && field.Type != FieldType.Calculated)
                        this.SiteInfo.SourceCtx.Load(item, i => i[field.InternalName]);
                }
                this.SiteInfo.SourceCtx.ExecuteQuery();
                //mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, string.Format("File: {0}", file.Name));


                foreach (var replaceItem in replaceFields)
                {
                    var val = item[replaceItem.Dest.InternalName];
                    mybg.ReportProgress((int)ReportProgress.REPORTDETAIL, ">>replaceItem [" + replaceItem.Dest.InternalName + "]:" + val);
                    if (val != null && (val + "") != "")
                        getFieldFromValues(item, replaceItem);
                }
                item.Update();// (listItemFormUpdateValueColl, true, "System update", true); 
                this.SiteInfo.SourceCtx.ExecuteQuery();
                return ret;

            }
            catch(Exception ex)
            {
                mybg.ReportProgress((int)ReportProgress.ERRORDETAIL, ex.Message);
                return false;
            }
        }


        private void getFieldFromValues(ListItem item, ReplaceField replaceItem)
        {
            string ret = "";
            string type = item[replaceItem.Dest.InternalName].GetType().FullName;
            var value = item[replaceItem.Dest.InternalName];
            if (type == "Microsoft.SharePoint.Client.FieldUserValue" && value!=null)
            {
                Microsoft.SharePoint.Client.FieldUserValue user = (Microsoft.SharePoint.Client.FieldUserValue)value;
                if (user != null)
                {
                    if ((replaceItem.DefaultValue + "") != "")
                    {
                        item[replaceItem.DefaultValue] = user.LookupId;
                    }
                    item[replaceItem.Source.InternalName] = user.LookupId;
                }
            } 
            else if (type == "System.DateTime")
            {
                DateTime dt;
                if (DateTime.TryParse(value.ToString(), out dt))
                {
                    item[replaceItem.Source.InternalName] = dt;
                }
            } 
        }
    }
}
