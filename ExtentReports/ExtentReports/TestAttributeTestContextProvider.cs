using System.Collections.Generic;
using System.Linq;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public class TestAttributeTestContextProvider<T> where T : TestAttribute
    {
        public List<TestAttributeTestContext<T>> TestAttributeContextCollection { get; private set; }

        public TestAttributeTestContextProvider()
        {
            TestAttributeContextCollection = new List<TestAttributeTestContext<T>>();
        }

        public void AddAttributeContext(T attrib, Test test)
        {
            var context = TestAttributeContextCollection.Where(x => x.Name.Equals(attrib.Name));

            if (context != null && context.Count() > 0)
            {
                if (context.First().TestCollection.Where(x => x.TestId == test.TestId).Count() == 0)
                {
                    context.First().TestCollection.Add(test);
                }
                context.First().RefreshTestStatusCounts();
            }
            else
            {
                var testAttrTestContext = new TestAttributeTestContext<T>(attrib);
                testAttrTestContext.AddTest(test);
                TestAttributeContextCollection.Add(testAttrTestContext);
            }
        }
    }
}
