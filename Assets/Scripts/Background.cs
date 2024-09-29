using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour
{
    public Color startColor = new Color(0.1f, 0.1f, 0.3f);
    public Color endColor = new Color(0.2f, 0.1f, 0.3f);
    public float duration = 5f;

    private SpriteRenderer spriteRenderer;
    private float scrollSpeed = 0.5f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //if (spriteRenderer == null)
        //{
        //    Debug.LogError("BackgroundGradient script requires a SpriteRenderer component.");
        //    enabled = false;
        //    return;
        //}

        //// Start gradient animation
        //StartCoroutine(AnimateGradient());
    }

    private void Update()
    {
    }

    IEnumerator AnimateGradient()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime / duration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, Mathf.PingPong(t, 1f));
            yield return null;
        }
    }
}
