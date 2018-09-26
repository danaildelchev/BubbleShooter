using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour
{
    public GameObject LocalMapHolder;
    public GameObject RemoteMapHolder;

    public MessageManager MessageManager;

    public RemoteMapCreator RemoteMapRenderer;

    public ScoreManager scoreManager;
    public ScoreManager remoteScoreManager;

    public int ScorePerBall = 50;
    public int IncrementOfScorePerBall = 10;

    public Bubble[] Balls;

    public Dictionary<BubbleShooter.BubbleColor, Bubble> BallsPerColor = new Dictionary<BubbleShooter.BubbleColor, Bubble>();

    public BubbleShooter.BubbleColor[] NextBallsColors = {
        BubbleShooter.BubbleColor.YELLOW,
        BubbleShooter.BubbleColor.YELLOW,
        BubbleShooter.BubbleColor.YELLOW,
        BubbleShooter.BubbleColor.YELLOW,
        BubbleShooter.BubbleColor.YELLOW};

    public static Main main;

    // Use this for initialization
    void Start()
    {
        main = this;

        foreach (Bubble bubble in Main.main.Balls)
        {
            BallsPerColor.Add(bubble.color, bubble);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
