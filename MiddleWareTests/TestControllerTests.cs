using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiddlewareExample.Controllers;
using MiddlewareExample.Services;
using Moq;
namespace MiddleWareTests
{
    [TestClass]
    public class TestControllerTests
    {
        [TestMethod]
        public void TestIfGetCountReturnValidResult()
        {
            var mockCounterService = Mock.Of<ICountService>(
                c=>c.IncCount()==2 && c.GetCount() == 1);
            var mockReadCountService = Mock.Of<IReadCountService>();
            var controller = new TestController(mockCounterService, mockReadCountService);
            var result = controller.GetCount() as ContentResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Count incremented:2 read:0",result.Content);
            Mock.Get(mockCounterService).Verify(c => c.IncCount(),Times.Once);
            Mock.Get(mockCounterService).Verify(c => c.GetCount(), Times.Never);
        }
    }
}