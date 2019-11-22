using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controls : MonoBehaviour
{
    public Camera camera;
    public float inputForce = 10;
    public float camRotationSpeed = 10;
    
    public FloatEvent onInputUp;
    public FloatEvent onInputDown;
    public FloatEvent onInputRight;
    public FloatEvent onInputLeft;
    
    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }
    
    [HideInInspector] public Vector3 center;
    
    private Vector3 cameraVector;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (onInputUp == null)
            onInputUp = new FloatEvent();
        if (onInputDown == null)
            onInputDown = new FloatEvent();
        if (onInputRight == null)
            onInputRight = new FloatEvent();
        if (onInputLeft == null)
            onInputLeft = new FloatEvent();
    }

    public void SaveCameraPosition()
    {
        cameraVector = camera.transform.position - transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A)) // LEFT
        {
            RotateCamera(center, -camRotationSpeed);
        }
        if (Input.GetKey(KeyCode.D)) // RIGHT
        {
            RotateCamera(center, camRotationSpeed);
        }
        if (Input.GetKey(KeyCode.W)) // UP
        {
            onInputUp.Invoke(inputForce);
        }
        if (Input.GetKey(KeyCode.S)) // DOWN
        {
            onInputDown.Invoke(inputForce);
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            onInputLeft.Invoke(inputForce);
        }
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            onInputRight.Invoke(inputForce);
        }
        //        if (Input.GetKey(KeyCode.Space)) // JUMP
        //        {
        //            foreach (var s in jumpSprings)
        //            {
        ////                s.
        //            }
        //        }
    }
    
    private void LateUpdate()
    {
        camera.transform.position = center + cameraVector;
    }

    private void RotateCamera(Vector3 centerPoint, float direction)
    {
        camera.transform.RotateAround(centerPoint, Vector3.up, direction);
        cameraVector = Quaternion.AngleAxis(direction, Vector3.up) * cameraVector;
    }
}
