using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private Vector3 _direction = Vector3.up;

    /// <summary>
    /// 
    /// </summary>
    void Update() {
        HandleMovement();
    }

    /// <summary>
    /// 
    /// </summary>
    void HandleMovement() {
        transform.Translate(_direction * (_speed * Time.deltaTime));
        CheckBoundaries();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.CompareTag("Player") && CompareTag("EnemyProjectile")) {
            var player = other.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void CheckBoundaries() {
        if (transform.position.y > 15f || transform.position.y < -10f) {
            DestroyObject();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void DestroyObject() {
        if (transform.parent != null) {
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}
