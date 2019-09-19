using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public int level = 10;
    public Text turnText;

    public bool introFinished = false;


    void Awake() {
        // Check if instance already exists
        if (instance == null) {
            instance = this;
            //If instance already exists and it's not this:
        } else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log("OnSceneLoaded: " + scene.name);
        switch (scene.name) {
            case "Game":
                break;
        }
    }

    // Use this for initialization
    void Start() {
        //instance.InitGame ();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnDisable() {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void InitGame() {
        //boardManager.SetupBoard (level);
    }

    public void StartEndless() {
        SceneManager.LoadScene("Game");
    }

    public void EnablePlayerInput() {
        Camera.main.GetComponent<Physics2DRaycaster>().enabled = true;
    }

    public void DisablePlayerInput() {
        Camera.main.GetComponent<Physics2DRaycaster>().enabled = false;
    }
}
