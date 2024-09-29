using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Drawing;
using Unity.Mathematics;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public enum jellyColor { brown, blue, green, purple };
    public enum jellyShape { square, circle, heart, star, trapeze, rhombus, flower, triangle };

    [Serializable]
    public struct JellyType
    {
        public Sprite jellySprite;
        public jellyColor jellyColor;
        public jellyShape jellyShape;
        public int id;
    }

    [SerializeField] private List<JellyType> jellyTypes;

    public enum ObjectShape { circle,  square };
    public enum ObjectColor { red, green, blue };

    [SerializeField] private GameObject jellyPrefab;

    //New Style
    public List<GameObject> jellyPrefabs = new List<GameObject>();

    private Sprite squareSprite;
    private Sprite circleSprite;

    private void Awake()
    {
        Instance = this;
    }

    //public GameObject GetObject(ObjectShape shape, ObjectColor color)
    //{
    //    GameObject jelly = Instantiate(jellyPrefab);
    //    switch(shape)
    //    {
    //        case ObjectShape.circle:
    //            jelly.GetComponent<SpriteRenderer>().sprite = circleSprite;
    //            break;
    //        case ObjectShape.square:
    //            jelly.GetComponent<SpriteRenderer>().sprite = squareSprite;
    //            break;
    //    }

    //    Color colorJelly = Color.black;
    //    switch (color)
    //    {
    //        case ObjectColor.red:
    //            colorJelly = Color.red;
    //            break;
    //        case ObjectColor.green:
    //            colorJelly = Color.green;
    //            break;
    //        case ObjectColor.blue:
    //            colorJelly = Color.blue;
    //            break;
    //    }

    //    jelly.GetComponent<Renderer>().material.color = colorJelly;
    //    return jelly;
    //}

    public GameObject GetJelly(jellyShape shape, jellyColor color)
    {
        GameObject jelly = Instantiate(jellyPrefab);
        for (int i = 0; i < jellyTypes.Count; i++)
        {
            if (jellyTypes[i].jellyShape == shape && jellyTypes[i].jellyColor == color)
            {
                jellyPrefab.GetComponent<SpriteRenderer>().sprite = jellyTypes[i].jellySprite;
                break;
            }
        }
        return jelly;
    }

    public GameObject GetRandomJelly()
    {
        int rand = UnityEngine.Random.Range(0, jellyPrefabs.Count);
        return jellyPrefabs[rand];
    }

    public Sprite GetRandomSprite()
    {
        int rand = UnityEngine.Random.Range(0, jellyTypes.Count);
        return jellyTypes[rand].jellySprite;
    }

    public Sprite GetSprite(jellyShape shape , jellyColor color)
    {
        //switch (shape)
        //{
        //    case ObjectShape.circle:
        //        return circleSprite;
        //    case ObjectShape.square:
        //        return squareSprite;
        //}
        for (int i = 0; i < jellyTypes.Count; i++)
        {
            if (jellyTypes[i].jellyShape == shape && jellyTypes[i].jellyColor == color)
            {
                return jellyPrefab.GetComponent<SpriteRenderer>().sprite = jellyTypes[i].jellySprite;
            }
        }
        return null;
    }

    //public Color GetColor(ObjectColor color)
    //{
    //    switch (color)
    //    {
    //        case ObjectColor.red:
    //            return Color.red;
    //        case ObjectColor.green:
    //            return Color.green;
    //        case ObjectColor.blue:
    //            return Color.blue;
    //    }
    //    return Color.white;
    //}
}
