using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public partial class Asset : AbstractDataObject
    {
        public int AssetID
        {
            get { return base.RecordID; }
            set { base.RecordID = value; }
        }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int AssetTypeID { get; set; }
        public string AssetType { get; set; }
        [Required]
        public string Tag { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        [Required]
        public string Configuration { get; set; }
        public string WiFiMAC { get; set; }
        public string LANMAC { get; set; }
        public string OS { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        public DateTime? WarantyExpDate { get; set; }
       // [RegularExpression("^0*[1-9]\\d*$")]
        public int? AssignedToID { get; set; }
        public string AssignedTo { get; set; }
        [Required]
        [RegularExpression("^0*[1-9]\\d*$")]
        public int StatusID { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }
}
