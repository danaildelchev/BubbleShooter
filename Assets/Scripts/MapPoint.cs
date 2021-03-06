﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour {

    private const bool DEBUG = true;

    public const float DISTANCE_BETWEEN_MAP_POINTS = 0.5f;
    public const int MAX_SIZE_GROUP = 4;

    public bool Sticky { get { return NeighbourBubbles.Count > 0; } }
    public bool LastLine;
    public Bubble CurrentBubble = null;
    public readonly List<Bubble> NeighbourBubbles = new List<Bubble>();

    public String ID;
    public bool IsRemote;

    private int currentScorePerBall;

    public List<MapPoint> neighbours = new List<MapPoint>();

	// Use this for initialization
	void Start () {
        System.Object[] mapPoints = FindObjectsOfType(typeof(MapPoint));
        if (IsRemote) {
                Main.main.RemoteMapRenderer.RemoteMap.Add(this.ID, this);
        }
        foreach (System.Object o in mapPoints) {
            MapPoint mp = (MapPoint) o;
            if (mp != this && mp.IsRemote == this.IsRemote)
            {
                float distanceSqr = (mp.gameObject.transform.position - transform.position).sqrMagnitude;
                if (Math.Sqrt(distanceSqr) < DISTANCE_BETWEEN_MAP_POINTS) {
                    neighbours.Add(mp);

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
                        line.startWidth = 0.01F;
                        line.endWidth = 0.01F;
                        // Set the number of vertex fo the Line Renderer
                        line.positionCount = 2;
                        line.SetPosition(0, this.transform.position);
                        line.SetPosition(1, mp.gameObject.transform.position);
                    }
                }
            }
        }
    }

    public void BallAdded(Bubble bubble)
    {
        if (!IsRemote)
        {
            Main.main.MessageManager.BallAttached(bubble.ID, this.ID);
        }
        currentScorePerBall = Main.main.ScorePerBall;

        CurrentBubble = bubble;
        MakeNeighborsSticky(bubble);

        if (!IsRemote)
        {
            CheckNeighbouringBubbles();
        }
    }

    public void CheckNeighbouringBubbles()
    {
        BubbleShooter.BubbleColor searchedColor = CurrentBubble.color;
        LinkedList<MapPoint> sameColorMapPoints = new LinkedList<MapPoint>();
        sameColorMapPoints.AddFirst(this);
        List<MapPoint> group = new List<MapPoint>();
        while (sameColorMapPoints.Count > 0) {
            MapPoint current = sameColorMapPoints.First.Value;
            sameColorMapPoints.RemoveFirst();
            if (current.CurrentBubble != null && current.CurrentBubble.color == searchedColor) {
                if (!group.Contains(current))
                {
                    group.Add(current);
                    foreach (MapPoint neighbour in current.neighbours)
                    {
                        sameColorMapPoints.AddFirst(neighbour);
                    }
                }
            }
        }
        List<Bubble> bubbles = new List<Bubble>();
        foreach (MapPoint mp in group) {
            bubbles.Add(mp.CurrentBubble);
        }
        if (group.Count > 1)
        {
            BlinkGroup(bubbles);
        }
        if (group.Count >= MAX_SIZE_GROUP) {
            if (!IsRemote)
            {
                Main.main.MessageManager.DestroyGroup(ID);
            }
            CheckNeighboursForRootAccessAfterDestruction(group);
            DestroyGroup(bubbles);
        }
    }

    private void CheckNeighboursForRootAccessAfterDestruction(List<MapPoint> groupToBeDestroyed)
    {
        List<List<Bubble>> graphs = new List<List<Bubble>>();
        foreach (MapPoint mpToBeDestroyed in groupToBeDestroyed) {
            foreach (MapPoint mpToCheck in mpToBeDestroyed.neighbours) {
                if (mpToCheck.CurrentBubble != null)
                {
                    bool alreadyProcessed = groupToBeDestroyed.Contains(mpToCheck);
                    foreach (List<Bubble> bubbles in graphs)
                    {
                        alreadyProcessed = alreadyProcessed || bubbles.Contains(mpToCheck.CurrentBubble);
                    }
                    if (!alreadyProcessed)
                    {
                        graphs.Add(BuildFullGraphExcept(mpToCheck, groupToBeDestroyed));
                    }
                }
            }
        }
        foreach (List<Bubble> bubbles in graphs) {
            bool hasRootAccess = false;

            foreach(Bubble bubble in bubbles) {
                if (bubble.CurrentMapPoint.LastLine){
                    hasRootAccess = true;
                    break;
                }
            }
            if (!hasRootAccess) {
                DestroyGroup(bubbles);
            }
        }
    }

    private List<Bubble> BuildFullGraphExcept(MapPoint mpToCheck, List<MapPoint> skipGroup)
    {
        List<Bubble> result = new List<Bubble>();
        //Debug.Log("Building graph " + result);
        LinkedList<MapPoint> nodesToVisit = new LinkedList<MapPoint>();
        nodesToVisit.AddFirst(mpToCheck);
        while (nodesToVisit.Count > 0) {
            MapPoint current = nodesToVisit.First.Value;
            nodesToVisit.RemoveFirst();
            //Debug.Log("Adding " + current.CurrentBubble + " " + current.CurrentBubble.transform.position + " to " + result);
            result.Add(current.CurrentBubble);
            foreach (MapPoint mp in current.neighbours)
            {
                if (mp.CurrentBubble != null) {
                    //Debug.Log("Checking " + mp.CurrentBubble + " " + mp.CurrentBubble.transform.position + ": " 
                    //          + !skipGroup.Contains(mp)  + " " + !result.Contains(mp.CurrentBubble));
                    if (!skipGroup.Contains(mp) && !result.Contains(mp.CurrentBubble))
                    {
                        nodesToVisit.AddFirst(mp);
                    }
                }
            }
        }
        return result;
    }

    void BlinkGroup(List<Bubble> bubbles)
    {
        foreach (Bubble bubble in bubbles) {
            bubble.GetComponent<BumpAnimation>().StartBump();
        }
    }

    void DestroyGroup(List<Bubble> bubbles)
    {
        foreach (Bubble bubble in bubbles)
        {
            bubble.DestroyBubble();
            ScoreManager scoreManager = IsRemote ? Main.main.remoteScoreManager : Main.main.scoreManager;
            scoreManager.AddScore(bubble, currentScorePerBall);
            currentScorePerBall += Main.main.IncrementOfScorePerBall;
        }
    }

    private void MakeNeighborsSticky(Bubble bubble)
    {
        foreach (MapPoint neighbor in neighbours)
        {
            neighbor.NeighbourBubbles.Add(bubble);

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

    public void RemoveCurrentBubble()
    {
        foreach (MapPoint mp in neighbours) {
            mp.NeighbourBubbles.Remove(CurrentBubble);
        }
        CurrentBubble = null;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
