using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5;

    Vector2 camSizeCap;

    public Camera cam;

    private void Start() {
        camSizeCap = new Vector2(100, 10);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.Q)){
            cam.orthographicSize += zoomSpeed;
        }
        if(Input.GetKey(KeyCode.E)){
            cam.orthographicSize -= zoomSpeed;
        }
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, camSizeCap.y, camSizeCap.x);

        Vector3 movePos = new Vector3();
        movePos.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        movePos.y = 0.0f;
        movePos.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        

        transform.position += movePos;

    }

}
