using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using JWTAuthentication.API.Infrastructure;
using JWTAuthentication.API.Models;
using JWTAuthentication.API.Providers;
using Microsoft.Owin.Security.Jwt;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Owin.Security.DataHandler.Encoder;

[assembly: OwinStartup(typeof(JWTAuthentication.API.Startup))]
namespace JWTAuthentication.API
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions GoogleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            #region Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(AuthContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            #endregion

            ConfigureOAuthTokenGeneration(app);
            //consume the JWT token
            //ConfigureOAuthTokenConsumption(app);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            
            app.UseWebApi(config);
        }

        /// <summary>
        /// Generate token based authentication by owin pipeline
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);

            var issuer = ConfigurationManager.AppSettings["issuer"];
            //Authentication refresh JWT token middlware
            app.UseOAuthAuthorizationServer(new Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new CustomAuthorisationServerProvider(),
                RefreshTokenProvider = new CustomRefreshTokenProvider(),
                AuthenticationType = "JWT",
                AccessTokenFormat = new CustomJwtFormat(issuer)
            });
            // Enable the application to use normal bearer tokens to authenticate users
            //OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            //app.UseOAuthBearerAuthentication(OAuthBearerOptions);
            // Enable the application to use or consume JWT tokens to authenticate users
            //To consume the JWT token, we must implement the Unprotectof the CustomJwtFormat
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            {
                AccessTokenFormat = new ConsummerJwtFormat(issuer)
            });


            //external authentication middleware
            /*//Configure Google External Login
            googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "xxxxxx",
                ClientSecret = "xxxxxx",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(googleAuthOptions);

            //Configure Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = "xxxxxx",
                AppSecret = "xxxxxx",
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(facebookAuthOptions);*/
        }
        /// <summary>
        /// Consume JWT token by ressource server (this should be a different web api with the job as a ressource server)
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            //url to authentication server
            var issuer = ConfigurationManager.AppSettings["issuer"];
            //ressource server client_id asking for authorization from the ressource server
            string clientId = ConfigurationManager.AppSettings["as:client_id"];
            //ressource server secret_id asking for authorization from the ressource server
            byte[] clientSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:client_secret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationType = "JWT",
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    AllowedAudiences = new[] { clientId },
                    IssuerSecurityKeyProviders = new List<IIssuerSecurityKeyProvider>(){
                        new SymmetricKeyIssuerSecurityKeyProvider(issuer,clientSecret)
                    }
                });
        }
    }
}