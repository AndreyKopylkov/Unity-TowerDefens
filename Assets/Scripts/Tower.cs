using System;
using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)] private float _targetingRange = 1.5f;
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private Transform _turret;
    [SerializeField] private LineRenderer _laserBeam;

    private TargetPoint _target;

    private void Awake()
    {
        ResetLaserBeam();
    }

    public override void GameUpdate()
    {
        if (IsTargetTracked())
        {
            Shoot();
        }
        else
        {
            if (IsAcquireTarget())
            {
                Shoot();
            }
        }

        // if (IsTargetTracked() || IsAcquireTarget())
        // {
        //     Shoot();
        // }
    }

    private void Shoot()
    {
        Vector3 point = _target.Position;
        _turret.LookAt(point);
        _laserBeam.SetPosition(0, _laserBeam.transform.position);
        _laserBeam.SetPosition(1, _target.Position);
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
        {
            OnTargetLost();
            return false;
        }

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

    private void OnTargetLost()
    {
        Debug.Log("Target was lost");
        ResetLaserBeam();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, _targetingRange);
        if (IsTargetTracked())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, _target.Position);
        }
    }

    private void ResetLaserBeam()
    {
        _laserBeam.SetPosition(0, _laserBeam.transform.position);
        _laserBeam.SetPosition(1, _laserBeam.transform.position);
    }
}