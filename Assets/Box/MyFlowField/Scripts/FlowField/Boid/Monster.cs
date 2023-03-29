using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private Rigidbody2D rb;

    private Transform target;
    private RaycastHit2D[] _hit2Ds;
    private bool _canMove = true;
    private Transform _transform;
    private Vector3 _dir;
    [SerializeField]
    private float _detectRange = 0.3f;

    [SerializeField] 
    private float _moveSpeed = 1;
    public bool CanMove => _canMove;

    public Vector3 Dir
    {
        get { return _dir; }
        set { _dir = value; }
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        _hit2Ds = new RaycastHit2D[3];
        _transform = this.transform;
        

    }

    private void FixedUpdate()
    {
        /*_dir = (target.position - _transform.position).normalized;
        Physics2D.RaycastNonAlloc(_transform.position, _dir, _hit2Ds, _detectRange);
        _canMove = _hit2Ds[2].collider == null;
        Array.Clear(_hit2Ds,0,3);
        if (!_canMove)
        {
            return;
        }
        _transform.position += _dir * _moveSpeed;*/
        
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.DrawLine(_transform.position,_transform.position +_dir*_detectRange);*/
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Vector3.Angle(_dir,collision.transform.position - _transform.position) > 70)
        {
            return;
        }
        _canMove = false;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _canMove = true;
    }
}
