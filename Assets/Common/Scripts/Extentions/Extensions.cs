using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public static class Extensions
{
    public static Vector2 GetDirection(this Vector2 vector2)
    {
        if (Math.Abs(vector2.x) > Math.Abs(vector2.y))
            return vector2.x > 0 ? Vector2.right : Vector2.left;
        else
            return vector2.y > 0 ? Vector2.up : Vector2.down;
    }

    public static void ForEach<T>(this T[] ts, Action<T> action)
    {
        foreach (var t in ts)
        {
            action.Invoke(t);
        }
    }

    public static string GetPathInHierarchy(this Transform transform)
    {
        StringBuilder path = new StringBuilder();
        Transform current = transform;
        while (current != null)
        {
            path.Insert(0, current.name);
            if (current.parent != null)
                path.Insert(0, "/");

            current = current.parent;
        }

        return path.ToString();
    }

    public static TimeSpan GetHourToAdd(DateTime now, int toHour)
    {
        int hour = now.Hour >= toHour
            ? 24 - now.Hour + toHour
            : toHour - now.Hour;

        return new TimeSpan(hour, 0, 0) - new TimeSpan(0, now.Minute, now.Second);
    }

    public static void CompleteIfActive(this Tween tween, bool withCallbacks = false)
    {
        if (tween != null && tween.IsActive())
            tween.Complete(withCallbacks);
    }

    public static void CompleteIfActiveAll(this Tween[] tweens, bool withCallbacks = false)
    {
        foreach (var tween in tweens)
        {
            if (tween != null && tween.IsActive())
                tween.Complete(withCallbacks);
        }
    }

    public static TextMeshProUGUI SetAssetText(this TextMeshProUGUI TMPText, AssetText assetText)
    {
        TMPText.text = assetText.Text.Translate;
        TMPText.fontMaterial = assetText.Preset;
        TMPText.color = assetText.Color;
        return TMPText;
    }

    public static bool withProbability(this System.Random rand, int percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        int[] array = new int[100];

        for (int i = 0; i < array.Length; i++)
            array[i] = 0;

        for (int i = 0; i < percentage;)
        {
            int index = rand.Next(100);
            if (array[index] == 0)
            {
                array[index] = 1;
                i++;
            }
        }
        return array[rand.Next(100)] == 1;
    }

    public static T Element<T>(this System.Random rand, Dictionary<T, int> array)
    {
        int totalWeight = 0;
        foreach (var weight in array)
        {
            totalWeight += weight.Value;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);

        foreach (var weight in array)
        {
            if (randomValue < weight.Value)
                return weight.Key;
            randomValue -= weight.Value;
        }

        return array.FirstOrDefault().Key;
    }

    public static T Element<T>(this System.Random rand, Dictionary<T, float> array)
    {
        float totalWeight = 0;
        foreach (var weight in array)
        {
            totalWeight += weight.Value;
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        foreach (var weight in array)
        {
            if (randomValue < weight.Value && randomValue != 0)
                return weight.Key;

            randomValue -= weight.Value;
        }
        return array.FirstOrDefault().Key;
    }

    public static T Element<T>(this System.Random rand, T[] elements, float[] chances)
    {
        if (elements.Length != chances.Length)
            throw new InvalidOperationException("Arrays must be the same size");

        float totalWeight = 0;
        foreach (var weight in chances)
        {
            totalWeight += weight;
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < chances.Length; i++)
        {
            if (randomValue < chances[i])
                return elements[i];
            randomValue -= chances[i];
        }

        return elements.FirstOrDefault();
    }

    public static void SwapValues<T>(ref T t0, ref T t1)
    {
        var temp = t1;
        t1 = t0;
        t0 = temp;
    }

    public static T[] Shuffled<T>(this T[] array)
    {
        if (array == null)
            throw new NullReferenceException();

        System.Random random = new System.Random();
        T[] shuffled = array.OrderBy(x => random.Next()).ToArray();

        return shuffled;
    }

    public static List<T> Shuffled<T>(this List<T> list)
    {
        if (list == null)
            throw new NullReferenceException();

        System.Random random = new System.Random();
        List<T> shuffled = list.OrderBy(x => random.Next()).ToList();

        return shuffled;
    }

    public static void SetAndPlay(this VideoPlayer player, VideoClip clip)
    {
        player.clip = clip;
        player.Play();
    }

    public static void SetAndPlay(this VideoPlayer player, string url)
    {
        player.url = url;
        player.Play();
    }

    public static IEnumerator WaitPreparationAndPlay(this VideoPlayer player, string url)
    {
        player.url = url;
        player.Prepare();
        yield return new WaitUntil(() => player.isPrepared);
        player.Play();
        yield return new WaitUntil(() => player.isPlaying);
    }

    public static IEnumerator WaitPreparation(this VideoPlayer player)
    {
        if (player.isPrepared)
            yield break;

        player.Prepare();
        yield return new WaitUntil(() => player.isPrepared);
    }

    public static IEnumerator WaitStarted(this VideoPlayer player)
    {
        player.Play();
        yield return new WaitUntil(() => player.isPlaying);
    }

    public static void DisableAll<T>(this T[] array) where T : Component
    {
        foreach (var element in array)
        {
            element.gameObject.SetActive(false);
        }
    }

    public static void DisableAll<T>(this IEnumerable<T> collection) where T : Component
    {
        foreach (var element in collection)
        {
            element.gameObject.SetActive(false);
        }
    }

    public static void FadeAll(this MaskableGraphic[] graphics, float fade = 0)
    {
        foreach (var graphic in graphics)
        {
            graphic.color = graphic.color.Fade(fade);
        }
    }

    public static Color Fade(this Color color, float fade = 0f)
    {
        return new Color(color.r, color.g, color.b, fade);
    }
}

