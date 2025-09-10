using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] AudioMixer myMixed;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider musicSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Kiểm tra và load hoặc đặt giá trị mặc định
        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("SFXVolume"))
        {
            LoadVolume();
        }
        else
        {
            // Đặt giá trị mặc định cho slider nếu chưa có PlayerPrefs
            sfxSlider.value = 1f;
            musicSlider.value = 1f;
            SetSFXVolume();
            SetMusicVolume();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void LoadVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume",1f);
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume",1f);

        SetSFXVolume();
        SetMusicVolume();
    }
    public void SetSFXVolume()
    {
        float sfxVolume = sfxSlider.value;
        myMixed.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    public void SetMusicVolume()
    {
        float musicVolume = musicSlider.value;
        myMixed.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }
    //Button
    public void OnClickSave()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ChangeUIGameObject(this.gameObject, UIManager.Instance.homeMenuPanel.gameObject);
            SetSFXVolume();
            SetMusicVolume();
            PlayerPrefs.Save();
            Debug.Log("Save");
        }
    }
    public void OnCLickCleardata()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Keys deleted");
        PlayerPrefs.SetInt(RaceInfoManager.Instance.raceToLoad + "_unlocked", 1);
        PlayerPrefs.Save();
    }
    public void OnClickClose()
    {
        if (UIManager.HasInstance)
        {
            UIManager.Instance.ChangeUIGameObject(this.gameObject, UIManager.Instance.homeMenuPanel.gameObject);
        }
    }
}
