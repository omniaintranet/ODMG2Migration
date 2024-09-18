using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.TeamApp
{
    public class PermissionSettings
    {
        public List<PermissionSetting> permissionSettings { get; set; }
        public PermissionContextParam permissionContextParam { get; set; }
    }

    public class PermissionSetting
    {
        public string roleId { get; set; }
        public List<string> identities { get; set; }
        public List<string> groups { get; set; }
        public List<object> userDefinedRules { get; set; }
    }

    public class PermissionContextParam
    {
        public string profileid { get; set; }
        public string appinstanceid { get; set; }
        public int pageid { get; set; }
    }
}
