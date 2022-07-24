using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LobbyWindowManager : MonoBehaviourPunCallbacks
{
    #region Fields

    [Header("Settings")]
    [SerializeField] private string _gameVersion = "1";
    [SerializeField] private string _region = "eu";
    
    [Header("Scripts")]
    [SerializeField] private LobbyView _lobby;
    [SerializeField] private RoomView _room;

    private LoadBalancingClient _client;

    #endregion

    #region Unity events

    private void Start()
    {
        PhotonNetwork.GameVersion                       = _gameVersion;
        PhotonNetwork.PhotonServerSettings.DevRegion    = _region;
        PhotonNetwork.NickName                          = Guid.NewGuid().ToString();
        PhotonNetwork.AutomaticallySyncScene            = true;
        
        _client = PhotonNetwork.NetworkingClient;
        
        _lobby.SubscribeRoomSearchEvent(OnRoomSearch);
        _lobby.SubscribeRoomManualCreationCallback(OnRoomCreation);
        _lobby.SubscribeRoomSelectionCallback(OnRoomSelection);

        Connect();
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
            Connect();
        };
    }

    #endregion

    #region Methods

    private void Connect()
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
    
    public void OnRoomSelection(RoomInfo roomInfo)
    {
        _room.JoinRoom(roomInfo.Name);
    }
    
    private void OnRoomCreation(RoomLinkView roomLink)
    {
        roomLink.SubscribeJoinRoomCallback(OnRoomSelection);
    }

    #endregion
}