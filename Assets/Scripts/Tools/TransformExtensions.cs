using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static IEnumerator Move(this Transform t, Vector3 target, float duration)
    {
        Vector3 startingPos = t.position;
        float timer = 0f;

        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            t.position = Vector3.Lerp(startingPos, target, timer / duration);
        }

        // Make sure that it snaps to the end position.
        t.position = target;
    }

    public static IEnumerator Scale(this Transform t, Vector3 target, float duration)
    {
        Vector3 startingScale = t.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(startingScale, target, timer / duration);
        }

        // Make sure that it snaps to the end scale.
        t.localScale = target;
    }
}
