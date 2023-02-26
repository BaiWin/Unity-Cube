using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableCollision : MonoBehaviour
{
    GameObject collisionObj;

    private void Update()
    {
        if(collisionObj != null)
        {
            Shader.SetGlobalVector("GLOBAL_ObjWorldPos", transform.position);

            float distance = Vector3.Distance(collisionObj.transform.position, transform.position);
            if(distance > 2f)
            {
                Shader.SetGlobalInt("GLOBAL_Attachment", 0);
                collisionObj = null;
            }
        }
        //Debug.Log(Shader.GetGlobalInt("GLOBAL_Attachment"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Trampoline")
        {
            Shader.SetGlobalInt("GLOBAL_Attachment", 1);
            Vector3 dir = transform.position - other.transform.position;
            dir.y = 0;
            Debug.Log(dir);
            Shader.SetGlobalFloat("GLOBAL_ObjDistance", Vector3.Magnitude(dir));
            dir = Vector3.Normalize(dir);
            dir.x = Mathf.RoundToInt(dir.x);
            dir.y = 0;
            dir.z = Mathf.RoundToInt(dir.z);
            Shader.SetGlobalVector("GLOBAL_Direction", dir);
            Debug.Log(dir);
            collisionObj = other.gameObject;

            this.GetComponent<PushableCube>().Push(-this.GetComponent<PushableCube>()._dir);
        }

        //if(other.gameObject.name == "Player" && this.GetComponent<CubeUnitMove>().isMoving)
        //{
        //    //this.GetComponent<CubeUnitMove>().TerminateMoving();
        //}
    }
}
