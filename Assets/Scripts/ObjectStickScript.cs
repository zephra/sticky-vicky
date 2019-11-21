using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStickScript : MonoBehaviour
{
    private Joint myJoint = null;

    private FixedJoint AddFixedJoint(GameObject vickyParticle)
    {
        var fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = vickyParticle.GetComponent<Rigidbody>();
        return fixedJoint;
    }
    private SpringJoint AddSpringJoint(GameObject vickyParticle)
    {
        var fixedJoint = gameObject.AddComponent<SpringJoint>();
        fixedJoint.connectedBody = vickyParticle.GetComponent<Rigidbody>();
        return fixedJoint;
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
            myJoint = AddFixedJoint(other.gameObject);
            AddSpringJoint(other.transform.parent.gameObject.GetComponent<VickySphere>().anchorParticle);
        }
    }
}
