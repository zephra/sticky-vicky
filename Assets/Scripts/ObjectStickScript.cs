using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStickScript : MonoBehaviour
{
//    private Joint myJoint = null;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public VickySolidSphere vicky;

    [HideInInspector] public bool stuckToVicky;

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


    private void OnTriggerEnter(Collider other)
    {
        if (!stuckToVicky && other.CompareTag("Player"))
        {
            vicky.AttachObject(this);
        }
        
        if (stuckToVicky && other.CompareTag("Sticky"))
        {
            var otherSticky = other.GetComponent<ObjectStickScript>();
            if (!otherSticky.stuckToVicky)
            {
                vicky.AttachObjectToAnother(this, otherSticky);
            }
        }
    }
}
