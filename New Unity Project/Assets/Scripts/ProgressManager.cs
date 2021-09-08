using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Touch;

using Facebook.Unity;


public class ProgressManager : MonoBehaviour
{

    public GameObject currentLevelObj;

    public List<GameObject[]> levels;

    private int currentLevel;
    private int currentSector;
    private float levelRotationAngle;

    private Vector3 zAxis;
    private int lastLevel;
    //private int[] levelStars;

    public GameObject wonSubmenu;
    public List<GameObject> wonStars;
    public TMP_Text wonSmashes;

    public Button wonNextBttn;
    public GameObject wonNeedmore;

    public TMP_Text loseSmashes;
    public LeanTouch leanTouch;
    public GameObject lostSubmenu;


    public List<Sprite> starSprites;
    public BreakingTool pickaxe;

    bool randomPlay = false;


    //below//29.06.2021 things

    private int totalCoins; //Сколько всего монет У ИГРОКА
    private int totalGems;  //Сколько всего кристаллов У ИГРОКА
    private int totalStars; //Сколько всего звезд У ИГРОКА
    
    private int lastSector; //Сектор, доступный игроку
    //private int lastSecLevel; //Уровень в секторе, доступный игроку


    private int[] sectorReqs = {0,7,20, 30};

    private List<int[]> secLevelStars; //Звезды на каждом уровне в каждом секторе


    public TMP_Text wonCoins;
    public TMP_Text loseCoins;

    private string regDay;

    [SerializeField]
    private ShopTrail trails;


    [SerializeField]
    private GameObject[] sector0Levels;

    Amplitude amplitude;

    private void Awake() {

         if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
            //Handle FB.Init
            FB.Init( () => {
            FB.ActivateApp();
            });
        }

        amplitude = Amplitude.getInstance();
        amplitude.logging = true;
        amplitude.trackSessionEvents(true);
        amplitude.init("56446dc30049e3da9b13c0b4b853b9a0"); 

        amplitude.setUserProperty("session_id", amplitude.getSessionId());
        Debug.Log("Айди сессии: " + amplitude.getSessionId());

        if (PlayerPrefs.HasKey("firstTime"))
        {
            regDay = PlayerPrefs.GetString("firstTime");
        }
        else
        {
            regDay = System.DateTime.Now.ToString("dd.MM.yy");
            PlayerPrefs.SetString("firstTime", regDay);
            amplitude.setOnceUserProperty("reg_day", regDay);
           
        }

        System.DateTime firstDay = System.DateTime.Parse(regDay);

        int daysPassed = (int)System.DateTime.Now.Subtract(firstDay).TotalDays;
        
