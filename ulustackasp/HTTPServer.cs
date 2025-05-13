using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ulustackasp;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;

using Microsoft.AspNetCore.SignalR.Client;

// ister inanın ister inanmayın, bu projenin kodlarının %95'i falan elle yazıldı. 
// chatgpt yok.

//HER REQUEST İLE BERABER BİR OTURUM KİMLİĞİ GÖNDERİLECEK. YAPACAK BİR ŞEY YOK KARDEŞ ÇÖZEMEDİM BAŞKA TÜRLÜ
//şurda kalmış 2 gün 
//hacı babam bu kodların hali nedir böyle
namespace ulustack
{



    public class AppHub : Hub
    {
        public static Dictionary<string, string> authToID { get; private set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> IDtoAuth { get; private set; } = new Dictionary<string, string>();
        public static FastAccess.HashSet<string> authSet = new FastAccess.HashSet<string>();
        public async Task SendMessageToClient(string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", message);

            // await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public void HandleOwnProfile(string session)
        {
            string userid = authToID[session];
            UserFlake user = UserOps.Instance.FetchUserByID(userid);
            var payload = new
            {
                status = "ownProfileResponse",
                payload = new
                {
                    name = user.Name,
                    imageurl = user.PictureURL,
                    bio = user.Biography,
                    joined = user.Timestamp
                },
            };
            SendMessageToClient(JsonConvert.SerializeObject(payload));

        }
        public void AcceptCookie() //kabul ediyorum! bunlar uydurma, kuralsız ve gelişigüzel uydurulmuş fonksiyonlar.
        {
            var payload = new
            {
                status = "cookieAccepted",
                payload = new { }
            };
            SendMessageToClient(JsonConvert.SerializeObject(payload));
        }
        public void DenyCookie()
        {
            var payload = new
            {
                status = "cookieDenied",
                payload = new { }
            };
            SendMessageToClient(JsonConvert.SerializeObject(payload));
        }
        public void HandleLogin(LoginRequest request)
        {
            int result = UserOps.Instance.CheckAuth(request.Email, request.Password);
            if (result == 0)
            {
                // send user a cookie (mmm)
                Guid sessionId = Guid.NewGuid();
                string sessionIdString = sessionId.ToString();
                string userID = UserOps.MailToID[request.Email];
                authToID.Add(sessionIdString, userID);
                authSet.Add(sessionIdString);
                authSet.Add("test");
                Console.WriteLine("\nBU EKLENEN KİMLİK: ");
                Console.WriteLine(sessionIdString);
                if (authSet.Contains(sessionIdString))
                {
                    Console.WriteLine("\r\nE BURDA İŞTE");
                }
                else
                {
                    Console.WriteLine("biraz önce eklediğim şeyi bulamadım.");
                }
                IDtoAuth.Add(userID, sessionIdString);
                var payload = new
                {
                    status = "loginAccepted",
                    payload = new
                    {
                        session = sessionIdString,
                    },
                    
                };
                SendMessageToClient(JsonConvert.SerializeObject(payload));
            }
            else
            {
                // dont say anything about why cannot be logged in, just reject, no cookies for today :(
                var payload = new
                {
                    status = "loginDenied",
                    payload = new
                    {

                    },

                };
                SendMessageToClient(JsonConvert.SerializeObject(payload));
            }
        }
        public async Task HandleMessage(string message)
        {
            Console.WriteLine(message);
            try
            {
                ClientRequest request = JsonConvert.DeserializeObject<ClientRequest>(message);
                string sess = ""; 
                switch (request.Type)
                {
                    case "loginRequest":
                        LoginRequest loginRequest = new LoginRequest();
                        loginRequest.Password = request.Payload.GetValue("password").ToString();
                        loginRequest.Email = request.Payload.GetValue("email").ToString();
                        HandleLogin(loginRequest);
                        break;
                    case "cookieCheckRequest":
                        sess = request.Payload.GetValue("session").ToString();
                        if (authSet.Contains(sess))
                        {
                            AcceptCookie();
                            Console.WriteLine("çerez kabul edildi.");
                        }
                        else
                        {
                            
                            Console.WriteLine("çerez reddedildi");
                            Console.WriteLine(sess);
                            DenyCookie();
                        }

                        break;
                    case "fetchOwnProfile":
                        sess = request.Payload.GetValue("session").ToString();
                        if (authSet.Contains(sess))
                        {
                            HandleOwnProfile(sess);
                        }
                        else
                        {
                            break;
                        }
                        break;
                    case "fetchPostStream":
                        sess = request.Payload.GetValue("session").ToString();
                        if (authSet.Contains(sess))
                        {
                            break;
                        }
                        else
                        {
                            break;
                        }
                        break;
                    case "fetchCommentsFromPost":
                        break;
                    case "fetchCountsFromPost":
                        break;
                    case "fetchRecommendedFriends":
                        break;
                    
                    //saat gecenin üçü. hadi nolur bitsin.
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.Message);
            }

        }
    }
    public class ClientRequest
    {
        public string Type { get; set; }

        public JObject Payload { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class FriendRequest
    {
        public string RecipientId { get; set; }
    }

    public class FollowUserRequest
    {
        public string UserId { get; set; }
    }

    public class FetchPostRequest
    {
        public string PostId { get; set; }
    }
   
    

    internal class HTTPServer
    {
        
        public static async Task AsyncServe(string[] args) //bro what ('task' type?)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            builder.Logging.SetMinimumLevel(LogLevel.Critical);
   
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(5000); // HTTP
            });
            /*builder.Services.AddAuthentication("cookie")
                .AddCookie("cookie", options => {
                    options.LoginPath = "/auth";
                });
            */
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins("http://localhost:5000"); // frontend adresi
                });
            });

            var app = builder.Build();
            app.MapHub<AppHub>("/hub");
            app.UseCors();
            var staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "ulustackasp/content");
            var fileProvider = new PhysicalFileProvider(staticFilesPath);



            ////////////////////////////////
            ////////////////////////////////
 
            




            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = fileProvider,
                RequestPath = ""
            });
            
            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 404)
                {
                    var response = context.HttpContext.Response;
                    response.ContentType = "text/html";

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "ulustackasp/content/pages", "404.html"); //bunu bile ekledim.
                    var html = await File.ReadAllTextAsync(path);
                    await response.WriteAsync(html);
                }
            });
            
            app.MapGet("/", async context =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "ulustackasp/content/pages", "mainpage.html");

                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(path);
            });
            app.MapGet("/404", async context =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "ulustackasp/content/pages", "404.html");

                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(path);
            });
            app.MapGet("/settings", async context =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "ulustackasp/content/pages", "settings.html");

                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(path);
            });
            app.MapGet("/auth", async context =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "ulustackasp/content/pages", "auth.html");

                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(path);
            });
            app.MapGet("/mainpage", async context =>
            {
                context.Response.Redirect("/");
            });
            app.MapGet("/profile", async context =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "ulustackasp/content/pages", "profile.html");

                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(path);
            });

            app.StartAsync(); // yeah async stuff.
            await app.WaitForShutdownAsync();
        }
    }
}
