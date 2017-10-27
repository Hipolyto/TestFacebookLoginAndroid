using System;

using Android.App;
using Android.Content;
using Android.Preferences;
using Android.Runtime;

using Object = Java.Lang.Object;

using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;


namespace testFbLogin
{
    public class MainApplication : Application
    {
        #region vars

        public static string FacebookUserId;
        public static string FacebookAccessToken;

        #endregion

        protected readonly string LogTag = "AppFacebookLogin";
        public static Context ProvidedContext { get; private set; }
        public static ISharedPreferences Preference { get; private set; }

        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base (javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            ProvidedContext = ApplicationContext;

            Preference = PreferenceManager.GetDefaultSharedPreferences(ProvidedContext);
        }

        public static T GetManager<T>(string name) where T : Object
        {
            return (ProvidedContext.GetSystemService(name) as T);
        }


        public static bool IsAuthenticated()
        {
            return !string.IsNullOrWhiteSpace(FacebookAccessToken);
        }
    }
}
