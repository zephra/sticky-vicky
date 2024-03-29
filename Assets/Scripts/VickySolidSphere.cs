﻿using System;
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
    private bool doExplosionLater;

    
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

    private void LateUpdate()
    {
        if (doExplosionLater)
        {
            var force = chargeForce + chargeForce * chargeTime * chargeMultiplier;

            //jump up a bit
            var totalForce = Vector3.zero;
            totalForce += force * 0.12f * Vector3.up;

            foreach (var script in stickedObjects)
            {
                var dir = (script.transform.position - transform.position).normalized;
                var massMultiplier = script.rb.mass * massFactor;
                var finalForce = force + force * massMultiplier;
                script.rb.AddForce(finalForce * dir);
                totalForce += finalForce * -dir;
                //                Debug.DrawLine(transform.position, transform.position + dir * 5, Color.red, 2);
                //                Debug.Log("Launching stuff with "+Mathf.Min(finalForce, maxVickyPushForce)+" force!");
                script.stuckToVicky = false;
            }
            stickedObjects.Clear();

            //minimise total force
            rb.AddForce(Math.Min(totalForce.magnitude, maxVickyPushForce) * totalForce.normalized);

            //Debug.DrawLine(transform.position, transform.position + totalForce * 5, Color.red, 2);

            chargeCooldown = chargeCooldownTime;
            chargeTime = -1;
            doExplosionLater = false;
        }
    }        

    public void InputRotate(Vector3 dir, Vector3 axis, float accel)
    {
        RotateSphereAroundAxis(dir, axis, accel);
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
            doExplosionLater = true;
        }
        chargeParticles.SetActive(false);
    }

    private void RotateSphereAroundAxis(Vector3 movementDirection, Vector3 axis, float accel)
    {
        //get total mass to calculate force needed for rotational acceleration
        var totalMass = rb.mass;
//        var totalMass = 0f;
        foreach(ObjectStickScript script in stickedObjects)
        {
            totalMass += script.rb.mass;

        }

        var baseForce = accel * totalMass * Time.fixedDeltaTime;
        var torque = baseForce * (1 + massRotateBonus * totalMass);
        //Debug.Log("torque = " + torque);

        //help push the objects around a little, don't rely on torque too much
        foreach (ObjectStickScript script in stickedObjects)
        {
            var particlePoint = script.transform.position;
            var vectorFromCenter = particlePoint - transform.position;
            var projectedPoint = Vector3.Project(vectorFromCenter, axis) + transform.position;
            var vectorFromProjected = particlePoint - projectedPoint;
            var direction = Vector3.Cross(axis, vectorFromProjected).normalized;

            var body = script.rb;
            var magnitudeInDir = Vector3.Dot(body.velocity, direction);

            Debug.DrawLine(script.transform.position, script.transform.position + direction*3);
            if (magnitudeInDir < maxSpin)
            {
                body.AddForce(Time.fixedDeltaTime * torque *0.08f * direction);
            }
        }

        //TODO add max spin?
        rb.AddTorque( torque * axis);
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
