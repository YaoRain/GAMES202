using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Matrix4x4 mat = this.transform.localToWorldMatrix;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
