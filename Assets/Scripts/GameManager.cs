using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool _isGameOver = false;
    public bool IsGameOver {
        get => _isGameOver;
        set => _isGameOver = value;
    }

    // Update is called once per frame
    void Update() {
        if (IsGameOver && Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } else if (Input.GetKey("escape")) {
            Application.Quit();
        }
    }
}
