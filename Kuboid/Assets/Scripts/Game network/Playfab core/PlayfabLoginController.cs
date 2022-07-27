using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayfabLoginController : IController
{
    #region Fields

    private bool _isConnected;

    #endregion

    #region Properties

    public bool IsConnected => _isConnected;

    #endregion

    #region Constructors

    public PlayfabLoginController(string titleID)
    {
        // Here we need to check whether TitleId property is configured in settings or not
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            // If not we need to assign it to the appropriate variable manually
            // Otherwise we can just remove this if statement at all
            PlayFabSettings.staticSettings.TitleId = titleID;
        };
    }

    #endregion

    #region Methods

    public void Connect(string customID, Action<bool> connectionCallback)
    {
        var request = new LoginWithCustomIDRequest();

        request.CustomId        = customID;
        request.CreateAccount   = true;
        
        PlayFabClientAPI
            .LoginWithCustomID(
                request,
                OnLoginSuccess,
                OnLoginFailure,
                connectionCallback);
    }

    public void ConnectPlayfab(string username, string password, Action<bool, string> connectionCallback)
    {
        var request = new LoginWithPlayFabRequest()
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI
            .LoginWithPlayFab(
                request,
                OnLoginSuccess,
                OnLoginFailure,
                connectionCallback);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made successful API call!");

        _isConnected = true;

        var connectionCallback = (Action<bool, string>) result.CustomData;

        connectionCallback.Invoke(true, result.PlayFabId);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        
        Debug.LogError($"Something went wrong: {errorMessage}");
        
        var connectionCallback = (Action<bool>) error.CustomData;

        connectionCallback.Invoke(false);
    }

    #endregion
}