using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private Vector3 direction = Vector3.up;

    // Update is called once per frame
    void Update() {
        HandleMovement();
    }

    void HandleMovement() {
        transform.Translate(direction * (_speed * Time.deltaTime));
        CheckBoundaries();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.CompareTag("Player") && CompareTag("EnemyProjectile")) {
            var player = other.GetComponent<Player>();
            if (player != null) {
                player.Damage();
            }
        }
    }

    void CheckBoundaries() {
        if (transform.position.y > 15f || transform.position.y < -10f) {
            DestroyObject();
        }
    }

    void DestroyObject() {
        if (transform.parent != null) {
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}
