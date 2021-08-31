using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class FortuneWheelManager : MonoBehaviour
{
    public static FortuneWheelManager instance = null; //Экземпляр объекта

    private bool _isStarted;
    private float[] _sectorsAngles;
    private float _finalAngle;
    private float _startAngle = 0;
    private float _currentLerpRotationTime;
    public Button TurnButton;

    //public Sprite rewardIcon;
    //pubic Text reward;

    public GameObject Circle; 			// Rotatable Object with rewards
    public Text AttemptsDeltaText; 		// Pop-up text with wasted or rewarded coins amount
    public Text CurrentAttemptsText; 		// Pop-up text with wasted or rewarded coins amount

    public Image rewardIcon;
    public Text currentValueText;
    public Text valueDeltaText;

    public int TurnCost = 1;			// How much coins user waste when turn whe wheel
    public int CurrentAttempts = 0;	// Started coins amount. In your project it can be set up from CoinsManager or from PlayerPrefs and so on
    public int PreviousAttemptsAmount;		// For wasted coins animation
    private int previousValueAmount;
    private int currentValue;

    private int[] circleType = new int[12]; //тип и количество для каждого сектора
    private int[] circleAmount = new int[12];

    private Image[] circleRewardIcons = new Image[12];
    private Text[] circleRewardNums = new Text[12];

    public Sprite[] rewardIcons;

    private int totalCoinsWon = 0; //ПОСЛЕ ТОГО КАК ПОПЫТКИ КОНЧАТСЯ ВЫВОДИТЬ СКОЛЬКО ВСЕГО ВЫИГРАЛ ИГРОК
    private int totalShardsWon = 0;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;

            rewardIcons = new Sprite[4];

            TurnButton = transform.parent.GetChild(1). GetChild(9).GetComponent<Button>();
            Circle = transform.parent.GetChild(1).GetChild(3).gameObject;
            AttemptsDeltaText = transform.parent.GetChild(1).GetChild(8).GetComponent<Text>();
            CurrentAttemptsText = transform.parent.GetChild(1).GetChild(7).GetComponent<Text>();
            rewardIcon = transform.parent.GetChild(1).GetChild(10).GetComponent<Image>();
            currentValueText = transform.parent.GetChild(1).GetChild(11).GetComponent<Text>();
            valueDeltaText = transform.parent.GetChild(1).GetChild(12).GetComponent<Text>();

            rewardIcons[0] = Resources.Load<Sprite>("Sprites/UI/coin"); //Иконка монеты
            rewardIcons[1] = Resources.Load<Sprite>("Sprites/UI/star"); //Shards иконка (звезда пока на заглушке)
            rewardIcons[2] = Resources.Load<Sprite>("Sprites/UI/lightning"); //Попытки
            rewardIcons[3] = Resources.Load<Sprite>("Sprites/UI/heart"); //Фриспины

            for (int i =0; i<12; i++)
            {
                circleRewardIcons[i] = Circle.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>();
                circleRewardNums[i] = Circle.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>();
            }

            TurnButton.onClick.AddListener(TurnWheel);
            currentValueText.gameObject.SetActive(false);
            valueDeltaText.gameObject.SetActive(false);
            PreviousAttemptsAmount = CurrentAttempts;
            CurrentAttemptsText.text = CurrentAttempts.ToString();
            GenerateWheel();

        }
        else
            if (instance == this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);
    }

    public void GenerateWheel()
    {
        int coinSectorMAX = 9;   int coinAmountMAX = PlayerManager.instance.GetLevel()*500;
        int attemptSectorMAX = 1; int attemptAmountMAX = 5;
        int shardSectorMAX = 2; int shardAmountMAX = PlayerManager.instance.GetLevel() * 10;
        int freeSectorMAX = 2; int freeAmountMAX = PlayerManager.instance.GetLevel() * 5;

        int coinCurrent = 0; int shardCurrent = 0;  int attemptCurrent = 0; int freeCurrent = 0;

        int i = 0;
        while (i !=12)
        {
            int value = UnityEngine.Random.Range(0,33);
            circleType[i] = value;
            switch (value)
            {
                case 0:

                    if (coinCurrent<coinSectorMAX)
                    {                                            
                        circleAmount[i] = 50*( Mathf.CeilToInt(UnityEngine.Random.Range(coinAmountMAX / 20, coinAmountMAX))/50);
                        circleRewardIcons[i].sprite = rewardIcons[0];
                        circleRewardNums[i].text = circleAmount[i].ToString();
                        coinCurrent++;
                        i++;
                    }

                    break;
                case 1:
                    if (shardCurrent<shardSectorMAX)
                    {
                        circleAmount[i] = 2 * (Mathf.CeilToInt(UnityEngine.Random.Range(shardAmountMAX / 10, shardAmountMAX)) / 2);
                        circleRewardIcons[i].sprite = rewardIcons[1];
                        circleRewardNums[i].text = circleAmount[i].ToString();
                        shardCurrent++;
                        i++;
                    }
                    break;
                case 2:
                    if (attemptCurrent < attemptSectorMAX)
                    {
                        circleAmount[i] = UnityEngine.Random.Range(1, attemptAmountMAX);
                        circleRewardIcons[i].sprite = rewardIcons[2];
                        circleRewardNums[i].text = circleAmount[i].ToString();
                        attemptCurrent++;
                        i++;
                    }
                    break;
                case 3:
                    if (freeCurrent < freeSectorMAX)
                    {
                        circleAmount[i] = UnityEngine.Random.Range(1, freeAmountMAX);
                        circleRewardIcons[i].sprite = rewardIcons[3];
                        circleRewardNums[i].text = circleAmount[i].ToString();
                        freeCurrent++;
                        i++;
                    }

                    break;

            }        
        }
        coinCurrent = 0; shardCurrent = 0; attemptCurrent = 0;
    }

    public void TurnWheel ()
    {
    	// Player has enough money to turn the wheel
        if (CurrentAttempts >= TurnCost) {
    	    _currentLerpRotationTime = 0f;
    	
    	    // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)
    	    _sectorsAngles = new float[] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360 };
    	
    	    int fullCircles = 5;
    	    float randomFinalAngle = _sectorsAngles [UnityEngine.Random.Range (0, _sectorsAngles.Length)];
    	
    	    // Here we set up how many circles our wheel should rotate before stop
    	    _finalAngle = -(fullCircles * 360 + randomFinalAngle);
    	    _isStarted = true;

            PreviousAttemptsAmount = CurrentAttempts;

            // Decrease attempt for the turn
            CurrentAttempts -= TurnCost;

            // Show wasted attempt
            AttemptsDeltaText.text = "-" + TurnCost;
            AttemptsDeltaText.gameObject.SetActive (true);
    	
    	    // Animate attempts
    	    StartCoroutine (HideDelta ());
    	    StartCoroutine (UpdateAmount(2));
    	}
    }

    private void GiveAwardByAngle ()
    {
        int angleNum;
    	// Here you can set up rewards for every sector of wheel
    	switch ((int)_startAngle) {
    	case 0:
            angleNum = 0;
    	    break;
    	case -330:
            angleNum = 1;
    	    break;
    	case -300:
                angleNum = 2;
    	    break;
    	case -270:
                angleNum = 3;
    	    break;
    	case -240:
                angleNum = 4;
    	    break;
    	case -210:
                angleNum = 5;
    	    break;
    	case -180:
                angleNum = 6;
    	    break;
    	case -150:
                angleNum = 7;
    	    break;
    	case -120:
                angleNum = 8;
    	    break;
    	case -90:
                angleNum = 9;
    	    break;
    	case -60:
                angleNum = 10;
    	    break;
    	case -30:
                angleNum = 11;
    	    break;
    	default:
                angleNum = 0;
    	    break;
        }

        switch(circleType[angleNum])
        {
            case 0:
                RewardCoins(circleAmount[angleNum]);
                totalCoinsWon += circleAmount[angleNum];
                break;
            case 1:
                RewardShards(circleAmount[angleNum]);
                totalShardsWon += circleAmount[angleNum];
                break;
            case 2:
                RewardAttepts(circleAmount[angleNum]);
                break;
            case 3:
                RewardFreeSpins(circleAmount[angleNum]);
                break;
        }


    }

    void Update ()
    {
        // Make turn button non interactable if user has not enough money for the turn
    	if (_isStarted || CurrentAttempts < TurnCost) {
    	    TurnButton.interactable = false;
    	    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
    	} else {
    	    TurnButton.interactable = true;
    	    TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1);
    	}

    	if (!_isStarted)
    	    return;

    	float maxLerpRotationTime = 4f;
    
    	// increment timer once per frame
    	_currentLerpRotationTime += Time.deltaTime;
    	if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle) {
    	    _currentLerpRotationTime = maxLerpRotationTime;
    	    _isStarted = false;
    	    _startAngle = _finalAngle % 360;
    
    	    GiveAwardByAngle ();
    	    StartCoroutine(HideDelta());
    	}
    
    	// Calculate current position using linear interpolation
    	float t = _currentLerpRotationTime / maxLerpRotationTime;
    
    	// This formulae allows to speed up at start and speed down at the end of rotation.
    	// Try to change this values to customize the speed
    	t = t * t * t * (t * (6f * t - 15f) + 10f);
    
    	float angle = Mathf.Lerp (_startAngle, _finalAngle, t);
    	Circle.transform.eulerAngles = new Vector3 (0, 0, angle);
    }

    public void RewardCoins (int awardCoins) //Награда в деньгах, должно вызываться из родительского метода
    {
        //rewardIcon = 
        valueDeltaText.text = "+" + awardCoins;
        valueDeltaText.gameObject.SetActive (true);
        previousValueAmount = PlayerManager.instance.GetCoins();
        PlayerManager.instance.ChangeCoins(awardCoins);
        currentValueText.gameObject.SetActive(true);
        rewardIcon.gameObject.SetActive(true);
        StartCoroutine (UpdateAmount (0));

    }

    private void RewardShards (int awardShards) //Награда в кристаллах
    {
        //rewardIcon = 
        valueDeltaText.text = "+" + awardShards;
        previousValueAmount = PlayerManager.instance.GetShards();
        valueDeltaText.gameObject.SetActive(true);
        currentValueText.gameObject.SetActive(true);
        rewardIcon.gameObject.SetActive(true);
        PlayerManager.instance.ChangeShards(awardShards);
        StartCoroutine(UpdateAmount(1));

    }

    private void RewardAttepts (int attepts) //Дополнительные попытки
    {
        AttemptsDeltaText.text = "+" + attepts;
        AttemptsDeltaText.gameObject.SetActive(true);
        CurrentAttempts += attepts;

        StartCoroutine(UpdateAmount(2));

    }

    private void RewardFreeSpins(int spins) //Бесплатные вращения
    {
        valueDeltaText.text = "+" + spins;
        previousValueAmount = ButtonHandler.freeSpins[2];
        valueDeltaText.gameObject.SetActive(true);
        currentValueText.gameObject.SetActive(true);
        rewardIcon.gameObject.SetActive(true);
        ButtonHandler.freeSpins[2] += spins;
        StartCoroutine(UpdateAmount(3));
    }

    public void SetAttempts(int value)
    {
        CurrentAttempts = value;
        CurrentAttemptsText.text = CurrentAttempts.ToString();
    }

    private IEnumerator HideDelta ()
    {
        yield return new WaitForSeconds (1f);
        AttemptsDeltaText.gameObject.SetActive (false);
        valueDeltaText.gameObject.SetActive(false);
        currentValueText.gameObject.SetActive(false);
        rewardIcon.gameObject.SetActive(false);
    }

    private IEnumerator UpdateAmount (int type) //0 - монеты, 1 - кристаллы, 2 - попытки, 3 - фриспины
    {
        rewardIcon.sprite = rewardIcons[type];

    	// Animation for increasing and decreasing of coins amount
    	const float seconds = 0.5f;
    	float elapsedTime = 0;
    
       

        switch (type)
        {
            case 0:
                rewardIcon.sprite = rewardIcons[type];
                rewardIcon.enabled = true;
                currentValue = PlayerManager.instance.GetCoins();

                while (elapsedTime < seconds)
                {
                    currentValueText.text = Mathf.Floor(Mathf.Lerp(previousValueAmount,currentValue , (elapsedTime / seconds))).ToString();
                    elapsedTime += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                previousValueAmount = currentValue;
                currentValueText.text = currentValue.ToString();

                break;
            case 1:
                rewardIcon.sprite = rewardIcons[type];
                rewardIcon.enabled = true;
                currentValue = PlayerManager.instance.GetShards();

                while (elapsedTime < seconds)
                {
                    currentValueText.text = Mathf.Floor(Mathf.Lerp(previousValueAmount, currentValue, (elapsedTime / seconds))).ToString();
                    elapsedTime += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                previousValueAmount = currentValue;
                currentValueText.text = currentValue.ToString();

                break;
            case 2:

                rewardIcon.enabled = false;
                while (elapsedTime < seconds)
                {
                    CurrentAttemptsText.text = Mathf.Floor(Mathf.Lerp(PreviousAttemptsAmount, CurrentAttempts, (elapsedTime / seconds))).ToString();
                    elapsedTime += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                PreviousAttemptsAmount = CurrentAttempts;
                CurrentAttemptsText.text = CurrentAttempts.ToString();
                break;

            case 3:
                rewardIcon.sprite = rewardIcons[type];
                rewardIcon.enabled = true;
                currentValue = ButtonHandler.freeSpins[2];
                while (elapsedTime < seconds)
                {
                    currentValueText.text = Mathf.Floor(Mathf.Lerp(previousValueAmount, currentValue, (elapsedTime / seconds))).ToString();
                    elapsedTime += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                previousValueAmount = currentValue;
                currentValueText.text = currentValue.ToString();

                break;

        }

        if (CurrentAttempts == 0)
            StartCoroutine(CloseWheelMenu());
    }

    private IEnumerator CloseWheelMenu()
    {
        yield return new WaitForSeconds(5f);
        Circle.transform.parent.gameObject.SetActive(false);
    }
}
