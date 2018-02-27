using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class TestAttributesTests : Base
    {
        private string[] _categories = {
            "_extent",
            "git",
            "tests",
            "heroku"
        };
        private string[] _authors = {
            "anshoo",
            "viren",
            "maxi",
            "vimal"
        };

        [Test]
        public void verifyIfTestHasAddedCategory()
        {
            var test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).AssignCategory(_categories[0]);
            test.Pass("Pass");

            Assert.AreEqual(test.GetModel().CategoryContext().Count, 1);
            Assert.AreEqual(test.GetModel().CategoryContext().Get(0).Name, _categories[0]);
        }

        [Test]
        public void verifyIfTestHasAddedCategories()
        {
            var test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            _categories.ToList().ForEach(c => test.AssignCategory(c));
            test.Pass("Pass");

            Assert.AreEqual(test.GetModel().CategoryContext().Count, _categories.Length);

            var categoryCollection = test.GetModel().CategoryContext().GetAllItems();
            _categories.ToList().ForEach(c => {
                var result = categoryCollection.Any(x => x.Name == c);
                Assert.True(result);
            });
        }

        [Test]
        public void verifyIfTestHasAddedAuthor()
        {
            var test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).AssignAuthor(_authors[0]);
            test.Pass("Pass");

            Assert.AreEqual(test.GetModel().AuthorContext().Count, 1);
            Assert.AreEqual(test.GetModel().AuthorContext().Get(0).Name, _authors[0]);
        }

        [Test]
        public void verifyIfTestHasAddedAuthors()
        {
            var test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            _authors.ToList().ForEach(a => test.AssignAuthor(a));
            test.Pass("Pass");

            Assert.AreEqual(test.GetModel().AuthorContext().Count, _authors.Length);

            var authorCollection = test.GetModel().AuthorContext().GetAllItems();
            _authors.ToList().ForEach(a => {
                var result = authorCollection.Any(x => x.Name == a);
                Assert.True(result);
            });
        }
    }
}
