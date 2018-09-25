using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    public int ScorePerBall = 50;
    public int IncrementOfScorePerBall = 10;

    public static Main main;

    public ScoreManager scoreManager;

    // Use this for initialization
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
