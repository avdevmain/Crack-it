using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Destructible2D;

public class Gem : MonoBehaviour
{


    //public bool isBroken;
    //public Sprite damagedSprite;
    private SpriteRenderer spriteRenderer;
    ProgressManager progress;
    private Coroutine cooldown;

    [SerializeField]
    private GameObject ThreeDimGem;

    
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        progress = FindObjectOfType<ProgressManager>();
    }

    /*
    public  void GemDamaged()
    {

        if (!isBroken)
        {
        spriteRenderer.sprite = damagedSprite;
        isBroken = true;

        FindObjectOfType<ProgressManager>().PlayerLost();
        Debug.Log("GAME OVER");
        }
    } */

    public void StoneFractured()
    {
        Collider2D col;
        Vector2 nextLookPoint;
        int layerMask = 1 << 14; 
        col = Physics2D.OverlapPoint(transform.position,layerMask); //Поиск камня под кристаллом

        if (col == null)
        {
            nextLookPoint = new Vector2(transform.position.x + 0.15f, transform.position.y);
            col = Physics2D.OverlapPoint(nextLookPoint,layerMask);
        }

        if (col == null)
        {
            nextLookPoint = new Vector2(transform.position.x - 0.15f, transform.position.y);
            col = Physics2D.OverlapPoint(nextLookPoint,layerMask);
        }

        if (col == null)
        {
            nextLookPoint = new Vector2(transform.position.x, transform.position.y + 0.15f);
            col = Physics2D.OverlapPoint(nextLookPoint,layerMask);
        }
        if (col == null)
        {
            nextLookPoint = new Vector2(transform.position.x, transform.position.y - 0.15f);
            col = Physics2D.OverlapPoint(nextLookPoint,layerMask);
        } 

        if (col == null)
        {
            col = Physics2D.OverlapCircle(transform.position, 0.5f ,layerMask);
            Debug.Log("Баг. Камень выбит из под гема и не может быть найден им.");
            if (col==null)
                return;
            Debug.Log("Баг разрешен при помощи сферы");
        }


        if (col.transform.parent.GetComponent<Stone>().GetPercentLeft() < 9)
        {
            Debug.Log("Победа");
            if (cooldown==null)
                {
                    cooldown = StartCoroutine(Cooldown());
                    //StartCoroutine(HasHeWon());
                    //PlayerWon();
                    int cracksMade = FindObjectOfType<BreakingTool>().smashCount;

                    progress.PlayerWon(cracksMade);
                    //FindObjectOfType<ProgressManager>().PlayerWon(cracksMade);
                    Debug.Log("Запуск корутины");
            }
        }
    }

    private IEnumerator HasHeWon()
    {
        yield return new WaitForSeconds(0.1f);
       // if (!isBroken)
        Debug.Log("перед PlayerWon()");
            PlayerWon();
            Debug.Log("после PlayerWon()");
    }

    private void PlayerWon()
    {
        Debug.Log("Поиск количества ударов");
        int cracksMade = FindObjectOfType<BreakingTool>().smashCount;
        Debug.Log("Найдено количество сделанных ударов");
        FindObjectOfType<ProgressManager>().PlayerWon(cracksMade);
    }

    
    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f);
        cooldown =null;
    }
}
