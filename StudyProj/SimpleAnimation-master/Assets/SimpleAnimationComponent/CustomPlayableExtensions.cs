using UnityEngine;
using UnityEngine.Playables;

public static class CustomPlayableExtensions
{
    /// <summary>
    /// ResetTime
    /// </summary>
    /// <param name="playable"></param>
    /// <param name="time"></param>
    public static void ResetTime(this Playable playable, float time)
    {
        playable.SetTime(time);
        playable.SetTime(time);
    }
}
