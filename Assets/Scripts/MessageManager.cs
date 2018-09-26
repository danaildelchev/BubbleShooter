using UnityEngine;
using System.Collections;

public class MessageManager : MonoBehaviour
{

    public RemoteMapCreator mockMapCreator;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void BallShot(Vector3 startingPosition, Vector3 force, string ballId, BubbleShooter.BubbleColor color)
    {
        Debug.Log("Ball shot! " + ballId + " " + color);
        mockMapCreator.OnBallShot(startingPosition, force, ballId, color);
    }

    public void BallAttached(string ballId, string mapPointId)
    {
        Debug.Log("Ball attached! " + ballId + " " + mapPointId);
        mockMapCreator.OnBallAttached(ballId, mapPointId);
    }

    public void DestroyGroup(string mapPointId)
    {
        Debug.Log("Group destroyed! " + mapPointId);
        mockMapCreator.DestroyGroup(mapPointId);
    }
}
