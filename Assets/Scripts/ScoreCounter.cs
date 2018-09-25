using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour
{
    public UnityEngine.UI.Text mainScore;
    public UnityEngine.UI.Text newScore;

    private int totalScore;
    private int currentTotalScore;
    private int currentAdditionalScore;

    public int NumberChangesPerSecond = 50;
    private float changeTime, lastChangeTime;
    private bool requiresCountdown;

    // Use this for initialization
    void Start()
    {
        changeTime = 1.0f / NumberChangesPerSecond;
    }

    // Update is called once per frame
    void Update()
    {
        if (requiresCountdown)
        {
            if (currentTotalScore >= totalScore)
            {
                finishCountdown();
            }
            else
            {
                if (Time.time > lastChangeTime)
                {
                    lastChangeTime = Time.time;
                    int change = Mathf.Min(5, currentAdditionalScore);
                    currentAdditionalScore -= change;
                    currentTotalScore += change;

                    newScore.text = "+" + currentAdditionalScore;
                    mainScore.text = currentTotalScore + "";
                }
            }
        }
    }

    public void BumpScore(int additional) {
        totalScore += additional;
        currentAdditionalScore += additional;
        requiresCountdown = true;
        newScore.GetComponent<BumpAnimation>().StartBump();
    }

    private void finishCountdown() {
        newScore.text = "";
        mainScore.text = totalScore + "";
        currentTotalScore = totalScore;
        currentAdditionalScore = 0;
        requiresCountdown = false;
    }
}
