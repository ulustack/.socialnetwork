using System;
using System.CodeDom;
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
using System.Configuration;


namespace ulustack
{
    public class PostFlake
    {
        public string postOwner { get; set; }
        public string postText { get; set; }
        public string postTimestamp { get; set; }
        public string postImageURL { get; set; }
        public int postLikeCount { get; set; }
        public int postCommentCount { get; set; }
        public string postLikeRefID { get; set; }
        public string postCommentRefID { get; set; }
        public bool isDeleted { get; set; }
        public string postID { get; set; }
    }

    public class CommentFlake
    {
        public string postRefID { get; set; }
        public string commentID { get; set; }
        public string commentTimestamp { get; set; }
        public string commentOwnerID { get; set; }
        public string commentText { get; set; }
        public bool isDeleted { get; set; }


    }

    public class LikeFlake
    {
        public string postRefID { get; set; }
        public string likeOwnerID { get; set; }
        public string likeTimestamp { get; set; }

    }
    internal class PostOps
    {
        public static string GetBCryptHash(string plaintext)
        {
            string hash = BCrypt.Net.BCrypt.EnhancedHashPassword(plaintext, 13);
            return hash;
        }
        public static Dictionary<string, PostFlake> Posts { get; private set; } //post kimliği, post içeriği
        public static Dictionary<string, List<LikeFlake>> Likes { get; private set; } //post referans idsi, like flake'leri

        public static Dictionary<string, List<CommentFlake>> Comments { get; private set; } //post referans idsi, comment flake'leri

        //public static Dictionary<string, CommentFlake>> CodeCommentStatement //buraya geri gel
        public void LoadPosts(string databaseFile)
        {
            string json = File.ReadAllText(".\\ulustackasp\\database\\posts.json");
            JObject rawData = JObject.Parse(json);
            JObject posts_j = (JObject)rawData.SelectToken("Posts");
            Posts = new Dictionary<string, PostFlake>();
            Posts = posts_j.ToObject<Dictionary<string, PostFlake>>();
            List<string> keysList = new List<string>(Posts.Keys);
        }

        public void LikePost(string postRefID, string userID)
        {
            string timestamp = string.Concat(DateTime.UtcNow.ToString("s"), "Z"); // iso 8601
            LikeFlake likeDetails = new LikeFlake();
            likeDetails.likeOwnerID = userID;
            likeDetails.postRefID = postRefID;
            likeDetails.likeTimestamp = timestamp;
            if (Likes.ContainsKey(postRefID))
            {
                Likes[postRefID].Add(likeDetails);
            }
            else
            {
                List<LikeFlake> likeList = new List<LikeFlake>();
                Likes.Add(postRefID, likeList);
                Likes[postRefID].Add(likeDetails);
            }
            
        }

        public void CreateComment(string postRefID, string userID, string commentText)
        {
            string timestamp = string.Concat(DateTime.UtcNow.ToString("s"), "Z"); // iso 8601
            CommentFlake commentDetails = new CommentFlake();
            Guid commentID = Guid.NewGuid();
            string commentIDString = commentID.ToString();
            commentDetails.commentID = commentIDString;
            commentDetails.commentText = commentText;
            commentDetails.commentOwnerID = userID;
            commentDetails.commentTimestamp = timestamp;
            commentDetails.isDeleted = false;
            commentDetails.postRefID = postRefID;

            if (Comments.ContainsKey(postRefID))
            {
                Comments[postRefID].Add(commentDetails);
            }
            else
            {
                List<CommentFlake> commentsList = new List<CommentFlake>();
                Comments.Add(postRefID, commentsList);
                Comments[postRefID].Add(commentDetails); //250505
            }
        }

        public void DeleteComment(string postRefID, string commentID)
        {
            CommentFlake tempComment = new CommentFlake();
            foreach (var comment in Comments[postRefID])
            {
                if (comment.commentID == commentID)
                {
                    tempComment = comment;
                }
            }

            Comments[postRefID].Remove(tempComment); //aşağıdaki fonksiyondan sonra yazdım ve aynısı geçerli.
        }

        public void UnlikePost(string postRefID, string UserID)
        {
            LikeFlake tempLike = new LikeFlake();
            foreach (var like in Likes[postRefID])
            {
                if (like.likeOwnerID == UserID)
                {
                    tempLike = like;
                }
            }
            Likes[postRefID].Remove(tempLike);//burada kontrol yok çünkü halihazırda kontrol yapılmış olmalı. yoksa ben de biliyorum berbat bir kod.

        }
        public void LoadInteractions(string databaseFile)
        {
            string json = File.ReadAllText(".\\ulustackasp\\database\\interactions.json");
            JObject rawData = JObject.Parse(json);
            JObject likes_j = (JObject)rawData.SelectToken("Likes");
            JObject comments_j = (JObject)rawData.SelectToken("Comments");
            Likes = new Dictionary<string, List<LikeFlake>>();
            Likes = likes_j.ToObject<Dictionary<string, List<LikeFlake>>>();
            Comments = new Dictionary<string, List<CommentFlake>>();
            Comments = comments_j.ToObject<Dictionary<string, List<CommentFlake>>>();

        }

        

        public PostFlake GetPostByID(string postID)
        {
            return Posts[postID];
        }
        public void CreatePost(string postOwnerID, string postText, string postImageURL)
        {
            string databaseFile = ".\\ulustackasp\\database\\posts.json";
            Guid postID = Guid.NewGuid();
            Guid likeRefID = Guid.NewGuid();
            Guid commentRefID = Guid.NewGuid();
            string postIDString = postID.ToString();
            string likeRefIDString = likeRefID.ToString();
            string commentRefIDString = commentRefID.ToString();
            string timestamp = string.Concat(DateTime.UtcNow.ToString("s"), "Z"); // iso 8601


            PostFlake postDetails = new PostFlake();
            postDetails.postOwner = postOwnerID;
            postDetails.postText = postText;
            postDetails.postImageURL = postImageURL;
            postDetails.postTimestamp = timestamp;
            postDetails.postLikeCount = 0;
            postDetails.postCommentCount = 0;
            postDetails.postLikeRefID = likeRefIDString;
            postDetails.postCommentRefID = commentRefIDString;
            postDetails.postID = postIDString;
            postDetails.isDeleted = false;
            //help

            //Console.WriteLine(j);
            
            Posts.Add(postIDString, postDetails);
        }

        public void DeletePost(string postID)
        {
            Posts.Remove(postID);
        }

        public bool CheckIfPostExists(string postID)
        {
            return Posts.ContainsKey(postID);
        }

        public void ReturnAllComments(string postID)
        {

        }

        public void ReturnAllLikes(string postID)
        {

        }

        public void ReturnUserPosts(string userID)
        {

        }


        

        public static void dumpDatabases()
        {
            string postDB = ".\\ulustackasp\\database\\posts.json";
            var toDump = new
            {
                Posts = Posts
            };
            File.WriteAllText(postDB, JsonConvert.SerializeObject(toDump, Formatting.Indented));
            string likeDB = ".\\ulustackasp\\database\\interactions.json";
            var toDump2 = new
            {
                Likes = Likes,
                Comments = Comments
            };
            File.WriteAllText(likeDB, JsonConvert.SerializeObject(toDump2, Formatting.Indented));
        }
    }
}
// chatgpt kullanmadım :güneş gözlüğü emojisi: