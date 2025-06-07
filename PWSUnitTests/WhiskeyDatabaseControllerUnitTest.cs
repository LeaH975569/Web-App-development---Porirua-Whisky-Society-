using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PWS.Data;
using PWS.Models.ViewModels;

namespace PWSUnitTests
{
    [TestClass]
    public class WhiskeyDatabaseControllerUnitTest
    {
        private static ApplicationDbContext _context;
        private static WhiskeyDatabaseController _controller;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // This runs once before any tests
            // If you want to run stuff after, use ClassCleanup
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new WhiskeyDatabaseController(_context);
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
            var model = result.Model as WhiskeyViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(_context.Whiskeys.Count(), model.Whiskeys.Count());
            Assert.AreEqual(w.WhiskeyName, model.Whiskeys.First().WhiskeyName);
        }

        [TestMethod]
        public async Task Index_FilterSearchString()
        {
            // Arrange: Seed the in-memory database
            var ws = new List<Whiskey> {
                new() { WhiskeyName = "Test1"},
                new() { WhiskeyName = "Test2"}
            };

            _context.Whiskeys.AddRange(ws);
            await _context.SaveChangesAsync();

            // Act: Call the Index action with viewModel
            var viewModel = new WhiskeyViewModel { SearchString = "Test1" };
            var result = await _controller.Index(viewModel) as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as WhiskeyViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Whiskeys.Count());
            Assert.AreEqual(
                ws.Where(w => w.WhiskeyName == ws[0].WhiskeyName).First().WhiskeyName,
                model.Whiskeys.First().WhiskeyName
                );
        }

        [TestMethod]
        public async Task Index_FilterMinMaxScore()
        {
            // Arrange: Seed the in-memory database
            var ws = new List<Whiskey> {
                new() { WhiskeyName = "Test1", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 50},
                new() { WhiskeyName = "Test2", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 60},
                new() { WhiskeyName = "Test3", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 70}
            };

            _context.Whiskeys.AddRange(ws);
            await _context.SaveChangesAsync();

            var test = _context.Whiskeys.ToList();

            // Act: Call the Index action with viewModel
            var viewModel = new WhiskeyViewModel { ScoreMin = 55, ScoreMax = 65 };
            var result = await _controller.Index(viewModel) as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as WhiskeyViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Whiskeys.Count());
            // Check if we got the desired item
            Assert.AreEqual(
                ws.Where(w => w.WhiskeyName == ws[1].WhiskeyName).First().WhiskeyName,
                model.Whiskeys.First().WhiskeyName
                );
        }

        [TestMethod]
        public async Task Index_FilterInvaildViewModelState()
        {
            // Arrange: Seed the in-memory database
            var ws = new List<Whiskey> {
                new() { WhiskeyName = "Test1", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 50},
                new() { WhiskeyName = "Test2", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 60},
                new() { WhiskeyName = "Test3", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 90}
            };

            _context.Whiskeys.AddRange(ws);
            await _context.SaveChangesAsync();

            var viewModel = new WhiskeyViewModel { ScoreMin = 90, ScoreMax = 101 };
            _controller.ModelState.AddModelError("ScoreMax", "Out of range");

            // Act: Call the Index action with viewModel
            var result = await _controller.Index(viewModel) as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as WhiskeyViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(ws.Count(), model.Whiskeys.Count()); // As the model is invaild, the filter does not get applied
        }

        [TestMethod]
        public void Whiskey_ReturnsViewWithItem()
        {
            int id = 1;
            // Arrange: Seed the in-memory database
            var w = new Whiskey
            {
                WhiskeyId = id,
                WhiskeyName = "TestWhisky1",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Whiskeys.Add(w);
            _context.SaveChanges();

            // Act: Call the Index action
            var result = _controller.Whiskey(1) as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as WhiskeyViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(w.WhiskeyName, model.Whiskey.WhiskeyName);
        }
    }
}
