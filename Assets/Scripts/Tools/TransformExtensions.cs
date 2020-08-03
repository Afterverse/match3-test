﻿using System.Collections;
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

    public static IEnumerator Fall(this Transform t, Vector3 target, float gravity)
    {
        if (t == null) { yield break; };

        Vector3 velocity = Vector3.zero;

        while (t.position.y > target.y)
        {
            yield return null;
            if (t == null) { yield break; };
            velocity = velocity + new Vector3(0f, -gravity, 0f) * Time.deltaTime;
            t.position = t.position + velocity * Time.deltaTime;
        }

        // Make sure that it snaps to the end position.
        t.position = target;
    }
}
