using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLoginController : IConnectionCallbacks, IDisposableAdvanced
{
    #region Fields

    private string _gameVersion;
    private string _region;

    #endregion

    #region Observers

    private event Action _onConnected;
    private event Action _onDisconnected;

    #endregion

    #region Interface properties

    public bool IsDisposed { get; private set; }

    #endregion

    #region Properties

    public string GameVersion
    {
        get => _gameVersion;
    }

    public string Region
    {
        get => _region;
    }

    #endregion

    #region Constructors

    public PhotonLoginController(
        string gameVersion,
        string region)
    {
        _gameVersion    = gameVersion;
        _region         = region;

        PhotonNetwork.GameVersion                       = _gameVersion;
        PhotonNetwork.AutomaticallySyncScene            = true;
        PhotonNetwork.PhotonServerSettings.DevRegion    = region;

        PhotonNetwork.AddCallbackTarget(this);
    }

    #endregion

    #region Interface methods: IConnectionCallbacks

    public void OnConnected()
    {
        Debug.Log("Client is connected.");

        _onConnected?.Invoke();
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Client is connected to master.");
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Client has been disconnected, the cause: {cause}");

        _onDisconnected?.Invoke();
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionListReceived");
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log("OnCustomAuthenticationFailed");
    }

    #endregion

    #region Interface methods: IDisposableAdvanced

    public void Dispose()
    {
        if (IsDisposed) return;

        IsDisposed = true;

        PhotonNetwork.RemoveCallbackTarget(this);

        if (PhotonNetwork.IsConnected)
        {
            Disconnect();
        };

        var onConnectedHandlers =
            _onConnected
                .GetInvocationList()
                .ToArray()
                .Cast<Action>()
                .ToArray();

        for(int i = 0; i < onConnectedHandlers.Length; i++)
        {
            _onConnected -= onConnectedHandlers[i];
        };

        var onDisconnectedHandlers =
            _onDisconnected
                .GetInvocationList()
                .ToArray()
                .Cast<Action>()
                .ToArray();

        for (int i = 0; i < onDisconnectedHandlers.Length; i++)
        {
            _onDisconnected -= onDisconnectedHandlers[i];
        };

        GC.SuppressFinalize(this);
    }

    #endregion

    #region Methods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        };
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.Log("Client is not connected to a server.");
        };
    }

    public void SubscribeConnection(Action handler)
    {
        _onConnected += handler;
    }

    public void SubscribeDisconnection(Action handler)
    {
        _onDisconnected += handler;
    }

    #endregion
}