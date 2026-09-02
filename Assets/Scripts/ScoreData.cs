using System;

[Serializable]
public class ScoreData
{
    public string username;
    public ScoreValues data;
}

[Serializable]
public class ScoreValues
{
    public int score;
}