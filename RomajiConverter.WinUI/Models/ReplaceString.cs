using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomajiConverter.WinUI.Models
{
    public class ReplaceString
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public bool IsSystem { get; set; }

        public ReplaceString(int id, string value, bool isSystem)
        {
            Id = id;
            Value = value;
            IsSystem = isSystem;
        }
        
        public override string ToString()
        {
            return Value;
        }
    }
}
