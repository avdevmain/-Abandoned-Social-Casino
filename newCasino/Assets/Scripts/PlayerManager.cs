using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance = null;
    
    public Text lobbyLevelNumText;
    public Text lobbyXpAmountText;
    public Text lobbyLevelCapText;
    public Text lobbyBalanceText;
    public Text lobbyShardsBalanceText;
    public Image lobbyPlayerIcon;

    public Text gameBetText;
    public Text gameXpAmountText;
    public Text gameLevelCapText;
    public Text gameLevelNumText;
    public Text gameBalanceText;
    public Text gameShardsBalanceText;
    public Image gamePlayerIcon;

    private GameObject _lobby;
    private GameObject _game;
    private GameObject _profile;

    private Slider _lobbyLevel;
    private Slider _gameLevel;

    private int xpAmount;
    private int level;
    private int coins;
    private int shards;
    private int bet = 5;

    /////////Статистика в профиле///////////
    private string player_name;
    private Text stat_name;
    private Text edit_nameplaceholder;

    private Sprite[] iconsToChoose = new Sprite[9];
    private Image stat_playerImg;
    private Image edit_playerImg;
    private byte playerImgId = 0;
    private Text stat_level;
    private int total_coins;
    public Text stat_coins;
    private int total_shards;
    public Text stat_shards;
    private int games_unlocked; //  x/y игр открыто. Unlocked = x , ingame = y
    private int games_ingame = 4;
    ////////////////////////////////////////

    public bool receivedDaily;
    public byte daysInARow;


    private int[] levelCaps = new int[] {10,25,45,60,75,90,110,130,155, 180, 220, 250};


    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            
            _lobby = transform.parent.GetChild(ButtonHandler.lobbyIndex).gameObject;
            _game = transform.parent.GetChild(ButtonHandler.slotPatternIndex).GetChild(0).GetChild(2).gameObject;
            _profile = transform.parent.GetChild(3).gameObject;

            _lobbyLevel = _lobby.transform.GetChild(2).GetComponent<Slider>();
            _gameLevel = _game.transform.GetChild(1).GetComponent<Slider>();

            lobbyLevelNumText = _lobby.transform.GetChild(2).GetChild(1).GetComponent<Text>();
            lobbyXpAmountText = _lobby.transform.GetChild(2).GetChild(2).GetComponent<Text>();
            lobbyLevelCapText = _lobby.transform.GetChild(2).GetChild(3).GetComponent<Text>();
            lobbyBalanceText = _lobby.transform.GetChild(3).GetChild(0).GetComponent<Text>();
            lobbyShardsBalanceText = _lobby.transform.GetChild(7).GetChild(0).GetComponent<Text>();
            lobbyPlayerIcon = _lobby.transform.GetChild(1).GetComponent<Image>();

            gameBetText = _game.transform.GetChild(2).GetChild(0).GetComponent<Text>();
            gameXpAmountText = _game.transform.GetChild(1).GetChild(2).GetComponent<Text>();
            gameLevelCapText = _game.transform.GetChild(1).GetChild(3).GetComponent<Text>();
            gameLevelNumText = _game.transform.GetChild(1).GetChild(1).GetComponent<Text>();
            gameBalanceText = _game.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            gameShardsBalanceText = _game.transform.GetChild(5).GetChild(0).GetComponent<Text>();
            gamePlayerIcon = _game.transform.parent.GetChild(1).GetChild(1).GetComponent<Image>();

            stat_playerImg = _profile.transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Image>();
            edit_playerImg = _profile.transform.GetChild(6).GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();

            stat_name = _profile.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>();
            edit_nameplaceholder = _profile.transform.GetChild(6).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();

            //Загрузка playerImgID, имени, всех количеств

            player_name = "MudakMudak";
            ChangePlayerName(player_name);

            for (int i =0; i<9; i++)
            {
                iconsToChoose[i] = Resources.Load<Sprite>("Sprites/UI/playerIcon/" + i);
                _profile.transform.GetChild(6).GetChild(3).GetChild(i).GetChild(0).GetComponent<Image>().sprite = iconsToChoose[i];
            }


            ChangePlayerIcon(playerImgId);

            stat_coins = _profile.transform.GetChild(2).GetChild(1).GetComponent<Text>();
            stat_shards = _profile.transform.GetChild(3).GetChild(1).GetComponent<Text>();
            stat_level = _profile.transform.GetChild(1).GetChild(1).GetChild(3).GetComponent<Text>();

            coins = 1000; //СТАРТОВЫЙ БАЛАНС
            shards = 10;
            level = 1;

            lobbyBalanceText.text = coins.ToString();
            lobbyLevelNumText.text = level.ToString();
            lobbyShardsBalanceText.text = shards.ToString();

            gameLevelNumText.text = level.ToString();
            gameBalanceText.text = coins.ToString();
            gameShardsBalanceText.text = shards.ToString();
            gameBetText.text = bet.ToString();


        }
        else
            if (instance == this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
    }

    public void ChangePlayerIcon(int iconId)
    {

        playerImgId = (byte)iconId;
        stat_playerImg.sprite = iconsToChoose[iconId];
        edit_playerImg.sprite = iconsToChoose[iconId];
        lobbyPlayerIcon.sprite = iconsToChoose[iconId];
        gamePlayerIcon.sprite = iconsToChoose[iconId];
    }

    public void ChangePlayerName(string value)
    {
        player_name = value;

        stat_name.text = player_name;
        edit_nameplaceholder.text = player_name;
    }

    public int GetLevel()
    {
        return level;
    }

    public int getXP()
    {
        return xpAmount;
    }

    public void SetLevel(int value)
    {
        level = value;
    }

    public int GetCoins()
    {
        return coins;
    }
    public int GetShards()
    {
        return shards;
    }

    public void ChangeCoins(int amount)
    {
        coins = coins + amount;
        if (amount>0)
        {
            total_coins += amount;
        }
        lobbyBalanceText.text = coins.ToString();
        gameBalanceText.text = coins.ToString();

    }

    public void SetCoins(int amount)
    {
        coins = amount;
        lobbyBalanceText.text = coins.ToString();
        gameBalanceText.text = coins.ToString();
    }


    public void ChangeXP(int amount)
    {
        xpAmount += amount;
        gameXpAmountText.text = "+" + amount;



        if (xpAmount >= levelCaps[level-1])
        {
            level++;        

            lobbyLevelNumText.text = level.ToString();
            gameLevelNumText.text = level.ToString();
        }

        gameLevelCapText.text = getXP() + "/" + levelCaps[level - 1];
        StartCoroutine("HideGainedXP");

        _lobbyLevel.value = (float)xpAmount / (float)levelCaps[level - 1];
        _gameLevel.value = (float)xpAmount / (float)levelCaps[level - 1];

    }

    public void SetXP(int value)
    {
        xpAmount = value;
    }

    public void ChangeShards(int amount)
    {
        shards += amount;
        if (amount>0)
        {
            total_shards += amount;
        }

        lobbyShardsBalanceText.text = shards.ToString();
        gameShardsBalanceText.text = shards.ToString();
    }

    public void SetShards(int value)
    {
        shards = value;
        lobbyShardsBalanceText.text = shards.ToString();
        gameShardsBalanceText.text = shards.ToString();
    }

    public int GetBet()
    {
        return bet;
    }

    public void SetBet(int amount)
    {
        bet = amount;
        gameBetText.text = bet.ToString();
    }

    public int GetTotalCoins()
    {
        return total_coins;
    }

    public void SetTotalCoints(int value)
    {
        total_coins = value;
    }

    public int GetTotalShards()
    {
        return total_shards;
    }

    public void SetTotalShards(int value)
    {
        total_shards = value;
    }

    public string GetPlayerName()
    {
        return player_name;
    }


    public int GetPlayerIconID()
    {
        return playerImgId;
    }

    IEnumerator HideGainedXP()
    {
        yield return new WaitForSeconds(2f);
        gameXpAmountText.text = "";
        gameLevelCapText.text = "";
    }
}
