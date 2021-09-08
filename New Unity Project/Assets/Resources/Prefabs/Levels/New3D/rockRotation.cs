using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rockRotation : MonoBehaviour
{
    new Renderer renderer;
    Vector3 axis;
    bool rotate;
    float rotationSpeed = 2f;




    [SerializeField]
    private GameObject TwoDimObject;

    [SerializeField]
    private GameObject GemObject;

    [SerializeField]
    private GameObject DownLayerObj;

    float rotationLeft;
    private void Start() {
        renderer = GetComponent<Renderer>();
        StartCoroutine(Rotate(1.25f));
    }



    IEnumerator Rotate(float duration) 
    {
    Quaternion startRot = transform.rotation;
    float t = 0.0f;
    Vector3 axis = new Vector3(0,1,0);
        while ( t  < duration )
        {
            t += Time.deltaTime;
            //transform.rotation = startRot * Quaternion.AngleAxis(t / duration * 360f, axis); //or transform.right if you want it to be locally based
            transform.RotateAround(renderer.bounds.center, axis, 360f/ duration * Time.deltaTime);
            yield return null;
        }
    transform.rotation = startRot;
    TwoDimObject.SetActive(true);
    GemObject.SetActive(true);
    DownLayerObj.SetActive(true);
    this.gameObject.SetActive(false);
    }
}
