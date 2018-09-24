using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBullet : MonoBehaviour {

    private bool hasEndDestination = false;
    private MapPoint endDestination;
    private bool arrivedAtEndDestination;

    public const float DRAG = 20.0f;
    public const float DRAGGED_FORCE = 1000.0f;
    private readonly float PERCENT_DISTANCE_TO_MAPPOINT = 50;
    public Application.BallColor color;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (hasEndDestination && !arrivedAtEndDestination) {
            rb.AddForce((endDestination.gameObject.transform.position - transform.position) * DRAGGED_FORCE);
            if ((endDestination.transform.position - this.transform.position).sqrMagnitude < 0.001f) {
                arrivedAtEndDestination = true;
                rb.velocity = Vector3.zero;
                transform.position = endDestination.transform.position;
                Debug.Log("arrived");
            }
        }
	}


    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<MapPoint>() != null)
        {
            if (!hasEndDestination)
            {
                MapPoint mp = other.GetComponent<MapPoint>();
                if (mp.Sticky)
                {
                    float distanceSqr =(other.gameObject.transform.position - transform.position).sqrMagnitude;
                    if (Mathf.Sqrt(distanceSqr) < (MapPoint.DISTANCE_BETWEEN_MAP_POINTS * PERCENT_DISTANCE_TO_MAPPOINT / 100.0f))
                    {
                        {
                            hasEndDestination = true;
                            endDestination = mp;
                            mp.BallAdded();
                            rb.drag = DRAG;
                        }
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
    }
}
