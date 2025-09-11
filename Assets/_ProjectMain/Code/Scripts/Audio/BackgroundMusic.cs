using UnityEngine;
using UnityEngine.Audio;

public class BackgroundMusic : BaseManager<BackgroundMusic>
{
    [SerializeField] AudioMixer myMixer;
    [SerializeField] AudioSource musicPlayer;
    private AudioClip[] currentClipList;
    private int currentClipIndex;
    [SerializeField] AudioClip[] menuAudioClips; //Menu
    [SerializeField] AudioClip[] raceAudioClips; //Race

    protected override void Awake()
    {
        base.Awake();
    }
    
    void Start()
    {
        // Get currentSceneName for UIManager
        string currentScene = UIManager.Instance.currentSceneName;
        LoadVolume(); //Load default volume data
        PlayMusicForScene(currentScene);
    }
    private void LoadVolume()
    {
        // Get value form PlayerPrefs hoặc or use default value
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);

        // Apply volume value to Audiomixer
        myMixer.SetFloat("SFX", Mathf.Log10(sfxVolume) * 20);
        myMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
    }
    void Update()
    {
        // Kiểm tra nếu musicPlayer không phát nhạc và có danh sách nhạc
        if (!musicPlayer.isPlaying && currentClipList != null && currentClipList.Length > 0)
        {
            PlayNextTrack();
        }
    }

    // Hàm chọn và phát nhạc dựa trên scene
    private void PlayMusicForScene(string sceneName)
    {
       // Chọn danh sách nhạc dựa trên scene
        switch (sceneName)
        {
            case "Menu":
                currentClipList = menuAudioClips;
                break;
            case "Race1":
            case "Race2":
            case "Race3":
                currentClipList = raceAudioClips;
                break;
            default:
                currentClipList = menuAudioClips; // Mặc định dùng nhạc Menu nếu scene không xác định
                break;
        }

        // Kiểm tra nếu danh sách nhạc không rỗng
        if (currentClipList != null && currentClipList.Length > 0)
        {
            // Chọn ngẫu nhiên một bài nhạc
            currentClipIndex = Random.Range(0, currentClipList.Length);
            musicPlayer.clip = currentClipList[currentClipIndex];
            musicPlayer.Play();
        }
        else
        {
            Debug.LogWarning("No audio clips found for scene: " + sceneName);
        }
    }
    //Next track
    private void PlayNextTrack()
    {
        if (currentClipList == null || currentClipList.Length == 0)
        {
            Debug.LogWarning("No audio clips available to play.");
            return;
        }

        //Random music for play
        int newIndex = Random.Range(0, currentClipList.Length);
        while (newIndex == currentClipIndex && currentClipList.Length > 1)
        {
            newIndex = Random.Range(0, currentClipList.Length); // Avoid repeating the current music
        }
        currentClipIndex = newIndex;
        musicPlayer.clip = currentClipList[currentClipIndex];
        musicPlayer.Play();
    }
    //Update music
    public void UpdateMusicForScene(string sceneName)
    {
        PlayMusicForScene(sceneName);
    }
}
