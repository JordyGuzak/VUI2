using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour {

    public GameObject log;
    
    private int treeHealth = 5;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag.Equals("Axe")) {
            treeHealth--;
            if (treeHealth <= 0)
            {
                Instantiate(log, transform.position + (Vector3.up * 2f), Quaternion.identity);
                Destroy(transform.gameObject);

                
            }
        }
        
    }
}
