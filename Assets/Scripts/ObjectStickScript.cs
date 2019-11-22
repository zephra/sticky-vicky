using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStickScript : MonoBehaviour
{
//    private Joint myJoint = null;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public VickySolidSphere vicky;

    public bool stuckToVicky;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vicky = GameObject.FindWithTag("Player").GetComponent<VickySolidSphere>();
        stuckToVicky = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

//    private FixedJoint AddFixedJoint(GameObject vickyParticle)
//    {
//        var fixedJoint = gameObject.AddComponent<FixedJoint>();
//        fixedJoint.connectedBody = vickyParticle.GetComponent<Rigidbody>();
//        return fixedJoint;
//    }
//    private SpringJoint AddSpringJoint(GameObject vickyParticle)
//    {
//        var springJoint = gameObject.AddComponent<SpringJoint>();
//        springJoint.connectedBody = vickyParticle.GetComponent<Rigidbody>();
//        springJoint.damper = 10;
//        springJoint.spring = 100;
//        return springJoint;
//    }

    private void OnTriggerEnter(Collider other)
    {
//        if (myJoint == null && other.name.StartsWith("Particle"))
//        {
//            VickySphere vicky = other.transform.parent.gameObject.GetComponent<VickySphere>();
//            myJoint = AddFixedJoint(other.gameObject);
//            AddSpringJoint(vicky.anchorParticle);
//            vicky.stickedObjects.Add(gameObject);
//        }

        if (!stuckToVicky && other.CompareTag("Player"))
        {
//            myJoint = other.GetComponent<VickySolidSphere>().AttachJoint(rb);
            vicky.AttachObject(this);
        }
        
        if (stuckToVicky && other.CompareTag("Sticky"))
        {
            var otherSticky = other.GetComponent<ObjectStickScript>();
            if (!otherSticky.stuckToVicky)
            {
//                Debug.Log("Sticking to another sticky?? "+other.name);
                vicky.AttachObjectToAnother(this, otherSticky);
            }
        }
    }
}
