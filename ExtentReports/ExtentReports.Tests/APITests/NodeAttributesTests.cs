using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class NodeAttributesTests : Base
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
    public void verifyIfNodeHasAddedCategory() {
        var node = _extent
                .CreateTest(TestContext.CurrentContext.Test.Name)
                .CreateNode("Child")
                .AssignCategory(_categories[0])
                .Pass("pass");
        
        Assert.AreEqual(node.GetModel().CategoryContext().Count, 1);
        var c = node.GetModel().CategoryContext().Get(0);
        Assert.AreEqual(c.Name, _categories[0]);
    }
    
    [Test]
    public void verifyIfTestHasAddedCategories() {
        var node = _extent.CreateTest(TestContext.CurrentContext.Test.Name).CreateNode("Child").Pass("pass");        
        _categories.ToList().ForEach(c => node.AssignCategory(c));
               
        Assert.AreEqual(node.GetModel().CategoryContext().Count, _categories.Length);

        var categoryCollection = node.GetModel().CategoryContext().GetAllItems();
        _categories.ToList().ForEach(c => {
            Boolean result = categoryCollection.Any(x => x.Name == c); 
            Assert.True(result);
        });
    }
    
    [Test]
    public void verifyIfTestHasAddedAuthor() {
        var node = _extent
                .CreateTest(TestContext.CurrentContext.Test.Name)
                .CreateNode("Child")
                .AssignAuthor(_authors[0])
                .Pass("pass");
        
        Assert.AreEqual(node.GetModel().AuthorContext().Count, 1);
        Assert.AreEqual(node.GetModel().AuthorContext().Get(0).Name, _authors[0]);
    }
    
    [Test]
    public void verifyIfTestHasAddedAuthors() {
        var node = _extent
                .CreateTest(TestContext.CurrentContext.Test.Name)
                .CreateNode("Child")
                .Pass("pass");
        _authors.ToList().ForEach(a => node.AssignAuthor(a));
               
        Assert.AreEqual(node.GetModel().AuthorContext().Count, _authors.Length);

        var authorCollection = node.GetModel().AuthorContext().GetAllItems();
        _authors.ToList().ForEach(a => {
            Boolean result = authorCollection.Any(x => x.Name == a); 
            Assert.True(result);
        });
    }
    }
}
