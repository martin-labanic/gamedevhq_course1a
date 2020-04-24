using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Powerups;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour {
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _shieldLives = 0;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _rightEngineDamage, _leftEngineDamage;
    
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _speedPowerupModifier = 2.0f;
    
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private bool _isSpeedActive = false;
    [SerializeField] private bool _isShieldActive = false;

    [SerializeField] private int _score = 0;
    private UIManager _uiManager;

    private Vector3 direction;

    [SerializeField] private float _fireRate = 0.15f;
    private float _nextFire = 0.0f;

    private SpawnManager _spawnManager; // Alternative implementation: delegates.

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _projectileAudioClip;
    [SerializeField] private AudioClip _powerupAudioClip;

    [SerializeField] private GameObject _explosionPrefab;
    
    void Start() {
        _audioSource = GetComponent<AudioSource>();
        direction = Vector3.zero;
        transform.position = Vector3.zero;
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null) {
            Debug.Log("Failed to find ui manager.");
        } else {
            _uiManager.UpdateScoreUI(_score);  
            _uiManager.UpdateLivesUI(_lives);
        }
        
        _shieldVisualizer.SetActive(false);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) {
            Debug.Log("SPAWN MANAGER: SHE NULL, MAN.");
        }
    }
    
    void Update() {
        HandleMovement();
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShootProjectile();
        }
    }

    bool CanShootProjectile() {
        return Time.time > _nextFire;
    }

    void ShootProjectile() {
        if (CanShootProjectile()) {
            if (_isTripleShotActive) {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else {
                Instantiate(_projectilePrefab, transform.position + new Vector3(0, 0.95f, 0), Quaternion.identity);
            }

            _nextFire = Time.time + _fireRate;
            
            _audioSource.PlayOneShot(_projectileAudioClip);
        }
    }

    public void CollectPowerup(Powerup id) {
        switch (id) {
            case Powerup.None:
                Debug.Log("collectPowerup: You forgot to override the powerup id.");
                break;
            case Powerup.TripleShot:
                _audioSource.PlayOneShot(_powerupAudioClip);
                EnableTripleShot();
                break;
            case Powerup.Speed:
                _audioSource.PlayOneShot(_powerupAudioClip);
                EnableSpeed();
                break;
            case Powerup.Shield:
                _audioSource.PlayOneShot(_powerupAudioClip);
                EnableShield();
                break;
            default:
                Debug.Log("collectPowerup: Unknown powerup id " + (int) id);
                break;
        }
    }

    public void EnableTripleShot() {
        _isTripleShotActive = true;
        StartCoroutine(DisableTripleShot());
    }

    private IEnumerator DisableTripleShot() {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
        
    }

    public void EnableSpeed() {
        _isSpeedActive = true;
        _speed *= _speedPowerupModifier;
        StartCoroutine(DisableSpeed());
    }

    private IEnumerator DisableSpeed() {
        yield return new WaitForSeconds(5f);
        _speed /= _speedPowerupModifier;
        _isSpeedActive = false;
    }
    
    public void EnableShield() {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
        _shieldLives = 1;
    }

    public void DisableShield() {
        _shieldVisualizer.SetActive(false);
        _isShieldActive = false;
    }

    void HandleMovement() {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");
        transform.Translate(direction * (_speed * Time.deltaTime)); //Time.deltaTime = realtime, equates to 1 seconds really.

        CheckBoundaries();
    }

    // Handles player boundaries.
    void CheckBoundaries() {
        // Can use `Mathf.Clamp(...)` if your not warping it.
        // Check the x values.
        float x = transform.position.x;
        if (transform.position.x < -11) {
            x = 11;
        } else if (transform.position.x > 11) {
            x = -11;
        }

        // Check the y values.
        float y = transform.position.y;
        if (transform.position.y < -6) {
            y = 6;
        } else if (transform.position.y > 6) {
            y = -6;
        }

        if (x != transform.position.x || y != transform.position.y) {
            transform.position = new Vector3(x, y, 0);
        }
    }

    public void Damage() {
        if (_isShieldActive) {
            _shieldLives--;
            if (_shieldLives <= 0) {
                DisableShield();   
            }
        } else {
            _lives--;
            _uiManager.UpdateLivesUI(_lives);
            switch (_lives) {
                case 0:
                    GameOver();
                    break;
                case 1:
                    _leftEngineDamage.SetActive(true);
                    break;
                case 2:
                    _rightEngineDamage.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }

    public void AddScore(int value) {
        _score += value;
        if (_uiManager != null) {
            _uiManager.UpdateScoreUI(_score);
        }
    }

    private void GameOver() {
        _uiManager.DisplayGameOver();
        _spawnManager.OnPlayerDeath();
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
