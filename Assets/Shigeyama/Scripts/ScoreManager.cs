using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    int score = 0;

    int claimPoint = 0;

    public void ScoreDecrement()
    {
        score -= 200;
        claimPoint++;
    }

    public void ScoreIncrement()
    {
        score += 200;
    }
}
