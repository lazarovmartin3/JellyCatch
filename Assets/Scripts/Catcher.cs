using System.Net.NetworkInformation;
using UnityEngine;
using static ObjectPool;
using static Unity.VisualScripting.StickyNote;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class Catcher : MonoBehaviour
{
    private bool isDragging = false;
    private ObjectPool.jellyShape currentShape;
    private ObjectPool.jellyColor currentColor;
    private const float TOP_Y_POS = 1.7f;

    private void Start()
    {
        ObjectPool.jellyColor newColor = (ObjectPool.jellyColor)Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.jellyColor)).Length);
        ObjectPool.jellyShape newShape = (ObjectPool.jellyShape)Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.jellyShape)).Length);
        ChangeShape(newShape, newColor);
    }

    public void ChangeShape(ObjectPool.jellyShape newShape, ObjectPool.jellyColor newColor)
    {
        currentShape = newShape;
        currentColor = newColor;
        if (ObjectPool.Instance.GetSprite(newShape, newColor) == null)
            GetComponent<SpriteRenderer>().sprite = ObjectPool.Instance.GetRandomSprite();
        else
            GetComponent<SpriteRenderer>().sprite = ObjectPool.Instance.GetSprite(newShape, newColor);
        //GetComponent<SpriteRenderer>().color = ObjectPool.Instance.GetColor(newColor);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            //if (hit.collider != null && hit.collider.gameObject == gameObject)
            //{
            //    isDragging = true;
            //}
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mousePosition.y = transform.position.y; // Keep the y position constant
            mousePosition.y = TOP_Y_POS; // Keep the y position constant
            float newX = Mathf.Clamp(mousePosition.x, GetScreenBounds().min.x, GetScreenBounds().max.x);
            transform.position = new Vector3(newX, mousePosition.y, transform.position.z);
        }
    }

    private Bounds GetScreenBounds()
    {
        Vector3 lowerLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 upperRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        return new Bounds((lowerLeftCorner + upperRightCorner) * 0.5f, upperRightCorner - lowerLeftCorner);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Jelly"))
        {
            //if (collider.gameObject.GetComponent<Jelly>().currentShape == currentShape &&
            //    collider.gameObject.GetComponent<Jelly>().currentColor == currentColor)
            //{
            //    GameManager.Instance.IncreaseScore();
            //}

            collider.gameObject.GetComponent<Jelly>().Collided();

            ObjectPool.jellyColor newColor = (ObjectPool.jellyColor)Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.jellyColor)).Length);
            ObjectPool.jellyShape newShape = (ObjectPool.jellyShape)Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.jellyShape)).Length);
            ChangeShape(newShape, newColor);
        }
    }

    private void Impact()
    {

    }
}
