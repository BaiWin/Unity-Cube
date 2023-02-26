using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class ObstacleDetection : MonoBehaviour
{
    CharacterMovement characterMovement;
    PlayerUnitMove player_unitMove;

    void Awake()
    {
        characterMovement = this.GetComponent<CharacterMovement>();
        player_unitMove = this.GetComponent<PlayerUnitMove>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag == "Stairs")
        //{
        //    characterMovement.player.action = PlayerInfo.Action.Climbing;
        //    player_unitMove.TerminateMoving();
        //}
        if (other.gameObject.tag == "Pushable")
        {
            if (other.gameObject.GetComponent<CubeUnitMove>().isMoving)
            {
                //Player stay still.Object move
                if(characterMovement.player.action == PlayerInfo.Action.Sliping && other.GetComponent<CubeUnitMove>().isMoving)
                {
                    player_unitMove.MoveTowards(other.GetComponent<CubeUnitMove>().direction);
                }
                //Player move.Object move
                if(characterMovement.player.action == PlayerInfo.Action.Sliding && other.GetComponent<CubeUnitMove>().isMoving)
                {
                    player_unitMove.TerminateMoving();
                    other.GetComponent<CubeUnitMove>().TerminateMoving();
                    characterMovement.player.action = PlayerInfo.Action.Sliping;
                }
            }
            //Player Push Object(Play move.Object stay still)
            if(player_unitMove.isMoving && !other.gameObject.GetComponent<CubeUnitMove>().isMoving)
            {
                characterMovement.isObstacle = true;
                //collision.transform.position += new Vector3(0, 0.1f, 0);
                other.transform.GetComponent<PushableCube>().Push(player_unitMove.direction);
            }
        }
        else
        {
            characterMovement.isObstacle = false;
        }

        if(other.gameObject.tag == "Trampoline")
        {
            player_unitMove.TerminateMoving();
        }
    }
}
