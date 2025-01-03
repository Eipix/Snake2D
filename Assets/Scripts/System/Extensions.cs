using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
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

        int randomValue = rand.Next(0, totalWeight);

        foreach (var weight in array)
        {
            if (randomValue < weight.Value)
                return weight.Key;
            randomValue -= weight.Value;
        }

        return array.FirstOrDefault().Key;
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

    public static Color Invisible(this Color color)
    {
        return new Color(color.r, color.g, color.b, 0f);
    }
}
