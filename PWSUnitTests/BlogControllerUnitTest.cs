using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PWS.Data;
using PWS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWSUnitTests
{
    [TestClass]
    public class BlogControllerUnitTest
    {
        private static ApplicationDbContext _context;
        private static BlogController _controller;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // This runs once before any tests
            // If you want to run stuff after, use ClassCleanup
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new BlogController(_context);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            _controller.ObjectValidator = objectValidator.Object;
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {

        }

        /// <summary>
        /// Ensure database is clear
        /// </summary>
        /// <returns></returns>
        [TestInitialize]
        public async Task TestInitialize()
        {
            _controller.ModelState.Clear();
            _context.Blogs.RemoveRange(_context.Blogs);
            await _context.SaveChangesAsync();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {

        }

        [TestMethod]
        public async Task Index_ReturnsViewWithItems()
        {
            // Arrange: Seed the in-memory database
            Blog[] bs =
            [
                new Blog { Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true},
                new Blog { Title = "Is Not Published", Content = "Test", Summary = "Test", IsPublished = false },

            ];
            _context.Blogs.AddRange(bs);
            await _context.SaveChangesAsync();

            // Act: Call the Index action
            var result = await _controller.Index() as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as IEnumerable<Blog>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Where(b => b.IsPublished == false).Count(), "Not published items appearing in blog list!");
            Assert.AreEqual(_context.Blogs.Where(b => b.IsPublished).Count(), model.Count());
        }

        [TestMethod]
        public async Task BlogEntry_ReturnsViewWithItem()
        {
            int id = 1;
            // Arrange: Seed the in-memory database
            Blog[] bs =
            [
                new Blog { Id = id, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true },
                new Blog { Id = 2, Title = "Is Not Published", Content = "Test", Summary = "Test", IsPublished = false },

            ];
            _context.Blogs.AddRange(bs);
            await _context.SaveChangesAsync();

            // Act: Call the Index action

            // Act: Call the Index action
            var result = _controller.BlogEntry(1) as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as Blog;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Title, bs[0].Title);
        }
    }
}
