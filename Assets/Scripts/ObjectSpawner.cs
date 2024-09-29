using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class ObjectSpawner : MonoBehaviour
{
    public static ObjectSpawner Instance;

    [SerializeField] private Transform[] spawningPoints;
    private List<GameObject> spawnedObjects;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnedObjects = new List<GameObject>();
        SpawnObject();
    }

    private void SpawnObject()
    {
        int rand = Random.Range(0, spawningPoints.Length);

        //ObjectPool.ObjectShape shape = (ObjectPool.ObjectShape)Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.ObjectShape)).Length);
        //ObjectPool.ObjectColor color = (ObjectPool.ObjectColor)Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.ObjectColor)).Length);

        ObjectPool.jellyShape jellyShape = (ObjectPool.jellyShape) Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.jellyShape)).Length);
        ObjectPool.jellyColor jellyColor = (ObjectPool.jellyColor)Random.Range(0, System.Enum.GetValues(typeof(ObjectPool.jellyColor)).Length);

        //GameObject jelly = ObjectPool.Instance.GetObject(shape, color);
        GameObject jelly = ObjectPool.Instance.GetJelly(jellyShape, jellyColor);
        jelly.transform.parent = this.transform;
        jelly.transform.position = spawningPoints[rand].position;
        //jelly.GetComponent<Jelly>().currentColor = jellyColor;
        //jelly.GetComponent<Jelly>().currentShape = jellyShape;
        spawnedObjects.Add(jelly);
    }

    public void RemoveObject(GameObject jelly)
    {
        spawnedObjects.Remove(jelly);
        SpawnObject();
    }
}
