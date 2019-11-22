using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStickScript : MonoBehaviour
{
    private Joint myJoint = null;

    private FixedJoint AddFixedJoint(GameObject vickyParticle)
    {
        var fixedJoint = vickyParticle.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = GetComponent<Rigidbody>();
        fixedJoint.enableCollision = true;
        return fixedJoint;
    }
    private SpringJoint AddSpringJoint(GameObject vickyParticle)
    {
        var springJoint = vickyParticle.AddComponent<SpringJoint>();
        springJoint.connectedBody = GetComponent<Rigidbody>();
        springJoint.damper = 10;
        springJoint.spring = 40;
        return springJoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
            myJoint = AddFixedJoint(vicky.anchorParticle);// other.gameObject);
            AddSpringJoint(vicky.anchorParticle);
            vicky.stickedObjects.Add(gameObject);
        }
    }
}
