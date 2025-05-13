using System.Runtime.InteropServices.Marshalling;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using ulustack;
using Graphs;
using Queues;
namespace ulustackasp
{
    public class FriendRequest
    {
        public string RequestTo { get; set; }
        public string RequestFrom { get; set; }
        public string Timestamp { get; set; }
    }
    public class Social
    {
        //şimdi bütün arkadaşlıkların ve takip ilişkilerinin nasıl tutulacağını belirleme zamanı.
        //arkadaşlıklar için iki tarafta da kayıt tutulacak. bu en başta böyle seçildi.
        public static Dictionary<string, List<string>> Friendships { get; private set; }
        
        //bir kullanıcının takip ettiklerini aşağıdaki veri tipi saklayacak:

        public static Dictionary<string, List<string>> Follows { get; private set; }

        //bir kullanıcıyı takip edenleri aşağıdaki saklayacak:

        public static Dictionary<string, List<string>> Followers { get; private set; }

        //bir kullanıcıya gelen arkadaşlık istekleri aşağıdaki gibi saklanacak:

        public static Dictionary<string, List<FriendRequest>> RecvdRequests { get; private set; }

        //bir kullanıcıdan giden arkadaşlık istekleri aşağıdaki gibi saklanacak:

        public static Dictionary<string, List<FriendRequest>> SentRequests { get; private set; }
        //

        //arkadaşlık ilişkileri veritabanından çekilip graf formatına dönüştürülmeli.
        //bunu aşağıdaki fonksiyona ekleyeceğim inşallah.



           
        public void LoadSocials(string databaseFile) //i hate my life i hate my life i hate my life
        {
            string json1 = File.ReadAllText(".\\ulustackasp\\database\\friends.json");
            JObject rawData1 = JObject.Parse(json1);
            string json2 = File.ReadAllText(".\\ulustackasp\\database\\follows.json");
            JObject rawData2 = JObject.Parse(json2);
            string json3 = File.ReadAllText(".\\ulustackasp\\database\\followers.json");
            JObject rawData3 = JObject.Parse(json3);
            JObject friends_j = (JObject)rawData1.SelectToken("Friends");
            JObject receivedrequests_j = (JObject)rawData1.SelectToken("RecvdRequests");
            JObject sentrequests_j = (JObject)rawData1.SelectToken("SentRequests");

            JObject follows_j = (JObject)rawData2.SelectToken("Follows");
            JObject followers_j = (JObject)rawData3.SelectToken("Followers");

            Friendships = new Dictionary<string, List<string>>();
            Follows = new Dictionary<string, List<string>>();
            Followers = new Dictionary<string, List<string>>();
            RecvdRequests = new Dictionary<string, List<FriendRequest>>();
            SentRequests = new Dictionary<string, List<FriendRequest>>();

            //initialize ettik

            Friendships = friends_j.ToObject<Dictionary<string, List<string>>>();
            Follows = follows_j.ToObject<Dictionary<string, List<string>>>();
            Followers = followers_j.ToObject<Dictionary<string, List<string>>>();
            RecvdRequests = receivedrequests_j.ToObject<Dictionary<string, List<FriendRequest>>>();
            SentRequests = sentrequests_j.ToObject<Dictionary<string, List<FriendRequest>>>();

            //yükledik.

            var fShipGraph = new Graphs.Graph<string>(); //ulan ben bile bu kadar iyi oturacağını düşünmemiştim.

            //uh oh! userops'a ihtiyacımız var. kahretsin.

            foreach (string user in UserOps.Users.Keys)
            {
                fShipGraph.KullaniciEkle(user);
            }

            //bütün kullanıcıları ekledik. şimdi hepsini el ele tutuşturalım.
            List<(string, string)> pairs = new List<(string, string)>();
            foreach (string user in Friendships.Keys)
            {
                foreach (string friend in Friendships["user"])
                {
                    if (pairs.Contains((user, friend)) || pairs.Contains((friend, user))){
                        //bunlar zaten arkadaş
                    }
                    else
                    {
                        fShipGraph.ArkadasEkle(user, friend);
                        pairs.Add((user, friend));
                    }
                        
                }
            }
            //grafa dönüştürme işlemi burada biter.



        }

