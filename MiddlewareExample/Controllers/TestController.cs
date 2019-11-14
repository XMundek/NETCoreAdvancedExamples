using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiddlewareExample.Services;
namespace MiddlewareExample.Controllers
{
    public class TestController:Controller
    {
        // ICountService _srvCount;
        Func<ICountService> _srvCountFactory;
        IReadCountService _srvReadCount;
        public TestController(
            //ICountService srvCount, 
            Func<ICountService> srvCountFactory,
            IReadCountService srvReadCount)
        {
            //_srvCount = srvCount;
            _srvCountFactory = srvCountFactory;
            _srvReadCount = srvReadCount;
        }
        public IActionResult GetCount(
           // [FromServices] ICountService _srvCount,
            //[FromServices] IReadCountService _srvReadCount
            )
        {
            return new ContentResult() {
                //Content = $"Count incremented:{_srvCount.IncCount()} read:{_srvReadCount.GetCount()}"
                Content = $"Count incremented:{_srvCountFactory().IncCount()} read:{_srvReadCount.GetCount()}"
            };
        }
    }
}
