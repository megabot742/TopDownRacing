using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    public void OnClickRestart()
    {
        if (UIEventManager.HasInstance && UIManager.HasInstance)
        {
            UIManager.Instance.ChangeUIGameObject(this.gameObject);
            UIManager.Instance.SetEndCountDown(UIManager.Instance.timeIfNotFinish); //Reset time to default
            UIManager.Instance.isCountingDown = false;
            UIEventManager.Instance.isPaused = true; //Call pause true, when reload is false
            UIEventManager.Instance.RestartGame();
            
        }
    }
    public void OnClickMenu()
    {
        if (UIEventManager.HasInstance)
        {
            UIEventManager.Instance.BackMenu();
        }
    }
}
