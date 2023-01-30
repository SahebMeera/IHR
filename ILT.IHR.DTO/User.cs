using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public class User : AbstractDataObject
    {
        public int UserID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        public int? EmployeeID { get; set; }
        public string EmployeeCode { get; set; }       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        public string Email { get; set; }
        public string RoleName { get; set; }        
        public string RoleShort { get; set; }        
        public string Password { get; set; }
        public Boolean IsOAuth { get; set; }
        public Boolean IsActive { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string ClientID { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }        
    }

    public class UserToVlidate
    {
        public string email { get; set; }
        public string password { get; set; }
        public string clientID { get; set; }
    }
}
