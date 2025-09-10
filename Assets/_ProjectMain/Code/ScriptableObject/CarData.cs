using UnityEngine;

[CreateAssetMenu(fileName = "NewCarData", menuName = "CarData", order = 0)]
public class CarData : ScriptableObject
{
    [SerializeField] int carUniqueID = 0;
    [SerializeField] Sprite carUISprite;
    [SerializeField] GameObject carPrefabs;


    public int GetCarUniqueID
    {
        get { return carUniqueID; }
    }

    public Sprite GetUISprite
    {
        get { return carUISprite; }
    }
    public GameObject GetCarPrefab
    {
        get { return carPrefabs; }
    }
}
