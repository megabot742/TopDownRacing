using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectRacePanel : MonoBehaviour
{
    [SerializeField] RaceDatabase raceDB;
    [SerializeField] Image trackImage;
    [SerializeField] TMP_Text trackName;
    [SerializeField] GameObject unclocklPanel;
    int selectedOption = 0;
    const string SELECTED_TRACK_KEY = "SelectedTrackIndex";
    void Start()
    {
        if (!PlayerPrefs.HasKey(raceDB.GetRaceSO(0).idTrack + "_unlocked"))
        {
            PlayerPrefs.SetInt(raceDB.GetRaceSO(0).idTrack + "_unlocked", 1);
        }
        LoadRace();
        selectedOption = Mathf.Clamp(selectedOption, 0, raceDB.TrackCount - 1); //check selectedOption valid
        UpdateRace(selectedOption);
    }
    void Update()
    {

    }
   
    void LoadRace()
    {
        selectedOption = PlayerPrefs.GetInt(SELECTED_TRACK_KEY);
    }
    void SaveRace()
    {
        PlayerPrefs.SetInt(SELECTED_TRACK_KEY, selectedOption);
    }
    void UpdateRace(int selectedOption)
    {
        RaceSO raceSO = raceDB.GetRaceSO(selectedOption);
        trackImage.sprite = raceSO.trackSprite;
        trackName.text = raceSO.trackName;

        bool isLocked = !PlayerPrefs.HasKey(raceSO.idTrack + "_unlocked") || PlayerPrefs.GetInt(raceSO.idTrack + "_unlocked") == 0;
        unclocklPanel.SetActive(isLocked);
        //Debug.Log($"Track {trackSO.trackName} is {(isLocked ? "locked" : "unlocked")}");
    }

    //Button 
     public void OnClickNext()
    {
        selectedOption++;
        if (selectedOption >= raceDB.TrackCount)
        {
            selectedOption = 0;
        }
        UpdateRace(selectedOption);
        SaveRace();
    }
    public void OnClickBack()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = raceDB.TrackCount - 1;
        }
        UpdateRace(selectedOption);
        SaveRace();
    }
    public void OnSelectRace()
    {
        
        if (UIManager.HasInstance && RaceInfoManager.HasInstance)
        {
            RaceSO raceSO = raceDB.GetRaceSO(selectedOption);
            bool isLocked = !PlayerPrefs.HasKey(raceSO.idTrack + "_unlocked") || PlayerPrefs.GetInt(raceSO.idTrack + "_unlocked") == 0;
            if (!isLocked && RaceInfoManager.HasInstance && UIManager.HasInstance)
            {
                RaceInfoManager.Instance.raceSprite = raceSO.trackSprite;
                RaceInfoManager.Instance.raceToLoad = raceSO.idTrack;
                RaceInfoManager.Instance.raceToUnlock = raceSO.idTrackUnlock; // Track to unlock
                UIManager.Instance.raceSetupPanel.raceSelectImage.sprite = trackImage.sprite;
                UIManager.Instance.ChangeUIGameObject(this.gameObject, UIManager.Instance.raceSetupPanel.gameObject);
            }
            else
            {
                Debug.Log($"Track {raceSO.trackName} is locked!");
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
