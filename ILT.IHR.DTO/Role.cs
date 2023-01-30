using System;
using System.Collections.Generic;

namespace ILT.IHR.DTO
{
    public partial class Role : AbstractDataObject
    {
        public int RoleID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public string RoleShort { get; set; }
        public string RoleName { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }
}
