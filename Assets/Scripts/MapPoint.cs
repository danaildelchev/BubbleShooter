using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour {

    private const bool DEBUG = true;

    public const float DISTANCE_BETWEEN_MAP_POINTS = 0.5f;

    public bool Sticky = false;
    public bool Occupied = false;

    public List<MapPoint> neighbours = new List<MapPoint>();

	// Use this for initialization
	void Start () {
        Object[] mapPoints = FindObjectsOfType(typeof(MapPoint));
        foreach (Object o in mapPoints) {
            MapPoint mp = (MapPoint) o;
            if (mp != this)
            {
                float distanceSqr = (((MapPoint)mp).gameObject.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr < DISTANCE_BETWEEN_MAP_POINTS * DISTANCE_BETWEEN_MAP_POINTS) {
                    neighbours.Add((MapPoint)mp);

                    if (DEBUG)
                    {
                        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Destroy(go.GetComponent<Rigidbody>());
                        Destroy(go.GetComponent<Collider>());
                        go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        go.transform.parent = this.transform;
                        go.transform.position = this.transform.position;
                        LineRenderer line = go.AddComponent<LineRenderer>();
                        // Set the width of the Line Renderer
                        line.SetWidth(0.01F, 0.01F);
                        // Set the number of vertex fo the Line Renderer
                        line.SetVertexCount(2);
                        line.SetPosition(0, this.transform.position);
                        line.SetPosition(1, mp.gameObject.transform.position);
                    }
                }
            }
        }
    }

    public void BallAdded() {
        foreach (MapPoint neighbor in neighbours) {
            neighbor.Sticky = true;

            if (false)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(go.GetComponent<Rigidbody>());
                Destroy(go.GetComponent<Collider>());
                go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                go.transform.parent = neighbor.transform;
                go.transform.position = neighbor.transform.position;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
