using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    CarController carController;
    
    void Awake()
    {
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        carController.SetInputVector(inputVector);


        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            carController.Jump(1.0f, 0.0f);
        }
    }
}
