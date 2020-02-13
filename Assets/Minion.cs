using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour {
    public float maxRadius;

    private bool isInFov = false;
    private Rigidbody2D rb;

    private Transform enemy;
    public Transform player;

    public LayerMask layerMask;
    public LayerMask layerMask2;
    public float speed = 1f;

    


    // Start is called before the first frame update
    void Start()
    {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("player").transform;
        }
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {

        if (player == null) {

            Animator anim = GetComponent<Animator>();
            anim.Play("Die");
            StartCoroutine(killHim());
        }

        isInFov = inFOV(transform, maxRadius);

        if (isInFov) {

            Vector2 newPos = Vector2.MoveTowards(transform.position, enemy.position, Time.deltaTime * speed);
            if (transform.position.x > enemy.position.x)
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            else {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            rb.MovePosition(newPos);

        }
        else {
            Vector2 newPos = Vector2.MoveTowards(transform.position, new Vector2(player.position.x - 0.5f, player.position.y + 1), Time.deltaTime * speed);
            if (transform.position.x > player.position.x)
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            else {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            rb.MovePosition(newPos);
        }
        

    }

    /*private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        if (!isInFov)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (enemy.position - transform.position).normalized * maxRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);


    }*/

    public bool inFOV(Transform checkingObject, float maxRadius) {

        Collider2D[] overlaps = new Collider2D[10];
        int count = Physics2D.OverlapCircleNonAlloc(checkingObject.position, maxRadius, overlaps, layerMask);

        for (int i = 0; i < count + 1; i++) {
            if (overlaps[i] != null) {
                if (overlaps[i].tag == "enemy") {
                    Ray ray = new Ray(checkingObject.position, overlaps[i].transform.position - checkingObject.position);
                    RaycastHit2D hit = Physics2D.Raycast(checkingObject.position, (overlaps[i].transform.position - checkingObject.position), maxRadius, layerMask2);
                    Debug.DrawRay(checkingObject.position, (overlaps[i].transform.position - checkingObject.position), Color.red);
                    if (!hit) {
                        return false;
                    }
                    if (hit.collider.tag == "enemy") {
                        enemy = hit.collider.gameObject.transform;
                        Debug.Log("Striking ENEMYYYYYYYYYYYYYYYYYYYYYYYYYYY");
                        return true;
                    }
                    else {
                        return false;
                    }

                }
            }
        }
        return false;

    }

    private GameObject destroyMe;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "enemy") {
            collision.attachedRigidbody.Sleep();
            rb.Sleep();
            Animator anim = GetComponent<Animator>();
            anim.Play("Die");
            destroyMe = collision.gameObject;
            StartCoroutine(killHim());
        }

    }



    IEnumerator killHim() {
        yield return new WaitForSeconds(.2f);
        if (destroyMe != null)
            Destroy(destroyMe);
        yield return new WaitForSeconds(.7f);
        killMe();
    }

    private void killMe() {
        Destroy(gameObject);
    }


}
