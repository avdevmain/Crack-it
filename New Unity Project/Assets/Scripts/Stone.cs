using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Destructible2D;

public class Stone : MonoBehaviour
{
    Rigidbody2D rb;
    public Gem gemStone;
    
    public GameObject startCracks;

    public TMP_Text stoneLeft; //Проценты на вывод для игрока
    private D2dDestructibleSprite desprite;


    private void Start() {
        rb=  GetComponent<Rigidbody2D>();
        desprite = GetComponent<D2dDestructibleSprite>();
        if (desprite.AlphaRatio == 1)
        {
            stoneLeft.text = "100%";
            StartCoroutine(PlaceCracks());
        }
        else
        {
            gemStone.StoneFractured();
        }

        
 
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.transform.tag == "tool")
        {
            other.transform.GetComponent<BreakingTool>().ReachedStone();
        }
    }


   public void PushItAway()
    {
        rb.isKinematic = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.AddTorque(10f);
        rb.AddForce(rb.worldCenterOfMass * 150f);

        desprite.enabled = false;
        gameObject.layer = 0;
        StartCoroutine("IsFalling");
        

    }

    public int GetPercentLeft()
    {
        int value = 200;
        if (desprite !=null)
        value = ((int)(100 * desprite.AlphaRatio));
        if (value!=200)
        stoneLeft.text = value.ToString() + "%";

        return value;
    }





   private IEnumerator IsFalling()
      {
            while(gameObject!=null)
            {
            yield return new WaitForSecondsRealtime(2f);
            if (transform.localPosition.y < -20)
                  Destroy(gameObject);
            else
                GetComponent<D2dDestroyer>().enabled = true;

            }

      }




    private IEnumerator PlaceCracks()
    {
        desprite.enabled = true;
        yield return null;
            startCracks.SetActive(true);
        yield return null;
            GetComponent<D2dPolygonCollider>().Rebuild();

        
    }
}
