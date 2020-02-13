using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    private Vector2 oldTransform;
    public Transform tiransform;
    public bool restart = true;
    public GameObject maMenu;
    public float sped;

    private void Start() {
        oldTransform = transform.position;
        Debug.Log(oldTransform + "" + transform.position);
    }

    void Update()
    {
        if (restart) {
            maMenu.SetActive(false);

            if (transform.position.y - tiransform.position.y < 0)
                transform.position = new Vector2(transform.position.x, transform.position.y + sped * Time.deltaTime);
            else {
                restart = false;
            }
        }
        else {
            maMenu.SetActive(true);
        }
    }


    public void RestartMe() {
        Debug.Log(oldTransform + "" + transform.position);
        transform.position = oldTransform;
        restart = true;
    }
}