        public void Befriend(string user1, string user2)
        {
            if (Friendships.ContainsKey(user1))
            {
                Friendships[user1].Add(user2);
            }
            else
            {
                List<string> friendsList = new List<string>();
                Friendships.Add(user1, friendsList);
                Friendships[user1].Add(user2);
            }
            if (Friendships.ContainsKey(user2))
            {
                Friendships[user2].Add(user1);
            }
            else
            {
                List<string> friendsList = new List<string>();
                Friendships.Add(user2, friendsList);
                Friendships[user2].Add(user1);
            }
        }
        public void Unfriend(string user1, string user2)
        {
            Friendships[user1].Remove(user2);
            Friendships[user2].Remove(user1);
        }

        public void SendFriendRequest(string sender, string recver)
        {
            FriendRequest request = new FriendRequest();
            string timestamp = string.Concat(DateTime.UtcNow.ToString("s"), "Z");

            request.RequestFrom = sender;
            request.RequestTo = recver;
            request.Timestamp = timestamp;

            if (SentRequests.ContainsKey(sender))
            {
                SentRequests[sender].Add(request);
            }
            else
            {
                List<FriendRequest> sentList = new List<FriendRequest>();
                SentRequests.Add(sender, sentList);
                SentRequests[sender].Add(request);
            }
            if (RecvdRequests.ContainsKey(recver))
            {
                RecvdRequests[recver].Add(request);
            }
            else
            {
                List<FriendRequest> recvdList = new List<FriendRequest>();
                RecvdRequests.Add(recver, recvdList);
                RecvdRequests[recver].Add(request);
            }
        }

        public void CancelFriendRequest(string sender, string recver)
        {
            FriendRequest tempRequest = new FriendRequest();
            foreach (var request in SentRequests[sender])
            {
                if (request.RequestFrom == sender)
                {
                    tempRequest = request;
                }
            }

            SentRequests[sender].Remove(tempRequest);
            foreach (var request in RecvdRequests[recver])
            {
                if (request.RequestTo == recver)
                {
                    tempRequest = request;
                }
            }

            RecvdRequests[recver].Remove(tempRequest);
        }

        public void FollowUser(string follower, string followed)
        {
            if (Follows.ContainsKey(follower))
            {
                Follows[follower].Add(followed);
            }
            else
            {
                List<string> followsList = new List<string>();
                Follows.Add(follower, followsList);
                Follows[follower].Add(followed);
            } //this is getting boring
        }

        public void UnfollowUser(string follower, string followed)
        {
            Follows[follower].Remove(followed); //tekrar söylüyorum. önceden kontrol et.
        }

        public bool CheckIfFriends(string user1, string user2)
        {
            if (!Friendships.ContainsKey(user1))
            {
                List<string> friendsList = new List<string>();
                Friendships.Add(user1, friendsList);

            }
            return Friendships[user1].Contains(user2);
        }

        public bool CheckIfFollows(string follower, string followed)
        {
            if (!Follows.ContainsKey(follower))
            {
                List<string> followsList = new List<string>();
                Follows.Add(follower, followsList);

            }
            return Follows[follower].Contains(followed);
        }

        public bool CheckIfFriendRequested(string sender, string recver)
        {
            FriendRequest tempRequest = new FriendRequest();
            tempRequest.RequestTo = "";
            if (!SentRequests.ContainsKey(sender))
            {
                return false;
            }
            foreach (var request in SentRequests[sender])
            {
                if (request.RequestFrom == sender)
                {
                    tempRequest = request;
                }
            }

            return (tempRequest.RequestTo == recver);
        }

        public List<FriendRequest> ReturnSentFriendRequests(string user)
        {
            if (!SentRequests.ContainsKey(user))
            {
                return new List<FriendRequest>();
            }
            else
            {
                return new List<FriendRequest>(SentRequests[user]);
            }
        }
        public List<FriendRequest> ReturnRecvdFriendRequests(string user)
        {
            if (!RecvdRequests.ContainsKey(user))
            {
                return new List<FriendRequest>();
            }
            else
            {
                return new List<FriendRequest>(RecvdRequests[user]);
            }
        }


        public static void dumpDatabases()
        {
            var toDump1 = new
            {
                Friends = Friendships,
                RecvdRequests = RecvdRequests,
                SentRequests = SentRequests
            };
            File.WriteAllText(".\\ulustackasp\\database\\friends.json", JsonConvert.SerializeObject(toDump1, Formatting.Indented));
            var toDump2 = new
            {
                Follows = Follows,
            };
            File.WriteAllText(".\\ulustackasp\\database\\follows.json", JsonConvert.SerializeObject(toDump2, Formatting.Indented));
            var toDump3 = new
            {
                Followers = Followers,
            };
            File.WriteAllText(".\\ulustackasp\\database\\followers.json", JsonConvert.SerializeObject(toDump3, Formatting.Indented));
        }
    }
}
