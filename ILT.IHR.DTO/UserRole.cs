using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public class UserRole : AbstractDataObject
    {
        public int UserRoleID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string RoleShort { get; set; }
        public string RoleName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelected { get; set; }
    }
}
