using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUnitMove : UnitMove
{
    PushableCube pushableCube;
    public override void Start()
    {
        base.Start();
        pushableCube = GetComponent<PushableCube>();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void TerminateMoving()
    {
        this.GetComponent<PushableCube>()._dir = Vector3Int.zero;
        targetPos = ConvinientFunc.Vector3FloatToInt(transform.position);
        isMoving = false;
    }

    public override void Gravity()
    {
        if (MapContainer.Instance.GetMapValue(pushableCube.x, pushableCube.y - 1, pushableCube.z) == 0 && !isMoving)
        {
            MoveTowards(Vector3Int.down);
        }
    }
}
