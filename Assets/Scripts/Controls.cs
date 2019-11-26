using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour
{
    public Camera camera;
    public float inputAccel = 500;
    public float camRotationSpeed = 10;
    
    public UnityEvent onSpaceDown;
    public UnityEvent onSpaceUp;
    
    public RotateEvent onRotateInput;

    private bool spaceDown;

    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [System.Serializable]
    public class RotateEvent : UnityEvent<Vector3, Vector3, float> { }

    [HideInInspector] public Vector3 center;
    
    private Vector3 cameraVector;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (onRotateInput == null)
            onRotateInput = new RotateEvent();
        
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

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SceneManager.LoadScene("MainScene");
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene("CatFishBattlefield");
        }


        Vector3 direction = Vector3.zero;
        Vector3 thumbAxis = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) // UP
        {
            direction += new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
            thumbAxis += camera.transform.right;
        }
        if (Input.GetKey(KeyCode.S)) // DOWN
        {
            direction -= new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
            thumbAxis -= camera.transform.right;
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) // LEFT
        {
            direction += new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
            thumbAxis += camera.transform.forward;
        }
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            direction -= new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
            thumbAxis -= camera.transform.forward;
        }
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) // BLEFT
        {
            thumbAxis += Vector3.down;
        }
        if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.DownArrow))
        {
            thumbAxis += Vector3.up;
        }
        direction = direction.normalized;
        thumbAxis = thumbAxis.normalized;
        if (direction + thumbAxis != Vector3.zero)
            onRotateInput.Invoke(direction, thumbAxis, inputAccel);


        if (!spaceDown && Input.GetKey(KeyCode.Space)) // SPACE DOWN
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
