﻿using System;
using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)] private float _targetingRange = 1.5f;
    [SerializeField] private LayerMask _targetLayerMask;

    private TargetPoint _target;
    
    public override void GameUpdate()
    {
        if (!IsTargetTracked())
        {
            if (IsAcquireTarget())
            {
                Debug.Log("Target founded");
            }
        }
    }

    private bool IsAcquireTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.localPosition, _targetingRange, _targetLayerMask);
        if (targets.Length > 0)
        {
            _target = targets[0].GetComponent<TargetPoint>();
            
            if(_target)
                return true;
        }

        return false;
    }

    private bool IsTargetTracked()
    {
        if (_target == null)
            return false;

        Vector3 myPosition = transform.localPosition;
        Vector3 targetPosition = _target.Position;
        if (Vector3.Distance(myPosition, targetPosition) >
            _targetingRange + _target.ColliderSize * _target.Enemy.Scale)
        {
            _target = null;
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, _targetingRange);
        if (_target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, _target.Position);
        }
    }
}