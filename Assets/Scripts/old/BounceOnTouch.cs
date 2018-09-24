using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOnTouch : MonoBehaviour {

    public Camera camera;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 position = GetTouchPosition();
        if ((position - Vector2.negativeInfinity).SqrMagnitude() > Vector2.kEpsilon)
        {
            Ray raycast = camera.ScreenPointToRay(position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.name == "ball")
                {
                    //this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 20, 0));
                }
            }
        }
    }

    private Vector2 GetTouchPosition()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            return Input.GetTouch(0).position;
        }
        if (Input.GetMouseButtonDown(0)) {
            return Input.mousePosition;
        }
        return Vector2.negativeInfinity;
    }
}
