using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PWS.Data;
using PWS.Models.ViewModels;
using PWS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWSUnitTests
{
    [TestClass]
    public class DbExtensionsUnitTest
    {
        private static ApplicationDbContext _context;
        static string sessionid1;
        static string sessionid2;
        static string sessionid3;
        static string uuid1;
        static string uuid2;
        static string uuid3;

        [ClassInitialize]
        public static void TestInitialize(TestContext testContext)
        {
            
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("InMemoryDb")
                .Options;
            _context = new ApplicationDbContext(options);

            var w1 = new Whiskey
            {
                WhiskeyName = "Test",
                WhiskeyDescription = "Test Description",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            var w2 = new Whiskey
            {
                WhiskeyName = "Test2",
                WhiskeyDescription = "Test Description2",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            var w3 = new Whiskey
            {
                WhiskeyName = "Test3",
                WhiskeyDescription = "Test Description3",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TotalScore = 50,
                TastedDate = DateTime.Today
            };
            _context.Add(w1);
            _context.Add(w2);
            _context.Add(w3);

            uuid1 = System.Guid.NewGuid().ToString();
            uuid2 = System.Guid.NewGuid().ToString();
            uuid3 = System.Guid.NewGuid().ToString();


            var survey = new Survey {Uuid = uuid1,Title = "Test Title",Subtitle = "Test Subtitle", Start = System.DateTime.Now.AddDays(-4).AddYears(-1), End = System.DateTime.Now.AddDays(-1).AddYears(-1), Published = true};
            var survey2 = new Survey { Uuid = uuid2, Title = "Test Title2", Subtitle = "Test Subtitle2", Start = System.DateTime.Now.AddDays(-2), End = System.DateTime.Now.AddDays(2), Published = true};
            var survey3 = new Survey { Uuid = uuid3, Title = "Test Title3", Subtitle = "Test Subtitle3", Start = System.DateTime.Now.AddDays(2), End = System.DateTime.Now.AddDays(3), Published = false};
            _context.Add(survey);
            _context.Add(survey2);
            _context.Add(survey3);

            sessionid1 = System.Guid.NewGuid().ToString();
            sessionid2 = System.Guid.NewGuid().ToString();
            sessionid3 = System.Guid.NewGuid().ToString();

            var ti1 = new TastingItem { Name = "s1 t1", Whiskey = w3, Survey = survey, Uuid = System.Guid.NewGuid().ToString() };
            var tr111 = new TastingResponse { Id = 1, SessionId = sessionid1, TastingItem = ti1, Taste = 50, Aroma = 50, Finish = 50, UserName = "user1", WhiskeyGuess = w1 };
            var tr112 = new TastingResponse { Id = 2, SessionId = sessionid2, TastingItem = ti1, Taste = 50, Aroma = 50, Finish = 50, UserName = "user2", WhiskeyGuess = w2 };
            var tr113 = new TastingResponse { Id = 3, SessionId = sessionid3, TastingItem = ti1, Taste = 50, Aroma = 50, Finish = 50, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti1);
            _context.Add(tr111);
            _context.Add(tr112);
            _context.Add(tr113);

            var ti2 = new TastingItem { Name = "s1 t2", Whiskey = w2, Survey = survey, Uuid = System.Guid.NewGuid().ToString() };
            var tr121 = new TastingResponse { Id = 4, SessionId = sessionid1, TastingItem = ti2, Taste = 60, Aroma = 60, Finish = 60, UserName = "user1", WhiskeyGuess = w1 };
            var tr122 = new TastingResponse { Id = 5, SessionId = sessionid2, TastingItem = ti2, Taste = 60, Aroma = 60, Finish = 60, UserName = "user2", WhiskeyGuess = w2 };
            var tr123 = new TastingResponse { Id = 6, SessionId = sessionid3, TastingItem = ti2, Taste = 60, Aroma = 60, Finish = 60, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti2);
            _context.Add(tr121);
            _context.Add(tr122);
            _context.Add(tr123);
            var ti3 = new TastingItem { Name = "s1 t3", Whiskey = w1, Survey = survey, Uuid = System.Guid.NewGuid().ToString() };
            var tr131 = new TastingResponse { Id = 7, SessionId = sessionid1, TastingItem = ti3, Taste = 70, Aroma = 70, Finish = 70, UserName = "user1", WhiskeyGuess = w1 };
            var tr132 = new TastingResponse { Id = 8, SessionId = sessionid2, TastingItem = ti3, Taste = 70, Aroma = 70, Finish = 70, UserName = "user2", WhiskeyGuess = w2 };
            var tr133 = new TastingResponse { Id = 9, SessionId = sessionid3, TastingItem = ti3, Taste = 70, Aroma = 70, Finish = 70, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti3);
            _context.Add(tr131);
            _context.Add(tr132);
            _context.Add(tr133);
            
            
            

            var ti21 = new TastingItem { Name = "s2 t1", Whiskey = w3, Survey = survey2, Uuid = System.Guid.NewGuid().ToString() };
            var tr211 = new TastingResponse { Id = 10, SessionId = sessionid1, TastingItem = ti21, Taste = 50, Aroma = 50, Finish = 50, UserName = "user1", WhiskeyGuess = w1 };
            var tr212 = new TastingResponse { Id = 11, SessionId = sessionid2, TastingItem = ti21, Taste = 50, Aroma = 50, Finish = 50, UserName = "user2", WhiskeyGuess = w2 };
            var tr213 = new TastingResponse { Id = 12, SessionId = sessionid3, TastingItem = ti21, Taste = 50, Aroma = 50, Finish = 50, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti21);
            _context.Add(tr211);
            _context.Add(tr212);
            _context.Add(tr213);
            var ti22 = new TastingItem { Name = "s2 t2", Whiskey = w1, Survey = survey2, Uuid = System.Guid.NewGuid().ToString() };
            var tr221 = new TastingResponse { Id = 13, SessionId = sessionid1, TastingItem = ti22, Taste = 60, Aroma = 60, Finish = 60, UserName = "user1", WhiskeyGuess = w1 };
            var tr222 = new TastingResponse { Id = 14, SessionId = sessionid2, TastingItem = ti22, Taste = 60, Aroma = 60, Finish = 60, UserName = "user2", WhiskeyGuess = w2 };
            var tr223 = new TastingResponse { Id = 15, SessionId = sessionid3, TastingItem = ti22, Taste = 60, Aroma = 60, Finish = 60, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti22);
            _context.Add(tr221);
            _context.Add(tr222);
            _context.Add(tr223);
            var ti23 = new TastingItem { Name = "s2 t3", Whiskey = w2, Survey = survey2, Uuid = System.Guid.NewGuid().ToString() };
            var tr231 = new TastingResponse { Id = 16, SessionId = sessionid1, TastingItem = ti23, Taste = 70, Aroma = 70, Finish = 70, UserName = "user1", WhiskeyGuess = w1 };
            var tr232 = new TastingResponse { Id = 17, SessionId = sessionid2, TastingItem = ti23, Taste = 70, Aroma = 70, Finish = 70, UserName = "user2", WhiskeyGuess = w2 };
            var tr233 = new TastingResponse { Id = 18, SessionId = sessionid3, TastingItem = ti23, Taste = 70, Aroma = 70, Finish = 70, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti23);
            _context.Add(tr231);
            _context.Add(tr232);
            _context.Add(tr233);

            var ti31 = new TastingItem { Name = "s3 t1", Whiskey = w3, Survey = survey3, Uuid = System.Guid.NewGuid().ToString() };
            var tr311 = new TastingResponse { Id = 19, SessionId = sessionid1, TastingItem = ti31, Taste = 50, Aroma = 50, Finish = 50, UserName = "user1", WhiskeyGuess = w1 };
            var tr312 = new TastingResponse { Id = 20, SessionId = sessionid2, TastingItem = ti31, Taste = 50, Aroma = 50, Finish = 50, UserName = "user2", WhiskeyGuess = w2 };
            var tr313 = new TastingResponse { Id = 21, SessionId = sessionid3, TastingItem = ti31, Taste = 50, Aroma = 50, Finish = 50, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti31);
            _context.Add(tr311);
            _context.Add(tr312);
            _context.Add(tr313);
            var ti32 = new TastingItem { Name = "s3 t2", Whiskey = w1, Survey = survey3, Uuid = System.Guid.NewGuid().ToString() };
            var tr321 = new TastingResponse { Id = 22, SessionId = sessionid1, TastingItem = ti32, Taste = 60, Aroma = 60, Finish = 60, UserName = "user1", WhiskeyGuess = w1 };
            var tr322 = new TastingResponse { Id = 23, SessionId = sessionid2, TastingItem = ti32, Taste = 60, Aroma = 60, Finish = 60, UserName = "user2", WhiskeyGuess = w2 };
            var tr323 = new TastingResponse { Id = 24, SessionId = sessionid3, TastingItem = ti32, Taste = 60, Aroma = 60, Finish = 60, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti32);
            _context.Add(tr321);
            _context.Add(tr322);
            _context.Add(tr323);
            var ti33 = new TastingItem { Name = "s3 t3", Whiskey = w2, Survey = survey3, Uuid = System.Guid.NewGuid().ToString() };
            var tr331 = new TastingResponse { Id = 25, SessionId = sessionid1, TastingItem = ti33, Taste = 70, Aroma = 70, Finish = 70, UserName = "user1", WhiskeyGuess = w1 };
            var tr332 = new TastingResponse { Id = 26, SessionId = sessionid2, TastingItem = ti33, Taste = 70, Aroma = 70, Finish = 70, UserName = "user2", WhiskeyGuess = w2 };
            var tr333 = new TastingResponse { Id = 27, SessionId = sessionid3, TastingItem = ti33, Taste = 70, Aroma = 70, Finish = 70, UserName = "user3", WhiskeyGuess = w3 };
            _context.Add(ti33);
            _context.Add(tr331);
            _context.Add(tr332);
            _context.Add(tr333);
            _context.SaveChanges();

        }
        [TestMethod]
        public async Task CompleteTastingItem()
        {
            var ti = _context.CompleteTastingItem().First();
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.Survey);
            Assert.IsNotNull(ti.Whiskey);
        }
        [TestMethod]
        public async Task TIById()
        {
            var id = _context.TastingItems.First().Id;
            var ti = _context.TastingItemById(id);
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.Survey);
            Assert.IsNotNull(ti.Whiskey);
            Assert.IsNotNull(ti.TastingResponses);

             ti = _context.TastingItemById(4);
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.Survey);
            Assert.IsNotNull(ti.Whiskey);
            Assert.IsNotNull(ti.TastingResponses);
        }
        [TestMethod]
        public async Task GetWhiskyYears()
        {
            var years = _context.GetAllWhiskeyYears();
            Assert.IsNotNull(years);
            Assert.IsTrue(years.Any());
            Assert.IsTrue(years.Contains(System.DateTime.Now.Year));
        }
        [TestMethod]
        public async Task ResponseBySessionClosed()
        {
            var response = _context.ResponseBySession(sessionid1, 1);
            Assert.IsNotNull(response);
            Assert.IsFalse(response.Any());
            Assert.IsFalse(response.Count() == 1);

        }
        [TestMethod]
        public async Task ResponseBySessionOpen()
        {
            var response = _context.ResponseBySession(sessionid1, 4);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Any());
            Assert.IsTrue(response.Count() == 1);

        }
        [TestMethod]
        public async Task SaveResponse()
        {
            var average = 60;
            var trs = new TastingResponseSave { SessionId = sessionid2, TastingItem = 4, Aroma = 40, Taste = 40, Finish = 40, WhiskeyGuess = _context.Whiskeys.Where(x => x.WhiskeyName == "Test3").FirstOrDefault().WhiskeyId, Id = 11};
            
            Assert.IsTrue(_context.SaveTastingResponse(trs));

            Assert.AreNotEqual(average, _context.TastingItemById(4).GetAverageScore());
            Assert.AreEqual(_context.ResponseBySession(sessionid2, 4).First().WhiskeyGuess.WhiskeyName, "Test3");
        }
        [TestMethod]
        public async Task SaveResponseClosed()
        {
            var trs = new TastingResponseSave { SessionId = sessionid2, TastingItem = 3, Aroma = 40, Taste = 40, Finish = 40, WhiskeyGuess = _context.Whiskeys.Where(x => x.WhiskeyName == "Test3").FirstOrDefault().WhiskeyId, Id = 8 };
            Assert.IsFalse(_context.SaveTastingResponse(trs));
            
        }
        [TestMethod]
        public async Task WhiskeysInSurvey()
        {
            var whiskys = _context.WhiskeysInSurvey(1);
            Assert.IsNotNull(whiskys);
            Assert.IsTrue(whiskys.Count() == 3);
        }
        [TestMethod]
        public async Task WhiskeysInSurveyInvalid()
        {
            var whiskys = _context.WhiskeysInSurvey(5);
            Assert.IsNotNull(whiskys);
            Assert.IsTrue(whiskys.Count() == 0);
        }
        [TestMethod]
        public async Task SurveyByYear()
        {
            var surveys = _context.SurveyByYear(System.DateTime.Now.Year);
            Assert.IsNotNull(surveys);
            Assert.IsTrue(surveys.Count() == 1);
            Assert.IsTrue(surveys.First().End.Year == System.DateTime.Now.Year);
            Assert.IsNotNull(surveys.First().Tastings);
            Assert.IsNotNull(surveys.First().Tastings.First().Whiskey);
            Assert.IsNotNull(surveys.First().Tastings.First().TastingResponses);
        }
        [TestMethod]
        public async Task SurveyByYearInvalid()
        {
            var surveys = _context.SurveyByYear(System.DateTime.Now.AddYears(-2).Year);
            Assert.IsNotNull(surveys);
            Assert.IsTrue(surveys.Count() == 0);
        }
        [TestMethod]
        public async Task GetSurveyYears()
        {
            var years = _context.GetAllSurveyYears();
            Assert.IsNotNull(years);
            Assert.IsTrue(years.Count() == 2);
            Assert.IsTrue(years.Contains(System.DateTime.Now.Year));
            Assert.IsTrue(years.Contains(System.DateTime.Now.AddYears(-1).Year));

        }
        [TestMethod]
        public async Task SurveyByUuid()
        {
            var survey = _context.SurveyByUuid(uuid1);
            Assert.IsNotNull(survey);
            Assert.IsFalse(survey.IsOpen());
            Assert.IsTrue(survey.Title == "Test Title");
        }
        [TestMethod]
        public async Task SurveyByUuidInvalid()
        {
            var survey = _context.SurveyByUuid(System.Guid.NewGuid().ToString());
            Assert.IsNull(survey);
        }
        [TestMethod]
        public async Task SurveyById()
        {
            var survey = _context.SurveyById(1);
            Assert.IsNotNull(survey);
            Assert.IsTrue(survey.End.Year == System.DateTime.Now.AddYears(-1).Year);
            Assert.IsNotNull(survey.Tastings);
            Assert.IsNotNull(survey.Tastings.First().Whiskey);
            Assert.IsNotNull(survey.Tastings.First().TastingResponses);
        }
        [TestMethod]
        public async Task SurveyByIdInvalid()
        {
            var survey = _context.SurveyById(55);
            Assert.IsNull(survey);
        }
        [TestMethod]
        public async Task SurveyFromTastingItem()
        {
            var survey = _context.SurveyFromTastingItem(4);
            Assert.IsNotNull(survey);
            Assert.IsTrue(survey.IsOpen());
            Assert.IsTrue(survey.Title == "Test Title2");
        }
        [TestMethod]
        public async Task SurveyFromTastingItemInvalid()
        {
            var survey = _context.SurveyFromTastingItem(55);
            Assert.IsNull(survey);
        }
        [TestMethod]
        public async Task AverageScore()
        {
            var rand = new Random();
            List<int> numbers = new List<int>();
            var tastintItem = _context.TastingItemById(3);
            foreach (var item in tastintItem.TastingResponses)
            {
                var score = rand.Next(1,100);
                numbers.Add(score);
                item.Aroma = score;

                score = rand.Next(1,100);
                numbers.Add(score);
                item.Taste = score;

                score = rand.Next(1,100); 
                numbers.Add(score);
                item.Finish = score;
            }
            Assert.AreEqual(tastintItem.GetAverageScore(), numbers.Average(),0.001);
        }
    }
}
