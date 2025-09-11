using UnityEngine;

[CreateAssetMenu]
public class RaceDatabase : ScriptableObject
{
    public RaceSO[] track;

    public int TrackCount
    {
        get
        {
            return track.Length;
        }
    }

    public RaceSO GetRaceSO(int index)
    {
        return track[index];
    }
}
