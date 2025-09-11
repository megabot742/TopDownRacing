using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceSetupPanel : MonoBehaviour
{
    public Image carSelectImage;
    public Image raceSelectImage;
    void Start()
    {
        if (RaceInfoManager.HasInstance)
        {
            if (RaceInfoManager.Instance.enteredRace) //true
            {
                raceSelectImage.sprite = RaceInfoManager.Instance.raceSprite;
                carSelectImage.sprite = RaceInfoManager.Instance.carSprite;

            }
            PlayerPrefs.SetInt(RaceInfoManager.Instance.raceToLoad + "_unlocked", 1);
        }
    }
    public void OnClickSelectCar()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.selectCar.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    public void OnClickSelectRace()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.selectRacePanel.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
    public void OnClickRace()
    {
        if (UIEventManager.HasInstance)
        {
            UIEventManager.Instance.Race();
            UIEventManager.Instance.isPaused = false;
        }
    }
    public void OnClickClose()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ChangeUIGameObject(this.gameObject, UIManager.Instance.homeMenuPanel.gameObject);
        }
    }
}
