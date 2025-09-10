using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnClickClose()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ChangeUIGameObject(this.gameObject, UIManager.Instance.homeMenuPanel.gameObject);
        }
    }
}
