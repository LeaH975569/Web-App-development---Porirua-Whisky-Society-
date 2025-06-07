using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PWS.Data;
using PWS.Models.ViewModels;

namespace PWSUnitTests
{
    [TestClass]
    public class WhiskeyAdminControllerUnitTest
    {
        private static ApplicationDbContext _context;
        private static WhiskeyAdminController _controller;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // This runs once before any tests
            // If you want to run stuff after, use ClassCleanup
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new WhiskeyAdminController(_context);
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
            _context.Whiskeys.RemoveRange(_context.Whiskeys);
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
            var w = new Whiskey
            {
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            await _context.SaveChangesAsync();

            // Act: Call the Index action
            var result = await _controller.Index() as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as WhiskeyAdminViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(_context.Whiskeys.Count(), model.Whiskeys.Count());
            Assert.AreEqual(w.WhiskeyName, model.Whiskeys.First().WhiskeyName);
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
            var w = new Whiskey
            {
                WhiskeyName = "Test",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };

            // Act
            var result = await _controller.Create(w) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsNotNull(_context.Whiskeys.Where(w => w.WhiskeyName == "Test").FirstOrDefault());
        }

        [TestMethod]
        public async Task Create_Post_InvalidModel()
        {
            // Arrange
            var w = new Whiskey
            {
                WhiskeyName = "Test",
                WhiskeyDescription = "",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TastedDate = DateTime.Today
            };

            _controller.ModelState.AddModelError("TotalScore", "Missing TotalScore");

            // Act
            var result = await _controller.Create(w) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(_context.Whiskeys.Where(w => w.WhiskeyName == "Test").FirstOrDefault());
        }

        [TestMethod]
        public async Task Edit_Get_ReturnsView()
        {
            // Arrange
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Edit(w.WhiskeyId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as Whiskey;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.WhiskeyId, w.WhiskeyId);
        }

        [TestMethod]
        public async Task Edit_Get_InvaildId()
        {
            // Arrange
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
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
            _context.Whiskeys.Add(new Whiskey
            {
                WhiskeyId = id,
                WhiskeyName = "Test",
                WhiskeyDescription = "",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 10,
                TastedDate = DateTime.Today,
            });
            await _context.SaveChangesAsync();

            var w = await _context.Whiskeys.FindAsync(id);
            w.WhiskeyName = "Tested";

            // Act
            var result = await _controller.Edit(1, w) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsTrue(_context.Whiskeys.Where(w => w.WhiskeyId == 1).FirstOrDefault().WhiskeyName == "Tested");
        }

        [TestMethod]
        public async Task Edit_Post_InvalidModel()
        {
            // Arrange: Seed db
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "Test",
                WhiskeyDescription = "",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 10,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            await _context.SaveChangesAsync();

            var newW = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "Tested",
                WhiskeyDescription = "",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TastedDate = DateTime.Today
            };

            _controller.ModelState.AddModelError("TotalScore", "Missing TotalScore");

            // Act
            var result = await _controller.Edit(w.WhiskeyId, w) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(_context.Whiskeys.Where(w => w.WhiskeyId == newW.WhiskeyId && w.WhiskeyName == "Tested").FirstOrDefault());
        }

        [TestMethod]
        public async Task Details_Get_ReturnsView()
        {
            // Arrange
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(w.WhiskeyId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as Whiskey;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.WhiskeyId, w.WhiskeyId);
        }

        [TestMethod]
        public async Task Details_Get_InvaildId()
        {
            // Arrange
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(-1) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Delete_Get_ReturnsView()
        {
            // Arrange
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(w.WhiskeyId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as Whiskey;
            Assert.IsNotNull(model);
            Assert.AreEqual(model.WhiskeyId, w.WhiskeyId);
        }

        [TestMethod]
        public async Task Delete_Get_InvaildId()
        {
            // Arrange
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
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
            var w = new Whiskey
            {
                WhiskeyId = 1,
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsNull(_context.Whiskeys.Where(w => w.WhiskeyId == 1).FirstOrDefault());
        }
    }
}
