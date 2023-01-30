using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ILT.IHR.DTO
{
    public class Element
    {
        public string Position { get; set; }
        public string ElementType { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string GridDisplay { get; set; }
        public string Required { get; set; }
        public string FullWidth { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
