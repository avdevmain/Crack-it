using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LevelMenu : MonoBehaviour
{
   public ProgressManager progress;


    public Button leftArrow;
    public Button rightArrow;

    public int lastSector;
    public int currSector;
    public int curLevel;

    private int totalSectors;//Сколько всего секторов

    private int[] sectorsValue; //Сколько уровней в каждом секторе
    public TMP_Text sectornum;

    [SerializeField]
    private TMP_Text totalStarsText;


    [SerializeField]
    private Transform stagesParent;

    public GameObject lockedLevel;
    public GameObject openedLevel;
    public GameObject finishedLevel;

    public GameObject sectorLocked;
    public TMP_Text sectorReq;

    public Button startButton;

    private int stars;
   private void OnEnable() {


    

    if (totalSectors == 0) //если параметры не были заданы до этого
    {
        totalSectors = progress.GetLevelCount(); //сколько всего секторов
        sectorsValue = new int[totalSectors];
        for (int i =0; i<totalSectors;i++)
        {
            sectorsValue[i] = progress.GetSectorValue(i); //Количество уровней в каждом секторе
        }
    }

    stars = progress.GetStars(); //Сколько всего звезд игрок набрал
    totalStarsText.text = stars.ToString();
    lastSector = progress.GetLastSector();
    if (lastSector > totalSectors)
        lastSector-=1; //В случае если игрок перешел в бесконечную игру

    currSector = lastSector; //По умолчанию открывать последний сектор
    curLevel = progress.GetLastLevel();



    SetLevels();
    

   }

    private void ClearLevels()
    {
        foreach(Transform child in stagesParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CheckBorders()
    {
        if (currSector==0)
        leftArrow.interactable = false;
        else
            leftArrow.interactable = true;
        
        if (currSector==totalSectors-1)
            rightArrow.interactable = false;
        else
            rightArrow.interactable = true;
    }
    private void SetLevels()
    {

        if (currSector>totalSectors-1)
            currSector = totalSectors-1;
        
        int[] sectorStars = new int[sectorsValue[currSector]];
        for (int i =0; i<sectorsValue[currSector]; i++)
            {
                sectorStars[i] = progress.GetProgress(currSector, i);
            }


        CheckBorders();

        sectornum.text = "Sector " + (currSector+1);


        GameObject stagePoint = null;
        for (int i =0; i<sectorsValue[currSector]; i++)
        {
            int number = i;

            if (currSector == lastSector)
            {
                if (i < curLevel)
                {
                    stagePoint = Instantiate(finishedLevel, stagesParent);
                    stagePoint.transform.GetChild(0).GetComponent<TMP_Text>().text = (i+1).ToString();

                    stagePoint.GetComponent<Button>().onClick.AddListener(delegate {TestMethod(number); });

                    GameObject firstStar = stagePoint.transform.GetChild(1).GetChild(0).gameObject;
                    GameObject secondStar = stagePoint.transform.GetChild(1).GetChild(1).gameObject;
                    GameObject thirdStar = stagePoint.transform.GetChild(1).GetChild(2).gameObject;
                    if (sectorStars[i]==0)
                    {
                        firstStar.SetActive(false);
                        secondStar.SetActive(false);
                        thirdStar.SetActive(false);
                    }
                    else if (sectorStars[i]==1)
                    {
                        secondStar.SetActive(false);
                        thirdStar.SetActive(false);
                    }
                    else if (sectorStars[i]==2)
                    {
                        thirdStar.SetActive(false);
                    }

                }
                else if (i == curLevel)
                {
                    stagePoint = Instantiate(openedLevel, stagesParent);
                    stagePoint.transform.GetChild(0).GetComponent<TMP_Text>().text = (i+1).ToString();
                    stagePoint.GetComponent<Button>().onClick.AddListener(delegate {TestMethod(number); });
                    
                }
                else
                {
                    Instantiate(lockedLevel,stagesParent);
                }
            }
            else if (currSector < lastSector)
            {
                stagePoint = Instantiate(finishedLevel, stagesParent);
                stagePoint.transform.GetChild(0).GetComponent<TMP_Text>().text = (i+1).ToString();
                stagePoint.GetComponent<Button>().onClick.AddListener(delegate {TestMethod(number); });
                GameObject firstStar = stagePoint.transform.GetChild(1).GetChild(0).gameObject;
                    GameObject secondStar = stagePoint.transform.GetChild(1).GetChild(1).gameObject;
                    GameObject thirdStar = stagePoint.transform.GetChild(1).GetChild(2).gameObject;
                    if (sectorStars[i]==0)
                    {
                        firstStar.SetActive(false);
                        secondStar.SetActive(false);
                        thirdStar.SetActive(false);
                    }
                    else if (sectorStars[i]==1)
                    {
                        secondStar.SetActive(false);
                        thirdStar.SetActive(false);
                    }
                    else if (sectorStars[i]==2)
                    {
                        thirdStar.SetActive(false);
                    }

            }
            else{
                Instantiate(lockedLevel,stagesParent);
            }
                
        }   
    }
   private void OnDisable()
   {
       ClearLevels();
   }

    private void CheckIfLocked()
    {
        //Текст для плашки //Required      : 7
      
        int req = progress.GetSectorReq(currSector);

        if (stars < req)
        {
            sectorLocked.SetActive(true);
            sectorReq.text = "Required      : " + req;
        }
        else
        sectorLocked.SetActive(false);
        


    }

    public void NextSector()
    {
        ClearLevels();
        currSector+=1;
        SetLevels();
        CheckBorders();
        CheckIfLocked();
    }

    public void PrevSector()
    {
        ClearLevels();
        currSector-=1;
        SetLevels();
        CheckBorders();
        CheckIfLocked();
    }


    void TestMethod(int level)
    {
        Debug.Log("Кликнута кнопка " + currSector + " " + level);

        progress.SetCurrentLevel(level);
        progress.SetCurrentSector(currSector);

        startButton.interactable = true;
    }
}

