using System.Collections.Generic;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public class TestAttributeTestContext<T> where T : TestAttribute
    {
        public List<Test> TestCollection { get; private set; }
        public string Name { get; private set; }
        public int Passed { get; private set; }
        public int Failed { get; private set; }
        public int Others { get; private set; }

        private T _testAttribute;

        public TestAttributeTestContext(T testAttribute)
        {
            Passed = Failed = Others = 0;

            Name = testAttribute.Name;
            _testAttribute = testAttribute;
        }

        public void AddTest(Test test)
        {
            if (TestCollection == null)
                TestCollection = new List<Test>();

            Passed += test.Status == Status.Pass ? 1 : 0;
            Failed += test.Status == Status.Fail ? 1 : 0;
            Others += test.Status != Status.Pass && test.Status != Status.Fail ? 1 : 0;

            TestCollection.Add(test);
        }
    }
}
