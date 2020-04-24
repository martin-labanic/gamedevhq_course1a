using System;
using System.Collections;
using System.Collections.Generic;
using Powerups;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    [SerializeField] private int _scoreValue = 10;
    [SerializeField] public float _speed = 4.0f;
    private bool _isAlive = true;
    private Vector3 direction = Vector3.down;
    private Player _player;
    private Animator _animator;
    [SerializeField] private GameObject _projectilePrefab;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _projectileAudioClip;
    [SerializeField] private AudioClip _explosionAudioClip;
    [SerializeField] private float _fastestFireRate = 3.0f;
    [SerializeField] private float _slowestRate = 7.0f;

    private void Start() {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) {
            Debug.LogError("Failed to find player.");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null) {
            Debug.LogError("Failed to find animator component.");
        }
        
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) {
            Debug.LogError("Failed to find audio source component.");
        }

        StartCoroutine(ShootProjectile());
    }

    void Update() {
        HandleMovement();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.CompareTag("Player")) {
            _isAlive = false;
            StopCoroutine(nameof(ShootProjectile));
            var player = other.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }
            if (_animator != null) {
                _animator.SetTrigger("OnEnemyDeath");
            }
            _speed = 0f;
            GetComponent<BoxCollider2D>().enabled = false;
            _audioSource.PlayOneShot(_explosionAudioClip);
            Destroy(gameObject, 2.3f);
        } else if (other.transform.CompareTag("Projectile")) {
            _isAlive = false;
            StopCoroutine(nameof(ShootProjectile));
            Destroy(other.gameObject);
            if (_animator != null) {
                _animator.SetTrigger("OnEnemyDeath");
            }
            if (_player != null) {
                _player.AddScore(_scoreValue);
            }
            _speed = 0f;
            GetComponent<BoxCollider2D>().enabled = false;
            _audioSource.PlayOneShot(_explosionAudioClip);
            Destroy(gameObject, 2.3f);
        }
    }
    
    IEnumerator ShootProjectile() {
        yield return  new WaitForSeconds(1f);
        while (_isAlive) {
            Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            _audioSource.PlayOneShot(_projectileAudioClip);
            yield return new WaitForSeconds(Random.Range(_fastestFireRate, _slowestRate));
        }
    }
    
    void HandleMovement() {
        transform.Translate(direction * (_speed * Time.deltaTime));
        CheckBoundaries();
    }

    void CheckBoundaries() {
        if (transform.position.y < -10.0f) {
            transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 8.0f, transform.position.z);
        }
    }
}
