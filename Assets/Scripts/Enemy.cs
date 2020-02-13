using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    // Start is called before the first frame update

    public int health;
    public float speed;
    private float xSpeed;

    public Transform start;
    public Transform stop;
    public bool go = true;

    public float dazedTimeCD;
    private float dazedTime;

    //private Animator anim;
    //public GameObject bloodEffect;

    void Start() {
        transform.position = start.position;
        xSpeed = speed;
      /*
       anim = GetComponent<Animator>();
       anim.SetBool("isRunning", true);
         */
    }

    // Update is called once per frame

    void Update() {
        if (health <= 0) {
            StartCoroutine(killHim());
        }
        if (dazedTime <= 0) {
            speed = xSpeed;
        } else {
            speed = 0;
            dazedTime -= Time.deltaTime;
        }



        if (transform.position.x - stop.position.x < 0.01f)
            go = false;
        if (start.position.x - transform.position.x < 0.01f)
            go = true;


        if (go) {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
            GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(transform.position, stop.position, Time.deltaTime * speed));
        }
        else {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            GetComponent<Rigidbody2D>().MovePosition(Vector2.MoveTowards(transform.position, start.position, Time.deltaTime * speed));
        }
    }

    void FixedUpdate() {



    }


    public void TakeDamage(int damage, float pos) {

        dazedTime = dazedTimeCD;
        //Instantiate(bloodEffect, transform.position, Quaternion.identity);
        health -= damage;
        if(pos == 180)
            GetComponent<Rigidbody2D>().velocity = (Vector2.left * speed * 10);
        else
            GetComponent<Rigidbody2D>().velocity = (Vector2.right * speed * 10);

    }

    private void OnCollisionEnter(Collision collision) {
     if(collision.collider.tag == "player") {
            collision.collider.attachedRigidbody.AddForce((transform.position - collision.collider.transform.position) * 5, ForceMode.VelocityChange);
        }   
    }

    IEnumerator killHim() {
        GetComponent<Rigidbody2D>().Sleep();
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(.4f);
        killMe();
    }

    private void killMe() {
        Destroy(gameObject);
    }
}
