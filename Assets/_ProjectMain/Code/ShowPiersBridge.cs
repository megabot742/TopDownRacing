using System.Collections.Generic;
using UnityEngine;

public class ShowPiersBridge : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> piersCollider = new List<SpriteRenderer>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    CarLayerHandler carLayerHandler;
    void Start()
    {
        carLayerHandler = GetComponentInParent<CarLayerHandler>();
        foreach (SpriteRenderer spriteRenderer in piersCollider)
        {
            spriteRenderer.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PiersCheck") && !carLayerHandler.IsDrivingOnOverPass())
        {
            //Debug.Log("PlayerIn");
            foreach (SpriteRenderer spriteRenderer in piersCollider)
            {
                spriteRenderer.enabled = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PiersCheck") && !carLayerHandler.IsDrivingOnOverPass())
        {
            //Debug.Log("PlayerOut");
            foreach (SpriteRenderer spriteRenderer in piersCollider)
            {
                spriteRenderer.enabled = false;
            }
        }
    }
}
