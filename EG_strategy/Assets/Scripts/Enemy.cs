using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    Idle,
    WalkToBuilding,
    WalkToUnit,
    Attack
}

public class Enemy : MonoBehaviour
{

    public EnemyState CarrentEnemyState;

    public int Health;
    private int _maxHealth;

    public Building TargetBuilding;
    public Unit TargetUnit;

    public float DistanceToFollow = 7f;
    public float DistanceToAttack = 1f;

    public NavMeshAgent NavMeshAgent;

    public float AttackPeriod = 1f;
    private float _timer;

    public GameObject HealthBarPrefab;
    private HealthBar _healthBar;
    

    private void Start() {
        SetState(EnemyState.WalkToBuilding);
        _maxHealth = Health;
        GameObject healthBar = Instantiate(HealthBarPrefab);
        _healthBar = healthBar.GetComponent<HealthBar>();
        _healthBar.Setup(transform);
    }

    private void Update() {
        if (CarrentEnemyState == EnemyState.Idle) {
            FindClosestBuilding();
            if (TargetBuilding) {
                SetState(EnemyState.WalkToBuilding);
            }
            FindClosestUnit();
        } else if (CarrentEnemyState == EnemyState.WalkToBuilding) {
            FindClosestUnit();
            if (TargetBuilding == null) {
                SetState(EnemyState.Idle);
            }
        } else if (CarrentEnemyState == EnemyState.WalkToUnit) {

            if (TargetUnit) {

                NavMeshAgent.SetDestination(TargetUnit.transform.position);

                float distance = Vector3.Distance(transform.position, TargetUnit.transform.position);
                if (distance > DistanceToFollow) {
                    SetState(EnemyState.WalkToBuilding);
                }
                if (distance < DistanceToAttack) {
                    SetState(EnemyState.Attack);
                }
            } else {
                SetState(EnemyState.WalkToBuilding);
            }
            
        } else if (CarrentEnemyState == EnemyState.Attack) {
            if (TargetUnit) {

                NavMeshAgent.SetDestination(TargetUnit.transform.position);

                float distance = Vector3.Distance(transform.position, TargetUnit.transform.position);
                if (distance > DistanceToAttack) {
                    SetState(EnemyState.WalkToUnit);
                }
                _timer += Time.deltaTime;
                if (_timer > AttackPeriod) {
                    _timer = 0;
                    TargetUnit.TakeDamage(1);
                }
            } else {
                SetState(EnemyState.WalkToBuilding);
            }
            
        }
    }

    public void SetState(EnemyState enemyState) {
        CarrentEnemyState = enemyState;
        if (CarrentEnemyState == EnemyState.Idle) {

        } else if (CarrentEnemyState == EnemyState.WalkToBuilding) {
            FindClosestBuilding();
            if (TargetBuilding) {
                NavMeshAgent.SetDestination(TargetBuilding.transform.position);
            } else {
                SetState(EnemyState.Idle);
            }
            
        } else if (CarrentEnemyState == EnemyState.WalkToUnit) {

        } else if (CarrentEnemyState == EnemyState.Attack) {
            _timer = 0;
        }
    }

    public void FindClosestBuilding() {
        Building[] allBuilding = FindObjectsOfType<Building>();

        float minDistance = Mathf.Infinity;
        Building closesBuilding = null;

        for (int i = 0; i < allBuilding.Length; i++) {
            float distance = Vector3.Distance(transform.position, allBuilding[i].transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                closesBuilding = allBuilding[i];
            }
        }

        TargetBuilding = closesBuilding;
    }

    public void FindClosestUnit() {
        Unit[] allUnit = FindObjectsOfType<Unit>();

        float minDistance = Mathf.Infinity;
        Unit closesUnit = null;

        for (int i = 0; i < allUnit.Length; i++) {
            float distance = Vector3.Distance(transform.position, allUnit[i].transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                closesUnit = allUnit[i];
            }
        }
        if (minDistance < DistanceToFollow) {
            TargetUnit = closesUnit;
            SetState(EnemyState.WalkToUnit);
        }
        
    }

    public void TakeDamage(int damageValue) {
        Health -= damageValue;
        _healthBar.SetHealth(Health, _maxHealth);
        if (Health <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        if (_healthBar) {
            Destroy(_healthBar.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, DistanceToAttack);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, DistanceToFollow);
    }
# endif
}
