namespace PWSUnitTests
{
    [TestClass]
    public class WhiskyUnitTests
    {
        // IMPORTANT! Make sure to have the same params as this!
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // This runs once before any tests
            // If you want to run stuff after, use ClassCleanup
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // This runs before each test, put code you need to run for all tests here
            // If you want to run stuff after each test, use TestCleanup
        }

        [TestMethod]
        public void TestWhiskyMiniDescriptionAboveThreshold()
        {
            // Arrange: Create whisky with more than 100 char length description
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Rerum sint officia mollitia. Rerum neque cupiditate quas expedita cumque aliquam. A qui temporibus aut cumque omnis illum. Quo animi est aut in quia esse. Velit quo quis animi. Error est numquam culpa quaerat ex.\r\n\r\nVoluptates ut quis non. Sapiente at consequatur est qui in. Quia ea debitis dicta amet voluptatem quos. Quaerat recusandae aut eos omnis et voluptatem quidem. Consequatur mollitia sit quaerat dolores perferendis blanditiis cumque.\r\n\r\nAb aperiam assumenda fuga ipsam quibusdam tempore explicabo rem. Enim inventore beatae qui. Fuga officia unde aut sit ex consequatur. Reiciendis ratione et tempora voluptates qui est.\r\n\r\nVoluptates natus soluta sit fuga qui atque et. Possimus tenetur et voluptatem eos neque at. Iste quia voluptate possimus itaque ea vel est. Suscipit corporis dolor ut temporibus necessitatibus praesentium id atque. Voluptatem voluptatum facilis corporis.\r\n\r\nArchitecto officia voluptates quisquam libero rerum culpa qui veniam. Eos quam sapiente saepe ut. Occaecati voluptate voluptatibus maiores harum repellendus. Omnis et eos nostrum laudantium reiciendis sed veniam. Rerum illum voluptatum doloribus in molestiae quo.\r\n"
            };
            // Assert: Is MiniDescription 100 chars
            Assert.IsTrue(w.MiniDescription.Length == Whiskey.MiniDescriptionLength);
        }
        [TestMethod]
        public void TestWhiskyMiniDescriptionBelowThreshold()
        {
            // Arrange: Create whisky below 100 char length description
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyDescription = "Test"
            };
            // Assert: Is MiniDescription same as normal description
            Assert.IsTrue(w.MiniDescription == w.WhiskeyDescription);
        }

        [TestMethod]
        public void WhiskeyScoreSetting_ManualSub()
        {
            // Arrange: Create test item using ManualSub score setting
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualSub,
                WhiskeyFinish = 50,
                WhiskeyAroma = 50,
                WhiskeyTaste = 50,
                TotalScore = 1 // This should be ignored
            };

            // Assert: Check if score returns a generated score
            Assert.IsTrue(w.TotalScore == 50); // (50 + 50 + 50) / 3
        }

        [TestMethod]
        public void WhiskeyScoreSetting_ManualTotal()
        {
            // Arrange: Create test item using ManualSub score setting
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                WhiskeyFinish = 50,
                WhiskeyAroma = 50,
                WhiskeyTaste = 50,
                TotalScore = 1 // This should be used instead
            };

            // Assert: Check if score returns the total score
            Assert.IsTrue(w.TotalScore == 1); // Should equal total score
        }

        /// <summary>
        /// TODO: Needs to be expanded later to include tasting details info
        /// </summary>
        [TestMethod]
        public void HasDateForGetTasteDetailsString()
        {
            // Arrange: Create Whisky with date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                TastedDate = new DateTime(2024, 1, 1),
            };

            // Assert: Check if method returns the month and year set
            Assert.IsTrue(w.GetTasteDetailsString().Contains("January"));
            Assert.IsTrue(w.GetTasteDetailsString().Contains("2024"));
        }

        [TestMethod]
        public void HasDateForGetTasteDateString()
        {
            // Arrange: Create Whisky with date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                TastedDate = new DateTime(2024, 1, 1),
            };

            // Assert: Check if method returns the month and year set
            Assert.IsTrue(w.GetTasteDetailsString().Contains("January"));
            Assert.IsTrue(w.GetTasteDetailsString().Contains("2024"));
        }

        [TestMethod]
        public void IsTastedDateVaild_NotScored()
        {
            // Arrange: Create whiskey with WhiskeyScoreSetting == NotScored with no date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.NotScored
            };

            // Assert: Check if vaild
            Assert.IsTrue(w.IsTastedDateVaild());
        }

        [TestMethod]
        public void IsTastedDateVaild_SurveyResults()
        {
            // Arrange: Create whiskey with WhiskeyScoreSetting == SurveyResults with no date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.SurveyResults
            };

            // Assert: Check if vaild
            Assert.IsTrue(w.IsTastedDateVaild());
        }

        [TestMethod]
        public void IsTastedDateVaild_ManualTotal_MissingDate()
        {
            // Arrange: Create whiskey with WhiskeyScoreSetting == SurveyResults with no date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal
            };

            // Assert: Check if vaild
            Assert.IsFalse(w.IsTastedDateVaild());
        }

        [TestMethod]
        public void IsTastedDateVaild_ManualTotal_WithDate()
        {
            // Arrange: Create whiskey with WhiskeyScoreSetting == SurveyResults with no date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal,
                TastedDate = new DateTime(1, 1, 1)
            };

            // Assert: Check if vaild
            Assert.IsTrue(w.IsTastedDateVaild());
        }

        [TestMethod]
        public void IsTastedDateVaild_ManualSub_MissingDate()
        {
            // Arrange: Create whiskey with WhiskeyScoreSetting == SurveyResults with no date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualSub
            };

            // Assert: Check if vaild
            Assert.IsFalse(w.IsTastedDateVaild());
        }

        [TestMethod]
        public void IsTastedDateVaild_ManualSub_WithDate()
        {
            // Arrange: Create whiskey with WhiskeyScoreSetting == SurveyResults with no date
            var w = new Whiskey()
            {
                WhiskeyName = "TestWhisky",
                WhiskeyScoreSetting = WhiskeyScoreSetting.ManualSub,
                TastedDate = new DateTime(1, 1, 1)
            };

            // Assert: Check if vaild
            Assert.IsTrue(w.IsTastedDateVaild());
        }
    }
}