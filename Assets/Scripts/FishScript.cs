using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishScript : MonoBehaviour
{
    public ObjectStickScript stickyScript;
    public LayerMask fleeFromLayer;
    public bool fleeWhileSticked = false;

    public float detectionRadius = 5;
    public float forgetRadius = 20;
    public float fleeForce = 500;
    public float maxForce = 2000;

    public float chooseTimeMax = 2f;
    public float chooseTimeMin = 0.5f;

    private ObjectStickScript chosenCat = null;

    private Collider[] results;
    private List<ObjectStickScript> cats;
    private List<ObjectStickScript> forgetCats;

    private float chooseTimer = 0;

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
        
        for(int i = 0; i < cats.Count; i++)
        {
            var cat = cats[i];
            var direction = (transform.position - cat.transform.position);
            var distance = direction.magnitude;
            
            if (distance > forgetRadius)
            {
                forgetCats.Add(cat);
            }
        }

        foreach (var cat in forgetCats)
        {
            cats.Remove(cat);
            if (chosenCat && chosenCat.Equals(cat))
            {
                chosenCat = null;
            }
        }
        forgetCats.Clear();

        if (chooseTimer > 0)
            chooseTimer -= Time.deltaTime;
        
        if ((!chosenCat || chooseTimer <= 0) && cats.Count > 0)
        {
            var index = Random.Range(0, cats.Count);
            chosenCat = cats[index];
            chooseTimer = Random.Range(chooseTimeMin, chooseTimeMax);
        }

        if (chosenCat)
        {
            var chosenDirection = (transform.position - chosenCat.transform.position);
            var dirNorm = chosenDirection.normalized;
            moveForce += dirNorm * fleeForce;
            moveForce.y += fleeForce * 0.1f;
        }

        if (moveForce.magnitude > maxForce)
        {
            moveForce = moveForce.normalized * maxForce;
        }
        if (fleeWhileSticked || !stickyScript.stuckToVicky)
            stickyScript.rb.AddForce(Time.deltaTime * moveForce);
        
        if (!fleeWhileSticked && stickyScript.stuckToVicky)
            cats.Clear();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var size = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, fleeFromLayer);
        if (size > 0)
        {
            var cat = results[0].gameObject.GetComponent<ObjectStickScript>();
            if (!cats.Contains(cat))
            {
                cats.Add(cat);
            }
        }
    }
}
