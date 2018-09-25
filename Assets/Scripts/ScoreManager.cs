using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ScoreManager : MonoBehaviour
{
    public ScoreCounter ScoreCounter;

    public float ScoreCelebrationDuration = 0.3f;

    public ParticleSystem celebrationParticleSystem;

    private float lastTimeChanged;
    private Vector3 lastBubblePosition, lastParticleSystemPosition;
    private bool needsUpdate;

    class Score {
        public int amount;
        public Bubble bubble;
    }

    Queue<Score> scores = new Queue<Score>();

    private float lastScoreTime;

    public void AddScore(Bubble bubble, int amount) {
        Score score = new Score();
        score.bubble = bubble;
        score.amount = amount;

        scores.Enqueue(score);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (scores.Count > 0 && lastTimeChanged + ScoreCelebrationDuration < Time.time)
        {
            if (scores.Count > 0)
            {
                needsUpdate = true;
                GrabNextScore();
                lastTimeChanged = Time.time;
            }
        }
        else
        {
            needsUpdate = false;
        }

        //if (needsUpdate) {
        //    celebrationParticleSystem.transform.position = Vector3.Lerp(
        //        lastParticleSystemPosition, lastBubblePosition, 
        //        (Time.time - lastTimeChanged) / ScoreCelebrationDuration);
        //}
    }

    private void GrabNextScore()
    {
        Score score = scores.Dequeue();
        ScoreCounter.BumpScore(score.amount);
        UpdateParticleSystem(score.bubble);
    }

    private void UpdateParticleSystem(Bubble bubble)
    {
        lastBubblePosition = bubble.transform.position;
        if (!celebrationParticleSystem.isPlaying)
        {
            celebrationParticleSystem.Play();
            lastParticleSystemPosition = lastBubblePosition;

        }
        else
        {
            lastParticleSystemPosition = celebrationParticleSystem.transform.position;
        }
        celebrationParticleSystem.transform.position = bubble.transform.position;
    }
}
