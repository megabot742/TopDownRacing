
using UnityEngine;
using UnityEngine.UI;

public class CarUIHandler : MonoBehaviour
{
    //Other component
    [Header("Car Details")]
    public Image carImage;


    Animator animator = null;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void SetupCar(CarSO carData)
    {
        carImage.sprite = carData.carSprite;
        // Các thiết lập khác nếu cần
    }

    public void StartCarEntranceAnimation(bool isAppearingOnRightSide)
    {
        if (isAppearingOnRightSide)
        {
            animator.Play("CarUIAppearFromRight");
        }
        else
        {
            animator.Play("CarUIAppearFromLeft");
        }
    }

    public void StartCarExitAnimation(bool isExitOnRightSide)
    {
        if (isExitOnRightSide)
        {
            animator.Play("CarUIDisappearToRight");
        }
        else
        {
            animator.Play("CarUIDisappearToLeft");
        }
    }

    //Events

    public void OnCarExitAnimationCompleted()
    {
        Destroy(gameObject);
    }
}
