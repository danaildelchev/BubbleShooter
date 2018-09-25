using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

    public MapPoint CurrentMapPoint;

    private bool hasEndDestination;
    private bool arrivedAtEndDestination;
    private bool destroyed;

    public const float DRAG = 20.0f;
    public const float DRAGGED_FORCE = 1000.0f;
    private readonly float PERCENT_DISTANCE_TO_MAPPOINT = 50;
    private readonly float PERCENT_DISTANCE_TO_MAPPOINT_LAST_LINE = 70;
    private const float DESTROY_FORCE = 200.0f;
    public BubbleShooter.BubbleColor color;

    private Vector3 initialScale;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        initialScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (hasEndDestination && !arrivedAtEndDestination && !destroyed) {
            rb.AddForce((CurrentMapPoint.gameObject.transform.position - transform.position) * DRAGGED_FORCE);
            if ((CurrentMapPoint.transform.position - this.transform.position).sqrMagnitude < 0.001f) {
                arrivedAtEndDestination = true;
                rb.mass = 20.0f;
                rb.velocity = Vector3.zero;
                transform.position = CurrentMapPoint.transform.position;
                Debug.Log("arrived");
            }
        }
        AnimateBump();
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

    void OnCollisionEnter(Collision collision)
    {
    }

    public const float BUMP_DURATION = 0.15f;
    public const float HALF_DURATION = BUMP_DURATION / 2.0f;
    public const float BUMP_PERCENT = 1.3f;
    private float startBump;

    public void StartBump() {
        startBump = Time.time;
    }

    private void AnimateBump()
    {
        if (Time.time > startBump && Time.time < (startBump + BUMP_DURATION)) {
            if (Time.time < startBump + HALF_DURATION)
            {
                transform.localScale = Vector3.Lerp(initialScale, initialScale * BUMP_PERCENT, (Time.time - startBump) / HALF_DURATION);
            } else {
                transform.localScale = Vector3.Lerp(initialScale * BUMP_PERCENT, initialScale, (Time.time - (startBump + HALF_DURATION)) / HALF_DURATION);
            }
        }
    }

    public void DestroyBubble() {
        if (!destroyed)
        {
            destroyed = true;
            GetComponent<Collider>().enabled = false;
            RemoveFromMapPoint();
            Destroy(gameObject, 3.0f);
        }
    }

    private void StickToMapPoint(MapPoint mp) {
        hasEndDestination = true;
        CurrentMapPoint = mp;
        rb.drag = DRAG;
        CurrentMapPoint.BallAdded(this);
    }

    private void RemoveFromMapPoint() {
        rb.velocity = Vector3.zero;
        rb.useGravity = true;
        rb.mass = 1.0f;
        rb.AddForce(Vector3.up * DESTROY_FORCE + Vector3.right * (UnityEngine.Random.value > 0.5f ? -1.0f : 1.0f) * UnityEngine.Random.Range(0, 100));
        rb.drag = 0.0f;
        CurrentMapPoint.RemoveCurrentBubble();
        CurrentMapPoint = null;
    }
}
