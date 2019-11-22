using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VickySolidSphere : MonoBehaviour
{
    public Camera camera;
    public Controls controls;
    [Space]
    public float maxSpin = 15;
    
    public List<GameObject> stickedObjects;

    private Rigidbody rb;

    
    // Start is called before the first frame update
    void Start()
    {

        controls.center = transform.position;

        rb = GetComponent<Rigidbody>();

        controls.SaveCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {
        controls.center = transform.position;
    }

    public void InputUp(float accel)
    {
        var forwardXZ = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
        RotateSphereAroundAxis(forwardXZ, camera.transform.right, accel);
    }

    public void InputDown(float accel)
    {
        var forwardXZ = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
        RotateSphereAroundAxis(-forwardXZ, -camera.transform.right, accel);
    }

    public void InputLeft(float accel)
    {
        var rightXZ = new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
        RotateSphereAroundAxis(-rightXZ, camera.transform.forward, accel);
    }

    public void InputRight(float accel)
    {
        var rightXZ = new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
        RotateSphereAroundAxis(rightXZ, -camera.transform.forward, accel);
    }

//    private SpringJoint AddSpring(GameObject particle1, GameObject particle2, float dampening, float spring)
//    {
//        var springJoint = particle1.AddComponent<SpringJoint>();
//        springJoint.connectedBody = particle2.GetComponent<Rigidbody>();
//        springJoint.damper = dampening;
//        springJoint.spring = spring;
//        return springJoint;
//    }

    private void RotateSphereAroundAxis(Vector3 movementDirection, Vector3 axis, float accel)
    {
        //get total mass to calculate force needed for rotational acceleration
        var totalMass = rb.mass;
        foreach(GameObject obj in stickedObjects)
        {
            totalMass += obj.GetComponent<Rigidbody>().mass;
        }

        var baseForce = accel * accel * totalMass * Time.fixedDeltaTime;
        //TODO add max spin?
        rb.AddTorque(baseForce * totalMass * axis);
        rb.AddForce(baseForce * 0.2f * movementDirection);
        
    }
    
    public void AttachObject(ObjectStickScript objectScript)
    {
        if (stickedObjects.Contains(objectScript.gameObject)) return;
//        objectScript.rb.isKinematic = true;
        //objectScript.rb.mass *= 0.01f;
//        objectScript.transform.SetParent(transform);
//        stickedObjects.Add(objectScript.gameObject);
//        AddSpringJoint(objectScript.rb);
        AddFixedJoint(objectScript.rb);
    }

//    public Joint AddSpringJoint(Rigidbody otherRB)
//    {
//        var joint = gameObject.AddComponent<SpringJoint>();
//        joint.connectedBody = otherRB;
//        joint.damper = 10;
//        joint.spring = 10;
//        stickedObjects.Add(otherRB.gameObject);
//        return joint;
//    }
    public Joint AddFixedJoint(Rigidbody otherRB)
    {
        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = otherRB;
        stickedObjects.Add(otherRB.gameObject);
        return joint;
    }
}
