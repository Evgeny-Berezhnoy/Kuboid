using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomView : MonoBehaviourPunCallbacks
{
    #region Events

    private event Action _onLeaveRoom;

    #endregion

    #region Fields

    [Header("Settings")]
    [SerializeField] private byte _maxPlayers;

    [Header("UI")]
    [SerializeField] private InputField _roomNameInput;
    [SerializeField] private Text _closeOpenText;
    [SerializeField] private Text _friendListText;

    [SerializeField] private Button _backButton;
    [SerializeField] private Button _closeOpenButton;
    [SerializeField] private Button _startGameButton;

    private string _roomName;

    private LoadBalancingClient _client => PhotonNetwork.NetworkingClient;

    #endregion

    #region Unity events

    private void OnDestroy()
    {
        _client.RemoveCallbackTarget(this);

        _roomNameInput
            .onValueChanged
            .RemoveAllListeners();

        _backButton
            .onClick
            .RemoveAllListeners();
        
        _closeOpenButton
            .onClick
            .RemoveAllListeners();
        
        _startGameButton
            .onClick
            .RemoveAllListeners();
        
        var leaveRoomHandlers =
            _onLeaveRoom
                ?.GetInvocationList()
                .ToList()
                .Cast<Action>()
                .ToList();

        if(leaveRoomHandlers != null)
        {
            for (int i = 0; i < leaveRoomHandlers.Count; i++)
            {
                _onLeaveRoom -= leaveRoomHandlers[i];
            };
        };
    }

    #endregion

    #region Interface methods

    public override void OnCreatedRoom()
    {
        UpdateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Could not create room {_roomName} due to {message}.");

        LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        gameObject.SetActive(true);

        UpdateRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Could not join room {_roomName} due to {message}.");

        LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (_client.CurrentRoom == null) return;

        for (int i = 0; i < roomList.Count; i++)
        {
            var roomInfo = roomList[i];

            if (_client.CurrentRoom.Name != roomInfo.Name) continue;
            
            UpdateRoom();
        };
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshFriendsList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshFriendsList();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        UpdateRoom();
    }

    #endregion

    #region Methods

    public void Initialize()
    {
        _client.AddCallbackTarget(this);

        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() => LeaveRoom());

        _roomNameInput
            .onValueChanged
            .RemoveAllListeners();
        
        _roomNameInput
            .onValueChanged
            .AddListener(_ => SetRoomName());
    }

    public void SubscribeLeaveRoomCallback(Action action)
    {
        _onLeaveRoom += action;
    }

    public void CreateRoom(string roomName)
    {
        _roomName = roomName;

        var roomOptions             = new RoomOptions();
        roomOptions.MaxPlayers      = _maxPlayers;

        var enterRoomParams         = new EnterRoomParams();
        enterRoomParams.RoomName    = _roomName;
        enterRoomParams.RoomOptions = roomOptions;

        _client.OpCreateRoom(enterRoomParams);
    }

    public void JoinRoom(string roomName)
    {
        _roomName = roomName;

        var roomOptions             = new RoomOptions();
        roomOptions.MaxPlayers      = _maxPlayers;

        var enterRoomParams         = new EnterRoomParams();
        enterRoomParams.RoomName    = _roomName;
        enterRoomParams.RoomOptions = roomOptions;

        _client.OpJoinRoom(enterRoomParams);
    }

    private void LeaveRoom()
    {
        _client.Disconnect();

        gameObject.SetActive(false);
    }

    private void CloseRoom()
    {
        _client.CurrentRoom.IsOpen      = false;
        _client.CurrentRoom.IsVisible   = false;
        
        _closeOpenButton.onClick.RemoveAllListeners();
        _closeOpenButton.onClick.AddListener(() => OpenRoom());

        _closeOpenText.text = "Open";

        _startGameButton.interactable = true;
    }

    private void OpenRoom()
    {
        _client.CurrentRoom.IsOpen      = true;
        _client.CurrentRoom.IsVisible   = true;
        
        _closeOpenButton.onClick.RemoveAllListeners();
        _closeOpenButton.onClick.AddListener(() => CloseRoom());
        
        _closeOpenText.text = "Close";

        _startGameButton.interactable = false;
    }

    private void UpdateRoom()
    {
        if (_client.CurrentRoom == null) return;

        SetRoomName();

        if (_client.CurrentRoom.IsOpen)
        {
            OpenRoom();
        }
        else
        {
            CloseRoom();
        };

        RefreshFriendsList();
    }

    private void RefreshFriendsList()
    {
        var friendsListText =
            _client
                .CurrentRoom
                .Players
                .Values
                .Select(x => x.NickName)
                .ToArray();

        _friendListText.text = string.Join("\n", friendsListText);
    }

    private void SetRoomName()
    {
        _roomNameInput.SetTextWithoutNotify(_client.CurrentRoom.Name);
    }

    #endregion
}