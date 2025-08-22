using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class CarSFXHandler : MonoBehaviour
{
    [Header("Mixers")]
    [SerializeField] AudioMixer audioMixer;

    [Header("Audio Sources")]
    [SerializeField] AudioSource tireScreenchingAudioSource;
    [SerializeField] AudioSource speedUpAudioSource;
    [SerializeField] AudioSource engineAudioSource;
    [SerializeField] AudioSource carHitAudioSource;
    [SerializeField] AudioSource carJumpAudioSource;
    [SerializeField] AudioSource carLandingAudioSource;

    private float desiredEnginePitch = 0.8f;
    private float tireScreenPtich = 0.5f;
    CarController carController;

    void Awake()
    {
        carController = GetComponentInParent<CarController>();
    }
    void Start()
    {
        audioMixer.SetFloat("SFXParam", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        EngineSound();
        Tiresound();
        SpeedUpSound();
    }
    void EngineSound()
    {
        float velocityMagnitude = carController.GetVelocityMagnitude();


        //Volume
        // float desireEngineVolume = velocityMagnitude * 0.05f;

        // desireEngineVolume = Mathf.Clamp(desireEngineVolume, 0.2f, 1.0f);

        //engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, desireEngineVolume, Time.deltaTime * 10);

        //Pitch
        desiredEnginePitch = velocityMagnitude * 0.2f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.8f, 2f);
        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);

    }
    void SpeedUpSound()
    {
        float speed = carController.GetVelocityMagnitude();
        float targetVolume = Mathf.Clamp01(speed / 15) * 1f;
        speedUpAudioSource.volume = targetVolume;

        float sppedSoundPitch = 0.8f + (Mathf.Abs(carController.GetVelocityMagnitude()) / 50f);
        speedUpAudioSource.pitch = sppedSoundPitch;
    }
    void Tiresound()
    {
        if (carController.IsTireScreeching(out float sidewaysVelocity, out bool isBraking))
        {
            if (isBraking)
            {
                tireScreenchingAudioSource.volume = Mathf.Lerp(tireScreenchingAudioSource.volume, 1.0f, Time.deltaTime * 10);
                tireScreenPtich = Mathf.Lerp(tireScreenPtich, 0.5f, Time.deltaTime * 10);
            }
            else
            {
                tireScreenchingAudioSource.volume = Mathf.Abs(sidewaysVelocity) * 0.05f;
                tireScreenPtich = Mathf.Abs(sidewaysVelocity) * 0.1f;
            }
        }
        else //Fade out the tire SFX
        {
            tireScreenchingAudioSource.volume = Mathf.Lerp(tireScreenchingAudioSource.volume, 0, Time.deltaTime * 10);
        }
    }
    public void JumpSFX()
    {
        carJumpAudioSource.Play();
    }
    public void LandingSFX()
    {
        carLandingAudioSource.Play();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Get the relative velocity of the collision
        float relativeVelocity = collision.relativeVelocity.magnitude;

        float volume = relativeVelocity * 0.1f;

        carHitAudioSource.pitch = Random.Range(0.95f, 1.05f);
        carHitAudioSource.volume = volume;
        if (!carHitAudioSource.isPlaying)
        {
            carHitAudioSource.Play();
        }
    }
}
