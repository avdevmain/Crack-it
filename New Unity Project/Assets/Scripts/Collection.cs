using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Collection : MonoBehaviour
{

    [SerializeField]
    private ProgressManager progress;

    [SerializeField]
    private TMP_Text coin_value;
 
 private void OnEnable() {
     coin_value.text = progress.GetCoins().ToString();
 }
}
