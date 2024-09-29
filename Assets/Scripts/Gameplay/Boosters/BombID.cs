using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BombID : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private Button bombBtn;
    [SerializeField] private GameObject bombPrefab;
    private GameObject bombInstance;
    private int targetID = -1;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (EventSystem.current.currentSelectedGameObject.name == bombBtn.name)
                {
                    PickBomb();
                }
            }
        }

        if (bombInstance)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics2D.GetRayIntersectionAll(ray, 1500f);
            foreach ( var hit in hits )
            {
                if (hit.collider.gameObject.GetComponent<Jelly>())
                   targetID  = hit.collider.gameObject.GetComponent<Jelly>().GetID();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bombInstance)
        {
            bombInstance.transform.position = Input.mousePosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (bombInstance)
        {
            bombInstance.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(bombInstance);
        bombInstance = null;
        Explode();
    }

    private void PickBomb()
    {
        bombInstance = Instantiate(bombPrefab,transform);
        bombInstance.transform.position = Input.mousePosition;
    }

    private void Explode()
    {
        List<GameObject> objects = new List<GameObject>(Grid.Instance.GetObjectsWithID(targetID));
        foreach (GameObject obj in objects)
        {
            obj.GetComponent<Jelly>().Collided();
        }
        StartCoroutine(Wait(1.5f, Grid.Instance.ReorderGrid));
    }

    private IEnumerator Wait(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action?.Invoke();

        yield return null;
    }
 
}
