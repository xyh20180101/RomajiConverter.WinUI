using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomajiConverter.WinUI.Models
{
    public class ReplaceString
    {
        public string Value { get; set; }

        public bool IsSystem { get; set; }

        public ReplaceString(string value, bool isSystem)
        {
            Value = value;
            IsSystem = isSystem;
        }
        
        public override string ToString()
        {
            return Value;
        }
    }
}
