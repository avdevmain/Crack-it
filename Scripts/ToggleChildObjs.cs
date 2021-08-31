using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ToggleChildObjs : MonoBehaviour
{


    [SerializeField]
    private GameObject[] childObjs;
    public void ToggleChildBttns()
    {
        for (int i =0; i<childObjs.Length; i++)
        {
            if (childObjs[i]!=null)
                childObjs[i].SetActive(!childObjs[i].activeSelf);
        }
    }


    [SerializeField]
    private TMP_Text text;

    public void ChangeTextColor(bool isActive)
    {
        if (text!=null)
        {
            //active = 226, 199, 153
            //inactive = 135, 120, 98
            if (isActive)
                text.color = new Color(226,199,153);
            else
                text.color = new Color(135,120,98);
        }
    }

}
