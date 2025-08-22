using System.Collections.Generic;
using UnityEngine;

public class CarLayerHandler : MonoBehaviour
{
    [SerializeField] SpriteRenderer carOutlineSpriteRenderer;
    [SerializeField] private List<SpriteRenderer> listSpriteRenderers = new List<SpriteRenderer>();
    [SerializeField] private List<Collider2D> overpassColliderList = new List<Collider2D>();
    [SerializeField] private List<Collider2D> underpassColliderList = new List<Collider2D>();
    
    //State
    [SerializeField] private bool isDrivingOnOverPass;

    Collider2D carCollider;

    void Awake()
    {
        foreach (SpriteRenderer spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (spriteRenderer.sortingLayerName == "Player")
            {
                listSpriteRenderers.Add(spriteRenderer);
            }
        }
        //OverPass
        foreach (GameObject overpassColliderGameObject in GameObject.FindGameObjectsWithTag("OverpassCollider"))
        {
            overpassColliderList.Add(overpassColliderGameObject.GetComponent<Collider2D>());
        }
        //UnderPass
        foreach (GameObject underpassColliderGameObject in GameObject.FindGameObjectsWithTag("UnderpassCollider"))
        {
            underpassColliderList.Add(underpassColliderGameObject.GetComponent<Collider2D>());
        }

        carCollider = GetComponentInChildren<Collider2D>();
    }
    
    void Start()
    {
        UpdateSortingLayers();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UnderpassTrigger"))
        {
            isDrivingOnOverPass = false;
            UpdateSortingLayers();
        }
        else if (collision.CompareTag("OverpassTrigger"))
        {
            isDrivingOnOverPass = true;
            UpdateSortingLayers();
        }
    }
    void UpdateSortingLayers()
    {
        if (isDrivingOnOverPass)
        {
            SetSortingLayers("RaceTrackOverpass");
            carOutlineSpriteRenderer.enabled = false;
        }
        else
        {
            SetSortingLayers("Player");
            carOutlineSpriteRenderer.enabled = true;
        }

        SetCollisionWithOverPass();
    }
    void SetSortingLayers(string layerName)
    {
        foreach (SpriteRenderer spriteRenderer in listSpriteRenderers)
        {
            spriteRenderer.sortingLayerName = layerName;
        }
    }
    void SetCollisionWithOverPass()
    {
        foreach (Collider2D collider2D in overpassColliderList)
        {
            Physics2D.IgnoreCollision(carCollider, collider2D, !isDrivingOnOverPass);
        }

        foreach (Collider2D collider2D in underpassColliderList)
        {
            if (isDrivingOnOverPass)
            {
                Physics2D.IgnoreCollision(carCollider, collider2D, true);
            }
            else
            {
                Physics2D.IgnoreCollision(carCollider, collider2D, false);
            }
        }
    }
    public bool IsDrivingOnOverPass()
    {
        return isDrivingOnOverPass;
    }
}
