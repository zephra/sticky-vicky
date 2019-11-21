using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VickySphere : MonoBehaviour
{
    public GameObject particle;
    public Camera camera;
    [Space]
    public float minThreshold = 0.3f;
    public float maxThreshold = 0.99f;
    public float springDampening = 0.75f;
    public float springStrength = 10f;
    [Space]
    public float inputForce = 10;
    public float camRotationSpeed = 10;
    public float maxSpin = 15;
    
    private Mesh mesh;
    private Vector3[] vertices;
//    private List<SpringJoint> jumpSprings;

    private GameObject[] particles;
    private Rigidbody[] rigidbodies;
    public Vector3 center { get; private set; }
    public GameObject anchorParticle { get; private set; }

    private Vector3 cameraVector;
    
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        vertices = mesh.vertices;
        
        particles = new GameObject[vertices.Length];
//        jumpSprings = new List<SpringJoint>();
        rigidbodies = new Rigidbody[vertices.Length];
        
        Debug.Log("Mesh been loaded fo sho, "+vertices.Length+" vertices");

        var posSum = Vector3.zero;
        for (int i = 0; i < vertices.Length; i++)
        {
            var pos1 = transform.TransformPoint(vertices[i]) + transform.position;
            particles[i] = Instantiate(particle, pos1, Quaternion.identity, transform);
            rigidbodies[i] = particles[i].GetComponent<Rigidbody>();
            posSum += pos1;
        }
        center = posSum / vertices.Length;
        foreach (GameObject p in particles)
        {
            p.transform.forward = center - p.transform.position;
        }

        for (int i = 0; i < particles.Length; i++)
        {
            var p = particles[i];
            for (int j = 0; j < particles.Length; j++)
            {
                if (i == j) continue;
                
                var q = particles[j];
                var dist = Vector3.Distance(p.transform.position, q.transform.position);
                if (dist < minThreshold || dist > maxThreshold)
                {
                    AddSpring(p, q, springDampening, springStrength);
                    
//                    var springJoint = AddSpring(p, q);
//                    if (dist > maxThreshold)
//                        jumpSprings.Add(springJoint);
                }
            }
        }

        cameraVector = camera.transform.position - transform.position;
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        //make anchor
        anchorParticle = Instantiate(particle, center, Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update()
    {
        var posSum = Vector3.zero;
        for (int i = 0; i < particles.Length; i++)
        {
            vertices[i] = particles[i].transform.position;
            posSum += vertices[i];
        }

        center = posSum / vertices.Length;
        anchorParticle.transform.position = center;
        anchorParticle.transform.forward = particles[0].transform.position - center;


        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();;
    }

    private void FixedUpdate()
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
            var forwardXZ = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
            PushParticlesAroundAxis(center, forwardXZ, camera.transform.right, inputForce);
        }
        if (Input.GetKey(KeyCode.S)) // DOWN
        {
            var forwardXZ = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
            PushParticlesAroundAxis(center, -forwardXZ, -camera.transform.right, inputForce);
        }
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            var rightXZ = new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
            PushParticlesAroundAxis(center, -rightXZ, camera.transform.forward, inputForce);
        }
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            var rightXZ = new Vector3(camera.transform.right.x, 0, camera.transform.right.z);
            PushParticlesAroundAxis(center, rightXZ, -camera.transform.forward, inputForce);

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

    private SpringJoint AddSpring(GameObject particle1, GameObject particle2, float dampening, float spring)
    {
        var springJoint = particle1.AddComponent<SpringJoint>();
        springJoint.connectedBody = particle2.GetComponent<Rigidbody>();
        springJoint.damper = dampening;
        springJoint.spring = spring;
        return springJoint;
    }

    private void ApplyForceToParticles(Vector3 direction, float force)
    {
        foreach (GameObject p in particles)
        {
            p.GetComponent<Rigidbody>().AddForce(Time.fixedDeltaTime * force * direction);
        }
    }

    private void RotateCamera(Vector3 centerPoint, float direction)
    {
        camera.transform.RotateAround(centerPoint, Vector3.up, direction);
        cameraVector = Quaternion.AngleAxis(direction, Vector3.up) * cameraVector;
    }

    private void PushParticlesAroundAxis(Vector3 centerPoint, Vector3 mvmtDir, Vector3 axis, float force)
    {
        for(int i = 0; i < particles.Length; i++)
        {
            var p = particles[i];
            
            var particlePoint = p.transform.position;
            var vectorFromCenter = particlePoint - centerPoint;
            var projectedPoint = Vector3.Project(vectorFromCenter, axis) + centerPoint;
            var vectorFromProjected = particlePoint - projectedPoint;
            var direction = Vector3.Cross(axis, vectorFromProjected).normalized;

            var magnitudeInDir = Vector3.Dot(rigidbodies[i].velocity, direction);
            
            if (magnitudeInDir < maxSpin)
            {
                rigidbodies[i].AddForce(Time.fixedDeltaTime * force * direction);
                //rigidbodies[i].AddTorque(Time.fixedDeltaTime * force * direction);
                rigidbodies[i].AddForce(Time.fixedDeltaTime * force * 0.4f * mvmtDir);
            }
        }
    }
}
