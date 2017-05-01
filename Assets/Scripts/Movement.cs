using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    float rot = 0;
    public Zoomer zoomer;
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            rot++;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            rot--;
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            rot = 0;
        }

        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, rot, zoomer.rot), 0.5f);
    }
}
