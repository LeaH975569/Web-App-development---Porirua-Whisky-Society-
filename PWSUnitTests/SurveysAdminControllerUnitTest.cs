using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PWS.Data;

namespace PWSUnitTests
{
    [TestClass]
    public class SurveysAdminControllerUnitTest
    {
        private ApplicationDbContext _context;
        private SurveysAdminController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new SurveysAdminController(_context);
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            _controller.ObjectValidator = objectValidator.Object;


        }

        [TestMethod]
        public async Task Index_ReturnsViewWithSurveys()
        {
            _context.Surveys.RemoveRange(_context.Surveys);
            await _context.SaveChangesAsync();
            // Arrange: Seed the in-memory database
            var survey = new Survey
            {
                Id = 1000000,
                Title = "Test Survey",
                Subtitle = "Test Subtitle",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            // Act: Call the Index action
            var result = await _controller.Index() as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as List<Survey>;
            Assert.IsNotNull(model);
            Assert.AreEqual(_context.Surveys.Count(), model.Count);
            Assert.AreEqual(survey.Title, model.First().Title);
        }

        [TestMethod]
        public void Create_ReturnsView()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Create_PostValidModel_RedirectsToEdit()
        {
            // Arrange
            var survey = new Survey
            {
                Title = "New Survey",
                Subtitle = "Subtitle",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true
            };

            // Act
            var result = await _controller.Create(survey) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
        }




        //[TestMethod]
        //public void Details_ReturnsViewWithSurvey()
        //{
        //    // Arrange: Seed the in-memory database
        //    int surveyId = 2;
        //    var survey = new Survey
        //    {
        //        Id = surveyId,
        //        Title = "Test Survey",
        //        Subtitle = "Test Subtitle",
        //        Start = DateTime.Now,
        //        End = DateTime.Now.AddDays(1),
        //        Published = true,
        //        Uuid = Guid.NewGuid().ToString()
        //    };
        //    _context.Surveys.Add(survey);
        //    _context.SaveChanges();

        //    // Act: Call the Details action
        //    var result = _controller.Details(surveyId) as ViewResult;

        //    // Assert: Ensure the view is returned with the correct model
        //    Assert.IsNotNull(result);
        //    var model = result.Model as Survey;
        //    Assert.IsNotNull(model);
        //    Assert.AreEqual(surveyId, model.Id);
        //}

        [TestMethod]
        public void Edit_Get_ReturnsViewWithSurvey()
        {
            // Arrange: Seed the in-memory database
            int surveyId = 3;
            var survey = new Survey
            {
                Id = surveyId,
                Title = "Survey to Edit",
                Subtitle = "Subtitle for Edit",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };
            _context.Surveys.Add(survey);
            _context.SaveChanges();

            // Act: Call the Edit GET action
            var result = _controller.Edit(surveyId) as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as Survey;
            Assert.IsNotNull(model);
            Assert.AreEqual(surveyId, model.Id);
        }


        [TestMethod]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange: Seed the in-memory database
            int surveyId = 56;
            var originalSurvey = new Survey
            {
                Id = surveyId,
                Title = "Survey to Edit",
                Subtitle = "Subtitle for Edit",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };
            _context.Surveys.Add(originalSurvey);
            await _context.SaveChangesAsync();

            // Retrieve the survey to update
            var surveyToUpdate = await _context.Surveys.FindAsync(surveyId);
            if (surveyToUpdate == null)
            {
                Assert.Fail("Survey not found.");
            }

            // Update properties
            surveyToUpdate.Title = "Updated Survey";
            surveyToUpdate.Subtitle = "Updated Subtitle";
            surveyToUpdate.Published = false;
            surveyToUpdate.Uuid = Guid.NewGuid().ToString(); // Optionally update the UUID

            // Act: Call the Edit POST action
            var result = _controller.Edit(surveyId, surveyToUpdate) as RedirectToActionResult;

            // Assert: Ensure redirection to Index action
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            // Verify that the survey was updated in the database
            var dbSurvey = await _context.Surveys.FindAsync(surveyId);
            Assert.IsNotNull(dbSurvey);
            Assert.AreEqual("Updated Survey", dbSurvey.Title);
            Assert.AreEqual("Updated Subtitle", dbSurvey.Subtitle);
            Assert.IsFalse(dbSurvey.Published);
        }

        [TestMethod]
        public void Delete_Get_ReturnsViewWithSurvey()
        {
            // Arrange: Seed the in-memory database
            int surveyId = 5;
            var survey = new Survey
            {
                Id = surveyId,
                Title = "Survey to Delete",
                Subtitle = "Subtitle for Delete",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };
            _context.Surveys.Add(survey);
            _context.SaveChanges();

            // Act: Call the Delete GET action
            var result = _controller.Delete(surveyId) as ViewResult;

            // Assert: Ensure the view is returned with the correct model
            Assert.IsNotNull(result);
            var model = result.Model as Survey;
            Assert.IsNotNull(model);
            Assert.AreEqual(surveyId, model.Id);
        }

