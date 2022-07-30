using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class CharacterDemo : MonoBehaviour
{
    #region Constants

    private const string CATALOG_VERSION = "16_07_2022";
    private const string CHARACTER_CREATION_TOKEN_ID = "character_creation_token";
    private const string VIRTUAL_CURRENCY = "GB";
    
    private const int VIRTUAL_CURRENCY_ADD = 100;

    private const string DAMAGE_STATISTIC_NAME = "Damage";
    private const string HEALTH_STATISTIC_NAME = "Health";
    private const string EXPERIENCE_STATISTIC_NAME = "Experience";

    private const int DAMAGE_STATISTIC_VALUE = 10;
    private const int HEALTH_STATISTIC_VALUE = 100;
    private const int EXPERIENCE_STATISTIC_VALUE = 0;

    #endregion

    #region Fields

    [Header("Playfab settings")]
    [SerializeField] private string _username = "berezhnoy92";
    [SerializeField] private string _password = "berezhnoy92";

    [Header("UI - character selection")]
    [SerializeField] private GameObject _characterSelectionPanel;
    [SerializeField] private Button _slotButton1;
    [SerializeField] private Button _slotButton2;
    [SerializeField] private Text _slotText1;
    [SerializeField] private Text _slotText2;
    [SerializeField] private Text _GBCurrency;

    [Header("UI - character creation")]
    [SerializeField] private GameObject _characterCreationPanel;
    [SerializeField] private InputField _characterNameInput;
    [SerializeField] private Button _createCharacterButton;

    [Header("UI - Game imitation")]
    [SerializeField] private GameObject _gameImitationPanel;
    [SerializeField] private Text _characterIntroduction;
    [SerializeField] private Text _damageStatistic;
    [SerializeField] private Text _healthStatistic;
    [SerializeField] private Text _experienceStatistic;

    private List<CharacterResult> _characters;

    #endregion

    #region Unity events

    private void Start()
    {
        _createCharacterButton.onClick.AddListener(() => CreateCharacter());

        Login();
    }

    private void OnDestroy()
    {
        _slotButton1.onClick.RemoveAllListeners();
        _slotButton2.onClick.RemoveAllListeners();

        _createCharacterButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    private void Login()
    {
        var request = new LoginWithPlayFabRequest();

        request.Username = _username;
        request.Password = _password;

        PlayFabClientAPI
            .LoginWithPlayFab(
                request,
                OnLoginSuccess,
                OnFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        OpenCharacterSelectionWindow();
    }

    private void GetCharacters()
    {
        _characters = null;

        _slotButton1.onClick.RemoveAllListeners();
        _slotButton2.onClick.RemoveAllListeners();

        _slotText1.text = "";
        _slotText2.text = "";

        var request = new ListUsersCharactersRequest();

        PlayFabClientAPI
            .GetAllUsersCharacters(
                request,
                OnGetCharactersSuccess,
                OnFailure);
    }

    private void OnGetCharactersSuccess(ListUsersCharactersResult result)
    {
        _characters = result.Characters;

        if (_characters.Count == 0)
        {

        };

        if (_characters.Count >= 1)
        {
            WriteDownCharacterInformationInSlot(
                _characters[0],
                ref _slotButton1,
                ref _slotText1);
        }
        else
        {
            WriteDownCharacterCreationInSlot(
                ref _slotButton1,
                ref _slotText1);
        };

        if (_characters.Count >= 2)
        {
            WriteDownCharacterInformationInSlot(
                _characters[1],
                ref _slotButton2,
                ref _slotText2);
        }
        else
        {
            WriteDownCharacterCreationInSlot(
                ref _slotButton2,
                ref _slotText2);
        };

        GetInventory();
    }

    private void WriteDownCharacterInformationInSlot(
        CharacterResult character,
        ref Button slotButton,
        ref Text slotText)
    {
        slotText.text = character.CharacterName;

        slotButton.onClick.AddListener(() => SelectCharacter(character.CharacterId, character.CharacterName));
    }

    private void WriteDownCharacterCreationInSlot(
        ref Button slotButton,
        ref Text slotText)
    {
        slotText.text = "CREATE CHARACTER";

        slotButton.onClick.AddListener(() => OpenCharacterCreationWindow());
    }

    private void CreateCharacter()
    {
        if (string.IsNullOrWhiteSpace(_characterNameInput.text))
        {
            Debug.Log("Character name can't be empty!");

            return;
        };

        var characterNameExists =
            _characters
                .Where(x => x.CharacterName == _characterNameInput.text)
                .Cast<string>()
                .ToArray()
                .Length > 1;

        if (characterNameExists)
        {
            Debug.Log($"You already have a character with a name {_characterNameInput.text}");
        };

        var request = new PurchaseItemRequest();

        request.CatalogVersion  = CATALOG_VERSION;
        request.ItemId          = CHARACTER_CREATION_TOKEN_ID;
        request.VirtualCurrency = VIRTUAL_CURRENCY;
        request.Price           = 1; 

        PlayFabClientAPI
            .PurchaseItem(
                request,
                OnPurchaseItemCharacterTokenSuccess,
                OnFailure,
                _characterNameInput.text);
    }

    private void SelectCharacter(string characterID, string characterName)
    {
        OpenGameImitationWindow();

        _characterIntroduction.text = $"Congratulations! You are playing on character {characterName}!";
        _damageStatistic.text = "Damage: 0";
        _healthStatistic.text = "Health: 0";
        _experienceStatistic.text = "Experience: 0";

        var request = new GetCharacterStatisticsRequest();

        request.CharacterId = characterID;

        PlayFabClientAPI
            .GetCharacterStatistics(
                request,
                OnGetCharacterStatisticsSuccess,
                OnFailure,
                characterName);
    }

    private void OnPurchaseItemCharacterTokenSuccess(PurchaseItemResult result)
    {
        var characterCreationToken = result.Items[0];

        var request = new GrantCharacterToUserRequest();

        request.CatalogVersion  = CATALOG_VERSION;
        request.CharacterName   = (string) result.CustomData;
        request.ItemId          = characterCreationToken.ItemId;

        PlayFabClientAPI
            .GrantCharacterToUser(
                request,
                OnGrantCharacterToUserSuccess,
                OnFailure);
    }

    private void OnGrantCharacterToUserSuccess(GrantCharacterToUserResult result)
    {
        var request = new UpdateCharacterStatisticsRequest();

        var updateStatistics = new Dictionary<string, int>();

        updateStatistics.Add(DAMAGE_STATISTIC_NAME, DAMAGE_STATISTIC_VALUE);
        updateStatistics.Add(HEALTH_STATISTIC_NAME, HEALTH_STATISTIC_VALUE);
        updateStatistics.Add(EXPERIENCE_STATISTIC_NAME, EXPERIENCE_STATISTIC_VALUE);
        
        request.CharacterId         = result.CharacterId;
        request.CharacterStatistics = updateStatistics;

        PlayFabClientAPI
            .UpdateCharacterStatistics(
                request,
                OnUpdateCharacterStatisticsSuccess,
                OnFailure);
    }

    private void OnUpdateCharacterStatisticsSuccess(UpdateCharacterStatisticsResult result)
    {
        OpenCharacterSelectionWindow();
    }

    private void OnGetCharacterStatisticsSuccess(GetCharacterStatisticsResult result)
    {
        foreach (var statistic in result.CharacterStatistics)
        {
            if(statistic.Key == "Damage")
            {
                _damageStatistic.text = $"Damage: {statistic.Value}";
            };

            if (statistic.Key == "Health")
            {
                _healthStatistic.text = $"Health: {statistic.Value}";
            };

            if (statistic.Key == "Experience")
            {
                _experienceStatistic.text = $"Experience: {statistic.Value}";
            };
        };
    }

    private void GetInventory()
    {
        var request = new GetUserInventoryRequest();

        PlayFabClientAPI
            .GetUserInventory(
                request,
                OnGetUserInventorySuccess,
                OnFailure);
    }

    private void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        if (result.VirtualCurrency.TryGetValue(VIRTUAL_CURRENCY, out var currencyAmount))
        {
            _GBCurrency.text = $"GB: {currencyAmount}";

            if(currencyAmount == 0)
            {
                BuyCurrency();
            };
        }
        else
        {
            _GBCurrency.text = $"GB: 0";

            BuyCurrency();
        };
    }

    private void BuyCurrency()
    {
        var request = new AddUserVirtualCurrencyRequest();

        request.VirtualCurrency = VIRTUAL_CURRENCY;
        request.Amount          = VIRTUAL_CURRENCY_ADD;

        PlayFabClientAPI
            .AddUserVirtualCurrency(
                request,
                OnBuyCurrencySuccess,
                OnFailure);
    }

    private void OnBuyCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        _GBCurrency.text = $"GB: {result.Balance}";
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OpenCharacterCreationWindow()
    {
        _characterSelectionPanel.SetActive(false);
        _characterCreationPanel.SetActive(true);
        _gameImitationPanel.SetActive(false);

        _characterNameInput.SetTextWithoutNotify("");
    }
    
    private void OpenCharacterSelectionWindow()
    {
        _characterSelectionPanel.SetActive(true);
        _characterCreationPanel.SetActive(false);
        _gameImitationPanel.SetActive(false);
        
        GetCharacters();
    }

    private void OpenGameImitationWindow()
    {
        _characterSelectionPanel.SetActive(false);
        _characterCreationPanel.SetActive(false);
        _gameImitationPanel.SetActive(true);
    }
    
    #endregion
}