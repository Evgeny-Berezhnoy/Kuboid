using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyView : MonoBehaviourPunCallbacks
{
    #region Constants

    private readonly TypedLobby CUSTOM_LOBBY = new TypedLobby("customLobby", LobbyType.Default);

    #endregion

    #region Events

    private event Action<string> _onRoomSearch;
    private event Action<string> _onRoomManualCreation;
    private event Action<RoomInfo> _onRoomSelection;

    #endregion

    #region Fields

    [Header("UI")]
    [SerializeField] private RectTransform _roomContainer;
    [SerializeField] private Button _createRoomManuallyButton;
    [SerializeField] private Button _searchButton;
    [SerializeField] private InputField _searchRoomValue;

    [Header("Prefabs")]
    [SerializeField] private RoomLinkView _prefab;

    private Dictionary<string, RoomLinkView> _cachedRooms = new Dictionary<string, RoomLinkView>();

    private LoadBalancingClient _client => PhotonNetwork.NetworkingClient;

    #endregion

    #region Unity events

    private void OnDestroy()
    {
        _createRoomManuallyButton.onClick.RemoveAllListeners();
        _searchButton.onClick.RemoveAllListeners();

        _cachedRooms.Clear();

        var roomSearchHandlers =
            _onRoomSearch
                ?.GetInvocationList()
                .Cast<Action<string>>()
                .ToList();

        if(roomSearchHandlers != null)
        {
            for (int i = 0; i < roomSearchHandlers.Count; i++)
            {
                _onRoomSearch -= roomSearchHandlers[i];
            };
        };

        var roomManualCreationHandlers =
            _onRoomManualCreation
                ?.GetInvocationList()
                .Cast<Action<string>>()
                .ToList();

        if(roomManualCreationHandlers != null)
        {
            for (int i = 0; i < roomManualCreationHandlers.Count; i++)
            {
                _onRoomManualCreation -= roomManualCreationHandlers[i];
            };
        };
        
        var roomSelectionHandlers =
            _onRoomSelection
                ?.GetInvocationList()
                .Cast<Action<RoomInfo>>()
                .ToList();

        if(roomSelectionHandlers != null)
        {
            for (int i = 0; i < roomSelectionHandlers.Count; i++)
            {
                _onRoomSelection -= roomSelectionHandlers[i];
            };
        };

        _client.RemoveCallbackTarget(this);
    }

    #endregion

    #region Interface methods

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            var roomInfo = roomList[i];

            if (roomInfo.RemovedFromList
                || !roomInfo.IsOpen
                || !roomInfo.IsVisible)
            {
                DeleteRoom(roomInfo);
            }
            else
            {
                UpdateRoom(roomInfo);
            };
        };
    }

    public override void OnJoinedLobby()
    {
        gameObject.SetActive(true);

        _cachedRooms.Clear();

        foreach(RectTransform child in _roomContainer)
        {
            Destroy(child.gameObject);
        };
    }

    public override void OnLeftLobby()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Methods

    public void Initialize()
    {
        _client.AddCallbackTarget(this);

        _cachedRooms = new Dictionary<string, RoomLinkView>();

        _createRoomManuallyButton.onClick.RemoveAllListeners();
        _createRoomManuallyButton.onClick.AddListener(() => CreateRoomManually());

        _searchButton.onClick.RemoveAllListeners();
        _searchButton.onClick.AddListener(() => SearchRoom());
    }
    
    public void SubscribeRoomSearchEvent(Action<string> action)
    {
        _onRoomSearch += action;
    }

    public void SubscribeRoomManualCreationCallback(Action<string> action)
    {
        _onRoomManualCreation += action;
    }
    
    public void SubscribeRoomSelectionCallback(Action<RoomInfo> action)
    {
        _onRoomSelection += action;
    }

    public void Join()
    {
        if (!_client.InLobby)
        {
            _client.OpJoinLobby(CUSTOM_LOBBY);
        }
        else
        {
            gameObject.SetActive(true);
        };
    }
    
    private void AddRoom(RoomInfo roomInfo)
    {
        var roomView = Instantiate(_prefab, _roomContainer);

        _cachedRooms.Add(roomInfo.Name, roomView);

        roomView.SubscribeJoinRoomCallback(value => SelectRoom(value));
    }

    private void UpdateRoom(RoomInfo roomInfo)
    {
        if (!_cachedRooms.ContainsKey(roomInfo.Name))
        {
            AddRoom(roomInfo);
        };

        _cachedRooms[roomInfo.Name].RoomInformation = roomInfo;
    }

    private void DeleteRoom(RoomInfo roomInfo)
    {
        if(!_cachedRooms.ContainsKey(roomInfo.Name)) return;

        _cachedRooms.TryGetValue(roomInfo.Name, out var room);

        if (_cachedRooms.Remove(roomInfo.Name))
        {
            Destroy(room.gameObject);
        };
    }

    private void CreateRoomManually()
    {
        gameObject.SetActive(false);

        _onRoomManualCreation?.Invoke(_searchRoomValue.text);
    }

    private void SearchRoom()
    {
        if (string.IsNullOrEmpty(_searchRoomValue.text))
        {
            Debug.Log("Room name is empty.");

            return;
        };

        gameObject.SetActive(false);

        _onRoomSearch?.Invoke(_searchRoomValue.text);
    }

    private void SelectRoom(RoomInfo roomInformation)
    {
        gameObject.SetActive(false);
        
        _onRoomSelection?.Invoke(roomInformation);
    }

    #endregion
}