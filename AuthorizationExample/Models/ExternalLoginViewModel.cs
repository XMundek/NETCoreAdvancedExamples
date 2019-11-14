using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationExample.Models
{
    public class ExternalLoginViewModel
    {
        public string[] Providers { get; set; }
        public string ReturnUrl;
    }
}
