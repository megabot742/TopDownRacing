using UnityEngine;

[CreateAssetMenu]
public class CarDatabase : ScriptableObject
{
    public CarSO[] cars;

    public int CarCount
    {
        get
        {
            return cars.Length;
        }
    }

    public CarSO GetCarSO(int index)
    {
        return cars[index];
    }
}
