using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

    public GameObject log;
    public GameObject woodImpact;

    private Transform impactSpawnPoint;
    private int treeHealth = 10;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag.Equals("Axe")) {
            //Set the rotation of the prefab to the same as spawn point
            impactSpawnPoint = collision.transform;
            woodImpact.transform.rotation = impactSpawnPoint.transform.rotation;

            //Spawn the wood prefab

            Instantiate(woodImpact, collision.contacts[0].point, Quaternion.Inverse(impactSpawnPoint.transform.rotation));
            
            treeHealth--;

            if (treeHealth <= 0)
            {
                Instantiate(log, transform.position + (Vector3.up), Quaternion.identity).GetComponent<Rigidbody>().AddForceAtPosition((Vector3.up * 10f), impactSpawnPoint.position);
                Instantiate(log, transform.position + (Vector3.up * 2f), Quaternion.identity).GetComponent<Rigidbody>().AddForceAtPosition((Vector3.up * 10f), impactSpawnPoint.position);
                Instantiate(log, transform.position + (Vector3.up * 3f), Quaternion.identity).GetComponent<Rigidbody>().AddForceAtPosition((Vector3.up * 10f), impactSpawnPoint.position);
                Destroy(transform.gameObject);
            }
        }
        
    }
}
