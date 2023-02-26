using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitMove : UnitMove
{
    CharacterMovement characterMovement;
    public override void Start()
    {
        base.Start();
        characterMovement = GetComponent<CharacterMovement>();
    }

    public override void Update()
    {
        base.Update();

    }

    public override void Gravity()
    {
        if (MapContainer.Instance.GetMapValue(characterMovement.current_pointerCube) == 0 && !isMoving)
        {
            if(characterMovement.player.action == PlayerInfo.Action.Climbing) { return; }
            MoveTowards(Vector3Int.down);
        }
    }
}
