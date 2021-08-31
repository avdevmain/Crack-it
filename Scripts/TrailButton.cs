using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lean;
public class TrailButton : MonoBehaviour
{

    [SerializeField]
    private ShopTrail shop;

    [SerializeField]
    private TMP_Text price;
    [SerializeField]
    private GameObject textConditionObj;
    [SerializeField]
    private GameObject chosenCircle;
    [SerializeField]
    private GameObject equippedGalka;
    [SerializeField]
    private GameObject boughtText;

    public int coin_price;
    public bool bought;
    public bool equipped;

    [SerializeField]
    Lean.Touch.LeanDragTrail trail;

    public Color color;

    
    private void Start() {
        this.price.text = coin_price.ToString();
        if (this.bought)
        {
            this.price.transform.parent.gameObject.SetActive(false);
            this.boughtText.SetActive(true);
        }
        else
        {
            this.boughtText.SetActive(false);
        }

        if (equipped)
        {
            equippedGalka.SetActive(true);
            this.chosenCircle.SetActive(true);
            boughtText.SetActive(false);
        }
        else
        {
            equippedGalka.SetActive(false);
            this.chosenCircle.SetActive(false);
        }
    }

    public void TryBuying()
    {
        if (this.bought) return;

        if (shop.GetCoins() < this.coin_price) return;

        this.bought = true;
        shop.MinusCoin(-this.coin_price);

        this.price.transform.parent.gameObject.SetActive(false);
        this.boughtText.SetActive(true);

    }

    public void Deequip()
    {
        if (!this.equipped) return;

        this.equipped = false;
        this.chosenCircle.SetActive(false);
        this.equippedGalka.SetActive(false);
        this.boughtText.SetActive(true);

    }

    public void Equip()
    {
        if (!this.bought) return;
        if (this.equipped) return;

        shop.DeequipEverything();

        this.equipped = true;

        this.chosenCircle.SetActive(true);
        this.boughtText.SetActive(false);
        this.equippedGalka.SetActive(true);

        trail.StartColor = this.color;
        trail.EndColor = this.color;

    }


}

