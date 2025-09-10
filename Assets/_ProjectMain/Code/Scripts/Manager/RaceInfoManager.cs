using UnityEngine;

public class RaceInfoManager : BaseManager<RaceInfoManager>
{
    public string raceToLoad;
    public CarController playerCar;
    public int numberOfLap;
    public int numberOfAI;
    public bool enteredRace; //false
    public Sprite carSprite;
    public Sprite raceSprite;
    public string raceToUnlock;


    
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
