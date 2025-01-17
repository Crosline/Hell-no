﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    private float length, startPos, startY;
    public bool upAndDown = false;
    public GameObject cam;
    public float parallaxEffect = 1;
    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
        startPos = transform.position.x;
        length = sprite.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        if (!upAndDown)
            transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
        else {
            float yDistance = cam.transform.position.y;
            transform.position = new Vector3(startPos + dist, startY + Mathf.Clamp(cam.transform.position.y * parallaxEffect*2, yDistance, yDistance + 1.5f), transform.position.z);
        }

        if (temp > startPos + length)
            startPos += length;
        else if (temp < startPos - length)
            startPos -= length;
    }
}
