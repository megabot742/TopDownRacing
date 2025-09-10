using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : BaseManager<UIManager>
{
    [Header("Menu")]
    public HomeMenuPanel homeMenuPanel;
    public RaceSetupPanel raceSetupPanel;
    public SelectCarPanel selectCar;
    public SelectRacePanel selectRacePanel;
    public SettingPanel settingPanel;
    public TutorialPanel tutorialPanel;

    [Header("Race")]
    public HUDPanel hUDPanel;
    public PausePanel pausePanel;
    public ResultPanel resultPanel;

    [Header("Scene")]
    public string currentSceneName;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Load Menu scene when game starts
        SwitchToScene("Menu");
    }
    public void SwitchToScene(string sceneName)
    {
        // Load the new scene
        SceneManager.LoadScene(sceneName);
        currentSceneName = sceneName;

        // Update UI based on the loaded scene
        UpdateUIForScene(sceneName);

        if (BackgroundMusic.HasInstance)
        {
            // Cập nhật nhạc nền cho scene mới
            BackgroundMusic.Instance.UpdateMusicForScene(sceneName);
        }
    }

    public void ReloadCurrentScene()
    {
        //check currentSceneName
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            SwitchToScene(currentSceneName);
        }
    }

    public void ChangeUIGameObject(GameObject currentObejct = null, GameObject activeObject = null)
    {
        if (currentObejct != null)
        {
            currentObejct.SetActive(false);
        }
        if (activeObject != null)
        {
            activeObject.SetActive(true);
        }
    }
    private void UpdateUIForScene(string sceneName)
    {
        // Disable all panels first
        ChangeUIGameObject(homeMenuPanel.gameObject);
        ChangeUIGameObject(raceSetupPanel.gameObject);
        ChangeUIGameObject(selectCar.gameObject);
        ChangeUIGameObject(selectRacePanel.gameObject);
        ChangeUIGameObject(settingPanel.gameObject);
        ChangeUIGameObject(tutorialPanel.gameObject);

        ChangeUIGameObject(hUDPanel.gameObject);
        ChangeUIGameObject(pausePanel.gameObject);
        ChangeUIGameObject(resultPanel.gameObject);

        // Enable the default panel based on the scene
        switch (sceneName)
        {
            case "Menu":
                ChangeUIGameObject(null, homeMenuPanel.gameObject);
                break;
            case "Race1":
            case "Race2":
            case "Race3":
                ChangeUIGameObject(null, hUDPanel.gameObject);
                break;
        }
    }
}
