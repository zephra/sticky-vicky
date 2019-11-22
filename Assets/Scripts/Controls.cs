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
    
//    public Vector3 center { get; private set; }
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
        
//        cameraVector = GetComponent<Camera>().transform.position - transform.position;
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
//            var forwardXZ = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
//            PushParticlesAroundAxis(center, forwardXZ, camera.transform.right, inputForce);
            onInputUp.Invoke(inputForce);
        }
        if (Input.GetKey(KeyCode.S)) // DOWN
        {
//            var forwardXZ = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
//            PushParticlesAroundAxis(center, -forwardXZ, -camera.transform.right, inputForce);
            onInputDown.Invoke(inputForce);
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
//            var rightXZ = new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
//            PushParticlesAroundAxis(center, -rightXZ, camera.transform.forward, inputForce);
            onInputLeft.Invoke(inputForce);
        }
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
//            var rightXZ = new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
//            PushParticlesAroundAxis(center, rightXZ, -camera.transform.forward, inputForce);
            onInputRight.Invoke(inputForce);
        }
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
