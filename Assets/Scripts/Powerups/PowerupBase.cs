using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Powerups {
    public class PowerupBase : MonoBehaviour {
        protected virtual Powerup Powerup => Powerup.None;
        
        [SerializeField] private float _speed = 3.0f;
        [SerializeField] private Vector3 _direction = Vector3.down;
        
        /// <summary>
        /// 
        /// </summary>
        void Update() {
            HandleMovement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.transform.CompareTag("Player")) {
                var player = other.GetComponent<Player>();
                if (player != null) {
                    player.CollectPowerup(Powerup);
                }

                Destroy(gameObject);
            }
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
        void CheckBoundaries() {
            if (transform.position.y < -10.0f) {
                Destroy(gameObject);
            }
        }
    }
}