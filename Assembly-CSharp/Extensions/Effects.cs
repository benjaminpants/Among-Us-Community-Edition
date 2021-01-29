using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Effects
{
    private static HashSet<Transform> activeShakes;

    public static IEnumerator Wait(float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            yield return null;
        }
    }

    public static IEnumerator All(params IEnumerator[] items)
    {
        Stack<IEnumerator>[] enums = new Stack<IEnumerator>[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            enums[i] = new Stack<IEnumerator>();
            enums[i].Push(items[i]);
        }
        int cap = 0;
        while (cap < 100000)
        {
            bool flag = false;
            for (int j = 0; j < enums.Length; j++)
            {
                if (enums[j].Count <= 0)
                {
                    continue;
                }
                flag = true;
                IEnumerator enumerator = enums[j].Peek();
                if (enumerator.MoveNext())
                {
                    if (enumerator.Current is IEnumerator)
                    {
                        enums[j].Push((IEnumerator)enumerator.Current);
                    }
                }
                else
                {
                    enums[j].Pop();
                }
            }
            if (flag)
            {
                yield return null;
                int num = cap + 1;
                cap = num;
                continue;
            }
            break;
        }
    }

    internal static IEnumerator ScaleIn(Transform self, float source, float target, float duration)
    {
        if ((bool)self)
        {
            Vector3 localScale = default(Vector3);
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                localScale.x = (localScale.y = (localScale.z = Mathf.SmoothStep(source, target, t / duration)));
                self.localScale = localScale;
                yield return null;
            }
            localScale.z = target;
            localScale.y = target;
            localScale.x = target;
            self.localScale = localScale;
        }
    }

    internal static IEnumerator CycleColors(SpriteRenderer self, Color source, Color target, float rate, float duration)
    {
        if ((bool)self)
        {
            self.enabled = true;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                float t2 = Mathf.Sin(t * (float)Math.PI / rate) / 2f + 0.5f;
                self.color = Color.Lerp(source, target, t2);
                yield return null;
            }
            self.color = source;
        }
    }

    internal static IEnumerator PulseColor(SpriteRenderer self, Color source, Color target, float duration = 0.5f)
    {
        if ((bool)self)
        {
            self.enabled = true;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                self.color = Color.Lerp(target, source, t / duration);
                yield return null;
            }
            self.color = source;
        }
    }

    public static IEnumerator ColorFade(SpriteRenderer self, Color source, Color target, float duration)
    {
        if ((bool)self)
        {
            self.enabled = true;
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                self.color = Color.Lerp(source, target, t / duration);
                yield return null;
            }
            self.color = target;
        }
    }

    public static IEnumerator Rotate2D(Transform target, float source, float dest, float duration = 0.75f)
    {
        Vector3 temp = target.localEulerAngles;
        for (float time = 0f; time < duration; time += Time.deltaTime)
        {
            float t = time / duration;
            temp.z = Mathf.SmoothStep(source, dest, t);
            target.localEulerAngles = temp;
            yield return null;
        }
        temp.z = dest;
        target.localEulerAngles = temp;
    }

    public static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration = 0.75f)
    {
        Vector3 temp = default(Vector3);
        temp.z = target.localPosition.z;
        for (float time = 0f; time < duration; time += Time.deltaTime)
        {
            float t = time / duration;
            temp.x = Mathf.SmoothStep(source.x, dest.x, t);
            temp.y = Mathf.SmoothStep(source.y, dest.y, t);
            target.localPosition = temp;
            yield return null;
        }
        temp.x = dest.x;
        temp.y = dest.y;
        target.localPosition = temp;
    }

    public static IEnumerator Bounce(Transform target, float duration = 0.3f, float height = 0.15f)
    {
        if (!target)
        {
            yield break;
        }
        Vector3 origin = target.localPosition;
        Vector3 temp = origin;
        for (float timer = 0f; timer < duration; timer += Time.deltaTime)
        {
            float num = timer / duration;
            float num2 = 1f - num;
            temp.y = origin.y + height * Mathf.Abs(Mathf.Sin(num * (float)Math.PI * 3f)) * num2;
            if (!target)
            {
                yield break;
            }
            target.localPosition = temp;
            yield return null;
        }
        if ((bool)target)
        {
            target.transform.localPosition = origin;
        }
    }

    public static IEnumerator Shake(Transform target, float duration = 0.75f, float halfWidth = 0.25f)
    {
        if (activeShakes.Add(target))
        {
            Vector3 origin = target.localPosition;
            for (float timer = 0f; timer < duration; timer += Time.deltaTime)
            {
                float num = timer / duration;
                target.localPosition = origin + Vector3.right * (halfWidth * Mathf.Sin(num * 30f) * (1f - num));
                yield return null;
            }
            target.transform.localPosition = origin;
            activeShakes.Remove(target);
        }
    }

    public static IEnumerator Bloop(float delay, Transform target, float duration = 0.5f)
    {
        for (float t = 0f; t < delay; t += Time.deltaTime)
        {
            yield return null;
        }
        Vector3 localScale = default(Vector3);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float z = ElasticOut(t, duration);
            localScale.x = (localScale.y = (localScale.z = z));
            target.localScale = localScale;
            yield return null;
        }
        localScale.x = (localScale.y = (localScale.z = 1f));
        target.localScale = localScale;
    }

    private static float ElasticOut(float time, float duration)
    {
        time /= duration;
        float num = time * time;
        float num2 = num * time;
        return 33f * num2 * num + -106f * num * num + 126f * num2 + -67f * num + 15f * time;
    }

    static Effects()
    {
        activeShakes = new HashSet<Transform>();
    }

    public static IEnumerator BloopHalf(float delay, Transform target, float duration = 0.5f)
    {
        for (float t = 0f; t < delay; t += Time.deltaTime)
        {
            yield return null;
        }
        Vector3 a = default(Vector3);
        Vector3 mult = new Vector3(0.5f, 1f, 1f);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float z = ElasticOut(t, duration);
            a.x = (a.y = (a.z = z));
            target.localScale = Vector3.Scale(a, mult);
            yield return null;
        }
        a.x = (a.y = (a.z = 1f));
        target.localScale = Vector3.Scale(a, mult);
    }

    public static IEnumerator BloopEntireHalf(float delay, Transform target, float duration = 0.5f)
    {
        for (float t = 0f; t < delay; t += Time.deltaTime)
        {
            yield return null;
        }
        Vector3 a = default(Vector3);
        Vector3 mult = new Vector3(0.5f, 0.5f, 1f);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float z = ElasticOut(t, duration);
            a.x = (a.y = (a.z = z));
            target.localScale = Vector3.Scale(a, mult);
            yield return null;
        }
        a.x = (a.y = (a.z = 1f));
        target.localScale = Vector3.Scale(a, mult);
    }
}
