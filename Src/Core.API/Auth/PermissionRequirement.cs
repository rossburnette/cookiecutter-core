using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.API.Auth
{
    public class PermissionRequirement: IAuthorizationRequirement
    {
        public IEnumerable<string> AllowedRoles { get; set; }

        public PermissionRequirement() { }

        public PermissionRequirement(IEnumerable<string>  allowedRoles) {
            AllowedRoles = allowedRoles;
        }
    }
}
