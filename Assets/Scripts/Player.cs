using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Powerups;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour {
    [SerializeField] private const int MAX_SHIELD_LIVES = 3;
    [SerializeField] private const int MAX_HEALTH_LIVES = 3;
    
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _shieldLives = 0;
    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _rightEngineDamage, _leftEngineDamage;
    
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _speedPowerupModifier = 2.0f;
    [SerializeField] private bool _isBoostActive = false;
    [SerializeField] private float _speedBoostModifier = 3.0f;
    [SerializeField] private float _speedBoostRecoveryDuration = 3.0f;
    [SerializeField] private bool _isSpeedBoostRecovering = false;
    
    [SerializeField] private GameObject _thrusterNormal;
    [SerializeField] private GameObject _thrusterBoosting;
    [SerializeField] private GameObject _thrusterRecovery;

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _spreadShotPrefab;
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private bool _isSpreadShotActive = false;
    [SerializeField] private bool _isSpeedActive = false;
    [SerializeField] private bool _isShieldActive = false;


    [SerializeField] private int _score = 0;
    private UIManager _uiManager;

    private Vector3 direction;

    [SerializeField] private float _fireRate = 0.15f;
    private float _nextFire = 0.0f;
    [SerializeField] private int _currentAmmo = 16;

    private SpawnManager _spawnManager; // Alternative implementation: delegates.

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _projectileAudioClip;
    [SerializeField] private AudioClip _powerupAudioClip;

    [SerializeField] private GameObject _explosionPrefab;

    [SerializeField] private GameObject _camera;
    
    /// <summary>
    /// 
    /// </summary>
    void Start() {
        _audioSource = GetComponent<AudioSource>();
        direction = Vector3.zero;
        transform.position = Vector3.zero;
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null) {
            Debug.Log("Player.Start: UI manage is null.");
        } else {
            _uiManager.UpdateScoreUI(_score);  
            _uiManager.UpdateLivesUI(_lives);
            _uiManager.UpdateAmmoUI(_currentAmmo);
        }
        
        _shieldVisualizer.SetActive(false);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) {
            Debug.LogError("Player.Start: Spawn manager is null.");
        }
        
        if (_camera == null) {
            Debug.LogError("Player.Start: Camera reference is null.");
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    void Update() {
        HandleMovement();
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShootProjectile();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (CanBoostSpeed()) {
                EnableSpeedBoost();   
            }
        } else if (Input.GetKeyUp(KeyCode.LeftShift)) {
            StartCoroutine(DisableSpeedBoost());
        }
    }

    private bool CanBoostSpeed() {
        return !_isBoostActive && !_isSpeedBoostRecovering;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void EnableSpeedBoost() {
        _isBoostActive = true;
        _thrusterNormal.SetActive(false);
        _thrusterBoosting.SetActive(true);
        _speed *= _speedBoostModifier;
    }

    /// <summary>
    /// 
    /// </summary>
    private IEnumerator DisableSpeedBoost() {
        _isBoostActive = false;
        _thrusterBoosting.SetActive(false);
        _thrusterRecovery.SetActive(true);
        _speed /= _speedBoostModifier;
        _isSpeedBoostRecovering = true;
        yield return new WaitForSeconds(_speedBoostRecoveryDuration);
        _isSpeedBoostRecovering = false;
        _thrusterRecovery.SetActive(false);
        _thrusterNormal.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool CanShootProjectile() {
        return Time.time > _nextFire && _currentAmmo > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    void ShootProjectile() {
        if (CanShootProjectile()) {
            if (_isSpreadShotActive) {
                Instantiate(_spreadShotPrefab, transform.position, Quaternion.identity);
            } else if (_isTripleShotActive) {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            } else {
                Instantiate(_projectilePrefab, transform.position + new Vector3(0, 0.95f, 0), Quaternion.identity);
            }

            _currentAmmo--;
            _uiManager.UpdateAmmoUI(_currentAmmo);
            _nextFire = Time.time + _fireRate;
            _audioSource.PlayOneShot(_projectileAudioClip);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void CollectPowerup(Powerup id) {
        switch (id) {
            case Powerup.None:
                Debug.Log("collectPowerup: You forgot to override the powerup id.");
                break;
            case Powerup.TripleShot:
                _audioSource.PlayOneShot(_powerupAudioClip);
                EnableTripleShotPowerup();
                break;
            case Powerup.Speed:
                _audioSource.PlayOneShot(_powerupAudioClip);
                EnableSpeedPowerup();
                break;
            case Powerup.Shield:
                _audioSource.PlayOneShot(_powerupAudioClip);
                EnableShieldPowerup();
                break;
            case Powerup.Ammo:
                _audioSource.PlayOneShot(_powerupAudioClip);
                PickupAmmo();
                break;
            case Powerup.Health:
                _audioSource.PlayOneShot(_powerupAudioClip);
                PickupHealth();
                break;
            case Powerup.SpreadShot:
                _audioSource.PlayOneShot(_powerupAudioClip);
                EnableSpreadShotPowerup();
                break;
            default:
                Debug.Log("collectPowerup: Unknown powerup id " + (int) id);
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PickupAmmo() {
        _currentAmmo += 15;
        if (_uiManager != null) {
            _uiManager.UpdateAmmoUI(_currentAmmo);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PickupHealth() {
        if (_lives < MAX_HEALTH_LIVES) {
            _lives++;
            if (_uiManager != null) {
                _uiManager.UpdateLivesUI(_lives);    
            }
            switch (_lives) {
                case 2:
                    _leftEngineDamage.SetActive(false);
                    break;
                case 3:
                    _rightEngineDamage.SetActive(false);
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableTripleShotPowerup() {
        _isTripleShotActive = true;
        StartCoroutine(DisableTripleShotPowerup());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableTripleShotPowerup() {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableSpreadShotPowerup() {
        _isSpreadShotActive = true;
        StartCoroutine(DisableSpreadShotPowerup());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableSpreadShotPowerup() {
        yield return new WaitForSeconds(5f);
        _isSpreadShotActive = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnableSpeedPowerup() {
        _isSpeedActive = true;
        _speed *= _speedPowerupModifier;
        StartCoroutine(DisableSpeedPowerup());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableSpeedPowerup() {
        yield return new WaitForSeconds(5f);
        _speed /= _speedPowerupModifier;
        _isSpeedActive = false;
    }
   
    /// <summary>
    /// 
    /// </summary>
    public void EnableShieldPowerup() {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
        _shieldLives = MAX_SHIELD_LIVES;
        _shieldVisualizer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    /// <summary>
    /// 
    /// </summary>
    public void DisableShieldPowerup() {
        _shieldVisualizer.SetActive(false);
        _isShieldActive = false;
    }

    /// <summary>
    /// 
    /// </summary>
    void HandleMovement() {
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");
        transform.Translate(direction * (_speed * Time.deltaTime)); //Time.deltaTime = realtime, equates to 1 seconds really.

        CheckBoundaries();
    }

    /// <summary>
    /// Handles player boundaries.
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public void Damage() {
        if (_isShieldActive) {
            _shieldLives--;
            if (_shieldLives <= 0) {
                DisableShieldPowerup();   
            } else {
                float shieldTransparency = _shieldLives / (float)MAX_SHIELD_LIVES;
                _shieldVisualizer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, shieldTransparency);
            }
        } else {
            _lives--;
            if (_camera != null) { // Shake the screen.
                StartCoroutine(_camera.GetComponent<CameraUtils>().Shake(0.05f));
            }
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
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void AddScore(int value) {
        _score += value;
        if (_uiManager != null) {
            _uiManager.UpdateScoreUI(_score);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void GameOver() {
        _uiManager.DisplayGameOver();
        _spawnManager.OnPlayerDeath();
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
