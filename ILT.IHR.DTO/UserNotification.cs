using System;
using System.Collections.Generic;
using System.Text;

namespace ILT.IHR.DTO
{
	public class UserNotification : AbstractDataObject
	{
		public int NotificationID { get; set; }
		public string Module { get; set; }
		public int UserID { get; set; }
		public Boolean IsAck { get; set; }
		public DateTime? AckDate { get; set; }

	}
}
