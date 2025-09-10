using UnityEngine;

public class WheelParticHandler : MonoBehaviour
{
    [SerializeField] private bool isOverpassEmission = false;
    float particleEmissionRate = 0;
    CarController carController;
    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleSystemEmissionModule;
    CarLayerHandler carLayerHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        carController = GetComponentInParent<CarController>();
        particleSystemSmoke = GetComponent<ParticleSystem>();
        carLayerHandler = GetComponentInParent<CarLayerHandler>();
        particleSystemEmissionModule = particleSystemSmoke.emission; //Get emissiion
        particleSystemEmissionModule.rateOverTime = 0; // Sest zero emission
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Reduce the particles over time
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if (carController.IsTireScreeching(out float sidewaysVelocity, out bool isBraking))
        {
            if (isBraking)
            {
                if (carLayerHandler.IsDrivingOnOverPass() && isOverpassEmission) //on Bridge
                {
                    particleEmissionRate = 30;
                }
                else if (!carLayerHandler.IsDrivingOnOverPass() && !isOverpassEmission) //on Race
                {
                    particleEmissionRate = 30;
                }     
            }
            else
            {
                if (carLayerHandler.IsDrivingOnOverPass() && isOverpassEmission) //on Bridge
                {
                    particleEmissionRate = Mathf.Abs(sidewaysVelocity) * 2;
                }
                else if (!carLayerHandler.IsDrivingOnOverPass() && !isOverpassEmission) //on Race
                {
                    particleEmissionRate = Mathf.Abs(sidewaysVelocity) * 2;
                }
                
            }
        }
    }
}
