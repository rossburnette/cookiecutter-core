using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.AuthStore
{
    public class ApiStore
    {
        private static readonly List<ApiItem> _apis = new List<ApiItem>()
        {
            new ApiItem {
                Api="authtest/tenant",
                Roles=new List<string>(new string[]{"admin","test1" })
            }
        };

        public List<ApiItem> GetAll()
        {
            return _apis;
        }

        public ApiItem Find(string id)
        {
            return _apis.Find(_ => _.Api == id);
        }

    }

    public class ApiItem
    {

        public string Api { get; set; }
        public IEnumerable<string> Roles { get; set; }
        //Roles=new List<string>(new string[]{"admin","test" }) 
    }
}
