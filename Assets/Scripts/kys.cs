using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kys : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("KillMe", 0.5f);
    }

    void KillMe() {
        Destroy(gameObject);
    }
}
