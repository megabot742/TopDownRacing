using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public enum SliderType
    {
        Laps,
        AI
    }
    [System.Serializable]
    public struct SliderData
    {
        public SliderType type;
        public Slider slider;
        public TMP_Text valueText;
        public float defaultValue;
        public string playerPrefsKey; //key
    }
    [SerializeField] SliderData[] sliders; //list of Slider
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    const string VALUE_SLIDER_KEY = "valueSlider";
    void Start()
    {
        foreach (var sliderData in sliders)
        {
            if (!PlayerPrefs.HasKey(sliderData.playerPrefsKey))
            {
                PlayerPrefs.SetFloat(sliderData.playerPrefsKey, sliderData.defaultValue); //default
                LoadValue(sliderData);
            }
            else
            {
                LoadValue(sliderData);
            }
        }
    }
    void Update()
    {
        if (RaceInfoManager.HasInstance)
        {
            foreach (var sliderData in sliders)
            {
                int value = Mathf.RoundToInt(sliderData.slider.value);
                switch (sliderData.type)
                {
                    case SliderType.Laps:
                        RaceInfoManager.Instance.numberOfLap = value;
                        break;
                    case SliderType.AI:
                        RaceInfoManager.Instance.numberOfAI = value;
                        break;
                }
                //Update Text
                if (sliderData.valueText != null)
                {
                    sliderData.valueText.text = value.ToString();
                }
            }

        }
    }
    void LoadValue(SliderData sliderData)
    {
        sliderData.slider.value = PlayerPrefs.GetFloat(sliderData.playerPrefsKey, sliderData.defaultValue);
        if (sliderData.valueText != null)
        {
            sliderData.valueText.text = Mathf.RoundToInt(sliderData.slider.value).ToString();
        }
    }
    public void ChangeValue(int sliderIndex) //index in sliders list
    {

        if (sliderIndex >= 0 && sliderIndex < sliders.Length)
        {
            var sliderData = sliders[sliderIndex];
            PlayerPrefs.SetFloat(sliderData.playerPrefsKey, sliderData.slider.value);
            PlayerPrefs.Save();
            if (sliderData.valueText != null)
            {
                sliderData.valueText.text = Mathf.RoundToInt(sliderData.slider.value).ToString();
            }
        }
    }
}
