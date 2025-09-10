using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.IK;

public class RaceController : MonoBehaviour
{
    public static RaceController Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public CheckPoint[] allCheckPoints;
    [SerializeField] public int totalLaps;

    [SerializeField] public CarController playerCar;
    [SerializeField] public List<CarController> allAICars = new List<CarController>();
    [SerializeField] int playerPosition = 1;

    [SerializeField] float timeBetweenPosCheck = 0.2f;
    private float positionCheckCounter = 0;

    [Header("Race Start Countdown")]
    [SerializeField] public bool isStarting;
    [SerializeField] float timeBetweenStartCount = 1f;
    [SerializeField] float countDownCurrent = 3;
    private float startCounter;

    [Header("StartPosition")]
    [SerializeField] int playerStartPosition;
    [SerializeField] int aiNumberToSpawn;
    [SerializeField] Transform[] startPoints;
    [SerializeField] Transform spawnCarParent;
    [SerializeField] List<CarController> carsToSpawn = new List<CarController>();

    [SerializeField] public bool raceCompleted;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (RaceInfoManager.HasInstance)
        {
            totalLaps = RaceInfoManager.Instance.numberOfLap;
            aiNumberToSpawn = RaceInfoManager.Instance.numberOfAI;
        }


        for (int i = 0; i < allCheckPoints.Length; i++)
        {
            allCheckPoints[i].checkPointNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;
        SpawnCarWithStartPoint();
        TrackPlayerPosition(); //TrackPos when star game
        DisplayCountDown(); //Show when star game
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if (startCounter <= 0)
            {
                countDownCurrent--;
                startCounter = timeBetweenStartCount;
                DisplayCountDown();

                if (countDownCurrent == 0)
                {
                    isStarting = false;
                    if (UIManager.HasInstance)
                    {
                        UIManager.Instance.hUDPanel.countDown_Text.gameObject.SetActive(false);
                        UIManager.Instance.hUDPanel.go_Text.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            TrackPlayerPosition(); //TrackPos over time
        }

    }
    void TrackPlayerPosition()
    {
        positionCheckCounter -= Time.deltaTime;
        if (positionCheckCounter <= 0)
        {
            playerPosition = 1;

            foreach (CarController aiCar in allAICars)
            {
                float aiDistance = Vector3.Distance(aiCar.transform.position, allCheckPoints[aiCar.nextCheckPoint].transform.position);
                float playerDistance = Vector3.Distance(playerCar.transform.position, allCheckPoints[playerCar.nextCheckPoint].transform.position);
                if (aiCar.currentLap > playerCar.currentLap) // Greater than Lap
                {
                    playerPosition++;
                }
                else if (aiCar.currentLap == playerCar.currentLap) //Same lap, but greater than Checkpoint
                {
                    if (aiCar.nextCheckPoint > playerCar.nextCheckPoint)
                    {
                        playerPosition++;
                    }
                    else if (aiCar.nextCheckPoint == playerCar.nextCheckPoint)
                    {
                        if (aiDistance < playerDistance) //Same lap, same check point, lessthan Distance
                        {
                            playerPosition++;
                        }
                    }

                }
            }
            positionCheckCounter = timeBetweenPosCheck;
        }
        if (UIManager.HasInstance)
        {
            UIManager.Instance.hUDPanel.position_Text.text = "Pos: " + playerPosition + "/" + (allAICars.Count + 1);
        }

    }

    void DisplayCountDown()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.hUDPanel.countDown_Text.text = countDownCurrent + "!";
        }
    }

    void SpawnCarWithStartPoint()
    {
        //Get player number to spawn
        playerStartPosition = Random.Range(0, aiNumberToSpawn + 1); //plus 1 because with Int Random.Range not include max value, only min value
        //Get info player car and spawn
        if (RaceInfoManager.HasInstance)
        {
            playerCar = Instantiate(RaceInfoManager.Instance.playerCar, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation, spawnCarParent);
            playerCar.SetAISetup(false);
            //Remove AI same car with Player, Set up from the bottom to avoid the influence of the top element
            for (int i = carsToSpawn.Count - 1; i >= 0; i--)
            {
                if (carsToSpawn[i] == RaceInfoManager.Instance.playerCar)
                {
                    Debug.Log(carsToSpawn[i]);
                    carsToSpawn.RemoveAt(i);
                }
            }
        }
        //Spawn AI
        for (int i = 0; i < aiNumberToSpawn + 1; i++) //example: 5 mean 5 AI to spawn, not 4 AI with 1 player, so plus 1
        {
            if (i != playerStartPosition)
            {
                int selectedCar = Random.Range(0, carsToSpawn.Count);

                allAICars.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation, spawnCarParent));

                if (carsToSpawn.Count > aiNumberToSpawn - i)
                {
                    carsToSpawn.RemoveAt(selectedCar);
                }
            }
        }
    }
    public void FinishRace()
    {
        if (!raceCompleted)
        {
            raceCompleted = true;
        }

        if (UIManager.HasInstance && RaceInfoManager.HasInstance)
        {
            string positionText = GetOrdinalText(playerPosition);
            UIManager.Instance.hUDPanel.raceResultText.text = "You finished: " + positionText;
            UIManager.Instance.resultPanel.gameObject.SetActive(true);
            //Unlock new track
            if (playerPosition < 4 && !string.IsNullOrEmpty(RaceInfoManager.Instance.raceToUnlock))
            {
                if (!PlayerPrefs.HasKey(RaceInfoManager.Instance.raceToUnlock + "_unlocked"))
                {
                    PlayerPrefs.SetInt(RaceInfoManager.Instance.raceToUnlock + "_unlocked", 1);
                    PlayerPrefs.Save(); // Lưu ngay để đảm bảo
                    UIManager.Instance.hUDPanel.unlockRaceText.gameObject.SetActive(true);
                }
            }
        }

    }
    string GetOrdinalText(int number)
    {
        if (number <= 0) return number.ToString();

        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return number + "th";
        }

        switch (number % 10)
        {
            case 1:
                return number + "st";
            case 2:
                return number + "nd";
            case 3:
                return number + "rd";
            default:
                return number + "th";
        }
    }
}
