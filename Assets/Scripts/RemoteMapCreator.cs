using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteMapCreator : MonoBehaviour {

    public const float MIRROR_PLANE_Y = 4.94f;

    public readonly Dictionary<string, MapPoint> RemoteMap = new Dictionary<string, MapPoint>();

    Dictionary<string, Bubble> remoteBalls = new Dictionary<string, Bubble>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnBallShot(Vector3 startingPosition, Vector3 force, string ballId, BubbleShooter.BubbleColor color) {
        GameObject nextBall = Main.main.BallsPerColor[color].gameObject;
        GameObject created = Instantiate(nextBall, startingPosition, transform.rotation);
        created.GetComponent<Bubble>().ID = ballId;
        created.transform.parent = Main.main.RemoteMapHolder.transform;
        created.transform.localPosition = startingPosition;
        created.GetComponent<Rigidbody>().AddForce(Main.main.RemoteMapHolder.transform.rotation * force);

        //TODO fix the leak here
        remoteBalls.Add(ballId, created.GetComponent<Bubble>());
    }

    public void OnBallAttached(string ballId, string mapPointId) {
        Bubble b = remoteBalls[ballId];
        if (b == null) {
            //TODO hmmm; recreate the ball
            return;
        }
        Debug.Log("attaching ball" + ballId);
        if (b.CurrentMapPoint != null) {
            if (b.CurrentMapPoint.ID.Equals(mapPointId)) {
                //nothing to do here
            } else {
                b.CurrentMapPoint.RemoveCurrentBubble();
            }
        }

        MapPoint mp = RemoteMap[mapPointId];
        b.StickToMapPoint(mp);
    }

    public void DestroyGroup(string mapPointId) {
        RemoteMap[mapPointId].CheckNeighbouringBubbles();
    }
}
