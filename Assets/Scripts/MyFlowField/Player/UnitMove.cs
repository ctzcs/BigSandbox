using System;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void MoveTo(Vector3 moveto)
    {
        rb.velocity = moveto;
        // transform.localPosition += new Vector3(
        //     moveto.x*Time.fixedDeltaTime,
        //     moveto.y*Time.fixedDeltaTime,
        //     0
        // );
    }
}
