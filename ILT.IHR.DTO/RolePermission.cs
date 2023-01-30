using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public class RolePermission: AbstractDataObject
    {

		public int RolePermissionID
		{
			get { return base.RecordID; }
			set { base.RecordID = value; }
		}
		public int RoleID { get; set; }
		public string RoleName { get; set; }
		[Required]
		[Range(1, 100000)]
		public int ModuleID { get; set; }
		public string ModuleShort { get; set; }
		public string ModuleName { get; set; }
		public bool View { get; set; }
		public bool Add { get; set; }
		public bool Update { get; set; }
		public bool Delete { get; set; }

	}
}
