using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyWindowManager : MonoBehaviourPunCallbacks
{
    #region Fields

    [Header("Playfab")]
    [SerializeField] private string _titleID = "63BDB";
    [SerializeField] private string _username = "berezhnoy92";
    [SerializeField] private string _password = "berezhnoy92";
    
    [Header("Photon")]
    [SerializeField] private string _gameVersion = "1";
    [SerializeField] private string _region = "eu";
    
    [Header("Scripts")]
    [SerializeField] private LobbyView _lobby;
    [SerializeField] private RoomView _room;
    [SerializeField] private SceneView _scene;

    private LoadBalancingClient _client;

    #endregion

    #region Unity events

    private void Start()
    {
        PlayfabConnect();
    }

    private void OnDestroy()
    {
        _client.RemoveCallbackTarget(this);
        _client.Disconnect();
    }

    #endregion

    #region Base methods

    public override void OnConnected(){}

    public override void OnConnectedToMaster()
    {
        _lobby.Join();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if(!PhotonHandler.AppQuits)
        {
            PhotonConnect();
        };
    }

    #endregion

    #region Methods

    private void PlayfabConnect()
    {
        var playfabLoginController = new PlayfabLoginController(_titleID);

        playfabLoginController.ConnectPlayfab(_username, _password, PlayfabConnected);
    }

    private void PhotonConnect()
    {
        _client.AddCallbackTarget(this);

        _lobby.Initialize();
        _room.Initialize();
        
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnRoomSearch(string roomName)
    {
        _room.JoinRoom(roomName);
    }

    private void OnRoomCreation(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = Guid.NewGuid().ToString();
        };

        _room.CreateRoom(roomName);
    }
    
    private void OnRoomSelection(RoomInfo roomInfo)
    {
        _room.JoinRoom(roomInfo.Name);
    }
    
    private void OnStartGame()
    {
        _scene.StartGame();
    }

    private void PlayfabConnected(bool connected, string playfabID)
    {
        if (!connected)
        {
            Debug.LogError("Playfab signing in failed.");

            return;
        };

        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.PhotonServerSettings.DevRegion = _region;
        PhotonNetwork.NickName = Guid.NewGuid().ToString();
        PhotonNetwork.AutomaticallySyncScene = true;

        _client = PhotonNetwork.NetworkingClient;

        _lobby.SubscribeRoomSearchEvent(OnRoomSearch);
        _lobby.SubscribeRoomManualCreationCallback(OnRoomCreation);
        _lobby.SubscribeRoomSelectionCallback(OnRoomSelection);

        _room.SubscribeStartRoomCallback(OnStartGame);

        _scene.PlayfabID = playfabID;

        PhotonConnect();
    }

    #endregion
}