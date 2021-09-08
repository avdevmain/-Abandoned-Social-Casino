using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;


public class DataManager : MonoBehaviour  //Загрузка и сохранение данных
{

    public static DataManager instance;



    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;           
            LoadUserData();
            StartCoroutine(CheckInternet());
        }
        else
        if (instance == this)
            Destroy(gameObject);
    }

    private void OnApplicationPause(bool pause)
    {
        instance.SaveUserData();
    }

    private void ClearPrefs()
    {
        PlayerPrefs.DeleteKey("coins");
        PlayerPrefs.DeleteKey("shards");
        PlayerPrefs.DeleteKey("total_coins");
        PlayerPrefs.DeleteKey("total_shards");
        PlayerPrefs.DeleteKey("xp");
        PlayerPrefs.DeleteKey("level");
        PlayerPrefs.DeleteKey("name");
    }


    public void SaveUserData()
    {

        StartCoroutine(SaveWWWRequest());

        PlayerPrefs.SetFloat("mainVolume", SoundManager.instance.GetMainVolume());
        PlayerPrefs.SetFloat("musicVolume", SoundManager.instance.GetMainVolume());
        PlayerPrefs.SetFloat("sfxVolume", SoundManager.instance.GetMainVolume());
        PlayerPrefs.SetInt("playerIcon", PlayerManager.instance.GetPlayerIconID());

        for (int i =0; i<4; i++)
        {
            PlayerPrefs.SetInt("freespins[" + i + "]", ButtonHandler.freeSpins[i]);
        }

        

    }

    public void LoadUserData()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("ИНТЕРНЕТ ВРУБИ");
        }
        else
        {
            StartCoroutine(StartWWWRequest());

            if (PlayerPrefs.HasKey("mainVolume"))
            {
                PlayerManager.instance.ChangePlayerIcon(PlayerPrefs.GetInt("playerIcon"));
                SoundManager.instance.SetMainVolume(PlayerPrefs.GetFloat("mainVolume"));
                SoundManager.instance.SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
                SoundManager.instance.SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));

                for (int i = 0; i < 4; i++)
                {
                    ButtonHandler.freeSpins[i] = PlayerPrefs.GetInt("freespins[" + i + "]");
                }
            }
        }
        
    }


    public void LoadSounds(int slotID)
    {
        //Подгрузка звуков для отдельных слотов
        SoundManager.instance.slotBackClip = Resources.Load<AudioClip>("Sound/" + slotID + "/backgroundClip");
        SoundManager.instance.rollClip = Resources.Load<AudioClip>("Sound/" + slotID + "/rollClip");
        SoundManager.instance.winClip = Resources.Load<AudioClip>("Sound/" + slotID + "/winClip");
        SoundManager.instance.megaWinClip = Resources.Load<AudioClip>("Sound/" + slotID + "/megaWinClip");
        
        switch (slotID)
        {
            case 2: //Слот с колесом
                SoundManager.instance.specialClips.Add(Resources.Load<AudioClip>("Sound/" + slotID + "/wheelRollClip"));
                SoundManager.instance.specialClips.Add(Resources.Load<AudioClip>("Sound/" + slotID + "/rewardClip"));
                break;
            case 3: //Слот с минькой про скелета
                SoundManager.instance.specialClips.Add(Resources.Load<AudioClip>("Sound/" + slotID + "/wildStolenClip"));
                SoundManager.instance.specialClips.Add(Resources.Load<AudioClip>("Sound/" + slotID + "/damageDoneClip"));
                SoundManager.instance.specialClips.Add(Resources.Load<AudioClip>("Sound/" + slotID + "/killedClip"));
                SoundManager.instance.specialClips.Add(Resources.Load<AudioClip>("Sound/" + slotID + "/resurrectionClip"));
                break;
        }

        SoundManager.instance.slotBackgroundMusic.clip = SoundManager.instance.slotBackClip;
        SoundManager.instance.rollSound.clip = SoundManager.instance.rollClip;
        SoundManager.instance.winSound.clip = SoundManager.instance.winClip;

        /*
        AudioClip slotBackClip;
        AudioClip winClip;
        AudioClip megaWinClip;
        AudioClip rollClip;
       List<AudioClip> specialClips = new List<AudioClip>(); 
        */
    }


    private IEnumerator StartWWWRequest()
    {
    
        int coinsValue;
        int shardsValue;
        int expAmount;
        int levelNum;
        string playerName;
        int total_coins;
        int total_shards;
 
        int offs;


        if (PlayerPrefs.HasKey("coins")) //Последнее сохранение произошло при отсутствии интернета
        {
            coinsValue = PlayerPrefs.GetInt("coins");
            shardsValue = PlayerPrefs.GetInt("shards");
            expAmount = PlayerPrefs.GetInt("xp");
            levelNum = PlayerPrefs.GetInt("level");
            playerName = PlayerPrefs.GetString("name");
            total_coins = PlayerPrefs.GetInt("total_coins");
            total_shards = PlayerPrefs.GetInt("total_shards");

            PlayerManager.instance.SetCoins(coinsValue);
            PlayerManager.instance.SetShards(shardsValue);
            PlayerManager.instance.SetXP(expAmount);
            PlayerManager.instance.SetLevel(levelNum);
            PlayerManager.instance.ChangePlayerName(playerName);
            PlayerManager.instance.SetTotalCoints(total_coins);
            PlayerManager.instance.SetTotalShards(total_shards);



        }
        else
        {

            //ID игрока в гугл плее  PlayGamesPlatform.Instance.localUser.id
            int time = 0;
            if (PlayerPrefs.HasKey("offs"))
                time = PlayerPrefs.GetInt("offs");
            else
                time = TimeZoneInfo.Local.BaseUtcOffset.Hours;

                WWW Query = new WWW("http://cheeseru.pythonanywhere.com/casinologin?id=" + "testid" + "&dmodel=" + SystemInfo.deviceModel + "&did=" + SystemInfo.deviceUniqueIdentifier + "&svtz=" + time);

            yield return Query;

            if (!string.IsNullOrEmpty(Query.error))
            {
                //Ошибка какая-то :( 
            }
            else
            {
                string[] requestData = Query.text.Split('/');


                string[] notifications = requestData[1].Split(';');

                requestData[0] = requestData[0].Replace("'", "");

                var N = JSON.Parse(requestData[0]);
                 coinsValue = N["coins"].AsInt;
                 shardsValue = N["shards"].AsInt;
                 expAmount = N["exp"].AsInt;
                 levelNum = N["level"].AsInt;
                 playerName = N["name"].Value;
                 total_coins = N["total_coins"].AsInt;
                 total_shards = N["total_shards"].AsInt;
                var last_online = N["last_online"].Value;

                if (!PlayerPrefs.HasKey("offs"))
                {
                    offs = N["timezone"].AsInt;
                    PlayerPrefs.SetInt("offs", offs);
                }
                Debug.Log(playerName + " has " + coinsValue + " coins, " + shardsValue + " shards and got " + levelNum + " level");
                Debug.Log("And notifications:");
                for (int i = 0; i < notifications.Length - 1; i++)
                    Debug.Log(notifications[i]);
            }
            


        }
        if (PlayerPrefs.HasKey("coins"))
        ClearPrefs(); //Не удаляю всё потому что есть ключи, содержащие настройки звука
    }

    private IEnumerator SaveWWWRequest()
    {
        int coins = PlayerManager.instance.GetCoins();
        int shards = PlayerManager.instance.GetShards();
        int total_coins = PlayerManager.instance.GetTotalCoins();
        int total_shards = PlayerManager.instance.GetTotalShards();
        int xp = PlayerManager.instance.getXP();
        int level = PlayerManager.instance.GetLevel();
        string pl_name = PlayerManager.instance.GetPlayerName();


        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            PlayerPrefs.SetInt("coins", coins);
            PlayerPrefs.SetInt("shards", shards);
            PlayerPrefs.SetInt("total_coins", total_coins);
            PlayerPrefs.SetInt("total_shards", total_shards);
            PlayerPrefs.SetInt("xp", xp);
            PlayerPrefs.SetInt("level", level);
            PlayerPrefs.SetString("name", pl_name);
        }
        else
        {
            string url = "http://cheeseru.pythonanywhere.com/casinoexit?id=" + "testid" + "&cns=" + coins + "&shds=" + shards + "&t_cns=" + total_coins + "&t_shds=" + total_shards + "&xp=" + xp + "&lvl=" + level + "&name=" + pl_name;
            WWW Query = new WWW(url);

            yield return Query;
            if (!string.IsNullOrEmpty(Query.error))
            {
                PlayerPrefs.SetInt("coins", coins);
                PlayerPrefs.SetInt("shards", shards);
                PlayerPrefs.SetInt("total_coins", total_coins);
                PlayerPrefs.SetInt("total_shards", total_shards);
                PlayerPrefs.SetInt("xp", xp);
                PlayerPrefs.SetInt("level", level);
                PlayerPrefs.SetString("name", pl_name);

                //Ошибка какая-то :(  -> Сохранять в память устройства всё
            }
            else
            { //Всё намальна сохранилось в базу и не требует повторки в память устройства
                if (PlayerPrefs.HasKey("coins"))
                {
                    ClearPrefs();
                }
            }
        }
    }

    private IEnumerator CheckInternet()
    {
        while (Application.isPlaying)
        {
            yield return new WaitForSecondsRealtime(150);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("ВЫРУБАЙ ДЕБИЛ НЕТ ИНТЕРНЕТА У ТЕБЯ");
            }
            else
            {
                yield return new WaitForSecondsRealtime(150);
                SaveUserData();
            }
        }
    }
}
