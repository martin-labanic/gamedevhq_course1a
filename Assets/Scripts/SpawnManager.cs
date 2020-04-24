using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    [SerializeField] private GameObject _enemy;
    [SerializeField] private GameObject[] _powerupPrefabs;
    
    [SerializeField] private float _enemyTimeDelay = 5.0f;
    [SerializeField] private float _powerupTimeDelayMin = 4.0f;
    [SerializeField] private float _powerupTimeDelayMax = 8.0f;
    [SerializeField] private bool _shouldSpawn = true;

    public void StartSpawning() {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerups());
    }

    public void OnPlayerDeath() {
        _shouldSpawn = false;
    }

    IEnumerator SpawnEnemy() {
        yield return  new WaitForSeconds(2f);
        while (_shouldSpawn) {
            GameObject spawnedObject = Instantiate(_enemy, GetRandomSpawnLocation(), Quaternion.identity);
            spawnedObject.transform.parent = transform; // Assign this to keep the heirarchy clean.
            yield return new WaitForSeconds(_enemyTimeDelay);
        }
    }

    IEnumerator SpawnPowerups() {
        yield return  new WaitForSeconds(2f);
        while (_shouldSpawn) {
            int i = Random.Range(0, _powerupPrefabs.Length);
            Instantiate(_powerupPrefabs[i], GetRandomSpawnLocation(), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(_powerupTimeDelayMin, _powerupTimeDelayMax));
        }
    }

    Vector3 GetRandomSpawnLocation() {
        return new Vector3(Random.Range(-8.5f, 8.5f), 9f);
    }
}
