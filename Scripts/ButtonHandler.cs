using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonHandler : MonoBehaviour
{
    public Button readyToStartBttn;

    public GameObject rewardsList;
    
    public static int lobbyIndex = 0;
    public static int slotPatternIndex = 1;
    public static int specialIndex = 2;
    
    public GameObject data_manager;
    public GameObject sound_manager;
    public GameObject player_manager;
    public GameObject wheel_manager;
    public GameObject notification_manager;

    public BoxCollider2D[] catchElements = new BoxCollider2D[5];
    public Transform[] up_lines = new Transform[5];
    public Transform[] down_lines = new Transform[5];
    public Text winText;
    private int[,,] elementsType = new int[2, 5, 10];
    private Sprite[] slotElements = new Sprite[10];

    public Image lockIcon;

    public GameObject winFramePrefab;
    public Image slotLogo;

    public static int[] lastElementsID = new int[5];

    public GameObject[] slotEffects = new GameObject[3]; // 0 - freeSpin, 1 - megaWin, 2 - great9

    private int[,] finalGrid = new int[3,5];
    private int[,] finalTypes = new int[3, 5];
    private int[,] extraGrid = new int[6, 5];

    private List<GameObject> winFrames = new List<GameObject>();
    private List<GameObject> spawnedWinLines = new List<GameObject>();
    public List<GameObject> greatElements = new List<GameObject>();

    public List<Coroutine> showPayLines = new List<Coroutine>();

    private List<Sprite> winLines = new List<Sprite>();
    public GameObject winLinesParent;
    public GameObject winLinePrefab;
    private int linesAmount = 50; //Сколько линий загружено в папку

    public Text freeSpinsText;

    /* private float[] lineRewards = {0.5f, 2f, 6f, 0.5f, 2f, 6f, 1f,2f,8f,1f,2f,8f,1.5f,4f,15f,1.5f, 4f, 15f, 4f,15f, 60f, 3f, 20f, 110f, 1f, 20f, 100f,
                                    0.5f, 2f, 6f, 0.5f, 2f, 6f, 1f,2f,8f,1f,2f,8f,1.5f,4f,15f,1.5f, 4f, 15f, 4f,15f, 60f, 3f, 20f, 110f, 1f, 20f, 100f,
                                    0.5f, 2f, 6f, 0.5f, 2f, 6f, 1f,2f,8f,1f,2f,8f,1.5f,4f,15f,1.5f, 4f, 15f, 4f,15f, 60f, 3f, 20f, 110f, 1f, 20f, 100f,
                                    0.5f, 2f, 6f, 0.5f, 2f, 6f, 1f,2f,8f,1f,2f,8f,1.5f,4f,15f,1.5f, 4f, 15f, 4f,15f, 60f, 3f, 20f, 110f, 1f, 20f, 100f,

     }; */

    private float[] lineRewards = {0, 0.5f, 2f, 6f, 0, 0.5f, 2f, 6f, 0, 1f, 2f, 8f, 0, 1f, 2f, 8f, 0, 1.5f, 4f, 15f, 0, 1.5f, 4f, 15f, 0, 4f, 15f, 60f, 2f, 5f, 20f, 110f, 0, 0, 0, 100f,
                                   0, 0.5f, 2f, 6f, 0, 0.5f, 2f, 6f, 0, 1f, 2f, 8f, 0, 1f, 2f, 8f, 0, 1.5f, 4f, 15f, 0, 1.5f, 4f, 15f, 0, 4f, 15f, 60f, 2f, 5f, 20f, 110f, 0, 0, 0, 100f,
                                   0, 0.5f, 2f, 6f, 0, 0.5f, 2f, 6f, 0, 1f, 2f, 8f, 0, 1f, 2f, 8f, 0, 1.5f, 4f, 15f, 0, 1.5f, 4f, 15f, 0, 4f, 15f, 60f, 2f, 5f, 20f, 110f, 0, 0, 0, 100f,
                                   0, 0.5f, 2f, 6f, 0, 0.5f, 2f, 6f, 0, 1f, 2f, 8f, 0, 1f, 2f, 8f, 0, 1.5f, 4f, 15f, 0, 1.5f, 4f, 15f, 0, 4f, 15f, 60f, 2f, 5f, 20f, 110f, 0, 0, 0, 100f
 };

    private ElementCheck checkLast;

    private int pityTimer = 5; //Таймер неудачника - если игрок ничего не выиграл, то его надо подмазать победкой if(looserTimer==0) GenerateWin()

    private int activeSlotID;

    private List<int> diffRoll = new List<int>(); //Айди слотов, линии в которых вращаются и останавливаются по-разному
    private List<int> simultRoll = new List<int>(); //Айди слотов, линии в которых вращаются и останавливаются одновременно
    private bool withTimer; // Переменные таймера для разного старта вращения и остановки линий 
    private float rollStartTimer = 0.15f;
    private float rollTime = 2f; //Переменная таймера для длительности вращения линий

    private bool hasYetToStart;

    private bool rolling;
    private bool[] lineMoving = new bool[5];
    private bool[] linesOnPlace = new bool[5];

    private bool stopped;
    private bool spinned;
    private bool checkSpec;

    private float moveDist = 0.1f;

    private int[] regularBets = {5, 10, 20, 50, 100 };
    private int[] highRollBets = { 200, 500 , 1000};


    private int betId;


    private int regularChance = 85;
    private int wildChance = 10;

    private bool typesProcessed;
    private bool checkedSpecial;
    private bool cycleCheck;
    public static int[] freeSpins = {0,0,0,0 }; //Временно публик стутик (потом отдельно слот манагер, буттон манагер)

    private GameObject specialSlider;
    private int specialState = 0;

    private int specReward;
    private int scatterCount;

    public Button backbttn;
    public Button spinBttn;
    public Button profileBttn;
    public Button plusBetBttn;
    public Button minBetBttn;


    private float difference;
    private float lineDifference;

    private bool moveRewardList;

    private void Start()
    {
        
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        finalGrid[1, 0] = -1;
        diffRoll.Add(0);   diffRoll.Add(2);
        simultRoll.Add(1); simultRoll.Add(3);
        betId = 0;
        

       for (int i =1; i<linesAmount+1; i++)
       {
            winLines.Add(Resources.Load<Sprite>("Sprites/UI/lines/" + i));
       }

        if (DataManager.instance == null)
            Instantiate(data_manager, transform);

        if (PlayerManager.instance == null)
            Instantiate(player_manager, transform);

        if (SoundManager.instance == null)
            Instantiate(sound_manager, transform);
        if (NotificationManager.instance == null)
            Instantiate(notification_manager, transform);

         difference = Mathf.Abs(up_lines[0].transform.position.y - down_lines[0].transform.position.y);

        StartCoroutine(StartWWWRequest());
    }

    private void Update()
    {


        if (hasYetToStart)
            StartSpin();

        if (rolling)
            ProcessMoving();
        else
            if (stopped)
            ProcessFinal();
        else
        if (spinned)
        {
            ProcessTypes();
        }
        else
        if (typesProcessed)
            CheckSpecial();
        else
        if (checkedSpecial)
            CheckPayLines();



        if (moveRewardList)
        {
            MoveReward();
        }
    }



    public void SetHasYetToStart()
    {
     

        scatterCount = 0;
        freeSpinsText.text = freeSpins[activeSlotID].ToString();


        foreach (Coroutine cor in showPayLines)
        {
            StopCoroutine(cor);
        }
        showPayLines.Clear();

        foreach (GameObject obj in winFrames)
        {
            Destroy(obj);
           
        }
        winFrames.Clear();

        foreach (GameObject obj in spawnedWinLines)
        {
            Destroy(obj);
            
        }
        spawnedWinLines.Clear();

        if (!lockIcon.enabled)
        if ((PlayerManager.instance.GetBet() > 0) && (PlayerManager.instance.GetCoins() >= PlayerManager.instance.GetBet()) && !(rolling || stopped || spinned || checkedSpecial))
        {
                ToggleButtons();

            winText.text = "spinning";
            hasYetToStart = true;
                if (!cycleCheck)
                {
                    if (freeSpins[activeSlotID] == 0)
                        PlayerManager.instance.ChangeCoins(-PlayerManager.instance.GetBet());
                    else
                    {
                        freeSpins[activeSlotID]--;
                        freeSpinsText.text = freeSpins[activeSlotID].ToString();
                    }
                }

            }
    }

    public void toggleElementCatching() //Включает-выключает коллайдеры, которые отвечают за "поимку" последних элементов (которые встанут в середину)
    {
        for (int i =0; i<5; i++)
        {
            catchElements[i].enabled = !catchElements[i].isActiveAndEnabled;
        }
    }

    public void PrepareSlotLoading(string slotName)
    {
        activeSlotID = int.Parse(slotName);
        freeSpinsText.text = freeSpins[activeSlotID].ToString();
        StartCoroutine(LoadSlot());
    }

    public void LoadIcons() //Загрузка иконок элементов и всего прочего
    {
        DataManager.instance.LoadSounds(activeSlotID);

        for (int i = 0; i < 10; i++)
        {
            slotElements[i] = Resources.Load<Sprite>("Sprites/Elements/" + activeSlotID + "/" + (i + 1));
        }
        winFramePrefab.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/" + activeSlotID + "/" + "frame");
        slotLogo.sprite = Resources.Load<Sprite>("Sprites/UI/" + activeSlotID + "/" + "icon");
        slotEffects[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Elements/" + activeSlotID + "/great9");

        if (activeSlotID == 3) //Слот с убийством скелета, который крадет вилды
        {
            GameObject special = transform.GetChild(specialIndex).GetChild(1).GetChild(0).gameObject;
            //Подгрузить ХП 
            special.SetActive(true); //Включить объект SpecialSomething
            special.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/" + activeSlotID + "/" + "icon"); //Монстряку какую-то типа
            specialSlider = special.transform.GetChild(1).gameObject;
            specialSlider.GetComponent<Slider>().value = specialSlider.GetComponent<Slider>().maxValue;
            specialSlider.transform.GetChild(1).GetComponent<Image>().color = Color.green;
            specialState = 0; //full hp
        }
        else
        if (activeSlotID == 2) //Слот с колесом фортуны
        {

            GameObject special = transform.GetChild(specialIndex).GetChild(1).gameObject; //Объект WheelOfFortune         
                                                                                          //  special.SetActive(true); //Для настройки колеса. ВРЕМЕНННО

            if (FortuneWheelManager.instance == null)
            {
                Instantiate(wheel_manager, special.transform.parent);
            }


        }

        readyToStartBttn.transform.GetChild(0).GetComponent<Text>().text = "Press to enter";
        readyToStartBttn.interactable = true;



    }

    public void UnloadSlotData() //При заходе в другой слот нужно выгрузить данные предыдущего
    {
        transform.GetChild(specialIndex).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(specialIndex).GetChild(1).gameObject.SetActive(false);

        if (FortuneWheelManager.instance != null)
        {
            Destroy(FortuneWheelManager.instance.gameObject);
            FortuneWheelManager.instance = null;
        }


        foreach (var obj in SoundManager.instance.specialClips)
        {
            Resources.UnloadAsset(obj);
        }
        SoundManager.instance.specialClips.Clear();


        readyToStartBttn.transform.parent.GetChild(2).gameObject.SetActive(true);
    }


    public void CallGenerateSlotWithButton(int partId)
    {
        GenerateSlot(partId);
    }

    private void GenerateSlot(int partId, int lineId = 0) //slotId передается нажимаемой кнопкой.  partId = 0 - генерировать полностью ; 1 - вернюю часть ; 2 - нижнюю часть; lineID отвечает за то, какая линия будет генерироваться //Шанс скаттера (особый элемент слота) = 100 - (обычный+вилд)
    {
        int randValue = 0;

        if (partId == 0) //Сгенерировать верхнюю и нижнюю часть часть
        {
            for (int i =0; i<5; i++)
            {
                for (int j = 0; j< 10; j++)
                {
                    randValue = Random.Range(0, 101); //что генерировать

                    if (randValue < regularChance+1)
                    {
                        randValue = Random.Range(1, 9);
                        elementsType[0,i,j] = randValue;
                        up_lines[i].GetChild(j).GetComponent<Image>().sprite = slotElements[randValue-1];
                    }
                    else
                    if (randValue < regularChance+wildChance+1)
                    {
                        elementsType[0, i, j] = 9;
                        up_lines[i].GetChild(j).GetComponent<Image>().sprite = slotElements[8];
                    }
                    else
                    {                       
                        elementsType[0, i, j] = 10;
                        up_lines[i].GetChild(j).GetComponent<Image>().sprite = slotElements[9];
                    }

                    randValue = Random.Range(0, 101); //что генерировать

                    if (randValue < regularChance + 1)
                    {
                        randValue = Random.Range(1, 9);
                        elementsType[1, i, j] = randValue;
                        down_lines[i].GetChild(j).GetComponent<Image>().sprite = slotElements[randValue - 1];
                    }
                    else
                    if (randValue < regularChance + wildChance + 1)
                    {
                        elementsType[1, i, j] = 9;
                        down_lines[i].GetChild(j).GetComponent<Image>().sprite = slotElements[8];
                    }
                    else
                    {
                        elementsType[1, i, j] = 10;
                        down_lines[i].GetChild(j).GetComponent<Image>().sprite = slotElements[9];
                    }

                }

            }
        }
        
        if (partId == 1) //Сгенерировть верхнюю часть
        {          
                for (int j = 0; j < 10; j++)
                {
                randValue = Random.Range(0, 101); //что генерировать

                if (randValue < regularChance + 1)
                {
                    randValue = Random.Range(1, 9);
                    elementsType[0, lineId, j] = randValue;
                    up_lines[lineId].GetChild(j).GetComponent<Image>().sprite = slotElements[randValue - 1];
                }
                else
                if (randValue < regularChance + wildChance + 1)
                {
                    elementsType[0, lineId, j] = 9;
                    up_lines[lineId].GetChild(j).GetComponent<Image>().sprite = slotElements[8];
                }
                else
                {
                    elementsType[0, lineId, j] = 10;
                    up_lines[lineId].GetChild(j).GetComponent<Image>().sprite = slotElements[9];
                }
            }
            
        }

        if (partId ==2) //Сгенерировать нижнюю часть
        {
            for (int j = 0; j<10; j++)
            {
                randValue = Random.Range(0, 101); //что генерировать

                if (randValue < regularChance + 1)
                {
                    randValue = Random.Range(1, 9);
                    elementsType[1, lineId, j] = randValue;
                    down_lines[lineId].GetChild(j).GetComponent<Image>().sprite = slotElements[randValue - 1];
                }
                else
                if (randValue < regularChance + wildChance + 1)
                {
                    elementsType[1, lineId, j] = 9;
                    down_lines[lineId].GetChild(j).GetComponent<Image>().sprite = slotElements[8];
                }
                else
                {
                    elementsType[1, lineId, j] = 10;
                    down_lines[lineId].GetChild(j).GetComponent<Image>().sprite = slotElements[9];
                }
            }
        }
    }
    
    private void StartSpin()
    {

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                finalGrid[i, j] = -1;
            }
        }

        rollTime = 2f;

        for (int i = 0; i < 5; i++)
            linesOnPlace[i] = false;

        if (diffRoll.Contains(activeSlotID)) //Линии стартуют в разное время
        {

            if (activeSlotID == 0) //Линии стартуют слева-направо
            {
                lineMoving[0] = true;
                withTimer = true;
                rollStartTimer -= 1f * Time.deltaTime;
                if (rollStartTimer <= 0)
                {
                    rollStartTimer = 0.5f;
                    for (int i = 1; i < 5; i++)
                    {
                        if (lineMoving[i] == false)
                        {
                            if (i == 4)
                                hasYetToStart = false;

                            lineMoving[i] = true;
                            break;
                        }
                    }
                }
            }
            else //Линии стартуют справа-налево
            {

                lineMoving[4] = true;
                withTimer = true;
                rollStartTimer -= 1f * Time.deltaTime;
                if (rollStartTimer <= 0)
                {
                    rollStartTimer = 0.5f;
                    for (int i = 4; i > -1; i--)
                    {
                        if (lineMoving[i] == false)
                        {
                            if (i == 0)
                                hasYetToStart = false;

                            lineMoving[i] = true;
                            break;
                        }
                    }
                }


            }
        }
        else
        if(simultRoll.Contains(activeSlotID))
        {
            withTimer = false;

            for (int i = 0; i < 5; i++)
            {
                if (lineMoving[i] == false)
                {
                    if (i == 4)
                        hasYetToStart = false;

                    lineMoving[i] = true;
                    break;
                }
            }
        }

        rolling = true; //После начала движения всех элементов
    }

    private void ProcessMoving()
    {
        if (withTimer)
        {
            for (int i=0; i<5;i++)
            {
                if (lineMoving[i])
                {
                    up_lines[i].transform.position -= transform.up * Time.deltaTime * 2f;
                    down_lines[i].transform.position -= transform.up * Time.deltaTime * 2f;

                }

                if (up_lines[i].transform.localPosition.y < -591.5f)
                {
                    up_lines[i].transform.localPosition = new Vector2(up_lines[i].transform.localPosition.x, 397.5f);
                    GenerateSlot(1,i);
                }

                if (down_lines[i].transform.localPosition.y < -97.5f)
                {
                    down_lines[i].transform.localPosition = new Vector2(down_lines[i].transform.localPosition.x, 890.5f);
                    GenerateSlot(2,i);
                }
            }

            if (!hasYetToStart) // Все линии уже крутятся
            {               
                rollTime -= 1f * Time.deltaTime;
                if ((rollTime <= 0.25f) &&  (!catchElements[0].isActiveAndEnabled))
                {
                    toggleElementCatching();
                }
                if (rollTime <=0f)
                {
                    if (activeSlotID == 0) //Линии останавливаются справа-налево
                    {
                        lineMoving[4] = false;
                        //Остановка вращения

                        rollStartTimer -= 1f * Time.deltaTime;
                        if (rollStartTimer <= 0)
                        {
                            rollStartTimer = 0.15f;
                            for (int i = 3; i > -1; i--)
                            {
                                if (lineMoving[i] == true)
                                {
                                    if (i == 0)
                                    {
                                        rolling = false;

                                        stopped = true;
                                    }

                                    lineMoving[i] = false;
                                    break;
                                }
                            }
                        }
                    }
                    else //Линии останавливаются слева-направо
                    {
                        lineMoving[0] = false;
                        //Остановка вращения

                        rollStartTimer -= 1f * Time.deltaTime;
                        if (rollStartTimer <= 0)
                        {
                            rollStartTimer = 0.15f;
                            for (int i = 1; i < 5; i++)
                            {
                                if (lineMoving[i] == true)
                                {
                                    if (i == 4)
                                    {
                                        rolling = false;
                                        stopped = true;
                                    }

                                    lineMoving[i] = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        else //Если движение одновременное
        {

            for (int i = 0; i < 5; i++)
            {
                if (lineMoving[i])
                {
                    up_lines[i].transform.position -= transform.up * Time.deltaTime * 2f;
                    down_lines[i].transform.position -= transform.up * Time.deltaTime * 2f;

                }

                if (up_lines[i].transform.localPosition.y < -591.5f)
                {
                    up_lines[i].transform.localPosition = new Vector2(up_lines[i].transform.localPosition.x, 397.5f);
                    GenerateSlot(1, i);
                }

                if (down_lines[i].transform.localPosition.y < -97.5f)
                {
                    down_lines[i].transform.localPosition = new Vector2(down_lines[i].transform.localPosition.x, 890.5f);
                    GenerateSlot(2, i);
                }
            }

            if (!hasYetToStart) // Все линии уже крутятся
            {
                rollTime -= 1f * Time.deltaTime;
                if ((rollTime <= 0.25f) && (!catchElements[0].isActiveAndEnabled))
                {
                    toggleElementCatching();
                }
                if (rollTime <= 0f)
                {
                    for (int i = 0; i < 5; i++)
                    { lineMoving[i] = false;
                        if (i==4)
                        {
                            rolling = false;
                            stopped = true;
                        }
                    }
                    
                    //Остановка вращения

                            
                        
                    
                }
            }


        }
    }

    private void ProcessFinal() //Докрутка элементов до центра
    {      

        //Debug.Log("BEFORE   " + (up_lines[0].transform.position.y - down_lines[0].transform.position.y));
        for (int i = 0; i < 5; i++)  //Сдвинуть все линии в нужную позицию
        {
            

            if (lastElementsID[i] < 10)
            {

                float dist = up_lines[i].GetChild(lastElementsID[i]).transform.position.y - catchElements[i].transform.position.y;

                if (Mathf.Abs(dist) > 0.05f)
                {
                    up_lines[i].transform.position += transform.up * Time.deltaTime * 3f;
                    down_lines[i].transform.position += transform.up * Time.deltaTime * 3f;
                }
                else
                {
                    //Debug.Log("Близко");
                    up_lines[i].transform.position = new Vector2(up_lines[i].transform.position.x, up_lines[i].transform.position.y - dist);
                    if (down_lines[i].transform.position.y < up_lines[i].transform.position.y)
                    {
                        down_lines[i].transform.position = new Vector2(down_lines[i].transform.position.x, up_lines[i].transform.position.y - difference);
                    }
                    else
                    {
                        down_lines[i].transform.position = new Vector2(down_lines[i].transform.position.x, up_lines[i].transform.position.y + difference);
                    }
                    linesOnPlace[i] = true;                  
                }

                //+ нужно докрутить нижние линии следом за верхними
            }
            else
            {

                float dist = down_lines[i].GetChild(lastElementsID[i] - 10).transform.position.y - catchElements[i].transform.position.y;

                if (Mathf.Abs(dist) > 0.05f)
                {
                    down_lines[i].transform.position += transform.up * Time.deltaTime * 3f;
                    up_lines[i].transform.position += transform.up * Time.deltaTime * 3f;
                }
                else
                {
                   // Debug.Log("Близко");
                    down_lines[i].transform.position = new Vector2(down_lines[i].transform.position.x, down_lines[i].transform.position.y - dist);
                    if (up_lines[i].transform.position.y < down_lines[i].transform.position.y)
                    {
                        up_lines[i].transform.position = new Vector2(up_lines[i].transform.position.x, down_lines[i].transform.position.y - difference);
                    }
                    else
                    {
                        up_lines[i].transform.position = new Vector2(up_lines[i].transform.position.x, down_lines[i].transform.position.y + difference);
                    }
                    linesOnPlace[i] = true;                   
                } 
                //тоже нужно докрутить, только верхние следом за нижними
            }
        }

        if (linesOnPlace[0] && linesOnPlace[1] && linesOnPlace[2] && linesOnPlace[3] && linesOnPlace[4])
        {
            SoundManager.instance.rollSound.Play();
            spinned = true;
            stopped = false;

        }
        withTimer = false;
        //Debug.Log("AFTER   " + (up_lines[0].transform.position.y - down_lines[0].transform.position.y));
    }

    private void SetColumnOWilds(int column)
    {
        if (finalGrid[0, column] < 10)
        {
            up_lines[column].GetChild(finalGrid[0, column]).GetComponent<Image>().sprite = slotElements[8];
        }
        else
        {
            down_lines[column].GetChild(finalGrid[0, column] - 10).GetComponent<Image>().sprite = slotElements[8];
        }

        if (finalGrid[1, column] < 10)
        {
            up_lines[column].GetChild(finalGrid[1, column]).GetComponent<Image>().sprite = slotElements[8];
        }
        else
        {
            down_lines[column].GetChild(finalGrid[1, column] - 10).GetComponent<Image>().sprite = slotElements[8];
        }

        if (finalGrid[2, column] < 10)
        {
            up_lines[column].GetChild(finalGrid[2, column]).GetComponent<Image>().sprite = slotElements[8];
        }
        else
        {
            down_lines[column].GetChild(finalGrid[2, column] - 10).GetComponent<Image>().sprite = slotElements[8];
        }

        for (int i = 0; i < 3; i++)
            finalTypes[i, column] = 9;
    }

    void SpawnGreatOne(float xOffset = 0)
    {
        GameObject newGreat = Instantiate(slotEffects[2], up_lines[0].parent.parent.parent.parent);
        newGreat.transform.SetSiblingIndex(1);     
        if (xOffset!=0)    
            newGreat.transform.position = new Vector3(newGreat.transform.position.x + xOffset, newGreat.transform.position.y, newGreat.transform.position.z);       
        winFrames.Add(newGreat);
    }

    private void CheckSpecial()
    { 
        bool itWas = false;
        switch(activeSlotID)
        {
            case 0:

                if (!cycleCheck)
                {
                    if ((finalTypes[0, 2] == 9) || (finalTypes[1, 2] == 9) || (finalTypes[2, 2] == 9))
                    {
                        int value = Random.Range(1, 11);
                        if (value > 0)
                        {
                            Debug.Log("Прокнул спешиал слота (1-ый раз)");
                            SetColumnOWilds(2);
                            SpawnGreatOne();
    
                            itWas = true;
                        }
                    }
                }
                else //Проверка спешала после выпадения спешала на прошлом спине
                {
                    SetColumnOWilds(2);
                    SpawnGreatOne();

                    Debug.Log("Циклическая тема спешиала");
                    if ((finalTypes[0, 1] == 9) || (finalTypes[1, 1] == 9) || (finalTypes[2, 1] == 9))
                    {
                        itWas = true;                       
                        SetColumnOWilds(1);
                        SpawnGreatOne(-1.05f);


                    }

                    if ((finalTypes[0, 3] == 9) || (finalTypes[1, 3] == 9) || (finalTypes[2, 3] == 9))
                    {
                        itWas = true;
                        SetColumnOWilds(3);
                        SpawnGreatOne(1.05f);
                    }
                }
                break;

            case 1:

                if (!cycleCheck)
                {
                    for (int i =0; i<5; i++)
                    if ((finalTypes[0, i] == 9) && (finalTypes[1, i] == 9) && (finalTypes[2, i] == 9))
                    {
                            regularChance = 75;
                            wildChance = 20;
                            SpawnGreatOne(-2.1f + (1.05f * i));
                            cycleCheck = true;
                            freeSpins[activeSlotID] = 10;
                            freeSpinsText.text = freeSpins.ToString();

                        }


                    //Если выпала хоть одна линия из вилдов, то дается Х фриспинов, во время которых шанс выпадения вилдов увеличивается   
                }
                else
                {
                    for (int i =0; i<5; i++)
                    {
                        if ((finalTypes[0, i] == 9) && (finalTypes[1, i] == 9) && (finalTypes[2, i] == 9))
                        {
                            freeSpins[activeSlotID] += 5;
                            freeSpinsText.text = freeSpins.ToString();
                        }
                    }

                    freeSpins[activeSlotID]--;
                    if (freeSpins[activeSlotID] == 0)
                    {
                        cycleCheck = false;
                        regularChance = 85; //Вернуть шансы в обычные значения
                        wildChance = 10;
                    }
                    freeSpinsText.text = freeSpins.ToString();
                }

                break;

            case 2:

                //Колесо фортуны

                if (scatterCount>3)
                {

                    GameObject special = transform.GetChild(2).GetChild(1).gameObject; //Объект WheelOfFortune  
                    special.SetActive(true);

                    
                    FortuneWheelManager.instance.SetAttempts(scatterCount-2);

                }


                break;

            case 3:

   
                List<int> extendCoords = new List<int>();

                  if (specialState != 3) //Если скелет ещё не побежден
                  {
                     
                      float maxValue = specialSlider.GetComponent<Slider>().maxValue;


                      if (finalTypes[2, 4] == 10) //Надо будет спавнить специальный элемент, а не скаттер. Либо в этом слоте убрать скаттер.
                      {
                          specialSlider.GetComponent<Slider>().value -= 1;

                      }



                      if (specialSlider.GetComponent<Slider>().value == (2 * maxValue / 3))
                      {
                          specialSlider.transform.GetChild(1).GetComponent<Image>().color = Color.yellow;
                          specialState = 1; //Yellow hp. Желтизна. Вилды раскрываются влево.
                      }
                      else
                      if (specialSlider.GetComponent<Slider>().value == (maxValue / 3))
                      {
                          specialSlider.transform.GetChild(1).GetComponent<Image>().color = Color.red;
                          specialState = 2; //Red hp. Краснота. Вилды раскрываются влево и вправо
                      }

                      if (specialSlider.GetComponent<Slider>().value <= 0)
                      {
                          transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/UI/" + activeSlotID + "/" + "dead_icon");
                          specialState = 3; //Dead. ПоХИБ. Дает Х бесплатных вращений с повышенным шансом вилдов

                          freeSpins[activeSlotID] += 10;
                        freeSpinsText.text = freeSpins.ToString();
                        regularChance = 75;
                          wildChance = 20;
                      }



                    switch (specialState)
                      {

                          case 0: //Жив-здоров. Можно например с шансом забирать ВИЛД
                            
                              for (int i =0; i<3; i++)
                              for (int j = 0; j<5; j++)
                                  {
                                      if (finalTypes[i,j] == 9)
                                      {
                                          int random = Random.Range(0, 101);
                                          if (random >90)
                                          {
                                              Debug.Log("ЗАБРАЛ ВИЛД " + i + " - " + j);

                                              random = Random.Range(1, 9); //Вилд заменяется случайным элементом (не вилдом и не скаттером)

                                              ChangeElement(i, j, random);

                                        }
                                      }
                                  }
                              
                              break;
                          case 1://Желтое/ Вилды раскрываются влево.
                              Debug.Log("Вилды раскрываются влево");
                            for (int i = 0; i < 3; i++)
                                for (int j = 1; j < 5; j++)
                                {
                                    if (finalTypes[i,j] ==9)
                                    {
                                        if (finalTypes[i,j-1]!=9)
                                        {
                                            extendCoords.Add(i); extendCoords.Add(j - 1);
                                        }
                                    }
                                }


                            break;
                          case 2://Красное. Вилды раскрываются влево и вправо
                              Debug.Log("Вилды раскрываются влево и вправо");
                            for (int i = 0; i < 3; i++)
                                for (int j = 0; j < 5; j++)
                                {
                                    if (finalTypes[i,j] == 9)
                                    {

                                        switch (j)
                                        {
                                            case 0:
                                                if ((finalTypes[i, j + 1] != 9) && (finalTypes[i, j + 2] != 9))
                                                {
                                                    extendCoords.Add(i); extendCoords.Add(j + 1);
                                                }
                                                break;
                                            case 1:

                                                if (finalTypes[i, j - 1] != 9)
                                                    extendCoords.Add(i); extendCoords.Add(j - 1);

                                                if ((finalTypes[i, j + 1] != 9) && (finalTypes[i,j+2]!=9))
                                                    extendCoords.Add(i); extendCoords.Add(j + 1);

                                                break;
                                            case 2:
                                                if ((finalTypes[i, j - 1] != 9) && (finalTypes[i, j-2]!=9))
                                                    extendCoords.Add(i); extendCoords.Add(j - 1);

                                                if ((finalTypes[i, j + 1] != 9) && (finalTypes[i, j + 2] != 9))
                                                    extendCoords.Add(i); extendCoords.Add(j + 1);
                                                break;
                                            case 3:
                                                if ((finalTypes[i, j - 1] != 9) && (finalTypes[i, j - 2] != 9))
                                                    extendCoords.Add(i); extendCoords.Add(j - 1);

                                                if (finalTypes[i, j + 1] != 9)
                                                    extendCoords.Add(i); extendCoords.Add(j + 1);
                                                break;
                                            case 4:
                                                if ((finalTypes[i, j - 1] != 9) && (finalTypes[i, j - 2] != 9))
                                                {
                                                    extendCoords.Add(i); extendCoords.Add(j - 1);
                                                }
                                                break;
                                        }
                                    }
                                }

                            break;
                      }


                    
                    if (extendCoords.Count>0)
                    {
                        
                        for (int i = 0; i<extendCoords.Count-1; i+=2)
                        {
                            Debug.Log("Меняется элемент " + extendCoords[i] + " - " + extendCoords[i+1] );
                            ChangeElement(extendCoords[i], extendCoords[i + 1], 9);
                        }

                          extendCoords.Clear();


                        
                    }


                  }
                  else //Если скелет уже побежден
                  { 
                      Debug.Log("Ты его уже убил, что пристал :(");
                      

                      if (freeSpins[activeSlotID]==0)
                      {
                          regularChance = 85;
                          wildChance = 10;
                          specialSlider.transform.GetChild(1).GetComponent<Image>().color = Color.green;
                          specialSlider.GetComponent<Slider>().value = specialSlider.GetComponent<Slider>().maxValue;
                          specialState = 0; //Восстановление скелета

                      } 
                  }

                  
                break;
            default:
                break;
        }

        if (itWas)
            cycleCheck = true;
        else
        {
         //   Debug.Log("Не выполнены условия для фриспина");
            cycleCheck = false;

            if (greatElements.Count>0)
            foreach (GameObject obj in greatElements)
            {
                Destroy(obj);
            } 
        }

        typesProcessed = false;
        checkedSpecial = true;

       //  Debug.Log(finalTypes[0,0] + " " + finalTypes[0, 1] + " " + finalTypes[0, 2] + " " + finalTypes[0, 3] + " " + finalTypes[0, 4]);
        // Debug.Log(finalTypes[1, 0] + " " + finalTypes[1, 1] + " " + finalTypes[1, 2] + " " + finalTypes[1, 3] + " " + finalTypes[1, 4]);
       //  Debug.Log(finalTypes[2, 0] + " " + finalTypes[2, 1] + " " + finalTypes[2, 2] + " " + finalTypes[2, 3] + " " + finalTypes[2, 4]);

      //  Debug.Log(finalGrid[0, 0] + " " + finalGrid[0, 1] + " " + finalGrid[0, 2] + " " + finalGrid[0, 3] + " " + finalGrid[0, 4]);
       // Debug.Log(finalGrid[1, 0] + " " + finalGrid[1, 1] + " " + finalGrid[1, 2] + " " + finalGrid[1, 3] + " " + finalGrid[1, 4]);
      //  Debug.Log(finalGrid[2, 0] + " " + finalGrid[2, 1] + " " + finalGrid[2, 2] + " " + finalGrid[2, 3] + " " + finalGrid[2, 4]);

    }

    private void ChangeElement(int final_line, int final_column, int type)
    {
        if (finalGrid[final_line, final_column] < 10)
        {
            up_lines[final_column].GetChild(finalGrid[final_line, final_column]).GetComponent<Image>().sprite = slotElements[type-1];
            elementsType[0, final_column, finalGrid[final_line, final_column]] = type;
        }
        else
        {
            down_lines[final_column].GetChild(finalGrid[final_line, final_column] - 10).GetComponent<Image>().sprite = slotElements[type - 1];
            elementsType[1, final_column, finalGrid[final_line, final_column]-10] = type;
        }

        finalTypes[final_line, final_column] = type;

    }

    private int CheckPayoutLine(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4, int x5, int y5, int optLineNum = 0)
    {
        int winAmount = 0;
        int lineType = 0;
        string reward = null;
        int currentBet = PlayerManager.instance.GetBet();        

        if (finalTypes[x1, y1] == 9) // 1 - вилд
        {
            if (finalTypes[x2, y2] < 9) //2- не вилд
            {
                lineType = finalTypes[x2, y2];

                if ((finalTypes[x3, y3] == lineType) || (finalTypes[x3, y3] == 9)) //3 - подходящий
                {
                    if ((finalTypes[x4, y4] == lineType) || (finalTypes[x4, y4] == 9)) //4 - подходящий
                    {
                        if ((finalTypes[x5, y5] == lineType) || (finalTypes[x5, y5] == 9)) //5 - подходящий
                        {
                            //Награда за 5
                            reward += "Линия " + optLineNum + " 5 элементов ТИП " + lineType + "; ";
                            winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 3] * currentBet);
                            controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4, x5, y5);
                            GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                            newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                            spawnedWinLines.Add(newLine);
                        }
                        else
                        {
                            //Награда за 4
                            reward += "Линия " + optLineNum + " 4 элементa ТИП " + lineType + "; ";
                            winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 2] * currentBet);
                            controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4);
                            GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                            newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                            spawnedWinLines.Add(newLine);
                        }
                    }
                    else
                    {
                        //Награда за 3
                        reward += "Линия " + optLineNum + " 3 элементa ТИП " + lineType + "; ";
                        winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 1] * currentBet);
                        controlWinFrames(x1, y1, x2, y2, x3, y3);
                        GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                        newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                        spawnedWinLines.Add(newLine);
                    }
                }
                else
                {
                    if (lineType==8) //НАГРАДА за 2 ЭЛЕМЕНТА ВОСЬМОГО ТИПА
                    {
                        reward += "Линия " + optLineNum + " 2 элементa ТИП " + lineType + "; ";
                        winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1)] * currentBet);                      
                        controlWinFrames(x1, y1, x2, y2);
                        GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                        newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                        spawnedWinLines.Add(newLine);
                    }
                }
            }
            else
            {
                if (finalTypes[x2, y2] == 9)
                {
                    if (finalTypes[x3, y3] < 9) //3 - не вилд
                    {
                        lineType = finalTypes[x3, y3];

                        if ((finalTypes[x4, y4] == lineType) || (finalTypes[x4, y4] == 9)) //4 - подходящий
                        {
                            if ((finalTypes[x5, y5] == lineType) || (finalTypes[x5, y5] == 9)) //3 - подходящий
                            {
                                //Награда за 5
                                reward += "Линия " + optLineNum + " 5 элементов ТИП " + lineType + "; ";
                                winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 3] * currentBet);
                                controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4, x5, y5);
                                GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                                newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                                spawnedWinLines.Add(newLine);
                            }
                            else
                            {
                                //Награда за 4
                                reward += "Линия " + optLineNum + " 4 элементa ТИП " + lineType + "; ";
                                winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 2] * currentBet);
                                controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4);
                                GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                                newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                                spawnedWinLines.Add(newLine);
                            }
                        }
                        else
                        {
                            //Награда за 3
                            reward += "Линия " + optLineNum + " 3 элементa ТИП " + lineType + "; ";
                            winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 1] * currentBet);
                            controlWinFrames(x1, y1, x2, y2, x3, y3);
                            GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                            newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                            spawnedWinLines.Add(newLine);
                        }
                    }
                    else
                    {
                        if (finalTypes[x3, y3] == 9)
                        { 
                        if (finalTypes[x4, y4] < 9) //4 - не вилд
                        {
                            lineType = finalTypes[x4, y4];
                            if ((finalTypes[x5, y5] == lineType) || (finalTypes[x5, y5] == 9))
                            {
                                //Награда за 5
                                reward += "Линия " + optLineNum + " 5 элементов ТИП " + lineType + "; ";
                                winAmount += (int)(lineRewards[(activeSlotID*36) + 4*(lineType-1) +3] * currentBet);
                                controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4, x5, y5);
                                    GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                                    newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                                    spawnedWinLines.Add(newLine);
                                }
                            else
                            {
                                //Награда за 4
                                reward += "Линия " + optLineNum + " 4 элементa ТИП " + lineType + "; ";
                                    winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 2] * currentBet);
                                    controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4);
                                    GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                                    newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                                    spawnedWinLines.Add(newLine);
                                }

                        }
                        else
                        {
                                if (finalTypes[x4, y4] == 9)
                                {
                                    

                                    if (finalTypes[x5, y5] == 9)
                                    {//Награда за 5 вилдов
                                        lineType = 9;
                                        reward += "Линия " + optLineNum + " 5 элементов ТИП ВИЛДЫ; ";
                                        winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 3] * currentBet);
                                        controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4, x5, y5);
                                        GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                                        newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                                        spawnedWinLines.Add(newLine);
                                    }
                                    else
                                    {
                                        if (finalTypes[x5,y5] < 9) //Последний элемент не вилд
                                        {
                                            lineType = finalTypes[x5, y5];
                                            reward += "Линия " + optLineNum + " 5 элементов ТИП " + finalTypes[x5,y5];
                                            winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 3] * currentBet);
                                            controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4, x5, y5);
                                            GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                                            newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                                            spawnedWinLines.Add(newLine);
                                        }
                                    }

                                }
                        }
                        }
                }
                }
            }
        }
        else
        if (finalTypes[x1, y1] < 9) //1 не вилд
        {
            lineType = finalTypes[x1, y1];

            if ((finalTypes[x2, y2] == lineType) || (finalTypes[x2, y2] == 9)) //2 подходящий
            {
                if ((finalTypes[x3, y3] == lineType) || (finalTypes[x3, y3] == 9)) //3 подходящий
                {
                    if ((finalTypes[x4, y4] == lineType) || (finalTypes[x4, y4] == 9)) //4 подходящий
                    {
                        if ((finalTypes[x5, y5] == lineType) || (finalTypes[x5, y5] == 9)) //5 подходящий
                        {
                            //Награда за 5
                            reward += "Линия " + optLineNum + " 5 элементов ТИП " + lineType + "; ";
                            winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 3] * currentBet);
                            controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4, x5, y5);
                            GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                            newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                            spawnedWinLines.Add(newLine);
                        }
                        else
                        {
                            //Награда за 4
                            reward += "Линия " + optLineNum + " 4 элемента ТИП " + lineType + "; ";
                            winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 2] * currentBet);
                            controlWinFrames(x1, y1, x2, y2, x3, y3, x4, y4);
                            GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                            newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                            spawnedWinLines.Add(newLine);
                        }
                    }
                    else
                    {
                        //Награда за 3
                        reward += "Линия " + optLineNum + " 3 элемента ТИП " + lineType + "; ";
                        winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1) + 1] * currentBet);
                        controlWinFrames(x1, y1, x2, y2, x3, y3);
                        GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                        newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                        spawnedWinLines.Add(newLine);
                    }
                }
                else
                {
                    if (lineType ==8) //НАГРАДА за 2 ЭЛЕМЕНТА ВОСЬМОГО ТИПА
                    {
                        reward += "Линия " + optLineNum + " 2 элементa ТИП " + lineType + "; ";
                        winAmount += (int)(lineRewards[(activeSlotID * 36) + 4 * (lineType - 1)] * currentBet);
                        controlWinFrames(x1, y1, x2, y2);
                        GameObject newLine = Instantiate(winLinePrefab, winLinesParent.transform);
                        newLine.GetComponent<Image>().sprite = winLines[optLineNum - 1];
                        spawnedWinLines.Add(newLine);
                    }
                }
            }
        }
        if (reward!=null)
        Debug.Log(reward);
        return winAmount;
    }
    
    private void controlWinFrames(int x1, int y1, int x2, int y2, int x3=-1, int y3=-1, int x4=-1, int y4=-1, int x5=-1, int y5=-1) //х - линия, у - столбец
    {     
        
        if (finalGrid[x1, y1] < 10)
        {
            if (up_lines[0].GetChild(finalGrid[x1, y1]).childCount==0)
            { 
            GameObject newFrame = Instantiate(winFramePrefab, up_lines[0].GetChild(finalGrid[x1, y1]));
            winFrames.Add(newFrame);
            }
        }
        else
        {
            if (down_lines[0].GetChild(finalGrid[x1, y1] - 10).childCount == 0)
            {
                GameObject newFrame = Instantiate(winFramePrefab, down_lines[0].GetChild(finalGrid[x1, y1] - 10));
                winFrames.Add(newFrame);
            }
        }

        if (finalGrid[x2, y2] < 10)
        {
            if (up_lines[1].GetChild(finalGrid[x2, y2]).childCount == 0)
            {
                GameObject newFrame = Instantiate(winFramePrefab, up_lines[1].GetChild(finalGrid[x2, y2]));
                winFrames.Add(newFrame);
            }
        }
        else
        {
            if (down_lines[1].GetChild(finalGrid[x2, y2] - 10).childCount == 0)
            {
                GameObject newFrame = Instantiate(winFramePrefab, down_lines[1].GetChild(finalGrid[x2, y2] - 10));
                winFrames.Add(newFrame);
            }
        }
        if (y3 != -1)
        {
            if (finalGrid[x3, y3] < 10)
            {
                if (up_lines[2].GetChild(finalGrid[x3, y3]).childCount == 0)
                {
                    GameObject newFrame = Instantiate(winFramePrefab, up_lines[2].GetChild(finalGrid[x3, y3]));
                    winFrames.Add(newFrame);
                }
            }
            else
            {
                if (down_lines[2].GetChild(finalGrid[x3, y3] - 10).childCount == 0)
                {
                    GameObject newFrame = Instantiate(winFramePrefab, down_lines[2].GetChild(finalGrid[x3, y3] - 10));
                    winFrames.Add(newFrame);
                }
            }

            if (y4 != -1)
            {

                if (finalGrid[x4, y4] < 10)
                {
                    if (up_lines[3].GetChild(finalGrid[x4, y4]).childCount == 0)
                    {
                        GameObject newFrame = Instantiate(winFramePrefab, up_lines[3].GetChild(finalGrid[x4, y4]));
                        winFrames.Add(newFrame);
                    }
                }
                else
                {
                    if (down_lines[3].GetChild(finalGrid[x4, y4] - 10).childCount == 0)
                    {
                        GameObject newFrame = Instantiate(winFramePrefab, down_lines[3].GetChild(finalGrid[x4, y4] - 10));
                        winFrames.Add(newFrame);
                    }
                }



                if (y5 != -1)
                {
                    if (finalGrid[x5, y5] < 10)
                    {
                        if (up_lines[4].GetChild(finalGrid[x5, y5]).childCount == 0)
                        {
                            GameObject newFrame = Instantiate(winFramePrefab, up_lines[4].GetChild(finalGrid[x5, y5]));
                            winFrames.Add(newFrame);
                        }
                    }
                    else
                    {
                        if (down_lines[4].GetChild(finalGrid[x5, y5] - 10).childCount == 0)
                        {
                            GameObject newFrame = Instantiate(winFramePrefab, down_lines[4].GetChild(finalGrid[x5, y5] - 10));
                            winFrames.Add(newFrame);
                        }
                    }
                }
            }
        }
    }

    private void ProcessTypes()
    {

        if (finalGrid[1, 0] == -1)
        {
            for (int i = 0; i < 5; i++)
            {
                finalGrid[1, i] = lastElementsID[i];

                if ((finalGrid[1, i] >= 0) && (finalGrid[1, i] <= 9))
                {
                    finalTypes[1, i] = elementsType[0, i, finalGrid[1, i]];
                    if (finalTypes[1, i] == 10)
                        scatterCount++;
                }
                else
                {
                    finalTypes[1, i] = elementsType[1, i, finalGrid[1, i] - 10];
                    if (finalTypes[1, i] == 10)
                        scatterCount++;
                }

                if ((finalGrid[1, i] > 0) && (finalGrid[1, i] < 19)) //Средний элемент это не крайний элемент линии
                {
                    finalGrid[0, i] = finalGrid[1, i] - 1;
                    finalGrid[2, i] = finalGrid[1, i] + 1;

                    if ((finalGrid[0, i] >= 0) && (finalGrid[0, i] <= 9))
                    { 
                        finalTypes[0, i] = elementsType[0, i, finalGrid[0, i]];
                        if (finalTypes[0, i] == 10)
                            scatterCount++;
                    }
                    else
                    {
                        finalTypes[0, i] = elementsType[1, i, finalGrid[0, i] - 10];
                        if (finalTypes[0, i] == 10)
                            scatterCount++;
                    }

                    if ((finalGrid[2, i] >= 0) && (finalGrid[2, i] <= 9))
                    {
                        finalTypes[2, i] = elementsType[0, i, finalGrid[2, i]];
                        if (finalTypes[2, i] == 10)
                            scatterCount++;
                    }
                    else
                    {
                        finalTypes[2, i] = elementsType[1, i, finalGrid[2, i] - 10];
                        if (finalTypes[2, i] == 10)
                            scatterCount++;
                    }
                }
                else //Средний элемент это крайний элемент линии
                {
                    switch (finalGrid[1, i])
                    {
                        case 0:
                            finalGrid[0, i] = 19;                          
                            finalGrid[2, i] = finalGrid[1, i] + 1;

                            finalTypes[0, i] = elementsType[1, i, 9];
                            finalTypes[2, i] = elementsType[0, i, 1];

                            if (finalTypes[0, i] == 10)
                                scatterCount++;

                            if (finalTypes[2, i] == 10)
                                scatterCount++;
                            break;
                        case 19:
                            finalGrid[0, i] = finalGrid[1, i] - 1;
                            finalGrid[2, i] = 0;

                            finalTypes[0, i] = elementsType[1, i, 8];
                            finalTypes[2, i] = elementsType[0, i, 0];

                            if (finalTypes[0, i] == 10)
                                scatterCount++;

                            if (finalTypes[2, i] == 10)
                                scatterCount++;
                            break;
                        default:
                            Debug.Log("THEFUCK " + finalGrid[1, i]);
                            break;

                    }
                }


            }
        } //Узнать какие ячейки установились

        spinned = false;
        typesProcessed = true;


    }

    private void CheckPayLines()
    {


        int winAmount = 0;

        switch (activeSlotID)
        {
            case 0: // 9 lines
                winAmount += CheckPayoutLine(0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 2);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 3);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 2, 2, 1, 3, 0, 4, 4);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 0, 2, 1, 3, 2, 4, 5);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 0, 2, 0, 3, 1, 4, 6);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 2, 2, 2, 3, 1, 4, 7);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 1, 2, 2, 3, 2, 4, 8);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 1, 2, 0, 3, 0, 4, 9);
                break;
            case 1: //12 lines
                winAmount += CheckPayoutLine(0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 2);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 3);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 2, 2, 1, 3, 0, 4, 4);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 0, 2, 1, 3, 2, 4, 5);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 0, 2, 0, 3, 1, 4, 6);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 2, 2, 2, 3, 1, 4, 7);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 1, 2, 2, 3, 2, 4, 8);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 1, 2, 0, 3, 0, 4, 9);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 1, 2, 2, 3, 1, 4, 10);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 1, 2, 0, 3, 1, 4, 11);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 1, 2, 1, 3, 0, 4, 12);


                break;
            case 2: //25 lines
                winAmount += CheckPayoutLine(0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 2);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 3);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 2, 2, 1, 3, 0, 4, 4);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 0, 2, 1, 3, 2, 4, 5);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 0, 2, 0, 3, 1, 4, 6);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 2, 2, 2, 3, 1, 4, 7);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 1, 2, 2, 3, 2, 4, 8);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 1, 2, 0, 3, 0, 4, 9);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 1, 2, 2, 3, 1, 4, 10);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 1, 2, 0, 3, 1, 4, 11);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 1, 2, 1, 3, 0, 4, 12);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 1, 2, 1, 3, 2, 4, 13);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 0, 2, 1, 3, 0, 4, 14);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 2, 2, 1, 3, 2, 4, 15);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 0, 2, 1, 3, 1, 4, 16);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 2, 2, 1, 3, 1, 4, 17);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 2, 2, 0, 3, 0, 4, 18);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 0, 2, 2, 3, 2, 4, 19);
                winAmount += CheckPayoutLine(0, 0, 2, 1, 2, 2, 2, 3, 0, 4, 20);
                winAmount += CheckPayoutLine(2, 0, 0, 1, 0, 2, 0, 3, 2, 4, 21);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 2, 2, 0, 3, 1, 4, 22);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 0, 2, 2, 3, 1, 4, 23);
                winAmount += CheckPayoutLine(0, 0, 2, 1, 0, 2, 2, 3, 0, 4, 24);
                winAmount += CheckPayoutLine(2, 0, 0, 1, 2, 2, 0, 3, 2, 4, 25);

                break;
            case 3: //50 lines
                winAmount += CheckPayoutLine(0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 2);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 3);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 2, 2, 1, 3, 0, 4, 4);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 0, 2, 1, 3, 2, 4, 5);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 0, 2, 0, 3, 1, 4, 6);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 2, 2, 2, 3, 1, 4, 7);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 1, 2, 2, 3, 2, 4, 8);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 1, 2, 0, 3, 0, 4, 9);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 1, 2, 2, 3, 1, 4, 10);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 1, 2, 0, 3, 1, 4, 11);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 1, 2, 1, 3, 0, 4, 12);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 1, 2, 1, 3, 2, 4, 13);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 0, 2, 1, 3, 0, 4, 14);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 2, 2, 1, 3, 2, 4, 15);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 0, 2, 1, 3, 1, 4, 16);
                winAmount += CheckPayoutLine(1, 0, 1, 1, 2, 2, 1, 3, 1, 4, 17);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 2, 2, 0, 3, 0, 4, 18);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 0, 2, 2, 3, 2, 4, 19);
                winAmount += CheckPayoutLine(0, 0, 2, 1, 2, 2, 2, 3, 0, 4, 20);
                winAmount += CheckPayoutLine(2, 0, 0, 1, 0, 2, 0, 3, 2, 4, 21);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 2, 2, 0, 3, 1, 4, 22);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 0, 2, 2, 3, 1, 4, 23);
                winAmount += CheckPayoutLine(0, 0, 2, 1, 0, 2, 2, 3, 0, 4, 24);
                winAmount += CheckPayoutLine(2, 0, 0, 1, 2, 2, 0, 3, 2, 4, 25);
                winAmount += CheckPayoutLine(0, 0, 2, 1, 1, 2, 0, 3, 2, 4, 26);
                winAmount += CheckPayoutLine(2, 0, 0, 1, 1, 2, 2, 3, 0, 4, 27);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 2, 2, 1, 3, 2, 4, 28);
                winAmount += CheckPayoutLine(0, 0, 2, 1, 1, 2, 2, 3, 0, 4, 29);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 0, 2, 0, 3, 1, 4, 30);
                winAmount += CheckPayoutLine(0, 1, 1, 1, 2, 2, 2, 3, 1, 4, 31);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 1, 2, 0, 3, 1, 4, 32);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 1, 2, 2, 3, 1, 4, 33);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 0, 2, 1, 3, 2, 4, 34);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 2, 2, 0, 3, 0, 4, 35);
                winAmount += CheckPayoutLine(2, 0, 0, 1, 0, 2, 1, 3, 2, 4, 36);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 2, 2, 0, 3, 0, 4, 37);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 1, 2, 1, 3, 2, 4, 38);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 0, 2, 1, 3, 0, 4, 39);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 2, 2, 2, 3, 2, 4, 40);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 0, 2, 0, 3, 0, 4, 41);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 0, 2, 1, 3, 2, 4, 42);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 2, 2, 1, 3, 0, 4, 43);
                winAmount += CheckPayoutLine(0, 0, 1, 1, 2, 2, 1, 3, 1, 4, 44);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 0, 2, 2, 3, 1, 4, 45);
                winAmount += CheckPayoutLine(1, 0, 0, 1, 1, 2, 2, 3, 2, 4, 46);
                winAmount += CheckPayoutLine(1, 0, 2, 1, 1, 2, 0, 3, 0, 4, 47);
                winAmount += CheckPayoutLine(2, 0, 2, 1, 1, 2, 2, 3, 2, 4, 48);
                winAmount += CheckPayoutLine(0, 0, 0, 1, 1, 2, 0, 3, 0, 4, 49);
                winAmount += CheckPayoutLine(2, 0, 1, 1, 2, 2, 0, 3, 1, 4, 50);

                break;


            default:
                Debug.Log("What are you up to? There is no game with this ID");
                //Нет такого слотика :(
                break;
        }

        if (winAmount > 0)
        {
            SoundManager.instance.winSound.Play();
            PlayerManager.instance.ChangeCoins(winAmount);
            PlayerManager.instance.ChangeXP((PlayerManager.instance.GetBet() / PlayerManager.instance.GetLevel()) + (winAmount / PlayerManager.instance.GetBet())); //Получение опыта игроком
            winText.text = winAmount.ToString();

            if (spawnedWinLines.Count>4)
            {
                Debug.Log("МНОГО ЛИНИЙ");

                for (int i = 0; i < spawnedWinLines.Count; i++)
                {
                       showPayLines.Add(StartCoroutine(ShowLotsOfWinLines(i)));

                }
              

            }

        }
        else
            winText.text = "Next time mb";


       
        checkedSpecial = false;

        if (cycleCheck)
            specReward += winAmount;


        ToggleButtons();
    }

    public void ChangeBet(int var)
    {
        if (var==1)
        {
            switch(activeSlotID)
            {
                case 0:
                case 1:                   
                case 2:
                case 3:
                    if (PlayerManager.instance.GetBet() < regularBets[PlayerManager.instance.GetLevel()+2])
                    {
                        betId++;
                        PlayerManager.instance.SetBet(regularBets[betId]);

                        if (PlayerManager.instance.GetBet() == regularBets[PlayerManager.instance.GetLevel()+2])
                            lockIcon.enabled = true;
                    }

                    break;
                default:
                    break;
            }
                
        }
        else
        {
            if (betId > 0)
            {
                switch (activeSlotID)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        if (PlayerManager.instance.GetBet() > 0)
                        {
                            betId--;
                            PlayerManager.instance.SetBet(regularBets[betId]);
                            lockIcon.enabled = false;
                        }

                        break;
                    default:
                        break;
                }
            }
        }

    }


    public void ChangeMainSound(float value)
    {

    }

    public void ChangeMusic(float value)
    {

    }

    public void ChangeSoundEffects(float value)
    {

    }


    public void CollectReward()
    {
        if (!PlayerManager.instance.receivedDaily)
        {
            moveRewardList = true;
            PlayerManager.instance.receivedDaily = true;

        }
    }

    private void MoveReward()  //   0.4583483 -> 1.800112 
    {

        if (rewardsList.transform.position.y < 1.75f)
            rewardsList.transform.position += transform.up * Time.deltaTime * 2f;
        else
        {
            rewardsList.transform.position = new Vector2(rewardsList.transform.position.x, 1.800112f);
        }

    }

    public void ToggleButtons()
    {
        bool value = !backbttn.IsInteractable();
    backbttn.interactable = value;
    spinBttn.interactable = value;
    profileBttn.interactable = value;
    plusBetBttn.interactable = value;
    minBetBttn.interactable = value;
    }

    public void ChangePlayerIcon(int id)
    {
        PlayerManager.instance.ChangePlayerIcon(id);
    }

    public void ChangePlayerName(string value)
    {
        PlayerManager.instance.ChangePlayerName(value);
    }

    private IEnumerator StartWWWRequest()
    {

        //ID игрока в гугл плее  PlayGamesPlatform.Instance.localUser.id

        yield return null;
       // Debug.Log("Device name = " + SystemInfo.deviceName);
       // Debug.Log("Device model = " + SystemInfo.deviceModel);
       // Debug.Log("Device unique id = " + SystemInfo.deviceUniqueIdentifier);



        /*
        WWW Query = new WWW("http://cheeseru.pythonanywhere.com/epta");
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            if (Query.text == "response") // что нам должен ответить сервер на наши данные
            {
                Debug.Log("Server responded correctly");
            }
            else
            {
                Debug.Log("Server responded : " + Query.text);
            }
        }
        Query.Dispose();
    */
    }

    private IEnumerator ShowLotsOfWinLines(int index)
    {     
        yield return new WaitForSecondsRealtime(1f * index);
        if (!hasYetToStart)
        {
            spawnedWinLines[index].transform.SetAsLastSibling();
            spawnedWinLines[index].GetComponent<Animator>().SetTrigger("startFading");
        }


        
    }

    private IEnumerator LoadSlot()
    {
        var resourceRequest = new ResourceRequest();
        //DataManager.instance.LoadSounds(activeSlotID);
        
        for (int i = 0; i < 10; i++)
        {
            resourceRequest = Resources.LoadAsync<Sprite>("Sprites/Elements/" + activeSlotID + "/" + (i + 1));
            yield return null;
            slotElements[i] = resourceRequest.asset as Sprite;
        }

        resourceRequest = Resources.LoadAsync<Sprite>("Sprites/UI/" + activeSlotID + "/" + "frame");
        yield return null;
        winFramePrefab.GetComponent<Image>().sprite = resourceRequest.asset as Sprite;

        resourceRequest = Resources.LoadAsync<Sprite>("Sprites/UI/" + activeSlotID + "/" + "icon");
        yield return null;
        slotLogo.sprite = resourceRequest.asset as Sprite;

        resourceRequest = Resources.LoadAsync<Sprite>("Sprites/Elements/" + activeSlotID + "/great9");
        yield return null;
        slotEffects[2].GetComponent<Image>().sprite = resourceRequest.asset as Sprite;


        if (activeSlotID == 3) //Слот с убийством скелета, который крадет вилды
        {
            GameObject special = transform.GetChild(specialIndex).GetChild(0).gameObject;
            //Подгрузить ХП 
            special.SetActive(true); //Включить объект SpecialSomething

            resourceRequest = Resources.LoadAsync<Sprite>("Sprites/UI/" + activeSlotID + "/" + "icon");
            yield return null;
            special.transform.GetChild(0).GetComponent<Image>().sprite = resourceRequest.asset as Sprite;

            specialSlider = special.transform.GetChild(1).gameObject;
            specialSlider.GetComponent<Slider>().value = specialSlider.GetComponent<Slider>().maxValue;
            specialSlider.transform.GetChild(1).GetComponent<Image>().color = Color.green;
            specialState = 0; //full hp
        }
        else
        if (activeSlotID == 2) //Слот с колесом фортуны
        {

            GameObject special = transform.GetChild(specialIndex).GetChild(1).gameObject; //Объект WheelOfFortune         
                                                                                          //  special.SetActive(true); //Для настройки колеса. ВРЕМЕНННО

            if (FortuneWheelManager.instance == null)
            {
                Instantiate(wheel_manager, special.transform.parent);
            }


        }

        resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/backgroundClip");
        yield return null;
        SoundManager.instance.slotBackClip = resourceRequest.asset as AudioClip;

        resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/rollClip");
        yield return null;
        SoundManager.instance.rollClip = resourceRequest.asset as AudioClip;

        resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/megaWinClip");
        yield return null;
        SoundManager.instance.megaWinClip = resourceRequest.asset as AudioClip;

        switch (activeSlotID)
        {
            case 2: //Слот с колесом

                resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/wheelRollClip");
                yield return null;
                SoundManager.instance.specialClips.Add(resourceRequest.asset as AudioClip);

                resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/rewardClip");
                yield return null;
                SoundManager.instance.specialClips.Add(resourceRequest.asset as AudioClip);

                break;
            case 3: //Слот с минькой про скелета

                resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/wildStolenClip");
                yield return null;
                SoundManager.instance.specialClips.Add(resourceRequest.asset as AudioClip);

                resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/damageDoneClip");
                yield return null;
                SoundManager.instance.specialClips.Add(resourceRequest.asset as AudioClip);

                resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/killedClip");
                yield return null;
                SoundManager.instance.specialClips.Add(resourceRequest.asset as AudioClip);

                resourceRequest = Resources.LoadAsync<AudioClip>("Sound/" + activeSlotID + "/resurrectionClip");
                yield return null;
                SoundManager.instance.specialClips.Add(resourceRequest.asset as AudioClip);

                break;
        }



        readyToStartBttn.transform.parent.GetChild(2).gameObject.SetActive(false) ;
        readyToStartBttn.transform.GetChild(0).GetComponent<Text>().text = "Press to enter";
        readyToStartBttn.interactable = true;
        CallGenerateSlotWithButton(0);
    }

}
