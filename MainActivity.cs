using System;
using System.Collections.Generic;

using Java.Lang;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;

using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Facebook.Share;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;
using Android.Content;

[assembly: Permission(Name = Android.Manifest.Permission.Internet)]
[assembly: Permission(Name = Android.Manifest.Permission.WriteExternalStorage)]
[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]


namespace testFbLogin
{
    [Activity(Label = "testFbLogin", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private TextView TxtFirstName;
        private TextView TxtLastName;
        private TextView TxtName;

        private MyProfileTracker mProfileTracker;
        //private ProfileTracker mProfileTracker;
        private ICallbackManager mFBCallbackManager;
        //ICallbackManager callbackManager;
        private ProfilePictureView mprofile;
        private LoginButton BtnFBLogin;


        //ShareDialog shareDialog;
        //IFacebookCallback shareCallback;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FacebookSdk.SdkInitialize(Application.Context);

            // create callback manager using CallbackManagerFactory
            mFBCallbackManager = CallbackManagerFactory.Create();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            TxtFirstName = FindViewById<TextView>(Resource.Id.TxtFirstname);
            TxtLastName = FindViewById<TextView>(Resource.Id.TxtLastName);
            TxtName = FindViewById<TextView>(Resource.Id.TxtName);
            mprofile = FindViewById<ProfilePictureView>(Resource.Id.ImgPro);

            BtnFBLogin = FindViewById<LoginButton>(Resource.Id.fblogin);
            if (BtnFBLogin != null)
            {
                BtnFBLogin.SetReadPermissions(new List<string> {
                "user_friends",
                "public_profile"
            });
                // BtnFBLogin.RegisterCallback(mFBCallManager, this);
                BtnFBLogin.RegisterCallback(mFBCallbackManager, new MyFacebookCallback<LoginResult>(this));
            }
            else
            {
                LoginManager.Instance.RegisterCallback(mFBCallbackManager, new MyFacebookCallback<LoginResult>(this));
            }

            mProfileTracker = new MyProfileTracker();
            mProfileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
            mProfileTracker.StartTracking();
        }

        void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            if (e.mProfile != null)
            {
                try
                {
                    TxtFirstName.Text = e.mProfile.FirstName;
                    TxtLastName.Text = e.mProfile.LastName;
                    TxtName.Text = e.mProfile.Name;
                    mprofile.ProfileId = e.mProfile.Id;
                }
                catch (Java.Lang.Exception ex) 
                { 
                    Log.Debug("fbLogin", ex.Message);
                }
            }
            else
            {
                TxtFirstName.Text = "First Name";
                TxtLastName.Text = "Last Name";
                TxtName.Text = "Name";
                mprofile.ProfileId = null;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            mFBCallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }
    }

    public class MyProfileTracker : ProfileTracker
    {
        public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
        {
            if (mOnProfileChanged != null)
            {
                mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(currentProfile));
            }
        }
    }

    /*
    public class MyProfileTracker2 : ProfileTracker
    {

        readonly MainActivity owner;

        public MyProfileTracker(MainActivity owner)
        {
            this.owner = owner;
        }

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
        {
            owner.UpdateUI();
            // It's possible that we were waiting for Profile to be populated in order to
            // post a status update.
            owner.HandlePendingAction();
        }
    }*/

    public class OnProfileChangedEventArgs : EventArgs
    {
        public Profile mProfile;
        public OnProfileChangedEventArgs(Profile profile)
        {
            mProfile = profile;
        }
    }


    class MyFacebookCallback<LoginResult> : Java.Lang.Object, IFacebookCallback where LoginResult : Java.Lang.Object
    {

        readonly MainActivity owner;

        public MyFacebookCallback(MainActivity owner)
        {
            this.owner = owner;
        }

        public void OnSuccess(Java.Lang.Object obj)
        {
            
        }

        public void OnCancel()
        {
            
        }

        public void OnError(FacebookException fbException)
        {
            
        }

        private void ShowAlert()
        {
            IDialogInterfaceOnClickListener listener = null;
            new AlertDialog.Builder(owner)
                .SetTitle(Resource.String.cancelled)
                .SetMessage(Resource.String.permission_not_granted)
                .SetPositiveButton(Resource.String.ok, listener)
                .Show();
        }
    }
}

