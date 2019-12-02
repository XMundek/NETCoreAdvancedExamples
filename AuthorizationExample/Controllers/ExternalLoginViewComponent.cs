using AuthorizationExample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationExample.Components
{
    public class ExternalLoginViewComponent:ViewComponent
    {
        IAuthenticationSchemeProvider _schemeProvider;
        public ExternalLoginViewComponent(IAuthenticationSchemeProvider schemeProvider)
        {
            _schemeProvider = schemeProvider;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var res = await _schemeProvider.GetRequestHandlerSchemesAsync();
            return
                View(new ExternalLoginViewModel()
                {
                    ReturnUrl = Request.Query["returnUrl"],
                    Providers = res.Select(p => p.Name).ToArray()
                });

        }
  
    }
}
