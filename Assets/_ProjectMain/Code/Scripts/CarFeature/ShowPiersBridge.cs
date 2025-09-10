using System.Collections.Generic;
using UnityEngine;

public class ShowPiersBridge : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> piersCollider = new List<SpriteRenderer>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] CarLayerHandler carLayerHandler;
    void Start()
    {
        carLayerHandler = GetComponentInParent<CarLayerHandler>();
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("UnderpassCollider");

        // Duyệt qua từng GameObject và lấy SpriteRenderer
        foreach (GameObject obj in objectsWithTag)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) // Kiểm tra xem có SpriteRenderer không
            {
                piersCollider.Add(spriteRenderer); // Thêm vào danh sách
                spriteRenderer.enabled = false; // Tắt SpriteRenderer
            }
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
