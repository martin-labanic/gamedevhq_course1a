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
    [SerializeField] private Text _ammoCountText;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="score"></param>
    public void UpdateScoreUI(int score) {
        _scoreText.text = "Score: " + score;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lives"></param>
    public void UpdateLivesUI(int lives) {
        if (lives >= 0 && lives < _livesSprites.Length) {
            _livesDisplayImage.sprite = _livesSprites[lives];    
        }
    }

    public void UpdateAmmoUI(int ammo) {
        _ammoCountText.text = "x" + ammo;
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisplayGameOver() {
        _gameManager.IsGameOver = true;
        _gameOverText.gameObject.SetActive(true);
        _gameRestartText.gameObject.SetActive(true);
        StartCoroutine(FlickerText(_gameOverText));
    }

    /// <summary>
    ///
    /// Note: Potential for a bug here where the run time is stopped when its blank. Mitigated atm because the scene reloads.
    /// </summary>
    /// <param name="textObj"></param>
    /// <returns></returns>
    private IEnumerator FlickerText(Text textObj) {
        String originalText = textObj.text;
        while (true) {
            textObj.text = originalText;
            yield return new WaitForSeconds(0.75f);
            textObj.text = "";
            yield return new WaitForSeconds(0.1f);
        }
    }
}
