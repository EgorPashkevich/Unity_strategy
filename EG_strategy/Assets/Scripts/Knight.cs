using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum UnitState {
    Idle,
    WalkToPoint,
    WalkToEmeny,
    Attack
}

public class Knight : Unit 
{
    public UnitState CarrentUnitState;

    public Building TargetPoint;
    public Enemy TargetEnemy;

    public float DistanceToFollow = 7f;
    public float DistanceToAttack = 1f;

    public float AttackPeriod = 1f;
    private float _timer;



    public override void Start() {
        base.Start();
        SetState(UnitState.WalkToPoint);
    }

    private void Update() {
        if (CarrentUnitState == UnitState.Idle) {
            FindClosestEnemy();
        } else if (CarrentUnitState == UnitState.WalkToPoint) {
            FindClosestEnemy();
            
        } else if (CarrentUnitState == UnitState.WalkToEmeny) {

            if (TargetEnemy) {

                NavMeshAgent.SetDestination(TargetEnemy.transform.position);

                float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);
                if (distance > DistanceToFollow) {
                    SetState(UnitState.WalkToPoint);
                }
                if (distance < DistanceToAttack) {
                    SetState(UnitState.Attack);
                }
            } else {
                SetState(UnitState.WalkToPoint);
            }

        } else if (CarrentUnitState == UnitState.Attack) {
            if (TargetEnemy) {

                NavMeshAgent.SetDestination(TargetEnemy.transform.position);

                float distance = Vector3.Distance(transform.position, TargetEnemy.transform.position);
                if (distance > DistanceToAttack) {
                    SetState(UnitState.WalkToEmeny);
                }
                _timer += Time.deltaTime;
                if (_timer > AttackPeriod) {
                    _timer = 0;
                    TargetEnemy.TakeDamage(1);
                }
            } else {
                SetState(UnitState.WalkToPoint);
            }

        }
    }

    public void SetState(UnitState unitState) {
        CarrentUnitState = unitState;
        if (CarrentUnitState == UnitState.Idle) {

        } else if (CarrentUnitState == UnitState.WalkToPoint) {
          
        } else if (CarrentUnitState == UnitState.WalkToEmeny) {

        } else if (CarrentUnitState == UnitState.Attack) {
            _timer = 0;
        }
    }



    public void FindClosestEnemy() {
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        float minDistance = Mathf.Infinity;
        Enemy closesEnemy = null;

        for (int i = 0; i < allEnemies.Length; i++) {
            float distance = Vector3.Distance(transform.position, allEnemies[i].transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                closesEnemy = allEnemies[i];
            }
        }
        if (minDistance < DistanceToFollow) {
            TargetEnemy = closesEnemy;
            SetState(UnitState.WalkToEmeny);
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
