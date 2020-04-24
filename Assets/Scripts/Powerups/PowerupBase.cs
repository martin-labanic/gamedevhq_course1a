using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Powerups {
    public class PowerupBase : MonoBehaviour {
        protected virtual Powerup Powerup => Powerup.None;
        
        [SerializeField] private float _speed = 3.0f;
        [SerializeField] private Vector3 direction = Vector3.down;
        
        void Update() {
            HandleMovement();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.CompareTag("Player")) {
                var player = other.GetComponent<Player>();
                if (player != null) {
                    player.CollectPowerup(Powerup);
                }

                Destroy(gameObject);
            }
        }

        void HandleMovement() {
            transform.Translate(direction * (_speed * Time.deltaTime));
            CheckBoundaries();
        }
        
        void CheckBoundaries() {
            if (transform.position.y < -10.0f) {
                Destroy(gameObject);
            }
        }
    }
}