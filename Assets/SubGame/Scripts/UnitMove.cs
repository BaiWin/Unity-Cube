using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    public bool isMoving;

    public float moveSpeed = 3;

    public Vector3Int direction;

    public float unit_length = 1;

    public Vector3 targetPos;

    public virtual void Start()
    {
        StopMoving();
    }

    public virtual void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) < 0.01)
        {
            StopMoving();
            Gravity();
        }
        else
        {
            isMoving = true;
        }

        if (isMoving)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, ConvinientFunc.Vector3FloatToInt(targetPos), Time.deltaTime * moveSpeed);
            transform.position = pos;
        }
    }

    public virtual void MoveTowards(Vector3Int dir)
    {
        isMoving = true;
        direction = dir;
        targetPos = ConvinientFunc.Vector3FloatToInt(transform.position) + dir;
    }

    public void StopMoving()
    {
        //direction = Vector3Int.zero;
        targetPos = ConvinientFunc.Vector3FloatToInt(transform.position);
        isMoving = false;
    }

    public virtual void TerminateMoving()
    {
        direction = Vector3Int.zero;
        targetPos = ConvinientFunc.Vector3FloatToInt(transform.position);
        isMoving = false;
    }

    public virtual void Gravity()
    {

    }
}
