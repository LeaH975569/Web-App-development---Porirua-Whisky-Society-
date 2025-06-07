using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PWS.Data;
using PWS.Models.ViewModels;

namespace PWSUnitTests
{
    [TestClass]
    public class BlogAdminControllerUnitTest
    {
        private static ApplicationDbContext _context;
        private static BlogAdminController _controller;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // This runs once before any tests
            // If you want to run stuff after, use ClassCleanup
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new BlogAdminController(_context);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            _controller.ObjectValidator = objectValidator.Object;
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
                new Blog { Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true },
                new Blog { Title = "Is Not Published", Content = "Test", Summary = "Test", IsPublished = false },

            ];
            _context.Blogs.AddRange(bs);
            await _context.SaveChangesAsync();

            // Act: Call the Index action
            var result = await _controller.Index() as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as IEnumerable<PWS.Models.Blog>;
            Assert.IsNotNull(model);
            Assert.AreEqual(_context.Blogs.Count(), model.Count());
            Assert.AreEqual(bs[0].Title, model.First().Title);
        }

        [TestMethod]
        public void Create_Get_ReturnsView()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var b = new Blog { Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };

            // Act
            var result = await _controller.Create(b) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsNotNull(_context.Blogs.Where(b => b.Title == b.Title).FirstOrDefault());
        }

        [TestMethod]
        public async Task Create_Post_InvalidModel()
        {
            // Arrange
            var b = new Blog { Title = "Is Published", IsPublished = true };

            _controller.ModelState.AddModelError("Content", "Missing Content");

            // Act
            var result = await _controller.Create(b) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(_context.Blogs.Where(b => b.Title == b.Title).FirstOrDefault());
        }

        [TestMethod]
        public async Task Edit_Get_ReturnsView()
        {
            // Arrange
            var b = new Blog { Id = 1, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Edit(b.Id) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as Blog;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Id, b.Id);
        }

        [TestMethod]
        public async Task Edit_Get_InvaildId()
        {
            // Arrange
            // Arrange
            var b = new Blog { Id = 1, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Edit(-1) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange: Seed db
            int id = 1;
            // Arrange
            var b = new Blog { Id = id, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            var w = await _context.Blogs.FindAsync(id);
            w.Title = "Tested";

            // Act
            var result = await _controller.Edit(id, w) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsTrue(_context.Blogs.Where(w => w.Id == id).FirstOrDefault().Title == "Tested");
        }

        [TestMethod]
        public async Task Edit_Post_InvalidModel()
        {
            // Arrange: Seed db
            var b = new Blog { Id = 1, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            var newB = new Blog { Id = 1, Title = "Tested", Summary = "Test", IsPublished = true };

            _controller.ModelState.AddModelError("Content", "Missing Content");

            // Act
            var result = await _controller.Edit(b.Id, newB) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(_context.Blogs.Where(b => b.Id == newB.Id && b.Title == "Tested").FirstOrDefault());
        }

        [TestMethod]
        public async Task Details_Get_ReturnsView()
        {
            // Arrange
            var b = new Blog { Id = 1, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(b.Id) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as Blog;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Id, b.Id);
        }

        [TestMethod]
        public async Task Details_Get_InvaildId()
        {
            // Arrange

            // Act
            var result = await _controller.Details(-1) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Delete_Get_ReturnsView()
        {
            // Arrange
            var b = new Blog { Id = 1, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(b.Id) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as Blog;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.Id, b.Id);
        }

        [TestMethod]
        public async Task Delete_Get_InvaildId()
        {
            // Arrange
            var b = new Blog { Id = 1, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(-1) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteConfrimed_Post_RedirectToIndex()
        {
            // Arrange
            var b = new Blog { Id = 1, Title = "Is Published", Content = "Test", Summary = "Test", IsPublished = true };
            _context.Blogs.Add(b);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsNull(_context.Blogs.Where(w => w.Id == 1).FirstOrDefault());
        }
    }
}
