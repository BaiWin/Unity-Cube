using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    public GameObject particle;
    public Material frozen_material;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name != "Player")
        {
            GameObject obj = Instantiate(particle, transform.position, Quaternion.identity);
            obj.AddComponent<DestroySelf>();
            Destroy(this.gameObject);
        }
        if(other.gameObject.tag == "Pushable")
        {
            int len = other.GetComponent<MeshRenderer>().materials.Length;
            if(len == 1)
            {
                Material[] materials = new Material[len + 1];
                materials[0] = other.GetComponent<MeshRenderer>().materials[0];
                materials[1] = frozen_material;
                other.GetComponent<MeshRenderer>().materials = materials;
                other.GetComponent<PushableCube>().pushableObj.state = PushableObjInfo.State.Frozen;
            }
        }
    }
}
