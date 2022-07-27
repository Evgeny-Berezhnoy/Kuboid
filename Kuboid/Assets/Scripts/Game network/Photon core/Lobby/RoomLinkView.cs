using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomLinkView : MonoBehaviour
{
    #region Events

    private event Action<RoomInfo> _onButtonClicked;

    #endregion

    #region Fields

    [SerializeField] private Button _joinRoomButton;

    [SerializeField] private Text _roomName;
    [SerializeField] private Text _occupied;
    [SerializeField] private Text _capacity;

    private RoomInfo _roomInformation;

    #endregion

    #region Properties

    public RoomInfo RoomInformation
    {
        set
        {
            _roomInformation = value;

            _roomName.text = _roomInformation.Name;
            _occupied.text = _roomInformation.PlayerCount.ToString();
            _capacity.text = _roomInformation.MaxPlayers.ToString();

            _joinRoomButton.interactable =
                !_roomInformation.RemovedFromList
                && (_roomInformation.PlayerCount < _roomInformation.MaxPlayers);
        }
    }

    #endregion

    #region Unity events

    private void Start()
    {
        _joinRoomButton.onClick.AddListener(() => JoinRoom());
    }

    private void OnDestroy()
    {
        _joinRoomButton.onClick.RemoveAllListeners();

        var handlers =
            _onButtonClicked
                ?.GetInvocationList()
                .Cast<Action<RoomInfo>>()
                .ToList();

        if(handlers != null)
        {
            for (int i = 0; i < handlers.Count; i++)
            {
                _onButtonClicked -= handlers[i];
            };
        };
    }

    #endregion

    #region Methods

    public void SubscribeJoinRoomCallback(Action<RoomInfo> action)
    {
        _onButtonClicked += action;
    }

    public void JoinRoom()
    {
        _onButtonClicked?.Invoke(_roomInformation);
    }

    #endregion
}