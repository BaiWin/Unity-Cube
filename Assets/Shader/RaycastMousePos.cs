using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastMousePos : MonoBehaviour
{
    Camera _camera;
    RaycastHit hit;
    Ray ray;
    Vector3 _mousePos, _smoothPoint;
    public float _radius, _softness, _smoothSpeed, _scaleFactor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            _radius += _scaleFactor * Time.deltaTime;
        }

        Mathf.Clamp(_radius, 0, 100);
        Mathf.Clamp(_softness, 0, 100);

        _mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        ray = Camera.main.ScreenPointToRay(_mousePos);

        if(Physics.Raycast(ray, out hit))
        {
            _smoothPoint = Vector3.MoveTowards(_smoothPoint, hit.point, _smoothSpeed * Time.deltaTime);
            Vector4 pos = new Vector4(_smoothPoint.x, _smoothPoint.y, _smoothPoint.z, 0);
            Shader.SetGlobalVector("GLOBAL_Position", pos);
        }
        Shader.SetGlobalFloat("GLOBAL_Radius", _radius);
        Shader.SetGlobalFloat("GLOBAL_Softness", _softness);
    }
}
