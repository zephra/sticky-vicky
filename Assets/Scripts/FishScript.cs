using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : MonoBehaviour
{
    public ObjectStickScript stickyScript;
    public LayerMask fleeFromLayer;
    public bool fleeWhileSticked = false;

    public float detectionRadius = 5;
    public float forgetRadius = 20;
    public float fleeForce = 500;
    public float maxForce = 2000;

    private Collider[] results;
    private List<ObjectStickScript> cats;
    private List<ObjectStickScript> forgetCats;

    // Start is called before the first frame update
    void Start()
    {
        results = new Collider[1];
        
        cats = new List<ObjectStickScript>();
        forgetCats = new List<ObjectStickScript>();
    }

    private void Update()
    {
        var moveForce = Vector3.zero;
        
        foreach (var cat in cats)
        {
            var direction = (transform.position - cat.transform.position);
            
            if (direction.magnitude > forgetRadius)
            {
                forgetCats.Add(cat);
                continue;
            }

            var dirNorm = direction.normalized;
            moveForce += dirNorm * fleeForce;
        }

        if (moveForce.magnitude > maxForce)
        {
            moveForce = moveForce.normalized * maxForce;
        }
        if (fleeWhileSticked || !stickyScript.stuckToVicky)
            stickyScript.rb.AddForce(Time.deltaTime * moveForce);
        
        if (!fleeWhileSticked && stickyScript.stuckToVicky)
            cats.Clear();

        foreach (var cat in forgetCats)
        {
            cats.Remove(cat);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var size = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, fleeFromLayer);
        if (size > 0)
        {
//            Debug.Log("Near cat!?, collider length: "+results.Length+", collider 1: "+results[0].gameObject.name);
            var cat = results[0].gameObject.GetComponent<ObjectStickScript>();
            if (!cats.Contains(cat))
            {
                cats.Add(cat);
            }
        }
    }
}
