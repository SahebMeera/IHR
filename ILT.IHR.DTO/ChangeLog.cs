using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ILT.IHR.DTO
{
    public partial class ChangeLog : AbstractDataObject
    {
        public string FieldName { get; set; }
        public string Value { get; set; }
        public string OldValue { get; set; }

    }
}
