using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UDO.LOB.RatingsApi.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]        
        public void TestDataLoad()
        {
            UpdateTable2 testUpdate = new UpdateTable2();
            string lettergenerationid = "C422C951-61DA-E511-9437-0050568DF261";
            testUpdate.UpdateTable(lettergenerationid);
            
        }
    }
}
