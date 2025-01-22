using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleSignOnUtility.DTOs
{
    public class AppleUserProfile
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Locale { get; set; }
    }
}
