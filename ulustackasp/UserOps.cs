using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace ulustack
{
    public class UserFlake
    {
        public string Name { get; set; } 
        public string Mail { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public string Timestamp { get; set; }
        public string UniqueID { get; set; }
        public string Biography { get; set; }
        public string PictureURL { get; set; }
    }

    public class UserOps

    {
        private static UserOps _instance;
        public static UserOps Instance => _instance ?? (_instance = new UserOps());
        public static string GetBCryptHash(string plaintext)
        {
            string hash = BCrypt.Net.BCrypt.EnhancedHashPassword(plaintext, 13);
            return hash;
        }

        public static string GetSHA256Hash(string plaintext)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
            return Convert.ToHexString(bytes);
        }
        public static void CheckFiles()
        {
            string userDatabase = ".\\ulustackasp\\database\\users.json";
            string postDatabase = ".\\ulustackasp\\database\\posts.json";
            string followDatabase = ".\\ulustackasp\\database\\follows.json";
            string followerDatabase = ".\\ulustackasp\\database\\followers.json";
            string friendDatabase = ".\\ulustackasp\\database\\friends.json";
            string interactionDatabase = ".\\ulustackasp\\database\\interactions.json";
            if (!File.Exists(userDatabase))
            {
                var dataToWrite = new
                {
                    Users = new { }
                };
                string jsonString = JsonConvert.SerializeObject(dataToWrite, Formatting.Indented);
                File.WriteAllText(userDatabase, jsonString);
            }
            if (!File.Exists(postDatabase))
            {
                var dataToWrite = new
                {
                    Posts = new { }
                };
                string jsonString = JsonConvert.SerializeObject(dataToWrite, Formatting.Indented);
                File.WriteAllText(postDatabase, jsonString);
            }

            if (!File.Exists(followDatabase))
            {
                var dataToWrite = new
                {
                    Follows = new { }
                };
                string jsonString = JsonConvert.SerializeObject(dataToWrite, Formatting.Indented);
                File.WriteAllText(followDatabase, jsonString);
            }
            if (!File.Exists(followerDatabase))
            {
                var dataToWrite = new
                {
                    Followers = new { }
                };
                string jsonString = JsonConvert.SerializeObject(dataToWrite, Formatting.Indented);
                File.WriteAllText(followerDatabase, jsonString);
            }
            if (!File.Exists(friendDatabase))
            {
                var dataToWrite = new
                {
                    Friends = new { },
                    SentRequests = new {},
                    RecvdRequests = new {}
                };
                string jsonString = JsonConvert.SerializeObject(dataToWrite, Formatting.Indented);
                File.WriteAllText(friendDatabase, jsonString);
            }
            if (!File.Exists(interactionDatabase))
            {
                var dataToWrite = new
                {
                    Likes = new { },
                    Comments = new { }
                };
                string jsonString = JsonConvert.SerializeObject(dataToWrite, Formatting.Indented);
                File.WriteAllText(interactionDatabase, jsonString);
            }


        }

        public void CreateUser(string visiblename, string email, string password, string url, string bio)
        {
            string databaseFile = ".\\ulustackasp\\database\\users.json";
            Guid userID = Guid.NewGuid();
            string userIDString = userID.ToString();
            string salt = KeyGenerator.GetUniqueKey(16);
            string timestamp = string.Concat(DateTime.UtcNow.ToString("s"), "Z"); // iso 8601
            UserFlake userDetails = new UserFlake();
            userDetails.Name = visiblename;
            userDetails.Mail = email;
            userDetails.PasswordSalt = salt;
            userDetails.PasswordHash = GetSHA256Hash(password + salt); //safer than sha256, says the internet 
            userDetails.Timestamp = timestamp;
            userDetails.UniqueID = userIDString;
            userDetails.PictureURL = url;
            userDetails.Biography = bio;
            //Console.WriteLine("totally safe printing practice btw");
            //i know i could definitely use sql instead of this abomination

            Users.Add(userIDString, userDetails);

            //Console.WriteLine(j);

        }

        public UserFlake FetchUserByID(string userid)
        {
            return Users[userid];
        }

        public UserFlake FetchUserByMail(string mail)
        {
            return Users[MailToID[mail]];
        }
        public static Dictionary<string, UserFlake> Users { get; private set; }
        public static Dictionary<string, string> MailToID { get; private set; }

        public void LoadUsers(string databaseFile)
        {
            string json = File.ReadAllText(".\\ulustackasp\\database\\users.json");
            JObject rawData = JObject.Parse(json);
            JObject users_j = (JObject)rawData.SelectToken("Users");
            Users = new Dictionary<string, UserFlake>();
            MailToID = new Dictionary<string, string>();
            Users = users_j.ToObject<Dictionary<string, UserFlake>>();
            List<string> keysList = new List<string>(Users.Keys);
            foreach (string key in keysList) 
            {
                MailToID.Add(Users[key].Mail, key);
                //Console.WriteLine(key);
                //Console.WriteLine(Users[key].Mail);
            }
        }

        public int CheckAuth(string email, string password)
        {
            if (!MailToID.ContainsKey(email))
            {
                return -1; //user doesn't even exist :(
            }
            else
            {
                string user = MailToID[email];
                UserFlake userFlake = new UserFlake();
                userFlake = Users[user];
                string salt = userFlake.PasswordSalt;
                string targetHash = GetSHA256Hash(password + salt);
                Console.WriteLine(userFlake.PasswordHash);
                Console.WriteLine(userFlake.PasswordSalt);
                Console.Write(targetHash);
                if (userFlake.PasswordHash == targetHash)
                {
                    return 0; // success!
                }
                else
                {
                    return 1; // wrong password :((
                }

            }
        }

        public static void dumpDatabases()
        {
            string userDB = ".\\ulustackasp\\database\\users.json";
            var toDump = new
            {
                Users = Users
            };
            File.WriteAllText(userDB, JsonConvert.SerializeObject(toDump, Formatting.Indented));
        }

    }
    }

