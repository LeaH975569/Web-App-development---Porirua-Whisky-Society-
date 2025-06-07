using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PWS.Data;
using PWS.Models;

namespace PWS.Services
{
    public class Initializer
    {
        // seed function add calls to seed data methods here
        public static async void seed(IApplicationBuilder builder, bool forceReset = false)
        {
            await AddRoles(builder);
            CreateWhiskyItems(builder, forceReset);
        }

        //seed an admin account with admin role 
        private static async Task AddRoles(IApplicationBuilder builder)
        {
            //get the required services for building users with roles and saving to DB
            var scope = builder.ApplicationServices.CreateScope();
            var roleMan = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userMan = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //If Admin role does not exist add the role
            var role = "Admin";
            if (!await roleMan.RoleExistsAsync(role)) await roleMan.CreateAsync(new IdentityRole(role));

            //Email for admin login
            var email = "Admin@admin.com";
            var password = "A_dmin1234";
            //If the email doesnt already exist add a new account with that email and password 
            if (await userMan.FindByEmailAsync(email) == null)
            {
                var admin = new IdentityUser();
                admin.Email = email;
                admin.UserName = email;
                await userMan.CreateAsync(admin, password);
                await userMan.AddToRoleAsync(admin, role);
            }
            //save the new user to the database
            context.SaveChanges();
        }

        private static void CreateWhiskyItems(IApplicationBuilder builder, bool forceReset = false)
        {
            var context = builder.ApplicationServices.CreateScope().
                ServiceProvider.GetRequiredService<ApplicationDbContext>();

            string dummyDescription = "Id illum rem incidunt. Voluptas non eius sed sed. Culpa totam molestias aut eligendi tempora expedita. Magnam quis dolor nulla temporibus quis quia deserunt aliquam. Totam nihil ipsum voluptas architecto dolore libero aliquam.\r\n\r\nEt neque expedita deserunt reprehenderit. Dolores ipsam ipsum saepe vel asperiores. Repellendus quia sequi est quidem molestiae pariatur. Deleniti est tenetur atque ab expedita mollitia ut. Perspiciatis et dolorum omnis voluptatem est impedit sed.\r\n\r\nSequi quae ullam expedita. Cum aperiam quos hic assumenda error nihil mollitia quas. Nihil a sint fuga ea ullam rem excepturi.\r\n\r\nAutem aut quia culpa. Ut doloribus est assumenda enim. Et voluptatum ea porro atque quos autem voluptatum ducimus. Sunt velit qui sunt aliquam. Vitae vitae eos est molestias animi perspiciatis facere officia. Accusamus magni voluptate nemo.";

            if (forceReset)
            {
                context.Whiskeys.ExecuteDelete();
                context.SaveChanges();
            }

            if (!context.Whiskeys.Any())
            {
                context.Whiskeys.AddRange(
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/BOWMORE15.png", WhiskeyName = "Bowmore 15 Yearold", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 85, TastedDate = new DateTime(2024, 2, 2), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/Bowmore-Legend.jpg", WhiskeyName = "Bowmore Legend", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 70, TastedDate = new DateTime(2024, 2, 2), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/Bowmore-17-Whitesands.jpg", WhiskeyName = "Bowmore 17 Whitesands", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2024, 2, 2), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/port-charlotte-pac-01.png", WhiskeyName = "Port Charlotte Pac:01", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 90, TastedDate = new DateTime(2023, 3, 10), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/Old-Pultney-18.jpg", WhiskeyName = "Old Pultney 18", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 87, TastedDate = new DateTime(2023, 3, 10), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/Coal-Ila-18.jpg", WhiskeyName = "Coal Ila 18", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 80, TastedDate = new DateTime(2023, 3, 10), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/Old-Particular-Tamdhu-16.jpg", WhiskeyName = "Old Particular Tamdhu 16", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 79, TastedDate = new DateTime(2023, 3, 10), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/tasted_whiskey/Longrow-Red.jpg", WhiskeyName = "Longrow Red", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 75, TastedDate = new DateTime(2023, 3, 10), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky1", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2022, 1, 1), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky2", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2021, 1, 1), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky3", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2022, 1, 1), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky4", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2019, 1, 1), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky5", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2023, 1, 1), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky6", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2021, 1, 1), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky7", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2019, 1, 1), WhiskeyDescription = dummyDescription },
                    new Whiskey { WhiskeyImageUrl = "/images/Whiskey/Untitled1.jpg", WhiskeyName = "Dummy Whisky8", WhiskeyScoreSetting = WhiskeyScoreSetting.ManualTotal, TotalScore = 93, TastedDate = new DateTime(2022, 1, 1), WhiskeyDescription = dummyDescription }
                    );
            }
            context.SaveChanges();
        }
    }
}
