using UnityEngine;

public class SpawnPoint : MonoBehaviour
{

    void Start()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //Load the car data
        CarData[] carDatas = Resources.LoadAll<CarData>("CarData/");

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i].transform;

            //int playerSelectedCarID = PlayerPrefs.GetInt($"{i + 1}SelectedCarID");
            int playerSelectedCarID = PlayerPrefs.GetInt("PlayerSelectedCarID");

            foreach (CarData carData in carDatas)
            {
                //We found the car data for the player
                if (carData.GetCarUniqueID == playerSelectedCarID)
                {
                    //Now spawn it on the spawnPoint
                    GameObject car = Instantiate(carData.GetCarPrefab, spawnPoint.position, spawnPoint.rotation);

                    //playerCar.GetComponent<CarInputHandler>().playerNumber = i;

                    break;
                }
            }

            
        }
    }

    
}
