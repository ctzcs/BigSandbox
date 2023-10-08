using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Optimization;

public class Seeker : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 20;
    public float turnSpeed = 3;
    public float turnDistance = 5;
    public float stoppingDst = 10;

    //目标移动后重新寻路
    public float pathUpdateMoveThresHold = 0.5f;
    public float minPathUpdateTime = 0.2f;

    Path path;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] points, bool success)
    {
        if (success)
        {
            //Vector3[] allPoints = new Vector3[points.Length + 1];
            //for (int i = 0; i < points.Length; i++) 
            //{
            //    allPoints[i] = points[i];
            //}
            //allPoints[points.Length] = target.position;
            path = new Path(points, transform.position, turnDistance, stoppingDst);
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }

    }

    IEnumerator UpdatePath() 
    {
        if (Time.timeSinceLevelLoad < .3f) 
        {
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThrehold = pathUpdateMoveThresHold * pathUpdateMoveThresHold;
        Vector3 targetPosOld = target.position;

        while (true) 
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThrehold)
            { 
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
           
        }
    }

    IEnumerator FollowPath() 
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[pathIndex]);

        float speedPercent = 1;

        while (followingPath) 
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            //HACK 平滑寻路路径
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos)) 
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else 
                {
                    pathIndex++;
                }
            }

            if (followingPath) 
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0) 
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos) / stoppingDst);
                    if (speedPercent < 0.01f) 
                    {
                        followingPath = false;
                    }
                }

                Vector3 dir = path.lookPoints[pathIndex] - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
                transform.Translate(dir.normalized * Time.deltaTime * moveSpeed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
            path.DrawWithGizmos();
    }
}
