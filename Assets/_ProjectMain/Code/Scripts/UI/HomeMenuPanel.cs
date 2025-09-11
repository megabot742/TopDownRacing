using UnityEditor;
using UnityEngine;

public class HomeMenuPanel : MonoBehaviour
{
    public void OnClickStartGame()
    {
        if (UIEventManager.HasInstance)
        {
            UIEventManager.Instance.StartGame();
        }
    }
    public void OnClickSettingGame()
    {
        if (UIEventManager.HasInstance)
        {
            UIEventManager.Instance.SettingGame();
        }
    }
    public void OnClickExitGame()
    {
        if (UIEventManager.HasInstance)
        {
            UIEventManager.Instance.QuitGame();
        }
    }
    public void OnClicktutorial()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.tutorialPanel.gameObject.SetActive(true);
        }
    }
}
