using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour {
    private int _scoreValue = 5;
    [SerializeField] private Vector3 _direction = Vector3.zero;
    [SerializeField] private Vector3 _rotationDirection = new Vector3(0f,0f, 1f);
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _rotationSpeed = 4f;
    private Player _player;
    private SpawnManager _spawnManager;

    [SerializeField] private GameObject _explosionPrefab;
    
    // Start is called before the first frame update
    void Start() {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) {
            Debug.LogError("Asteroid: Failed to find player.");
        }
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) {
            Debug.LogError("Asteroid: spawn manager is null.");
        }
    }

    void Update() {
        HandleMovement();
    }

    void HandleMovement() {
        transform.Rotate(_rotationDirection * (_rotationSpeed * Time.deltaTime));
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.CompareTag("Player")) {
            var player = other.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }
            _speed = 0f;
            
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            if (_spawnManager != null) {
                _spawnManager.StartSpawning();
            }
            Destroy(gameObject);
        } else if (other.transform.CompareTag("Projectile")) {
            Destroy(other.gameObject);
            if (_player != null) {
                _player.AddScore(_scoreValue);
            }
            _speed = 0f;
            
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            if (_spawnManager != null) {
                _spawnManager.StartSpawning();
            }
            Destroy(gameObject, 0.25f);
        }
    }
}