        amplitude.setUserProperty("days_after", daysPassed);
        
    }
    

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastLevel = 0;
            lastSector = 0;

            secLevelStars[0][0] = 0;
            secLevelStars[0][1] = 0;
            secLevelStars[0][2] = 0;
            secLevelStars[0][3] = 0;
            secLevelStars[0][4] = 0;
            secLevelStars[0][5] = 0;

            /*
            secLevelStars[1][0] = 0;
            secLevelStars[1][1] = 0;
            secLevelStars[1][2] = 0;
            secLevelStars[1][3] = 0;
            secLevelStars[1][4] = 0;

            secLevelStars[2][0] = 0;
            secLevelStars[2][1] = 0;
            secLevelStars[2][2] = 0;
            secLevelStars[2][3] = 0;
            secLevelStars[2][4] = 0;

            secLevelStars[3][0] = 0;
            secLevelStars[3][1] = 0;
            secLevelStars[3][2] = 0;
            secLevelStars[3][3] = 0;
            secLevelStars[3][4] = 0; */
        }
    }

    
    public int GetCoins()
    {
        return totalCoins;
    }

    public void ChangeCoins(int value)
    {
        totalCoins+=value;
    }

    public int GetGems()
    {
        return totalGems;
    }

    public int GetStars()
    {
        int stars = 0;
        for (int i =0 ; i< secLevelStars.Count; i++)
        {
            for (int j =0 ; j<secLevelStars[i].Length;j++)
                stars+=secLevelStars[i][j];
        }
        return stars;
    }

 
    private void Start() {     

        if (PlayerPrefs.HasKey("lastlevel"))
            lastLevel = PlayerPrefs.GetInt("lastlevel");
        else
            lastLevel = 0;

        if (PlayerPrefs.HasKey("lastSector"))
            lastSector = PlayerPrefs.GetInt("lastSector");
        else
            lastSector = 0;

        if (PlayerPrefs.HasKey("totalStars"))
            totalStars = PlayerPrefs.GetInt("totalStars");
            
        if (PlayerPrefs.HasKey("totalCoins"))
            totalCoins = PlayerPrefs.GetInt("totalCoins");


        if (PlayerPrefs.HasKey("totalGems"))
            totalGems = PlayerPrefs.GetInt("totalGems");


        levels = new List<GameObject[]>();

          //levels.Add(Resources.LoadAll<GameObject>("Prefabs/Levels/New3D"));
        levels.Add(sector0Levels);

        secLevelStars = new List<int[]>();
        secLevelStars.Add(new int[6] {0, 0, 0, 0 ,0 ,0});
        

        if (PlayerPrefs.HasKey("levelstars"))
        {
            string starsString = PlayerPrefs.GetString("levelstars");


            string[] sectorsStars = starsString.Split(';');

            if (sectorsStars.Length == levels.Count)
            {

                char[] starInfo;
                for (int i = 0; i< sectorsStars.Length;i++)
                {

                    starInfo = sectorsStars[i].ToCharArray();
                    
                    for (int j =0; j< starInfo.Length; j++)
                    {
                        int.TryParse("" + starInfo[j], out secLevelStars[i][j]);
                    }
                }
            }
        }

        if (PlayerPrefs.HasKey("equipTrail"))
        {
            trails.DeequipEverything();
            trails.trailButtons[PlayerPrefs.GetInt("equipTrail")].equipped = true;
        }

        if (PlayerPrefs.HasKey("boughtTrail"))
        {
            char[] boughtInfo = PlayerPrefs.GetString("boughtTrail").ToCharArray();
            for (int i =0; i<boughtInfo.Length;i++)
            {
                if (boughtInfo[i] == 't')
                {
                    trails.trailButtons[i].bought = true;
                }
                else if (boughtInfo[i] == 'f')
                {
                    trails.trailButtons[i].bought = false;
                }
            }
        }


        zAxis = new Vector3(0,0,1);

        pickaxe.TouchWork(false);
        
        amplitude.logEvent("game_start");
        amplitude.setUserProperty("current_soft", totalCoins);
    }

    public int GetLevelCount()
    {
        return levels.Count;
    }

    public int GetSectorValue(int sectorIndex)
    {
        return levels[sectorIndex].Length;
    }

    public void ResetLevel()
    {
        if (currentLevelObj!=null)
            {Destroy(currentLevelObj);
            Stone[] rocks = FindObjectsOfType<Stone>();
            foreach(var rock in rocks)
            {
                Destroy(rock.gameObject);
            }
            }
            pickaxe.smashCount = 0;
            currentLevelObj = null;
    }

    public int GetLastLevel()
    {
        return lastLevel;
    }

    public int GetLastSector()
    {
        return lastSector;
    }

    public int GetProgress(int sectorIndex,int levelIndex)
    {  
        return secLevelStars[sectorIndex][levelIndex];
        //return levelStars[levelIndex];
    }

    public void MenuPlay()
    {

        
        if (lastSector > levels.Count-1)
            randomPlay = true;
        else randomPlay = false;

        if (randomPlay)
        {
            int prevSector = currentSector;
            int prevLevel = currentLevel;

            if (levels[currentSector].Length > 1)
            {
                while ((prevSector == currentSector) && (prevLevel == currentLevel))
                {
                    currentSector = Random.Range(0, levels.Count);
                    currentLevel = Random.Range(0,levels[currentSector].Length);
                }
            }

            Debug.Log("Задано: lastsector=" + lastSector + ", lastLevel=" + lastLevel + " , currentSector=" + currentSector + " , currentLevel=" + currentLevel);

            levelRotationAngle = Random.Range(0,360);
            Replay();
            return;
        }

        ResetLevel();

        currentSector = lastSector;
        currentLevel = lastLevel;

        Debug.Log("Сектор: " + lastSector + " уровень: " + lastLevel);
        currentLevelObj = Instantiate(levels[currentSector][currentLevel]);
        levelRotationAngle = 0;
        

       amplitude.logEvent("level_start");
       amplitude.setUserProperty("level_last", GetLevelNumber());
    }

    public int GetLevelNumber()
    {
        int number = 1 + currentLevel;

        for (int i =0; i<currentSector; i++)
        {
            number+=secLevelStars[i].Length;
        }

        return number;
    }

    public int GetSectorReq(int sectorIndex)
    {
        return sectorReqs[sectorIndex];
    }

    public void Replay()
    {
        ResetLevel();
        currentLevelObj = Instantiate(levels[currentSector][currentLevel]);
        
        currentLevelObj.transform.RotateAround(currentLevelObj.transform.position, zAxis, levelRotationAngle * 50); 

        amplitude.logEvent("level_start");
    }

    public void SetLevelStars(int sector,int level, int stars)
    {
        //levelStars[level] = stars;
        secLevelStars[sector][level] = stars;
    }

    public void SetCurrentSector(int number)
    {
        currentSector = number;
    }

    public void SetCurrentLevel(int number)
    {
        currentLevel = number;
    }


    public void PlayerToMainMenu()
    {
        amplitude.logEvent("main_menu");
    }


    public void PlayerWon(int cracksMade) 
    {
        if (wonSubmenu.activeSelf) return;

        amplitude.logEvent("level_win");

        //Определить количество звезд, исходя из количества линий
        leanTouch.RecordFingers = false;
        int starsResult;
        if (cracksMade < 11)
            starsResult = 3;
        else if (cracksMade < 21)
            starsResult = 2;
        else
            starsResult = 1;
        
      
        if (secLevelStars[currentSector][currentLevel] < starsResult)
            secLevelStars[currentSector][currentLevel] = starsResult;
        



        if ((currentLevel == lastLevel) && (currentSector == lastSector))
        {
            if (lastLevel+1 <= levels[currentSector].Length-1) //Если это был не последний уровень в секторе
            {
                lastLevel+=1;
                wonNextBttn.interactable = true;
                wonNeedmore.SetActive(false);
            }
            else //Если это последний уровень в секторе
            {

                

                if (currentSector < levels.Count-1) //Не самый последний сектор в игре
                {
                    if (GetStars() >= sectorReqs[lastSector+1])
                    {
                        wonNextBttn.interactable = true;
                        wonNeedmore.SetActive(false);
                        lastSector+=1;
                        lastLevel = 0;
                    }
                    else
                    {
                        wonNeedmore.SetActive(true);
                        wonNextBttn.interactable = false;
                    }
                }
                else
                {
                    lastSector +=1;
                    lastLevel = 0;

                    Debug.Log("Самый последний сектор и уровень в игре. Дальше рандом.");

                    
                    randomPlay = true;
                    
                }
            }
        }

           

        StartCoroutine(OpenWonWindowInATime());

        if (starsResult == 2)
        {
            wonStars[2].SetActive(false);
            wonStars[1].SetActive(true);
        }
        else if (starsResult == 1)
        {
            wonStars[2].SetActive(false);
            wonStars[1].SetActive(false);
        }
        else{
            wonStars[2].SetActive(true);
            wonStars[1].SetActive(true);
        }

        
        wonSmashes.text = pickaxe.smashCount.ToString();

        int coinsWon =  2 * currentSector + currentLevel + 5 * starsResult;
        wonCoins.text = coinsWon.ToString();
        totalCoins+= coinsWon;

        totalGems +=1;
        
    }
    private IEnumerator OpenWonWindowInATime()
    {
        yield return new WaitForSecondsRealtime(0.7f);
         wonSubmenu.SetActive(true);
    }

