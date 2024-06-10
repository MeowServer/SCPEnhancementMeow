using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCPEnhancementMeow.Enhancer
{

    [AttributeUsage(AttributeTargets.Class)]
    public class EnhancerAttributeAttribute : Attribute
    {
        private readonly RoleTypeId roleType;

        public EnhancerAttributeAttribute(RoleTypeId roleType)
        {
            this.roleType = roleType;
        }

        public RoleTypeId fitRoleTypes
        {
            get 
            { 
                return roleType; 
            }
        }
    }
}
