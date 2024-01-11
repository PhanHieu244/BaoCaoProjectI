using ColorRings.Runtime.Board.Entity;
// using Firebase.Crashlytics;
using Firebase.Extensions;
// using Firebase.RemoteConfig;

public class CustomBoardManager : MonoSingleton<CustomBoardManager>
{
    public Firebase.FirebaseApp app = null;
    
    private void Start()
    {
        
    }

    public void Initialize()
    {
        // Firebase.FirebaseApp.CheckAndFixDependenciesAsync()
        //     .ContinueWithOnMainThread(
        //         previousTask =>
        //         {
        //             var dependencyStatus = previousTask.Result;
        //             if (dependencyStatus == Firebase.DependencyStatus.Available) {
        //                 // Create and hold a reference to your FirebaseApp,
        //                 app = Firebase.FirebaseApp.DefaultInstance;
        //                 // Set the recommended Crashlytics uncaught exception behavior.
        //                 Crashlytics.ReportUncaughtExceptionsAsFatal = true;
        //                 SetRemoteConfigDefaults();
        //             } else {
        //                 UnityEngine.Debug.LogError(
        //                     $"Could not resolve all Firebase dependencies: {dependencyStatus}\n" +
        //                     "Firebase Unity SDK is not safe to use here");
        //             }
        //         });
    }

    private void SetRemoteConfigDefaults()
    {
        
        // var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        // remoteConfig.SetDefaultsAsync(defaults).ContinueWithOnMainThread(
        //     previousTask =>
        //     {
        //         FetchRemoteConfig(InitializeCommonDataAndStartGame);
        //     }
        // );
    }
    
    public BoardShape GetAdvancedModeBoard()
    {
        return default;
    }
}