/*
    public void PlayerLost()
    {
        leanTouch.RecordFingers = false;

        int smashes = pickaxe.smashCount;
        loseSmashes.text = smashes.ToString();

        int coinsGot = 0;
        if (smashes >= 20) //Не за один удар же давать деньги
        {
            coinsGot =  2 * currentSector + currentLevel;
            
            totalCoins+= coinsGot;
        }
        


        loseCoins.text = coinsGot.ToString();
        StartCoroutine(OpenLostWindowInATime());
       
        
    }

    private IEnumerator OpenLostWindowInATime()
    {
        yield return new WaitForSecondsRealtime(0.7f);
         lostSubmenu.SetActive(true);
    }
*/
    public void ResetGameProgress()
    {
        lastSector = 0;
        lastLevel = 0;
        currentLevel = 0;
        currentSector = 0;
    
        for (int i =0; i< secLevelStars.Count; i++)
            System.Array.Clear(secLevelStars[i],0, secLevelStars[i].Length);

 
    }

    private void OnApplicationPause(bool pauseStatus) {
         if (pauseStatus)
        {
        string levelStarsToSave ="";


        for (int i =0; i<secLevelStars.Count; i++)
        {
            for (int j = 0; j < secLevelStars[i].Length; j++)
                {
                levelStarsToSave+=secLevelStars[i][j];
                if ((j== secLevelStars[i].Length-1) && (i != secLevelStars.Count-1))
                {
                    levelStarsToSave+=";";
                }
                }
        }


        PlayerPrefs.SetString("levelstars", levelStarsToSave);
        PlayerPrefs.SetInt("lastlevel", lastLevel);;
        PlayerPrefs.SetInt("lastSector", lastSector);

        PlayerPrefs.SetInt("totalStars", totalStars);
        PlayerPrefs.SetInt("totalCoins", totalCoins);
        PlayerPrefs.SetInt("totalGems", totalGems);

        string boughtTrails ="";
        int equippedTrail = 0;
        for (int i = 0; i<trails.trailButtons.Count;i++)
        {
            if (trails.trailButtons[i].bought)
                boughtTrails+="t";
            else
                boughtTrails+="f";

            if (trails.trailButtons[i].equipped)
                equippedTrail = i;
        }

        PlayerPrefs.SetString("boughtTrail", boughtTrails);
        PlayerPrefs.SetInt("equipTrail", equippedTrail);


        }

        if (!pauseStatus)
        {
            //app resume
        if (FB.IsInitialized) {
        FB.ActivateApp();
        } else {
        //Handle FB.Init
        FB.Init( () => {
            FB.ActivateApp();
        });
        }
        }
    }
}



