using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class PlayerManager : SingletonComponent<PlayerManager>
    {

        public Text text;
        // #region Unity Methods

        // protected override void Awake()
        // {
        //     if (Exists())
        //     {
        //         Destroy(gameObject);
        //     }
        //     else
        //     {
        //         base.Awake();
        //         DontDestroyOnLoad(this);
        //     }
        //     CheckApiAvailability();
        // }

        // #endregion

        // #region Private methodes

        // private void CheckApiAvailability()
        // {

        //     int result = AN_GoogleApiAvailability.IsGooglePlayServicesAvailable();
        //     if (result != AN_ConnectionResult.SUCCESS)
        //     {
        //         text.text += "\nPlay Services does not available on this device. Resolving....";
        //         Debug.Log("Play Services does not available on this device. Resolving....");
        //         Toast.Show("Play Services does not available on this device. Resolving....");
        //         AN_GoogleApiAvailability.MakeGooglePlayServicesAvailable(resolution =>
        //         {
        //             if (resolution.IsSucceeded)
        //             {
        //                 text.text += "\nResolved! Play Services is available on this device";
        //                 Debug.Log("Resolved! Play Services is available on this device");
        //                 Toast.Show("Resolved! Play Services is available on this device");
        //             }
        //             else
        //             {
        //                 text.text += $"\nFailed to resolve: {resolution.Error.Message}";
        //                 Debug.LogError($"Failed to resolve: {resolution.Error.Message}");
        //                 Toast.Show($"Failed to resolve: {resolution.Error.Message}");
        //             }
        //         });
        //     }
        //     else
        //     {
        //         text.text += "\nPlay Services is available on this device";
        //         Debug.Log("Play Services is available on this device");
        //         Toast.Show("Play Services is available on this device");
        //         SignIn();
        //     }
        // }

        // private void SignIn()
        // {
        //     if (isSignedIn())
        //     {
        //         text.text += "\nUser already signed in";
        //         Debug.Log("User already signed in");
        //         Toast.Show("User already signed in");
        //         text.text += "Welcome, " + AN_GoogleSignIn.GetLastSignedInAccount().GetDisplayName();
        //         Debug.Log("Welcome, " + AN_GoogleSignIn.GetLastSignedInAccount().GetDisplayName());
        //         Toast.Show("Welcome, " + AN_GoogleSignIn.GetLastSignedInAccount().GetDisplayName());
        //     }
        //     else
        //     {

        //         text.text += "\nPlay Services Sign In started....";
        //         Debug.Log("Play Services Sign In started....");
        //         Toast.Show("Play Services Sign In started....");
        //         var builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
        //         //Google play documentation says that
        //         // you don't need to use this, however, we recommend you still
        //         // add those option to your Sing In builder. Some version of play service lib
        //         // may retirn a signed account with all fileds empty if you will not add this.
        //         // However according to the google documentation this step isn't required
        //         // So the decision is up to you.
        //         builder.RequestId();
        //         builder.RequestEmail();
        //         builder.RequestProfile();
        //         // Add the APPFOLDER scope for Snapshot support.
        //         builder.RequestScope(AN_Drive.SCOPE_APPFOLDER);
        //         var gso = builder.Build();
        //         text.text += "\nLet's try Silent SignIn first";
        //         Debug.Log("Let's try Silent SignIn first");
        //         Toast.Show("Let's try Silent SignIn first");
        //         var client = AN_GoogleSignIn.GetClient(gso);

        //         client.SilentSignIn(result =>
        //         {
        //             if (result.IsSucceeded)
        //             {
        //                 text.text += "\nSilentSignIn Succeeded";
        //                 Debug.Log("SilentSignIn Succeeded");
        //                 Toast.Show("\nSilentSignIn Succeeded");
        //                 Debug.Log("Welcome, " + result.Account.GetDisplayName());
        //                 Toast.Show("Welcome, " + result.Account.GetDisplayName());
        //             }
        //             else
        //             {
        //                 text.text += $"\nSilentSignIn Failed with code: {result.Error.FullMessage}";
        //                 // Player will need to sign-in explicitly using via UI
        //                 Debug.Log($"SilentSignIn Failed with code: {result.Error.FullMessage}");
        //                 Toast.Show($"SilentSignIn Failed with code: {result.Error.FullMessage}");
        //                 Debug.Log("Starting the default Sign in flow");
        //                 text.text += "\nStarting the default Sign in flow";
        //                 Toast.Show("Starting the default Sign in flow");
        //                 //Starting the interactive sign-in
        //                 client.SignIn(signInResult =>
        //                 {
        //                     text.text += $"\nSign In StatusCode: {signInResult.StatusCode}";
        //                     Debug.Log($"Sign In StatusCode: {signInResult.StatusCode}");
        //                     if (signInResult.IsSucceeded)
        //                     {
        //                         text.text += "\nSignIn Succeeded";
        //                         Debug.Log("SignIn Succeeded");
        //                         Toast.Show("SignIn Succeeded");
        //                         text.text += $"\nWelcome, {signInResult.Account.GetDisplayName()}";
        //                         Debug.Log("Welcome, " + signInResult.Account.GetDisplayName());
        //                         Toast.Show("Welcome, " + signInResult.Account.GetDisplayName());
        //                     }
        //                     else
        //                     {
        //                         text.text += $"\nSignIn failed: {signInResult.Error.FullMessage}";
        //                         text.text += $"{signInResult.Error.FullMessage}";
        //                         Debug.LogError($"SignIn failed: {signInResult.Error.FullMessage}");
        //                         Toast.Show($"SignIn failed: {signInResult.Error.FullMessage}");
        //                     }
        //                 },text);
        //             }
        //         });
        //     }
        // }

        // #endregion

        // #region  Public Methodes

        // public bool isSignedIn()
        // {
        //     return AN_GoogleSignIn.GetLastSignedInAccount() != null;
        // }
        // #endregion

    }
}
