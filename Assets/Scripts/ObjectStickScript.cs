using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStickScript : MonoBehaviour
{
    private Joint myJoint = null;

    [HideInInspector] public Rigidbody rb;

    private FixedJoint AddFixedJoint(GameObject vickyParticle)
    {
        var fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = vickyParticle.GetComponent<Rigidbody>();
        return fixedJoint;
    }
    private SpringJoint AddSpringJoint(GameObject vickyParticle)
    {
        var springJoint = gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = vickyParticle.GetComponent<Rigidbody>();
        springJoint.damper = 10;
        springJoint.spring = 100;
        return springJoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (myJoint == null && other.name.StartsWith("Particle"))
        {
            VickySphere vicky = other.transform.parent.gameObject.GetComponent<VickySphere>();
            myJoint = AddFixedJoint(other.gameObject);
            AddSpringJoint(vicky.anchorParticle);
            vicky.stickedObjects.Add(gameObject);
        }

        if (myJoint == null && other.CompareTag("Player"))
        {
//            myJoint = other.GetComponent<VickySolidSphere>().AttachJoint(rb);
            other.GetComponent<VickySolidSphere>().AttachObject(this);
        }
    }
}
