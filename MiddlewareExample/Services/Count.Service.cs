using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewareExample.Services
{
    public interface ICountService
    {
        int IncCount();
        int GetCount();
    }
    public interface IReadCountService
    {
        int GetCount();
    }

    public class CountService:ICountService
    {
        int Count;
        public int IncCount()
        {
            return ++Count;
        }
        public int GetCount()
        {
            return Count;
        }
    }
    public class ReadCountService : IReadCountService
    {
        private ICountService _srv;
        public ReadCountService(ICountService srv)
        {
            _srv = srv;
        }
        public int GetCount()
        {
            return _srv.GetCount();
        }
    }

}
