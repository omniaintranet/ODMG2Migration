using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public class HTTPSv:BaseService
    {
        public UserGroupRoot postData(string api, List<HeaderData> headers, object body)
        { 
            UserGroupRoot ret = CreatePostRequestWithHeaders<UserGroupRoot>(api, body, headers);
            return ret;
        }
    }
}
