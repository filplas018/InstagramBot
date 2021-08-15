using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Instabot
{
    class Program
    {
        private static UserSessionData user;
        private static IInstaApi api;


        static void Main(string[] args)
        {
            
            user = new UserSessionData();
            user.UserName = "kocour_endorfin"; //User name
            user.Password = "Falco77!"; //password

            Login();
            
            Console.Read();

        }
        public static async void Login()
        {
            api = InstaApiBuilder.CreateBuilder()
                .SetUser(user)
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .SetRequestDelay(RequestDelay.FromSeconds(8, 8))
                .Build();

            var loginRequest = await api.LoginAsync();
            if (loginRequest.Succeeded)
            {
                Console.WriteLine("LOGGED IN!");
                SendMessage("total_filin"); //měníš tady
            }
            else
            {
                Console.WriteLine("ERROR LOGGING IN! :(\n" + loginRequest.Info.Message);
            }
        }
        public static async void SendMessage(string recipient)
        {
            IResult<InstaUser> userToContact = await api.GetUserAsync(recipient); //samostatný příjemce
            InstaRankedRecipientThread threadik = new InstaRankedRecipientThread(); //thread příjemce s userem
            var usersFollowers = await api.GetUserFollowersAsync(user.UserName, PaginationParameters.MaxPagesToLoad(5)); // seznam followerů
            var recipientsResult = await api.GetRankedRecipientsAsync(); //seznam chatů
            if (!recipientsResult.Succeeded)
            {
                Console.WriteLine("Unable to get ranked recipients");
                return;
            }
            foreach (var thread in recipientsResult.Value.Threads)
            {
                
                if (thread.ThreadTitle == recipient)
                {
                    threadik = thread;
                }
            }
            
           var feed = await api.GetTagFeedAsync("meme", PaginationParameters.MaxPagesToLoad(1)); //seznam příspěvků podle hashtagu
            var feedToSend = feed.Value.Medias.Where(x=>x.MediaType == InstaMediaType.Image || x.MediaType == InstaMediaType.Carousel).Take(6); // pouze 5
            foreach (var i in feedToSend)
            {
                var result =
                    await api.ShareMedia(i.Caption.MediaId, i.MediaType, threadik.ThreadId);
            }
            var inboxThreads = await api.GetDirectInboxAsync();
            if (!inboxThreads.Succeeded)
            {
                Console.WriteLine("Unable to get inbox");
                return;
            }
            //
            
        }
       
    }
}




//Console.WriteLine("User: " + userToContact.Value.FullName);
//IResult<InstaDirectInboxThreadList> mess = await api.SendDirectMessage(userToContact.Value.Pk.ToString(), string.Empty, "Cau vole");
/*var recipientsResult = await api.GetRankedRecipientsAsync(); //seznam chatů
if (!recipientsResult.Succeeded)
{
    Console.WriteLine("Unable to get ranked recipients");
    return;
}
Console.WriteLine($"Got {recipientsResult.Value.Threads.Count} ranked threads");
foreach (var thread in recipientsResult.Value.Threads)
{
    Console.WriteLine($"Threadname: {thread.ThreadTitle}, users: {thread.Users.Count}");
    if(thread.ThreadTitle == recipient)
    {
        IResult<InstaDirectInboxThreadList> mess = await api.SendDirectMessage(userToContact.Value.Pk.ToString(), string.Empty, "Cau vole");
        var result =
        await api.ShareMedia("1866111698328767752_3255807", InstaMediaType.Image, thread.ThreadId);
    }
}*/
