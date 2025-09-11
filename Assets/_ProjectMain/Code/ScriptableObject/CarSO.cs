using UnityEngine;

[System.Serializable]
public class CarSO
{
    public GameObject carPrefab;

    [Header("Car Setting")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30f; //default 30f
    public float turnFactor = 3.5f; //default 3.5f 
    public float maxSpeed = 20f;

    [Header("Car Interface")]
    public string carName;
    public Sprite carSprite;

}
