using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

    public Light light;
    public CheckSpace targetCheckSpace;

    // Update is called once per frame
    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag.Equals("CheckPoint") || hit.transform.tag.Equals("Pawn"))
            {
                //Enable area highlight
                this.light.enabled = true;
                this.gameObject.transform.LookAt(hit.transform);

                //Set current target
                if (hit.transform.tag.Equals("CheckPoint"))
                {
                    this.targetCheckSpace = hit.transform.gameObject.GetComponent<CheckSpace>();
                }
                else
                {
                    this.targetCheckSpace = hit.transform.gameObject.GetComponent<Pawn>().checkSpace;
                }
            }
            else
            {
                this.light.enabled = false;
                this.targetCheckSpace = null;
            }
        }
        else
        {
            this.light.enabled = false;
            this.targetCheckSpace = null;
        }
    }
}
