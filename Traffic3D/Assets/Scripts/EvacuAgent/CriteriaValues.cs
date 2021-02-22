using System;
using System.Collections.Generic;
using UnityEngine;

public static class CriteriaValues
{
    private static Dictionary<string, bool> criteriaDictionary = new Dictionary<string, bool>()
    {
        { "footfall", true },
        { "distance", false }
    };

    public static bool GetCriteriaValueFromName(string name)
    {
        if (criteriaDictionary.ContainsKey(name))
        {
            return criteriaDictionary[name];
        }

        Debug.Log($"The name \"{name}\" is not in the CriteriaValueDictionary");
        throw new ArgumentException();
    }
}
