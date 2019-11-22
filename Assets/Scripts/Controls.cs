using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controls : MonoBehaviour
{
    public Camera camera;
    public float inputAccel = 10;
    public float camRotationSpeed = 10;
    
    public UnityEvent onSpaceDown;
    public UnityEvent onSpaceUp;
    
    public FloatEvent onInputUp;
    public FloatEvent onInputDown;
    public FloatEvent onInputRight;
    public FloatEvent onInputLeft;

    private bool spaceDown;
    
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
        
        if (onSpaceDown == null)
            onSpaceDown = new UnityEvent();
        if (onSpaceUp == null)
            onSpaceUp = new UnityEvent();

        spaceDown = false;
    }

    public void SaveCameraPosition()
    {
        cameraVector = camera.transform.position - transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A)) // CAMERA LEFT
        {
            RotateCamera(center, -camRotationSpeed);
        }
        if (Input.GetKey(KeyCode.D)) // CAMERA RIGHT
        {
            RotateCamera(center, camRotationSpeed);
        }
        if (Input.GetKey(KeyCode.W)) // UP
        {
            onInputUp.Invoke(inputAccel);
        }
        if (Input.GetKey(KeyCode.S)) // DOWN
        {
            onInputDown.Invoke(inputAccel);
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) // LEFT
        {
            onInputLeft.Invoke(inputAccel);
        }
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            onInputRight.Invoke(inputAccel);
        }
        
        if (Input.GetKeyDown(KeyCode.Space)) // SPACE DOWN
        {
            onSpaceDown.Invoke();
            spaceDown = true;
        }
        if (spaceDown && !Input.GetKey(KeyCode.Space)) // SPACE UP
        {
            onSpaceUp.Invoke();
            spaceDown = false;
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
