using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabLoginTest : MonoBehaviour
{
    #region Constants

    private const string PlayfabUsernameAddress = "authorization_username";
    private const string PlayfabPasswordAddress = "authorization_password";
    
    #endregion

    #region Fields

    [Header("Options")]
    [SerializeField] private GameObject _optionsWindow;

    [SerializeField] private Button _createAccountWindowButton;
    [SerializeField] private Button _signInWindowButton;

    [SerializeField] private GameObject _optionsLoadImage;
    
    [Header("Create account")]
    [SerializeField] private GameObject _createAccountWindow;
    
    [SerializeField] private Button _createAccountBackButton;
    [SerializeField] private Button _createAccountButton;
    
    [SerializeField] private InputField _createAccountUser;
    [SerializeField] private InputField _createAccountMail;
    [SerializeField] private InputField _createAccountPassword;
    
    [SerializeField] private GameObject _createAccountLoadImage;
    
    [Header("Sign in")]
    [SerializeField] private GameObject _signInWindow;
    
    [SerializeField] private Button _signInBackButton;
    [SerializeField] private Button _signInButton;

    [SerializeField] private InputField _signInUser;
    [SerializeField] private InputField _signInPassword;

    [SerializeField] private GameObject _signingInLoadImage;
    
    [Header("Sign in automatically")]
    [SerializeField] private GameObject _signInSuccessWindow;

    [SerializeField] private Button _signInSuccessBackButton;

    [SerializeField] private Text _signInSuccessAccountInfo;

    private string _username;
    private string _mail;
    private string _password;

    #endregion

    #region Unity events

    private void Start()
    {
        // Options
        _createAccountWindowButton.onClick.AddListener(CreateAccountWindow);
        _signInWindowButton.onClick.AddListener(SignInAutomatically);

        // Create account UI
        _createAccountBackButton.onClick.AddListener(OptionsWindow);
        _createAccountBackButton.onClick.AddListener(Back);
        _createAccountButton.onClick.AddListener(CreateAccount);

        _createAccountUser
            .onValueChanged
            .AddListener(value => _username = value);

        _createAccountMail
            .onValueChanged
            .AddListener(value => _mail = value);

        _createAccountPassword
            .onValueChanged
            .AddListener(value => _password = value);

        // Sign in UI
        _signInBackButton.onClick.AddListener(OptionsWindow);
        _signInBackButton.onClick.AddListener(Back);
        _signInButton.onClick.AddListener(SignIn);

        _signInUser
            .onValueChanged
            .AddListener(value => _username = value);

        _signInPassword
            .onValueChanged
            .AddListener(value => _password = value);

        // Sign in success UI
        _signInSuccessBackButton.onClick.AddListener(OptionsWindow);
        _signInSuccessBackButton.onClick.AddListener(Back);
    }

    private void OnDestroy()
    {
        // Options
        _createAccountWindowButton.onClick.RemoveAllListeners();
        _signInWindowButton.onClick.RemoveAllListeners();

        // Create account UI
        _createAccountBackButton.onClick.RemoveAllListeners();
        _createAccountButton.onClick.RemoveAllListeners();

        _createAccountUser
            .onValueChanged
            .RemoveAllListeners();

        _createAccountMail
            .onValueChanged
            .RemoveAllListeners();

        _createAccountPassword
            .onValueChanged
            .RemoveAllListeners();

        // Sign in UI
        _signInBackButton.onClick.RemoveAllListeners();
        _signInButton.onClick.RemoveAllListeners();

        _signInUser
            .onValueChanged
            .RemoveAllListeners();

        _signInPassword
            .onValueChanged
            .RemoveAllListeners();

        // Sign in success UI
        _signInSuccessBackButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    private void OptionsWindow()
    {
        _optionsWindow.SetActive(true);
        _signInWindow.SetActive(false);
        _signInSuccessWindow.SetActive(false);
        _createAccountWindow.SetActive(false);
    }

    private void SignInWindow()
    {
        _optionsWindow.SetActive(false);
        _signInWindow.SetActive(true);
        _signInSuccessWindow.SetActive(false);
        _createAccountWindow.SetActive(false);
    }
    
    private void CreateAccountWindow()
    {
        _optionsWindow.SetActive(false);
        _signInWindow.SetActive(false);
        _signInSuccessWindow.SetActive(false);
        _createAccountWindow.SetActive(true);
    }
    
    private void SignInSuccessWindow()
    {
        _optionsWindow.SetActive(false);
        _signInWindow.SetActive(false);
        _signInSuccessWindow.SetActive(true);
        _createAccountWindow.SetActive(false);
    }

    private void Back()
    {
        // Fields
        _username = "";
        _mail = "";
        _password = "";

        // Create account
        _createAccountUser.text = "";
        _createAccountMail.text = "";
        _createAccountPassword.text = "";

        // Sign in
        _signInUser.text = "";
        _signInPassword.text = "";
    }

    private void CreateAccount()
    {
        var request = new RegisterPlayFabUserRequest()
        {
            Username    = _username,
            Email       = _mail,
            Password    = _password
        };

        var taskCompletionProxy = new TaskCompletionProxy();

        RotateLoadingImageAsync(_createAccountLoadImage, taskCompletionProxy, 1f, 100);

        PlayFabClientAPI.
            RegisterPlayFabUser(
                request,
                OnRegisterSuccess,
                OnRegisterFailure,
                taskCompletionProxy);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        var taskcompletionProxy = (TaskCompletionProxy) result.CustomData;

        taskcompletionProxy.IsCompleted = true;

        Debug.Log($"Success: {_username}.");
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        var taskcompletionProxy = (TaskCompletionProxy)error.CustomData;

        taskcompletionProxy.IsCompleted = true;

        Debug.LogError($"Failure: {error.ErrorMessage}.");
    }

    private void SignInAutomatically()
    {
        var accountIsCreated = PlayerPrefs.HasKey(PlayfabUsernameAddress);

        if (!accountIsCreated)
        {
            SignInWindow();
        };

        _username   = PlayerPrefs.GetString(PlayfabUsernameAddress);
        _password   = PlayerPrefs.GetString(PlayfabPasswordAddress);
        
        var taskCompletionProxy = new TaskCompletionProxy();

        RotateLoadingImageAsync(_optionsLoadImage, taskCompletionProxy, 1f, 100);

        var request = new LoginWithPlayFabRequest()
        {
            Username = _username,
            Password = _password
        };

        PlayFabClientAPI
            .LoginWithPlayFab(
                request,
                OnSignInSuccess,
                OnSignInFailure,
                taskCompletionProxy);
    }

    private void SignIn()
    {
        var request = new LoginWithPlayFabRequest()
        {
            Username = _username,
            Password = _password
        };

        var taskCompletionProxy = new TaskCompletionProxy();

        RotateLoadingImageAsync(_signingInLoadImage, taskCompletionProxy, 1f, 100);

        PlayFabClientAPI.
            LoginWithPlayFab(
                request,
                OnSignInSuccess,
                OnSignInFailure,
                taskCompletionProxy);
    }

    private void OnSignInSuccess(LoginResult result)
    {
        var request = new GetAccountInfoRequest();

        PlayFabClientAPI
            .GetAccountInfo(
                request,
                OnGetAccountInfoSuccess,
                OnGetAccountInfoFailure,
                result.CustomData);
    }

    private void OnSignInFailure(PlayFabError error)
    {
        var taskcompletionProxy = (TaskCompletionProxy)error.CustomData;

        taskcompletionProxy.IsCompleted = true;

        Debug.LogError($"Failure: {error.ErrorMessage}.");
        
        SignInWindow();
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        var taskCompletionProxy = (TaskCompletionProxy)result.CustomData;

        taskCompletionProxy.IsCompleted = true;

        Debug.Log($"Success: {_username}.");
        
        SignInSuccessWindow();

        _signInSuccessAccountInfo.text =
            $"Welcome,\n" +
            $"\n" +
            $"Playfab ID = {result.AccountInfo.PlayFabId},\n" +
            $"Username {result.AccountInfo.Username},\n" +
            $"Email = {result.AccountInfo.PrivateInfo.Email}!\n" +
            $"\n" +
            $"We are glad you've come back!";

        PlayerPrefs.SetString(PlayfabUsernameAddress, _username);
        PlayerPrefs.SetString(PlayfabPasswordAddress, _password);
    }

    private void OnGetAccountInfoFailure(PlayFabError error)
    {
        var taskCompletionProxy = (TaskCompletionProxy) error.CustomData;

        taskCompletionProxy.IsCompleted = true;

        Debug.Log($"Failure: {_username}.");

        SignInWindow();
    }

    private async void RotateLoadingImageAsync(GameObject image, TaskCompletionProxy taskCompletionProxy, float frequancy, int awaitMilliseconds)
    {
        var angleSpeed = 360f * frequancy * (awaitMilliseconds / 1000f);

        image.SetActive(true);

        var localRotationZ = 0f;

        while (!taskCompletionProxy.IsCompleted)
        {
            localRotationZ -= angleSpeed; 

            image.transform.localRotation = Quaternion.Euler(0, 0, localRotationZ);

            await Task.Delay(awaitMilliseconds);
        };

        image.transform.localRotation = Quaternion.identity;

        image.SetActive(false);
    }

    #endregion
}