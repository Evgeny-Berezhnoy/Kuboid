using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using Photon.Pun;
using PlayFab.ClientModels;

public class PlayerView : MonoBehaviour
{
    #region Constants

    private const float STRAIGHT_ANGLE = 90f;
    private const string CURRENT_HEALTH_KEY = "CurrentHealth";
    private const string MAXIMUM_HEALTH_KEY = "MaximumHealth";
    private const int MAXIMUM_HEALTH_START_VALUE = 100;

    private List<string> USER_DATA_KEYS;

    #endregion

    #region Fields

    [Header("Photon")]
    [SerializeField] private PhotonView _photonView;
    
    [Header("Components")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _cameraHolder;

    [Header("UI")]
    [SerializeField] private Text _healthIndicator;

    [Header("Settings")]
    [SerializeField] private float _speed = 10;
    [SerializeField] [Range(0f, 100f)] private float _sensetivity = 100f;
    [SerializeField] [Range(0, 100)] private int _healthDecrement = 10;
    [SerializeField] [Range(0, 100)] private int _healthIncrement = 10;
    

    private Transform _camera;
    
    private Vector2 _movementDirection;
    
    private float _aroundXRotation;

    private int _maxHealth;
    private int _currentHealth;

    #endregion
    
    #region Properties

    public string PlayfabID { get; set; }
    public Text HealthIndicator
    {
        set => _healthIndicator = value;
    }

    #endregion

    #region Unity events

    private void Update()
    {
        if (!_photonView.IsMine) return;

        // Player position
        var movementDirection =
            new Vector2(
                Input.GetAxis("Horizontal") * Time.deltaTime,
                Input.GetAxis("Vertical") * Time.deltaTime);

        _movementDirection += movementDirection;

        // Camera rotation
        var axisXShift = Input.GetAxis("Mouse X") * _sensetivity * Time.deltaTime;
        var axisYShift = Input.GetAxis("Mouse Y") * _sensetivity * Time.deltaTime;

        transform.Rotate(Vector3.up, axisXShift);

        _aroundXRotation -= axisYShift;
        _aroundXRotation = Mathf.Clamp(_aroundXRotation, -STRAIGHT_ANGLE, STRAIGHT_ANGLE);

        _camera.localRotation = Quaternion.Euler(_aroundXRotation, 0, 0);

        if (Input.GetMouseButtonDown(0))
        {
            _currentHealth = Mathf.Clamp(_currentHealth - _healthDecrement, 0, _maxHealth);

            _healthIndicator.text = _currentHealth.ToString();

            PlayfabSendUserData();
        };

        if (Input.GetMouseButtonDown(1))
        {
            _currentHealth = Mathf.Clamp(_currentHealth + _healthIncrement, 0, _maxHealth);

            _healthIndicator.text = _currentHealth.ToString();
            
            PlayfabSendUserData();
        };
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine) return;
        
        if (_movementDirection == Vector2.zero) return;

        var movementVector =
            transform.position +
            _movementDirection.x * transform.right * _speed +
            _movementDirection.y * transform.forward * _speed;

        _rigidbody.MovePosition(movementVector);

        _movementDirection = Vector2.zero;
    }

    #endregion

    #region Methods

    public void Initialize()
    {
        if (!_photonView.IsMine) return;
            
        _movementDirection = Vector2.zero;

        _camera                 = Camera.main.transform;
        _camera.parent          = _cameraHolder;
        _camera.localPosition   = Vector3.zero;
        _camera.localRotation   = Quaternion.identity;
        
        USER_DATA_KEYS = new List<string>();
        USER_DATA_KEYS.Add(CURRENT_HEALTH_KEY);
        USER_DATA_KEYS.Add(MAXIMUM_HEALTH_KEY);

        var getUserDataRequest          = new GetUserDataRequest();
        getUserDataRequest.PlayFabId    = PlayfabID;
        getUserDataRequest.Keys         = USER_DATA_KEYS;

        PlayFabClientAPI
            .GetUserData(
                getUserDataRequest,
                OnPlayfabGetUserData,
                OnPlayfabOperationFailure);
    }

    private void PlayfabSendUserData()
    {
        var userData    = new Dictionary<string, string>();

        userData.Add(CURRENT_HEALTH_KEY, _currentHealth.ToString());
        userData.Add(MAXIMUM_HEALTH_KEY, _maxHealth.ToString());

        var request     = new UpdateUserDataRequest();
        request.Data    = userData;

        PlayFabClientAPI.UpdateUserData(
            request,
            null,
            OnPlayfabOperationFailure);
    }

    private void OnPlayfabGetUserData(GetUserDataResult result)
    {
        if(result.Data.TryGetValue(CURRENT_HEALTH_KEY, out var currentHealth))
        {
            int.TryParse(currentHealth.Value, out _currentHealth);
            
            _healthIndicator.text = currentHealth.Value;
        }
        else
        {
            _currentHealth = MAXIMUM_HEALTH_START_VALUE;

            _healthIndicator.text = _currentHealth.ToString();
        };

        if (result.Data.TryGetValue(MAXIMUM_HEALTH_KEY, out var maximumHealth))
        {
            int.TryParse(maximumHealth.Value, out _maxHealth);
        }
        else
        {
            _maxHealth = MAXIMUM_HEALTH_START_VALUE;
        };
    }

    private void OnPlayfabOperationFailure(PlayFabError error)
    {
        Debug.Log($"Got error retrieving user data: {error.GenerateErrorReport()}");
    }

    #endregion
}