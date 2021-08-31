using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Destructible2D;
using TMPro;
using Lean.Touch;
public class BreakingTool : MonoBehaviour
{

    public float minPower = 0.1f;
   
Rigidbody2D rb;
CircleCollider2D circleCollider;
Camera cam;

private bool isSmashing;

Vector2 startPos;
Vector2 endPos;
float startTime;
float endTime;


private Coroutine cooldown;
private Coroutine hideTextCorout;


//	[SerializeField]
  // private TMP_Text text; //Show power of the last hit




public LeanTouch leanTouch;

[SerializeField]
private CrackGenerator crackGenerator;

public int smashCount;


[SerializeField]
private TMP_Text hit_power;

private void OnEnable() {
    smashCount = 0;
}
private void Start() {
    
    
		cam = Camera.main;
		rb = GetComponent<Rigidbody2D>();
		circleCollider = GetComponent<CircleCollider2D>(); 
        leanTouch.RecordFingers = false;
}

public void TouchWork(bool value)
{
    leanTouch.RecordFingers = value;
}

private void Update() {
    if (Input.GetMouseButtonDown(0))
    {
        StartSmash();
    }
    else if (Input.GetMouseButtonUp(0) && isSmashing)
    {
        EndSmash();
    }

    if (isSmashing)
    {
        UpdateSmash();
    }
}


private void StartSmash()
{
    isSmashing = true;
    startPos = cam.ScreenToWorldPoint(Input.mousePosition);
    startTime = Time.time;
    circleCollider.enabled = true;  
    leanTouch.RecordFingers = true;
}


private void EndSmash()
{
    endTime = Time.time;
    isSmashing = false;
    circleCollider.enabled = false;
      
    leanTouch.RecordFingers = false;

}
private void UpdateSmash()
{
    rb.position = cam.ScreenToWorldPoint(Input.mousePosition);
}

public void ReachedStone()
{

     EndSmash();

    if (cooldown!=null)
        return;
    else
        cooldown = StartCoroutine(Cooldown());
   

    endPos = cam.ScreenToWorldPoint(Input.mousePosition);

    RaycastHit2D hit = Physics2D.Raycast(startPos, endPos-startPos);
    
    if (hit.transform!=null)
    {
        ProcessSmash(hit.point);
    }

}

private void ProcessSmash(Vector2 point)
{

    float smashPower = Vector2.Distance(startPos,point) / (endTime - startTime) / 10;

    if (smashPower < minPower)
        return;

    
    //text.text = "Сила удара: " + smashPower;

    CrackGenerator.CrackType crackType;
    crackType = CrackGenerator.CrackType.normal;
    /*
    if (smashPower < 1)
    {
        crackType = CrackGenerator.CrackType.easy;
        hit_power.text = "WEAK";
        hit_power.color = Color.white;
    }
    else if (smashPower < 1.7)
    {
        crackType = CrackGenerator.CrackType.normal;
        hit_power.text = "OK";
        hit_power.color = Color.white;
    }
    else if (smashPower < 2.5)
    {
        crackType = CrackGenerator.CrackType.strong;
        hit_power.text = "STRONG";
        hit_power.color = Color.white;
    }
    else
    {
        crackType = CrackGenerator.CrackType.insane;
        hit_power.text = "INSANE!";
        hit_power.color = Color.red;
    } */

    //if (hideTextCorout!=null) StopCoroutine(hideTextCorout);
    //hideTextCorout = StartCoroutine(HideText());
       
    Vector3 pos = new Vector3(point.x, point.y, -1f);
    var angle = D2dHelper.Atan2(endPos-startPos) * Mathf.Rad2Deg;
    Quaternion rotat  = Quaternion.Euler(0.0f, 0.0f, -angle);
    
    crackGenerator.MakeNewLine(crackType, pos, rotat);

    smashCount++;
}

private IEnumerator Cooldown()
{
    yield return new WaitForSeconds(0.2f);
    cooldown =null;
}

/*
private IEnumerator HideText()
{
    yield return new WaitForSeconds(0.5f);
    hit_power.text = "";
    hideTextCorout = null;
} */
}
