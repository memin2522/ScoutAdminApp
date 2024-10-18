using UnityEngine;
using Firebase.Extensions;
using Firebase;
using System.Collections;

public class AppFlow : MonoSingleton<AppFlow>
{
    public Firebase.FirebaseApp app;
    private bool isInit = false;

    [Header("Important References")]
    public FirebaseImplementation databaseImplementation;

    public override void Init()
    {
        Debug.Log("Initializing Firebase...");
        InitializeFirebase();
        databaseImplementation.Initialize();
    }

    private void InitializeFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                app = Firebase.FirebaseApp.DefaultInstance;
            }
            else
            {
                // Log error if dependencies could not be resolved
                Debug.LogError($"Could not resolve Firebase dependencies: {dependencyStatus}");
            }
        });
    }

}

