using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : MonoBehaviour {

    public float rot = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") > 0)
        {
            rot++;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            rot--;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            rot = 0;
        }
    }
}
