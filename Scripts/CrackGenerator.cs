using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Destructible2D;
using Destructible2D.Examples;
public class CrackGenerator : MonoBehaviour
{

    public int angleMinMax;
   
    public GameObject[] crackParentPrefab;
    public GameObject segmentPrefab;
    public GameObject holeCheckPrefab;

    public int chanceToAdditionalLineOnInsane;

    public enum CrackType
    {
        easy,
        normal,
        strong,
        insane
    }
    public void MakeNewLine(CrackType type, Vector3 position, Quaternion rotation)
    {
       
        Vector2 check = Vector2.zero;
        List<GameObject> segments = new List<GameObject>();
        GameObject crackParent; //= Instantiate(crackParentPrefab);    

        int length = 0;
        //bool isInsane = false;
        switch(type)
        {
            //case (CrackType.easy):
            //crackParent = Instantiate(crackParentPrefab[0]);
            //length = 2;
           // angleMinMax = 6;
            //break;
            case (CrackType.normal):
            crackParent = Instantiate(crackParentPrefab[1]);
            angleMinMax = 7; // было 7
            length = 3; // было 3
            break;
          //  case (CrackType.strong):
          //  crackParent = Instantiate(crackParentPrefab[2]);
          //  angleMinMax = 9;
           // length = 4;
           // break;
            //case (CrackType.insane):
            //crackParent = Instantiate(crackParentPrefab[3]);
           // angleMinMax = 12;
           // length = 5;
           // isInsane = true;
           // break;
            default:
            crackParent = Instantiate(crackParentPrefab[1]);
            length = 3;
            angleMinMax = 7;
            break;
        }
        crackParent.transform.position = position;
        crackParent.transform.rotation = rotation;
        Destroy(crackParent, 1.5f); 

        int angle = 0;
        for (int i =0; i<length; i++)
        {   

  
            int newangle = 0;
            segments.Add(Instantiate(segmentPrefab, crackParent.transform, false));
            if (i>0)
            {
                while (newangle == angle)
                {
                    newangle = Random.value >0.5f ?  
                    Random.Range(-angleMinMax, -2) * 8 :
                    Random.Range(2, angleMinMax+1) * 8 ;
                }
            }
            else
            {
                newangle = Random.Range(-2,3) * 8;
            }
            angle = newangle;
            segments[i].transform.localRotation = Quaternion.Euler(0.0f,0.0f,-angle);

            if (i > 0)
            {
                segments[i].transform.position = segments[i-1].transform.GetChild(0).position;
                segments[i].transform.parent = segments[i-1].transform.GetChild(0);
            }
            
            if (ShouldIStop(segments[i].transform))
            {
                segments[i].SetActive(true);
                break;
            }
            segments[i].SetActive(true);
           

            /*
            if (isInsane && i!=length-1)
            {
                
                if (Random.Range(0,101) <= chanceToAdditionalLineOnInsane) //
                {   
                    List<GameObject> extraSegments = new List<GameObject>();
                    int amount = Random.Range(1,3);
                    for (int j = 0; j<amount; j++)
                    {
                        int sign = Random.Range(-1,0) * -1;
                        int extraAngle = Random.Range(20, 80) * sign;

                        extraSegments.Add(Instantiate(segmentPrefab, segments[i].transform.GetChild(1), false));
                        
                        extraSegments[j].transform.localRotation = Quaternion.Euler(0.0f,0.0f,-extraAngle);

                        if (j > 0)
                        {
                            extraSegments[j].transform.position = extraSegments[j-1].transform.GetChild(0).position;
                            extraSegments[j].transform.parent = extraSegments[j-1].transform.GetChild(0);
                        }
                        
                        if (ShouldIStop(extraSegments[j].transform))
                        {
                            extraSegments[j].SetActive(true);
                            break;
                        }
                        extraSegments[j].SetActive(true);
                    }
                } 
            } */

            
        }
        
    }

    private bool ShouldIStop(Transform objTransform)
    {
         
        Vector2 origin = objTransform.position;
        Vector2 nextLineOrigin = objTransform.GetChild(0).position;
        Vector2 direction = (nextLineOrigin-origin).normalized * Vector2.Distance(origin, nextLineOrigin);
        
        int layerMask = 1 << 14 | 1 << 9; //Слой камней это 14, а 9 это GEm 
        
        for (int i =1; i< 11; i++)
        {
            Vector2 overlapPosition = direction * 0.1f * i + origin;
            Collider2D col = Physics2D.OverlapPoint(overlapPosition, layerMask);
            if ((col == null) || (col.gameObject.layer == 9) )
            {
                if ((col!=null) && (col.gameObject.layer == 9))
                {
                    Debug.Log("ААА ПОПАЛ В ГЕМ");
                }
               
                var scale = Vector2.Distance(origin, overlapPosition) * 2;
                objTransform.localScale = new Vector3(objTransform.localScale.x, scale, objTransform.localScale.z);                
                
                return true;
            }
        }
        return false;
    }
        
    void SpawnRedSphere(Vector2 position) //Для дебага
    {
        var sphere =  Instantiate(holeCheckPrefab);
        sphere.transform.position = position;
        Destroy(sphere,2f);
    }


}
