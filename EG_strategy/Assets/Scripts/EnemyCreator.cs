using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
    public Transform Spawn;
    public float CreatorPeriod;
    public GameObject EnemyPrefab;

    private float _timer;

    private void Update() {
        _timer += Time.deltaTime;
        if (_timer > CreatorPeriod) {
            _timer = 0;
            Instantiate(EnemyPrefab, Spawn.position, Spawn.rotation);
        }
    }
}