        [TestMethod]
        public async Task Delete_Post_RemovesSurveyAndRedirects()
        {
            // Arrange: Seed the in-memory database
            int surveyId = 6;
            var survey = new Survey
            {
                Id = surveyId,
                Title = "Survey to Delete",
                Subtitle = "Subtitle for Delete",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            // Act: Call the Delete POST action
            var result = await _controller.DeleteConfirmed(surveyId) as RedirectToActionResult;

            // Assert: Ensure redirection to Index action
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            // Verify that the survey was removed from the database
            var dbSurvey = _context.Surveys.FirstOrDefault(s => s.Id == surveyId);
            Assert.IsNull(dbSurvey);
        }


        [TestMethod]
        public async Task Delete_ReturnsViewWithSurvey()
        {
            // Arrange
            var survey = new Survey
            {
                Id = 7,
                Title = "Delete Test",
                Subtitle = "Test Subtitle", // Ensure Subtitle is provided
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            // Act
            var result = _controller.Delete(7) as ViewResult;
            var model = result?.Model as Survey;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Delete Test", model?.Title);
        }



        [TestMethod]
        public async Task DeleteConfirmed_RemovesSurveyAndRedirects()
        {
            // Arrange
            var survey = new Survey
            {
                Id = 8,
                Title = "To Be Deleted",
                Subtitle = "Subtitle for Deletion",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteConfirmed(8) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            // Refresh the context and verify deletion
            var dbSurvey = await _context.Surveys.FindAsync(8);
            Assert.IsNull(dbSurvey); // Ensure the survey is deleted
        }


        [TestMethod]
        public async Task ItemEdit_Post_ValidModel_RedirectsToEdit()
        {
            // Arrange: Seed the in-memory database
            int tastingItemId = 26;
            int whiskeyId = 26;
            var originalTastingItem = new TastingItem
            {
                Id = tastingItemId,
                Name = "Original Tasting Item",
                Uuid = Guid.NewGuid().ToString(),
                Survey = new Survey
                {
                    Id = 26,
                    Title = "Sample Survey",
                    Subtitle = "Sample Subtitle",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(1),
                    Published = true,
                    Uuid = Guid.NewGuid().ToString()
                },
                Whiskey = new Whiskey
                {
                    WhiskeyId = whiskeyId,
                    WhiskeyName = "Original Whiskey"
                }
            };
            _context.TastingItems.Add(originalTastingItem);
            _context.SaveChanges();

            var updatedTastingItem = new TastingItem
            {
                Id = tastingItemId,
                Name = "Updated Tasting Item",
                Uuid = originalTastingItem.Uuid
            };

            // Act: Call the ItemEdit POST action
            var result = await _controller.ItemEdit(tastingItemId, updatedTastingItem, whiskeyId) as RedirectToActionResult;

            // Assert: Ensure redirection to the Edit action with the correct survey ID
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(originalTastingItem.Survey.Id, result.RouteValues["id"]);

            // Verify that the item was updated in the database
            var editedItem = await _context.TastingItems.FindAsync(tastingItemId);
            Assert.IsNotNull(editedItem);
            Assert.AreEqual("Updated Tasting Item", editedItem.Name);
        }

        [TestMethod]
        public void ItemEdit_Get_ReturnsViewWithTastingItem()
        {
            // Arrange: Seed the in-memory database
            int tastingItemId = 44;
            var tastingItem = new TastingItem
            {
                Id = tastingItemId,
                Name = "Sample Tasting Item",
                Uuid = Guid.NewGuid().ToString(),
                Survey = new Survey
                {
                    Id = 44,
                    Title = "Sample Survey",
                    Subtitle = "Sample Subtitle",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(1),
                    Published = true,
                    Uuid = Guid.NewGuid().ToString()
                },
                Whiskey = new Whiskey
                {
                    WhiskeyId = 44,
                    WhiskeyName = "Sample Whiskey"
                }
            };
            _context.TastingItems.Add(tastingItem);
            _context.SaveChanges();

            // Act: Call the ItemEdit GET action
            var result = _controller.ItemEdit(tastingItemId) as ViewResult;

            // Assert: Ensure the view is returned correctly
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(TastingItem));
            Assert.AreEqual(tastingItemId, ((TastingItem)result.Model).Id);
        }

        [TestMethod]
        public void ItemCreate_Get_ReturnsView()
        {
            // Arrange: Seed the in-memory database
            int surveyId = 11;
            _context.Surveys.Add(new Survey
            {
                Id = surveyId,
                Title = "Survey for ItemCreate",
                Subtitle = "Subtitle for ItemCreate",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            });
            _context.SaveChanges();

            // Act: Call the ItemCreate GET action
            var result = _controller.ItemCreate(surveyId) as ViewResult;

            // Assert: Ensure the view is returned correctly
            Assert.IsNotNull(result);
            Assert.AreEqual(surveyId, result.ViewData["surveyId"]);
        }

        [TestMethod]
        public void ItemCreate_Get_InvalidId_ReturnsNotFound()
        {
            // Act: Call the ItemCreate GET action with an invalid ID
            var result = _controller.ItemCreate(null) as NotFoundResult;

            // Assert: Ensure NotFound result is returned
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ItemCreate_Post_ValidModel_RedirectsToEdit()
        {
            // Arrange: Seed the in-memory database with necessary data
            int surveyId = 101;
            int whiskeyId = 201;

            var survey = new Survey
            {
                Id = surveyId,
                Title = "Survey for ItemCreate",
                Subtitle = "Subtitle for ItemCreate",
                Start = DateTime.Now,
                End = DateTime.Now.AddDays(1),
                Published = true,
                Uuid = Guid.NewGuid().ToString()
            };

            var whiskey = new Whiskey
            {
                WhiskeyId = whiskeyId,
                WhiskeyName = "Sample Whiskey"
            };

            _context.Surveys.Add(survey);
            _context.Whiskeys.Add(whiskey);
            await _context.SaveChangesAsync();

            var tastingItem = new TastingItem
            {
                Name = "New Tasting Item",
                // Note: Uuid will be generated in the controller
            };

            // Act: Call the ItemCreate POST action
            var result = await _controller.ItemCreate(tastingItem, surveyId, whiskeyId) as RedirectToActionResult;

            // Assert: Ensure the redirection is to the Edit action with the correct survey ID
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(surveyId, result.RouteValues["id"]);

            // Verify that the TastingItem was added to the database
            var dbTastingItem = await _context.TastingItems.FirstOrDefaultAsync(ti => ti.Name == "New Tasting Item");
            Assert.IsNotNull(dbTastingItem);
            Assert.AreEqual(surveyId, dbTastingItem.Survey.Id);
            Assert.AreEqual(whiskeyId, dbTastingItem.Whiskey.WhiskeyId);
        }






        [TestMethod]
        public void ItemDelete_Get_ReturnsViewWithTastingItem()
        {
            // Arrange: Seed the in-memory database
            int tastingItemId = 22;
            var tastingItem = new TastingItem
            {
                Id = tastingItemId,
                Name = "Sample Tasting Item",
                Uuid = Guid.NewGuid().ToString(),
                Survey = new Survey
                {
                    Id = 22,
                    Title = "Sample Survey",
                    Subtitle = "Sample Subtitle",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(1),
                    Published = true,
                    Uuid = Guid.NewGuid().ToString()
                },
                Whiskey = new Whiskey
                {
                    WhiskeyId = 22,
                    WhiskeyName = "Sample Whiskey"
                }
            };
            _context.TastingItems.Add(tastingItem);
            _context.SaveChanges();

            // Act: Call the ItemDelete GET action
            var result = _controller.ItemDelete(tastingItemId) as ViewResult;

            // Assert: Ensure the view is returned correctly
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(TastingItem));
            Assert.AreEqual(tastingItemId, ((TastingItem)result.Model).Id);
        }


        [TestMethod]
        public async Task ItemDelete_Post_RemovesTastingItemAndRedirects()
        {
            // Arrange: Seed the in-memory database
            int tastingItemId = 23;
            var tastingItem = new TastingItem
            {
                Id = tastingItemId,
                Name = "Sample Tasting Item",
                Uuid = Guid.NewGuid().ToString(),
                Survey = new Survey
                {
                    Id = 23,
                    Title = "Sample Survey",
                    Subtitle = "Sample Subtitle",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(1),
                    Published = true,
                    Uuid = Guid.NewGuid().ToString()
                },
                Whiskey = new Whiskey
                {
                    WhiskeyId = 23,
                    WhiskeyName = "Sample Whiskey"
                }
            };
            _context.TastingItems.Add(tastingItem);
            _context.SaveChanges();

            // Act: Call the ItemDelete POST action
            var result = await _controller.ItemDeleteConfirmed(tastingItemId) as RedirectToActionResult;

            // Assert: Ensure redirection to the Edit action with the correct survey ID
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(tastingItem.Survey.Id, result.RouteValues["id"]);

            // Verify that the item was removed from the database
            var deletedItem = await _context.TastingItems.FindAsync(tastingItemId);
            Assert.IsNull(deletedItem);
        }



    }
}