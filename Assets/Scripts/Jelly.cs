using UnityEngine;
using System.Collections;
using System;

public class Jelly : MonoBehaviour
{
    public int id;
    public Vector2 gridPos = Vector2.zero;

    [SerializeField] private Animator animator;
    private const float DEATH_ANIMATION_LENGTH = 1.417f;
    private bool isDestroyed = false;

    public void Collided()
    {
        if (isDestroyed) return;
        // Mark the Jelly as being destroyed to prevent further animations
        isDestroyed = true;

        // Trigger the death animation
        animator.SetTrigger("Death");

        // Start the coroutine to wait for the animation to finish
        StartCoroutine(WaitForAnimation(DEATH_ANIMATION_LENGTH, () => Destroy(this.gameObject)));
    }

    public void TriggerAnimation(string animationName)
    {
        if(animator == null)
        {
            Wait(0.5f, () => animator?.SetTrigger(animationName));
        }
        else
            animator?.SetTrigger(animationName);
    }

    IEnumerator WaitForAnimation(float time, Action action)
    {
        // Wait for the next frame to ensure the animation has started playing
        yield return new WaitForSeconds(time);

        // Execute the action only if the Jelly is not already destroyed
        if (isDestroyed)
        {
            action();
        }
    }

    private IEnumerator Wait(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action?.Invoke();

        yield return null;
    }

    public int GetID()
    {
        return id;
    }
}
