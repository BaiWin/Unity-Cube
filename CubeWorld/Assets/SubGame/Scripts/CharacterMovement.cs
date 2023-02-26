using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour, ICubePointer
{
    //Move & Rotate
    private PlayerUnitMove unit_move;
    private PlayerInput input;

    public PlayerInfo player;

    public bool isObstacle;

    public Vector3Int current_pointerCube
    {
        get; set;
    }

    //Cinemachine
    public CinemachineVirtualCamera virtualCamera;
    private CinemachinePOV pov;
    private float alterAngle;

    //Hold Item
    public GameObject hold;

    //Fire
    public Transform shootPoint;
    public GameObject projectile;

    //Helper
    Vector2 lastInput;

    void Awake()
    {
        input = new PlayerInput();
        input.Player.Shoot.performed += ctx =>
        {
            Fire();
        };
        Transform trans = this.transform;
        int height = Mathf.RoundToInt(trans.GetComponent<CapsuleCollider>().height);
        player = new PlayerInfo(this.gameObject, height);

        if(virtualCamera != null)
        {
            pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }
        unit_move = player.playerObj.GetComponent<PlayerUnitMove>();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        //Initialize the pointerCube
        UpdatePointerCube();
        //////next_pointerCube = current_pointerCube;
    }

    void Update()
    {
        //Camera
        AlterAxis();
        //
        HoldRotAdjust();
        //Update the cube player points to
        UpdatePointerCube();

        //Get Input and convert it to world coord
        var move = input.Player.Move.ReadValue<Vector2>();
        Vector3 alteredDir;
        Vector3Int movePos = InputToWorldPos(move, out alteredDir);
        Vector3Int moveDir = ConvinientFunc.Vector3FloatToInt(alteredDir);

        if (unit_move.isMoving && player.action != PlayerInfo.Action.Climbing)
        {
            MovingDetection();
        }

        //Input
        if(move == lastInput && unit_move.isMoving) return;
        lastInput = move;

        if(player.action == PlayerInfo.Action.Climbing)
        {
            if (CheckStairsUpward(move))
            {
                Climb(move);
            }
            

            if(LocatePointerClimb(movePos))
            {
                player.action = PlayerInfo.Action.Moving;
                unit_move.MoveTowards(moveDir);
            }
        }

        if (player.action == PlayerInfo.Action.Moving)
        {
            bool isMove = true;
            if (CheckIfStairsForward(movePos))
            {
                isMove = false;
                player.action = PlayerInfo.Action.Climbing;
            }
            if (CheckIfStairsForDownward(movePos))
            {
                player.action = PlayerInfo.Action.Climbing;
                isMove = true;
            }
            if(isMove)
            {
                //Check if it is able to move
                if (LocatePointerMove(movePos))
                {
                    unit_move.MoveTowards(moveDir);
                }
            }
        }

        if (player.action == PlayerInfo.Action.Sliding)
        {
            Vector3Int slidPos = unit_move.direction + current_pointerCube;
            if (LocatePointerSlide(slidPos))
            {
                unit_move.MoveTowards(unit_move.direction);
            }
        }

        if (player.action == PlayerInfo.Action.Sliping)
        {
            Debug.Log("wait for hit");
        }
    }

    void HoldRotAdjust()
    {
        pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        float x = pov.m_HorizontalAxis.Value;
        float y = pov.m_VerticalAxis.Value;
        hold.transform.eulerAngles = new Vector3(y, x, 0);
    }

    void AlterAxis()
    {
        pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        float x = pov.m_HorizontalAxis.Value;
        if ((x < 45 && x > 0) || (x < 0 && x > -45))
        {
            //Debug.Log("+z");
            alterAngle = 0;
        }
        if (x > 45 && x < 135)
        {
            //Debug.Log("+x");
            alterAngle = 90;
        }
        if ((x > 135 && x < 180) || (x > -180 && x < -135))
        {
            //Debug.Log("-z");
            alterAngle = 180;
        }
        if (x > -135 && x < -45)
        {
            //Debug.Log("-x");
            alterAngle = 270;
        }
    }

    int PointerDetection()
    {
        int index = MapContainer.Instance.GetMapValue(current_pointerCube);
        return index;
    }

    void Fire()
    {
        var newProjectile = Instantiate(projectile, shootPoint.position, shootPoint.rotation);
        newProjectile.GetComponent<Rigidbody>().AddForce(shootPoint.transform.forward * 10f, ForceMode.Impulse);
    }

    Vector3Int InputToWorldPos(Vector2 dir, out Vector3 altered_dir)
    {
        altered_dir = Vector3.zero;
        Vector3Int v = Vector3Int.zero;
        if (dir.sqrMagnitude < 0.01) return v;
        if (dir.x != 0 && dir.y != 0) return v; //Limit player move only in 1 axis

        altered_dir = Quaternion.Euler(0, alterAngle, 0) * new Vector3(dir.x, 0, dir.y);
        int aim_x = Mathf.RoundToInt(current_pointerCube.x + altered_dir.x); // World X
        int aim_z = Mathf.RoundToInt(current_pointerCube.z + altered_dir.z); // World Z
        //Make sure the cube not go out of the limitation of the index
        if (aim_x > MapContainer.Instance.max_X || aim_x < MapContainer.Instance.min_X || aim_z > MapContainer.Instance.max_Z || aim_z < MapContainer.Instance.min_Z) return v;

        v = new Vector3Int(aim_x, 0, aim_z);
        return v;
    }

    //Input: Dir.X-X, Dir.Y-Z
    bool LocatePointerMove(Vector3Int dir)
    {
        if(dir.sqrMagnitude < 0.01) return false;

        //Do animation depends on dir ...

        //Avoid the double movement
        //if(LimitDoubleMovement(new Vector3(aim_x, current_pointerCube.y, aim_z))) return false;
        
        // + Obstacle detection
        if(isObstacle)
        {

        }

        int y = current_pointerCube.y;
        //The case has block forward
        if (!CheckBlockForward(dir.x, y, dir.z))
        {
            if (CheckNextMove(dir.x, y, dir.z)|| CheckDropDown(dir.x, y, dir.z))
            {
                return true;
            }
        }
        return false;
    }

    bool LocatePointerClimb(Vector3Int dir)
    {
        if(dir.sqrMagnitude < 0.01) return false;

        //Do animation depends on dir ...
 
        // + Obstacle detection
        if(isObstacle)
        {

        }

        int y = (int)current_pointerCube.y;
        if (CheckNextMove(dir.x, y, dir.z)) return true;
        return false;
    }

    bool LocatePointerSlide(Vector3Int dir)
    {
        if (dir.sqrMagnitude < 0.01) return false;
        //Do animation depends on dir ...

        // + Obstacle detection
        if (isObstacle)
        {

        }

        int y = (int)current_pointerCube.y;
        if (MapContainer.Instance.GetMapValue(dir.x, y, dir.z) != 0)
        {
            return true;
        }
        return false;
    }

    public void UpdatePointerCube()
    {
        Vector3Int v = ConvinientFunc.Vector3FloatToInt(player.playerObj.transform.position);
        current_pointerCube = v - new Vector3Int(0, player.playerHeight, 0);
    }

    void Climb(Vector2 dir)
    {
        int vertical = Mathf.RoundToInt(dir.y);
        if(vertical == 0) { return; }
        unit_move.MoveTowards(new Vector3Int(0, vertical, 0));
    }

    void MovingDetection()
    {
        switch (PointerDetection())
        {
            case 1:
                if(player.action != PlayerInfo.Action.Climbing)
                {
                    player.action = PlayerInfo.Action.Moving;
                }
                //PlayAudio
                AudioManager.instance.Play("Walk_Grass");
                break;
            case 3:
                //Start sliding
                player.action = PlayerInfo.Action.Sliding;
                //If obstacle,switch to slip
                if (unit_move.isMoving == false)
                {
                    unit_move.StopMoving();
                    player.action = PlayerInfo.Action.Sliping;
                }
                break;
            case 4:
                //PlayAudio
                AudioManager.instance.Play("Walk_Sand");
                break;
            case 6:
                //PlayAudio
                AudioManager.instance.Play("Walk_Ice");
                break;
            default:
                
                //Start normal moving


                break;
        }
    }

    private bool CheckIfStairsForward(Vector3Int dir)
    {
        if (dir.sqrMagnitude < 0.01) return false;

        //Do animation depends on dir ...

        int y = current_pointerCube.y;
        //The case has block forward
        if (MapContainer.Instance.GetMapValue(dir.x, y + 1, dir.z) == 5)
        {
            return true;
        }
        return false;
    }

    private bool CheckIfStairsForDownward(Vector3Int dir)
    {
        if (dir.sqrMagnitude < 0.01) return false;

        //Do animation depends on dir ...

        int y = current_pointerCube.y;
        if (MapContainer.Instance.GetMapValue(dir.x, y, dir.z) == 5)
        {
            return true;
        }
        return false;
    }

    private bool CheckStairsUpward(Vector2 dir)
    {
        int vertical = Mathf.RoundToInt(dir.y);
        if(vertical == 0) { return false; }
        if(vertical < 0) { return true; }
        Vector3Int targetPointer = current_pointerCube + new Vector3Int(0, vertical, 0);
        if (MapContainer.Instance.GetMapValue(targetPointer.x, targetPointer.y, targetPointer.z) == 5)
        {
            return true;
        }
        return false;
    }

    private bool CheckBlockForward(int x, int y, int z)
    {
        if (MapContainer.Instance.GetMapValue(x, y + 1, z) == 1)
        {
            return true;
        }
        return false;
    }

    private bool CheckNextMove(int x, int y, int z)
    {
        if (MapContainer.Instance.GetMapValue(x, y, z) != 0)
        {
            return true;
        }
        return false;
    }

    private bool CheckDropDown(int x, int y, int z)
    {
        //while (y >= MapContainer.Instance.min_Y)
        //{
        //    if (MapContainer.Instance.GetMapValue(x, y, z) != 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        y--;
        //    }
        //}
        return false;
    }
}
