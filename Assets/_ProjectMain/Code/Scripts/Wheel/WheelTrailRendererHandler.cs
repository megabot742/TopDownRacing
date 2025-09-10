using Unity.VisualScripting;
using UnityEngine;

public class WheelTrailRendererHandler : MonoBehaviour
{

    [SerializeField] private bool isOverpassEmitter = false;
    CarController carController;
    TrailRenderer trailRenderer;
    CarLayerHandler carLayerHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        carController = GetComponentInParent<CarController>();
        trailRenderer = GetComponent<TrailRenderer>();
        carLayerHandler = GetComponentInParent<CarLayerHandler>();
        trailRenderer.emitting = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        trailRenderer.emitting = false;

        if (carController.IsTireScreeching(out float sidewaysVelocity, out bool isBraking))
        {
            if (carLayerHandler.IsDrivingOnOverPass() && isOverpassEmitter) //on Bridge
            {
                trailRenderer.emitting = true;
            }
            else if (!carLayerHandler.IsDrivingOnOverPass() && !isOverpassEmitter) //on Race
            {
                trailRenderer.emitting = true;
            }  
        }
    }
}
