using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HUDPanel : MonoBehaviour
{
    public TMP_Text lapCounter_Text;
    public TMP_Text currentLapTime_Text;
    public TMP_Text bestLapTime_Text;
    public TMP_Text position_Text;
    public TMP_Text speedDometer_Text;
    public TMP_Text countDown_Text;
    public TMP_Text go_Text;
    public TMP_Text countDownIfNotFinish_Text;
    public TMP_Text raceResultText;
    public TMP_Text unlockRaceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DisplayCountDown();
    }
    public void DisplayCountDown()
    {
        if (UIManager.HasInstance && UIManager.Instance.isCountingDown) // True
        {
            int timeLeft = Mathf.RoundToInt(UIManager.Instance.GetEndCountDown());
            UIManager.Instance.hUDPanel.countDownIfNotFinish_Text.gameObject.SetActive(true);
            UIManager.Instance.hUDPanel.countDownIfNotFinish_Text.text = "Time left: " + timeLeft;
        }
        else
        {
            UIManager.Instance.hUDPanel.countDownIfNotFinish_Text.gameObject.SetActive(false);
        }
    }
    void OnDisable()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.hUDPanel.countDown_Text.gameObject.SetActive(true);
            UIManager.Instance.hUDPanel.countDownIfNotFinish_Text.gameObject.SetActive(false);
            UIManager.Instance.hUDPanel.unlockRaceText.gameObject.SetActive(false);
        }
    }
    public void OnClickPause()
    {
        if (UIEventManager.HasInstance)
        {
            UIEventManager.Instance.PauseGame();
        }
    }
}
