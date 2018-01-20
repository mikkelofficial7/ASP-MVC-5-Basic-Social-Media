using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "Error",
              url: "Error404",
              defaults: new { controller = "Home", action = "Error"}
            );

            routes.MapRoute(
                name: "CheckEmail",
                url: "ForgotPassword",
                defaults: new { controller = "Main", action = "CheckEmail" }
            );

            routes.MapRoute(
                name: "ChangePassword",
                url: "ChangePassword/{id}",
                defaults: new { controller = "Main", action = "ChangePassword", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Register",
                url: "Register",
                defaults: new { controller = "Main", action = "Register" }
            );

            routes.MapRoute(
                name: "Verify",
                url: "Register/Verify/{id}",
                defaults: new { controller = "Main", action = "VerifyRegister", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Home",
                url: "Home",
                defaults: new { controller = "Home", action = "Home" }
            );
            routes.MapRoute(
                name: "Main",
                url: "Homes",
                defaults: new { controller = "Home", action = "Main" }
            );

            routes.MapRoute(
                name: "Profile",
                url: "User",
                defaults: new { controller = "Home", action = "Profile"}
            );

            routes.MapRoute(
                name: "Logout",
                url: "Logout",
                defaults: new { controller = "Home", action = "Logout" }
            );

            routes.MapRoute(
               name: "FriendProfile",
               url: "Users/{id}",
               defaults: new { controller = "Home", action = "FriendProfile", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "DeletePhoto",
               url: "Profile",
               defaults: new { controller = "Home", action = "Profiles"}
           );
            routes.MapRoute(
               name: "EditProfile",
               url: "Edit",
               defaults: new { controller = "Home", action = "EditProfile"}
           );
            routes.MapRoute(
               name: "DeleteTweet",
               url: "DelTweet/{id}",
               defaults: new { controller = "Home", action = "DeleteTweet", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "DeleteTweets",
               url: "DelTweets/{id}",
               defaults: new { controller = "Home", action = "DeleteTweets", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "RemoveFriend",
               url: "Remove/{id}",
               defaults: new { controller = "Home", action = "RemoveFriend", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "AddFriend",
               url: "Add/{id}",
               defaults: new { controller = "Home", action = "AddFriend", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "LikeTweet",
               url: "Like/{id}/{idTeman}",
               defaults: new { controller = "Home", action = "LikeTweet", id = UrlParameter.Optional, idTeman = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "UnlikeTweet",
               url: "Unlike/{id}/{idTeman}",
               defaults: new { controller = "Home", action = "UnlikeTweet", id = UrlParameter.Optional, idTeman = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "LikeTweetMain",
              url: "Likes/{idTeman}/{id}",
              defaults: new { controller = "Home", action = "LikeTweetMain", idTeman = UrlParameter.Optional, id = UrlParameter.Optional }
          );

            routes.MapRoute(
               name: "UnlikeTweetMain",
               url: "Unlikes/{idTeman}/{id}",
               defaults: new { controller = "Home", action = "UnlikeTweetMain", idTeman = UrlParameter.Optional, id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "RefreshTweet",
               url: "Ref",
               defaults: new { controller = "Home", action = "RefreshTweet" }
           );
            routes.MapRoute(
               name: "Following",
               url: "MyFollowing",
               defaults: new { controller = "Home", action = "Following" }
           );
            routes.MapRoute(
               name: "F-Following",
               url: "FriendFollowing/{id}",
               defaults: new { controller = "Home", action = "FriendFollowing", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Follower",
               url: "MyFollower",
               defaults: new { controller = "Home", action = "Follower" }
           );
            routes.MapRoute(
               name: "F-Follower",
               url: "FriendFollower/{id}",
               defaults: new { controller = "Home", action = "FriendFollower", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "ListOfFriend",
               url: "FriendList",
               defaults: new { controller = "Home", action = "FriendList" }
           );
            routes.MapRoute(
               name: "Login",
               url: "Login",
               defaults: new { controller = "Main", action = "Login" }
           );
            routes.MapRoute(
               name: "EditThemes",
               url: "Themes",
               defaults: new { controller = "Home", action = "ThemeUpload" }
           );
            routes.MapRoute(
               name: "DeleteThemes",
               url: "RemoveThemes",
               defaults: new { controller = "Home", action = "ThemeRestore" }
           );
            routes.MapRoute(
               name: "SimpleTour",
               url: "SimpleTour",
               defaults: new { controller = "Main", action = "BasicTutorial" }
           );

            /* LETAKKAN ROUTE YANG INGIN DIGANTI SEBELUM "DEFAULT" ROUTENYA */

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Main", action = "Intro" }
            );
        }
    }
}
