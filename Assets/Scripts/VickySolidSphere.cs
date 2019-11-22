using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VickySolidSphere : MonoBehaviour
{
    public Camera camera;
    public Controls controls;
    public GameObject chargeParticles;
    [Space]
    public float massRotateBonus = 2.1f;
    public float slideMultiplier = 0.1f;
    public float maxSpin = 15;

    [Space]
    public float maxChargeTime = 2;
    public float minChargeTime = 0.3f;
    public float chargeCooldownTime = 0.5f;
    public float chargeForce = 500;
    public float chargeMultiplier = 5;
    public float massFactor = 1.5f;
    public float maxVickyPushForce = 2000;

    [Space]
    public List<ObjectStickScript> stickedObjects;
    public List<Joint> joints;


    private Rigidbody rb;

    private float chargeTime;
    private bool spaceDown;
    private float chargeCooldown;

    
    // Start is called before the first frame update
    void Start()
    {

        controls.center = transform.position;

        rb = GetComponent<Rigidbody>();

        controls.SaveCameraPosition();

        chargeTime = -1;
        chargeCooldown = 0;
        chargeParticles.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        controls.center = transform.position;
        chargeParticles.transform.position = transform.position;
    }

    private void FixedUpdate()
    {
        if (chargeTime >= 0 && chargeTime < maxChargeTime) chargeTime += Time.fixedDeltaTime;
        if (chargeTime > maxChargeTime) chargeTime = maxChargeTime;
        if (chargeCooldown > 0) chargeCooldown -= Time.fixedDeltaTime;
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

    public void StartExplodeCharge()
    {
        if (chargeTime < 0)
        {
            chargeTime = 0;
            chargeParticles.SetActive(true);
        }
        
    }
    public void ReleaseExplodeCharge()
    {
        if (chargeTime >= minChargeTime)
        {
            foreach (var j in joints)
            {
                Destroy(j);
            }
            joints.Clear();
            
            var force = chargeForce + chargeForce * chargeTime * chargeMultiplier;
            foreach (var script in stickedObjects)
            {
                var dir = (script.transform.position - transform.position).normalized;
                var massMultiplier = script.rb.mass * massFactor;
                var finalForce = force + force * massMultiplier;
                script.rb.AddForce( finalForce  * dir);
                rb.AddForce(Mathf.Min(finalForce, maxVickyPushForce) * -dir);
//                Debug.DrawLine(transform.position, transform.position + dir * 5, Color.red, 2);
//                Debug.Log("Launching stuff with "+Mathf.Min(finalForce, maxVickyPushForce)+" force!");
                script.stuckToVicky = false;
            }
            stickedObjects.Clear();

            chargeCooldown = chargeCooldownTime;
        }
        chargeTime = -1;
        chargeParticles.SetActive(false);
    }

    private void RotateSphereAroundAxis(Vector3 movementDirection, Vector3 axis, float accel)
    {
        //get total mass to calculate force needed for rotational acceleration
        var totalMass = rb.mass;
        foreach(ObjectStickScript script in stickedObjects)
        {
            var obj = script.gameObject;
            totalMass += obj.GetComponent<Rigidbody>().mass;
        }

        var baseForce = accel * totalMass * Time.fixedDeltaTime;
        //TODO add max spin?
        rb.AddTorque(baseForce * (1 + massRotateBonus * totalMass) * axis);
        rb.AddForce(baseForce * slideMultiplier * movementDirection);
        
    }
    
    public void AttachObjectToAnother(ObjectStickScript anchorScript, ObjectStickScript extendedScript)
    {
        if (chargeCooldown > 0 || !stickedObjects.Contains(anchorScript) || stickedObjects.Contains(extendedScript)) return;
        AddFixedJoint(anchorScript.gameObject, extendedScript.rb);
        stickedObjects.Add(extendedScript);
        extendedScript.stuckToVicky = true;
    }
    
    public void AttachObject(ObjectStickScript objectScript)
    {
        if (chargeCooldown > 0 || stickedObjects.Contains(objectScript)) return;
        AddFixedJoint(gameObject, objectScript.rb);
        stickedObjects.Add(objectScript);
        objectScript.stuckToVicky = true;
    }

    public Joint AddFixedJoint(GameObject anchor, Rigidbody extendedRB)
    {
        var joint = anchor.AddComponent<FixedJoint>();
        joint.connectedBody = extendedRB;
        joints.Add(joint);
        return joint;
    }
}
