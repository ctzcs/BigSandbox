using UnityEngine;

public class UnitMove : MonoBehaviour
{
    public void MoveTo(Vector3 moveto)
    {
        transform.localPosition += new Vector3(
            moveto.x*Time.fixedDeltaTime,
            moveto.y*Time.fixedDeltaTime,
            0
        );
    }
}
