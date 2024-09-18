using Newtonsoft.Json;
using Models;
using Models.DocumentTypes;
using Models.DraftObjectResult;
using Models.EnterpriseProperties; 
using Models.TeamApp;
using Models.UnProcessedDocument; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OmniaTeamAppServices : BaseService
    {
        public ConnectionDetail SiteInfo { get; set; }
        public OmniaTeamAppServices(ConnectionDetail siteInfo)
        {
            SiteInfo = siteInfo;
        }

        public Root GetBusinessProfile()
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaAPI + "/api/businessprofiles");
                Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root GetPermissionBindings(string profileid, string appinstanceid)
        {
            try
            {
                //https://novolocus.omniacloud.net/api/security/roles/getpermissionbindings?profileid=24a20be2-f8cf-478e-b69c-544866dff28a&appinstanceid=d596ccf0-b1b8-45b1-83d6-0ed90d5255fb&pageid=3
                string api = EnsureCorrectAPI(SiteInfo.OmniaAPI + "/api/security/roles/getpermissionbindings?profileid={0}&appinstanceid={1}&pageid=3");
                api = string.Format(api, profileid, appinstanceid);
                List<string> data = new List<string>();
                data.Add("f17d076c-d46b-43fd-94e2-e664dd43ed92");
                Root ret = CreatePostRequestWithHeaders<Root>(api, data, SiteInfo.NetworkHeaders);

                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public Root GetAllTeamApps(string profileid, AppInstancesRequest appInstancesRequest)
        {
            try
            {
                string api = EnsureCorrectAPI(SiteInfo.OmniaAPI + "/api/apps/profiles/definitions/d2240d7b-af3c-428c-bae8-5b8bfc08e3ac/instances?profileId={0}&showInPublicListingsOnly=true");
                api = string.Format(api, profileid);
                Root ret = CreatePostRequestWithHeaders<Root>(api, appInstancesRequest, SiteInfo.NetworkHeaders);

                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public Root UpdatePermissionBindings(string profileid, string appinstanceid, PermissionSettings permissionSettings)
        {
            try
            {
                //https://novolocus.omniacloud.net/api/security/roles/getpermissionbindings?profileid=24a20be2-f8cf-478e-b69c-544866dff28a&appinstanceid=d596ccf0-b1b8-45b1-83d6-0ed90d5255fb&pageid=3
                string api = EnsureCorrectAPI(SiteInfo.OmniaAPI + "/api/security/update/settings?profileid={0}&appinstanceid={1}&pageid=3");
                api = string.Format(api, profileid, appinstanceid);
                Root ret = CreatePostRequestWithHeaders<Root>(api, permissionSettings, SiteInfo.NetworkHeaders);

                return ret;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
