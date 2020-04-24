using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UIManager : MonoBehaviour { // TODO There has to be a better way to access the UI Manager.
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _gameRestartText;
    [SerializeField] private Image _livesDisplayImage;
    [SerializeField] private Sprite[] _livesSprites;

    private GameManager _gameManager;

    private void Start() {
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) {
            Debug.LogError("GameManager is null.");
        }
    }

    public void UpdateScoreUI(int score) {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLivesUI(int lives) {
        _livesDisplayImage.sprite = _livesSprites[lives];
    }

    public void DisplayGameOver() {
        _gameManager.IsGameOver = true;
        _gameOverText.gameObject.SetActive(true);
        _gameRestartText.gameObject.SetActive(true);
        StartCoroutine(FlickerText(_gameOverText));
    }

    private IEnumerator FlickerText(Text text) {
        while (true) {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.75f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.1f);
        }
    }
}
