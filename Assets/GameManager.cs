using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool lockCursor = true;
    public GameObject exit;
    // Start is called before the first frame update
    void Start()
    {
        Cursori();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("player") == null) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetButtonDown("Cancel")) {
            Cursori();
        }
    }

    void Cursori() {
        if (lockCursor) {
            Time.timeScale = 1f;
            exit.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lockCursor = false;
        }
        else {
            Time.timeScale = 0f;
            exit.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            lockCursor = true;
        }
    }

    public void KillmeHealme() {
        Application.Quit();
    }

    public void ManimiMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
