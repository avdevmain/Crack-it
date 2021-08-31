using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShopTrail : MonoBehaviour
{
   [SerializeField]
   private ProgressManager progress;

    [SerializeField]
    private TMP_Text coinsTotal;

    public List<TrailButton> trailButtons;

    private void OnEnable() {
        coinsTotal.text = progress.GetCoins().ToString();
    }
    public void DeequipEverything()
    {
        for (int i =0; i<trailButtons.Count; i++)
        {
            trailButtons[i].Deequip();
        }
    }


    public int GetCoins()
    {
        return progress.GetCoins();
    }

    public void MinusCoin(int value)
    {
        progress.ChangeCoins(value);
        coinsTotal.text = progress.GetCoins().ToString();
    }
}
