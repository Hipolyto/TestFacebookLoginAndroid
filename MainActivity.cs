using System;
using System.Collections.Generic;

using Java.Lang;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content;

using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

using Xamarin.Facebook.Share;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;


[assembly: Permission(Name = Android.Manifest.Permission.Internet)]
[assembly: Permission(Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]
[assembly: MetaData("com.facebook.sdk.ApplicationName", Value = "@string/app_name")]


namespace testFbLogin
{
    [Activity(Label = "testFbLogin", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private TextView TxtFirstName;
        private TextView TxtLastName;
        private TextView TxtName;
        private Button LoginButton;

        private ProfilePictureView mprofile;
        private LoginButton BtnFBLogin;


        private ICallbackManager _FBCallbackManager;
        private ProfileTracker _FBProfileTracker;
        private AccessTokenTracker _FBAccessTokenTracker;
        //private MyProfileTracker _FBProfileTracker;
        //private MyAccessTokenTracker mAccessTokenTraker;

        private List<string> _permissionList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            // create callback manager using CallbackManagerFactory
            _FBCallbackManager = CallbackManagerFactory.Create();

            // Set our view from the "main" layout resource


            TxtFirstName = FindViewById<TextView>(Resource.Id.TxtFirstname);
            TxtLastName = FindViewById<TextView>(Resource.Id.TxtLastName);
            TxtName = FindViewById<TextView>(Resource.Id.TxtName);
            LoginButton = FindViewById<Button>(Resource.Id.LoginButton);
            LoginButton.Tag = "login";

            mprofile = FindViewById<ProfilePictureView>(Resource.Id.ImgPro);
            BtnFBLogin = FindViewById<LoginButton>(Resource.Id.fblogin);

            _permissionList = new List<string>
            {
                "public_profile",
                "user_friends"
            };

            var btnLoginCallback = new FacebookCustomClass.FacebookCallback<LoginResult>();
            btnLoginCallback.HandleSuccess += (loginResult) => {
                UpdateVars();
                UpdateProfile(Profile.CurrentProfile);
                LoginButton.Tag = "logout";
                Toast.MakeText(this, "Login Success", ToastLength.Short).Show();
            };
            btnLoginCallback.HandleCancel += () => {
                UpdateUI(true);
                Toast.MakeText(this, "Login Cancel", ToastLength.Short).Show();
            };
            btnLoginCallback.HandleError += (loginError) => {
                if (loginError is FacebookAuthorizationException)
                {
                    Toast.MakeText(this, loginError.Message, ToastLength.Long).Show();
                }
                UpdateVars();
                UpdateUI(true);
            };

            // Registra la devolucion de llamada del login
            LoginManager.Instance.RegisterCallback(_FBCallbackManager, btnLoginCallback);

            // LoginManager.Instance.SetLoginBehavior(LoginBehavior.NativeWithFallback);
            // LoginManager.Instance.SetDefaultAudience(DefaultAudience.Everyone);

            if (BtnFBLogin != null)
            {
                BtnFBLogin.SetReadPermissions(_permissionList);
                BtnFBLogin.RegisterCallback(_FBCallbackManager, btnLoginCallback);
                BtnFBLogin.LoginBehavior = LoginBehavior.NativeWithFallback;
                BtnFBLogin.DefaultAudience = DefaultAudience.Everyone;
            }

            _FBProfileTracker = new FacebookCustomClass.CustomProfileTracker
            {
                HandleCurrentProfileChanged = (oldProfile, currentProfile) =>
                {
                    UpdateVars();
                    UpdateProfile(currentProfile);
                }
            };
            _FBProfileTracker.StartTracking();

            _FBAccessTokenTracker = new FacebookCustomClass.CustomAcessTokenTracker
            {
                HandleCurrentAccessTokenChanged = (oldAcessToken, currentAcessToken) =>
                {
                    UpdateVars();
                }
            };
            _FBAccessTokenTracker.StartTracking();


            LoginButton.Click += (sender, e) => {
                if(LoginButton.Tag.ToString() == "login")
                {
                    LoginManager.Instance.LogInWithReadPermissions(this, _permissionList);
                    Toast.MakeText(this, "Try Login", ToastLength.Short).Show();
                }
                else
                {
                    LoginManager.Instance.LogOut();
                    Toast.MakeText(this, "Logout Success", ToastLength.Short).Show();
                    LoginButton.Tag = "login";
                    UpdateProfile(null);
                }
            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            UpdateVars();

            CheckLogin();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_FBProfileTracker != null)
            {
                _FBProfileTracker.StopTracking();
            }
            if(_FBAccessTokenTracker != null)
            {
                _FBAccessTokenTracker.StopTracking();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            _FBCallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        void UpdateVars()
        {
            if(AccessToken.CurrentAccessToken != null)
            {
                MainApplication.FacebookAccessToken = AccessToken.CurrentAccessToken.Token;
                MainApplication.FacebookUserId = AccessToken.CurrentAccessToken.UserId;
            }
            else
            {
                MainApplication.FacebookAccessToken = null;
                MainApplication.FacebookUserId = null;
            }
        }

        void CheckLogin()
        {
            if (!string.IsNullOrWhiteSpace(MainApplication.FacebookAccessToken))
            {
                Toast.MakeText(this, "checklogin token ok", ToastLength.Short).Show();
                if (MainApplication.FacebookUserId != null)
                {
                    mprofile.ProfileId = MainApplication.FacebookUserId;
                }
                UpdateProfile(Profile.CurrentProfile);
            }
            else
            {
                Toast.MakeText(this, "checklogin token-no", ToastLength.Short).Show();
                UpdateProfile(null);
            }
        }

        void UpdateProfile(Profile CurrentProfile)
        {
            if (CurrentProfile != null)
            {
                try
                {
                    TxtFirstName.Text = CurrentProfile.FirstName;
                    TxtLastName.Text = CurrentProfile.LastName;
                    TxtName.Text = CurrentProfile.Name;
                    mprofile.ProfileId = CurrentProfile.Id;
                    UpdateUI(false);
                }
                catch (Java.Lang.Exception ex) 
                { 
                    Log.Debug("fbLogin", ex.Message);
                    Log.Info("fbLogin", ex.Message);
                }
            }
            else
            {
                TxtFirstName.Text = "First Name";
                TxtLastName.Text = "Last Name";
                TxtName.Text = "Name";
                mprofile.ProfileId = null;
                UpdateUI(true);
            }
        }

        void UpdateUI(bool login)
        {
            if(login)
            {
                LoginButton.Text = "Log In Facebook";
                LoginButton.Tag = "login";
            }
            else
            {
                LoginButton.Text = "log Out Facebook";
                LoginButton.Tag = "logout";
            }
        }
    }
}

