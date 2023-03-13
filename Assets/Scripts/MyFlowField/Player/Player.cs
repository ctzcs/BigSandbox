using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;

    public float Vspeed, Hspeed;

    public float rate;

    private UnitMove _unitMove;
    // Start is called before the first frame update
    void Start()
    {
        
        anim = GetComponent<Animator>();
        _unitMove = GetComponent<UnitMove>();

    }

    private void FixedUpdate()
    {
        Vspeed = Input.GetAxis("Vertical");
        Hspeed = Input.GetAxis("Horizontal");
        anim.SetFloat("Vspeed",Vspeed);
        anim.SetFloat("Hspeed",Hspeed);
        Vector3 final = (transform.up * Vspeed + transform.right * Hspeed).normalized;//获取方向
        _unitMove.MoveTo(new Vector3(final.x * rate,final.y*rate,0));
        
    }

}
