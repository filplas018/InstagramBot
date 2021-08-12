using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Classes.Models;
using InstaSharper.Logger;
using System;

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
                SendMessage("kucerova_v"); //měníš tady
            }
            else
            {
                Console.WriteLine("ERROR LOGGING IN! :(\n" + loginRequest.Info.Message);
            }
        }
        public static async void SendMessage(string recipient)
        {
            IResult<InstaUser> userToContact = await api.GetUserAsync(recipient);

            Console.WriteLine("User: " + userToContact.Value.FullName);
            IResult<InstaDirectInboxThreadList> mess = await api.SendDirectMessage(userToContact.Value.Pk.ToString(), string.Empty, "Ahoj prdelko, jsem robot");
            
        }
    }
}
