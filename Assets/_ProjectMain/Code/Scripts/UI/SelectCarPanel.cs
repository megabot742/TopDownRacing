using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectCarPanel : MonoBehaviour
{
    [Header("Car Prefab")]
    [SerializeField] GameObject carPrefabAnimation;

    [Header("Spawn On")]
    [SerializeField] Transform spawnOnTransformParent;

    [Header("Car Database")]
    [SerializeField] CarDatabase carDatabase;

    [Header("Attribute")]
    [SerializeField] TMP_Text carName;
    [SerializeField] Slider accelerate_Slider;
    [SerializeField] TMP_Text accelerate_Txt;

    [SerializeField] Slider speed_Slider;
    [SerializeField] TMP_Text speed_Txt;

    [SerializeField] Slider turnRateSlider;
    [SerializeField] TMP_Text turenRate_Txt;

    [SerializeField] Slider gripSlider;
    [SerializeField] TMP_Text grip_Txt;
    bool isChangingCar = false;

    //CarData[] carDatas;

    //Other components
    int selectedCarIndex = 0;

    CarUIHandler carUIHandler = null;


    void Start()
    {
        //Load the car data
        //carDatas = Resources.LoadAll<CarData>("CarData/");
        if (carDatabase == null)
        {
            Debug.LogWarning("CarDatabase is not assigned in SelectCarPanel!");
            return;
        }
        // Thiết lập giá trị tối đa cho các Slider
        accelerate_Slider.maxValue = 50f; //50
        speed_Slider.maxValue = 400f; //400
        turnRateSlider.maxValue = 90f; //90
        gripSlider.maxValue = 2f; //2

        StartCoroutine(SpawnCarCO(true));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            OnPreviousCar();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            OnNextCar();
        }
    }
    public void OnPreviousCar()
    {
        if (isChangingCar)
            return;

        selectedCarIndex--;

        if (selectedCarIndex < 0)
        {
            selectedCarIndex = carDatabase.CarCount - 1;
        }

        StartCoroutine(SpawnCarCO(true));
    }
    public void OnNextCar()
    {
        if (isChangingCar)
            return;

        selectedCarIndex++;

        if (selectedCarIndex > carDatabase.CarCount - 1)
        {
            selectedCarIndex = 0;
        }

        StartCoroutine(SpawnCarCO(false));
    }
    IEnumerator SpawnCarCO(bool isCarAppearingOnRightSide)
    {
        isChangingCar = true;
        if (carUIHandler != null)
        {
            carUIHandler.StartCarExitAnimation(!isCarAppearingOnRightSide);
        }
        GameObject instantiatedCar = Instantiate(carPrefabAnimation, spawnOnTransformParent);

        carUIHandler = instantiatedCar.GetComponent<CarUIHandler>();

        CarSO carSO = carDatabase.GetCarSO(selectedCarIndex);
        carUIHandler.SetupCar(carSO);

        // Cập nhật UI với dữ liệu từ CarSO
        carName.text = carSO.carName;
        //Accelerate Value
        accelerate_Slider.value = carSO.accelerationFactor;
        accelerate_Txt.text = carSO.accelerationFactor.ToString();
        //Speed Value
        speed_Slider.value = carSO.maxSpeed * 16;
        speed_Txt.text = (carSO.maxSpeed * 16).ToString();
        //TurnRate Value
        turnRateSlider.value = carSO.turnFactor * 10;
        turenRate_Txt.text = (carSO.turnFactor * 10).ToString();
        //Grip Value
        gripSlider.value = carSO.driftFactor;
        grip_Txt.text = carSO.driftFactor.ToString();

        carUIHandler.StartCarEntranceAnimation(isCarAppearingOnRightSide);
        yield return new WaitForSeconds(0.4f);

        isChangingCar = false;
    }
    //Button
    public void OnSelectCar()
    {
        // Lưu selectedCarIndex vào PlayerPrefs
        PlayerPrefs.SetInt("PlayerSelectedCarID", selectedCarIndex);
        PlayerPrefs.Save();

        // Lấy CarSO được chọn
        CarSO selectedCarSO = carDatabase.GetCarSO(selectedCarIndex);

        // Gán carSprite vào RaceSetupPanel.carSelectImage
        if (UIManager.HasInstance && RaceInfoManager.HasInstance)
        {
            if (UIManager.Instance.raceSetupPanel != null)
            {
                UIManager.Instance.raceSetupPanel.carSelectImage.sprite = selectedCarSO.carSprite;
                RaceInfoManager.Instance.carSprite = selectedCarSO.carSprite;
                RaceInfoManager.Instance.playerCar = selectedCarSO.carPrefab.GetComponent<CarController>();
                UIManager.Instance.ChangeUIGameObject(this.gameObject, UIManager.Instance.raceSetupPanel.gameObject);
            }
        }
    }
    public void OnClickClose()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ChangeUIGameObject(this.gameObject, UIManager.Instance.raceSetupPanel.gameObject);
        }
    }
}
