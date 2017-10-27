using System;
using Android.Runtime;
using Android.Widget;
using Java.Lang;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

namespace testFbLogin
{
    public class FacebookCustomClass
    {
        public class CustomAcessTokenTracker : AccessTokenTracker
        {
            public delegate void CurrentAccessTokenChangedDelegate(AccessToken oldAcessToken, AccessToken currentAcessToken);

            public CurrentAccessTokenChangedDelegate HandleCurrentAccessTokenChanged { get; set; }

            protected override void OnCurrentAccessTokenChanged(AccessToken oldAcessToken, AccessToken currentAcessToken)
            {
                HandleCurrentAccessTokenChanged?.Invoke(oldAcessToken, currentAcessToken);

                if (currentAcessToken != null)
                {
                    MainApplication.FacebookAccessToken = currentAcessToken.Token;
                }
                else
                {
                    MainApplication.FacebookAccessToken = null;
                }
                Toast.MakeText (Android.App.Application.Context, "Facebook AccessTokenUpdated", ToastLength.Short).Show ();
            }
        }

        public class CustomProfileTracker : ProfileTracker
        {
            public delegate void CurrentProfileChangedDelegate(Profile oldProfile, Profile currentProfile);

            public CurrentProfileChangedDelegate HandleCurrentProfileChanged { get; set; }

            protected override void OnCurrentProfileChanged(Profile oldProfile, Profile currentProfile)
            {
                HandleCurrentProfileChanged?.Invoke(oldProfile, currentProfile);

                if (currentProfile != null)
                {
                    MainApplication.FacebookUserId = currentProfile.Id;
                }
                else
                {
                    MainApplication.FacebookUserId = null;
                }
            }
        }

        public class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
        {
            public Action HandleCancel { get; set; }
            public void OnCancel()
            {
                HandleCancel?.Invoke();
            }

            public Action<FacebookException> HandleError { get; set; }
            public void OnError(FacebookException error)
            {
                HandleError?.Invoke(error);
            }

            public Action<TResult> HandleSuccess { get; set; }
            public void OnSuccess(Java.Lang.Object result)
            {
                HandleSuccess?.Invoke(result.JavaCast<TResult>());
            }
        }
    }
}
