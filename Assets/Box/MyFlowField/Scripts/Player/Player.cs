using System;
using System.Collections;
using System.Collections.Generic;
using MyFlowField.Scripts.Player;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim;

    public float Vspeed, Hspeed;

    public float rate;

    private UnitMove _unitMove;

    public HitBox hitBox;
    // Start is called before the first frame update
    void Start()
    {
        
        anim = GetComponent<Animator>();
        _unitMove = GetComponent<UnitMove>();
        /*Application.targetFrameRate = 75;*/
        hitBox.onHitBoxEnter.AddListener(HitBox);
        
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Monster"))
        {
            col.transform.position -= (col.transform.position - this.transform.position)*10;
        }
    }

    void HitBox()
    {
        Debug.Log("在Player中使用Hitbox");
    }
}
