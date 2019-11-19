using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableSphere : MonoBehaviour
{
    public GameObject particle;
    public float minThreshold = 0.3f;
    public float maxThreshold = 0.99f;
    public float springDampening = 0.75f;
    public float springSpring = 10f;
    public float midSpringDampening = 0.75f;
    public float midSpringSpring = 10f;
    public float inputForce = 10;
    
    private Mesh mesh;
    private Vector3[] vertices;
//    private List<SpringJoint> jumpSprings;

    private GameObject[] particles;
    
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        vertices = mesh.vertices;
        
        particles = new GameObject[vertices.Length+1];
//        jumpSprings = new List<SpringJoint>();
        
        Debug.Log("Mesh been loaded fo sho, "+vertices.Length+" vertices");

        for (int i = 0; i < vertices.Length; i++)
        {
            var pos1 = transform.TransformPoint(vertices[i]);
            particles[i] = Instantiate(particle, pos1, Quaternion.identity);
        }

        var pos2 = gameObject.transform.position;
        var midParticle = Instantiate(particle, pos2, Quaternion.identity);
        particles[particles.Length - 1] = midParticle;

        for (int i = 0; i < particles.Length-1; i++)
        {
            var p = particles[i];
            for (int j = 0; j < particles.Length-1; j++)
            {
                if (i == j) continue;
                
                var q = particles[j];
                var dist = Vector3.Distance(p.transform.position, q.transform.position);
                if (dist < minThreshold)
                {
                    AddSpring(p, q, springDampening, springSpring);
                    
//                    var springJoint = AddSpring(p, q);
//                    if (dist > maxThreshold)
//                        jumpSprings.Add(springJoint);
                }
            }
            AddSpring(p, midParticle, midSpringDampening, midSpringSpring);
        }
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < particles.Length-1; i++)
        {
            vertices[i] = particles[i].transform.position;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A)) // LEFT
        {
            ApplyForceToParticles(Vector3.left, inputForce);
        }
        if (Input.GetKey(KeyCode.D)) // RIGHT
        {
            ApplyForceToParticles(Vector3.right, inputForce);
        }
        if (Input.GetKey(KeyCode.W)) // UP
        {
            ApplyForceToParticles(Vector3.forward, inputForce);
//            particles[particles.Length-1].transform.Rotate(10f, 0, 0, Space.World);
        }
        if (Input.GetKey(KeyCode.S)) // DOWN
        {
            ApplyForceToParticles(Vector3.back, inputForce);
        }
        
//        if (Input.GetKey(KeyCode.Space)) // JUMP
//        {
//            foreach (var s in jumpSprings)
//            {
////                s.
//            }
//        }
    }

    private SpringJoint AddSpring(GameObject particle1, GameObject particle2, float dampening, float spring)
    {
        var springJoint = particle1.AddComponent<SpringJoint>();
        springJoint.connectedBody = particle2.GetComponent<Rigidbody>();
        springJoint.damper = dampening;
        springJoint.spring = spring;
        return springJoint;
    }
    
    private FixedJoint AddFixed(GameObject particle1, GameObject particle2, float dampening, float spring)
    {
        var fixedJoint = particle1.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = particle2.GetComponent<Rigidbody>();
        return fixedJoint;
    }

    private void ApplyForceToParticles(Vector3 direction, float force)
    {
        foreach (GameObject p in particles)
        {
            p.GetComponent<Rigidbody>().AddForce(Time.fixedDeltaTime * force * direction);
        }
    }
}
