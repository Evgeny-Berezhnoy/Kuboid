using UnityEngine;
using UnityEngine.UI;

public class GameNetworkTest : MonoBehaviour
{
    #region Fields

    [Header("Playfab")]
    [SerializeField] private string _titleID        = "63BDB";
    [SerializeField] private string _customID       = "EvgenyBerezhnoy";
    
    [Header("Photon")]
    [SerializeField] private string _gameVersion    = "1";
    [SerializeField] private string _region         = "ru";

    [Header("Playfab UI")]
    [SerializeField] private Button _playfabConnectButton;
    [SerializeField] private Image _playfabConnectImage;
    [SerializeField] private Text _playfabConnectText;

    [Header("Photon UI")]
    [SerializeField] private Button _photonConnectButton;
    [SerializeField] private Image _photonConnectImage;
    [SerializeField] private Text _photonConnectText;

    private PlayfabLoginController _playfabLoginController;
    private PhotonLoginController _photonLoginController;

    #endregion

    #region Unity events

    private void Awake()
    {
        _playfabLoginController = new PlayfabLoginController(_titleID);
        _photonLoginController  = new PhotonLoginController(_gameVersion, _region);
    }

    private void Start()
    {
        _playfabConnectButton.onClick.AddListener(PlayfabConnect);
        _photonConnectButton.onClick.AddListener(PhotonConnect);

        _photonLoginController.SubscribeConnection(OnPhotonConnected);
        _photonLoginController.SubscribeDisconnection(OnPhotonDisconnected);
    }

    private void OnDestroy()
    {
        _photonLoginController.Dispose();

        _playfabConnectButton.onClick.RemoveAllListeners();
        _photonConnectButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    private void PlayfabConnect()
    {
        _playfabLoginController.Connect(_customID, OnPlayfabConnect);
    }

    private void OnPlayfabConnect(bool isConnected)
    {
        if (isConnected)
        {
            _playfabConnectButton.interactable  = false;
            _playfabConnectImage.color          = Color.green;
            _playfabConnectText.text            = "Connected";
        }
        else
        {
            _playfabConnectButton.interactable  = true;
            _playfabConnectImage.color          = Color.red;
            _playfabConnectText.text            = "Reconnect";

            _photonLoginController.Disconnect();
        };
    }

    private void PhotonConnect()
    {
        if (!_playfabLoginController.IsConnected)
        {
            Debug.Log("Playfab authentification has not been passed!");

            return;
        };

        _photonLoginController.Connect();
    }

    private void PhotonDisconnect()
    {
        _photonLoginController.Disconnect();
    }

    private void OnPhotonConnected()
    {
        _photonConnectImage.color   = Color.green;
        _photonConnectText.text     = "Connected";

        _photonConnectButton.onClick.RemoveAllListeners();
        _photonConnectButton.onClick.AddListener(PhotonDisconnect);
    }

    private void OnPhotonDisconnected()
    {
        _photonConnectImage.color   = Color.white;
        _photonConnectText.text     = "Connect";

        _photonConnectButton.onClick.RemoveAllListeners();
        _photonConnectButton.onClick.AddListener(PhotonConnect);
    }

    #endregion
}