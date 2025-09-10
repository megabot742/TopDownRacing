using UnityEngine;

public class CheckPointChecker : MonoBehaviour
{
    public CarController CarController;
    void Awake()
    {
        CarController = GetComponent<CarController>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            CarController.CheckPointHit(collision.GetComponent<CheckPoint>().checkPointNumber);
        }
    }
}
