using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

    public MapPoint CurrentMapPoint;
    public String ID;
    public bool IsRemote;

    private bool hasEndDestination;
    private bool arrivedAtEndDestination;
    private bool destroyed;

    public const float DRAG = 20.0f;
    public const float DRAGGED_FORCE = 1000.0f;
    private readonly float PERCENT_DISTANCE_TO_MAPPOINT = 60;
    private readonly float PERCENT_DISTANCE_TO_MAPPOINT_LAST_LINE = 70;
    private const float DESTROY_FORCE = 200.0f;
    public BubbleShooter.BubbleColor color;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasEndDestination && !arrivedAtEndDestination && !destroyed)
        {
            if (arrivedAtEndDestination) {
                arrivedAtEndDestination = (CurrentMapPoint.transform.position - this.transform.position).sqrMagnitude > 0.001f;
            }
            if (!arrivedAtEndDestination)
            {
                rb.AddForce((CurrentMapPoint.gameObject.transform.position - transform.position) * DRAGGED_FORCE);
                if ((CurrentMapPoint.transform.position - this.transform.position).sqrMagnitude < 0.001f)
                {
                    arrivedAtEndDestination = true;
                    rb.mass = 20.0f;
                    rb.velocity = Vector3.zero;
                    transform.position = CurrentMapPoint.transform.position;
                    Debug.Log("arrived");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<MapPoint>() != null)
        {
            if (!hasEndDestination)
            {
                MapPoint mp = other.GetComponent<MapPoint>();
                if (mp.CurrentBubble == null && (mp.Sticky || mp.LastLine))
                {
                    float distanceSqr =(other.gameObject.transform.position - transform.position).sqrMagnitude;
                    float percentDistanceToMapPoint = mp.LastLine ? PERCENT_DISTANCE_TO_MAPPOINT_LAST_LINE : PERCENT_DISTANCE_TO_MAPPOINT;
                    if (Mathf.Sqrt(distanceSqr) < (MapPoint.DISTANCE_BETWEEN_MAP_POINTS * percentDistanceToMapPoint / 100.0f))
                    {
                        {
                            StickToMapPoint(mp);
                        }
                    }
                }
            }
        }
    }

    public void DestroyBubble() {
        if (!destroyed)
        {
            destroyed = true;
            GetComponent<BumpAnimation>().ManageCollider = false;
            GetComponent<Collider>().enabled = false;
            RemoveFromMapPoint();
            Destroy(gameObject, 3.0f);
        }
    }

    public void StickToMapPoint(MapPoint mp) {
        if (IsRemote) {
            GetComponent<Collider>().enabled = false;
        }
        hasEndDestination = true;
        CurrentMapPoint = mp;
        rb.drag = DRAG;
        CurrentMapPoint.BallAdded(this);
    }

    private void RemoveFromMapPoint() {
        GetComponent<Collider>().enabled = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = true;
        rb.mass = 1.0f;
        rb.AddForce(Vector3.up * DESTROY_FORCE + Vector3.right * (UnityEngine.Random.value > 0.5f ? -1.0f : 1.0f) * UnityEngine.Random.Range(0, 100));
        rb.drag = 0.0f;
        CurrentMapPoint.RemoveCurrentBubble();
        CurrentMapPoint = null;
    }
}
