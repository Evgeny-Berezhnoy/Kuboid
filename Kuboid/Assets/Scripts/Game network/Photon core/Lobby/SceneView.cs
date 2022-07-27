using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SceneView : MonoBehaviour
{
    #region Constants

    private const string PLAYER_PREFAB = "Prefabs/Player";

    #endregion

    #region Fields

    [Header("Components")]
    [SerializeField] private List<Transform> _spawnPoints;

    [Header("UI")]
    [SerializeField] private RectTransform _gameplayScreen;
    [SerializeField] private Text _healthIndicator;

    #endregion

    #region Properties

    public string PlayfabID { get; set; }

    private LoadBalancingClient _client => PhotonNetwork.NetworkingClient;
    
    #endregion

    #region Methods

    public void StartGame()
    {
        gameObject.SetActive(true);

        _gameplayScreen.gameObject.SetActive(true);

        var spawnPointNumber = 0;

        foreach(var playerKVP in _client.CurrentRoom.Players)
        {
            if (playerKVP.Value.UserId == _client.UserId) break;

            spawnPointNumber++;
        };

        var spawnPoint = _spawnPoints[spawnPointNumber];

        var playerView =
            PhotonNetwork
                .Instantiate(
                    PLAYER_PREFAB,
                    spawnPoint.position,
                    spawnPoint.rotation)
                .GetComponent<PlayerView>();

        playerView.PlayfabID = PlayfabID;
        playerView.HealthIndicator = _healthIndicator;
        playerView.Initialize();
    }

    #endregion
}