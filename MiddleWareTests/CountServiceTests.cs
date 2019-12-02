using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiddlewareExample.Services;
namespace MiddleWareTests
{
    [TestClass]
    public class CountServiceTests
    {
        [TestMethod]
        public void VerifyIfCountServiceWorksCorrectly()
        {
            var srvInner =Moq.Mock.Of<ICountService>(p=>p.GetCount()==10);

            var srv = new ReadCountService(srvInner);

            var res = srv.GetCount();
            Moq.Mock.Get<ICountService>(srvInner).Verify(s=>s.GetCount(), Moq.Times.Once);
            Assert.AreEqual(10, res);
        }
    }
}
