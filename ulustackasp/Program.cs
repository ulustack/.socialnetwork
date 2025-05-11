using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ulustackasp;
using Newtonsoft.Json;
namespace ulustack
{
    internal class Program
    {
        public static async Task Main(string[] args) //public, static, asynchronous, returns Task type, and it is the Main function! how fun.
        {

            UserOps userops = new UserOps();
            PostOps postops = new PostOps();
            Social social = new Social();

            UserOps.CheckFiles();

            userops.LoadUsers("");
            postops.LoadPosts("");
            postops.LoadInteractions("");
            social.LoadSocials("");
            //social.Unfriend("def", "abc");
            //social.SendFriendRequest("asdasfamanas", "qwe zayras");

            //postops.CreateComment("b6ffee70-8dfb-4cda-8afc-b7265861ce27", "kullanci", "bu asp.net e geçildikten sonra oluşturulmuş bir yorum!");
            // postops.DeleteComment("b6ffee70-8dfb-4cda-8afc-b7265861ce27", "7f11f8f3-3d2b-4901-80c1-b9cc0d01f1ea");
            // postops.LikePost("b6ffee70-8dfb-4cda-8afc-b7265861ce27", "0ca17e82-83c2-40c7-98d7-3bf6cc3d5c81");
            //postops.UnlikePost("b6ffee70-8dfb-4cda-8afc-b7265861ce27", "0ca17e82-83c2-40c7-98d7-3bf6cc3d5c81");
            //postops.CreatePost("aksdlksajdsad", "asdfas emre", "google.com.tr");
            //userops.CreateUser("john doe", "john@gmail.com", "1234", "", "");


            //userops.CreateUser("walter h. white", "walterwhite@gmail.com", "heisenberg", "", "crystal");

            var captions = File.ReadLines("ulustackasp/converted/captions.txt");
            List<string> captionList = new List<string>();
            foreach (var line in captions)
            {
                captionList.Add(line);
            }

            var users = File.ReadLines("ulustackasp/converted/users.txt");
            List<string> userList = new List<string>();
            foreach (var line in userList)
            {
                userList.Add(line);
            }



            PostOps.dumpDatabases();
            UserOps.dumpDatabases();
            Social.dumpDatabases();
            await HTTPServer.AsyncServe(args); //does c# have a lock or something on threads?
            //Console.WriteLine(postops.GetPostByID("cd06433c-580c-4b10-8148-59d922974b97").postLikeRefID);
            
        }
    }
}
