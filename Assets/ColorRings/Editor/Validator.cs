using UnityEditor;
using UnityEngine;

public static class Validator
{
    [MenuItem("GameObject/Validator/Validate Active", false, 10)]
    public static void ValidateActive(MenuCommand menuCommand)
    {
        if (menuCommand.context is GameObject gameObject)
        {
            Validate(gameObject, false);
        }
    }

    [MenuItem("GameObject/Validator/Validate", false, 10)]
    public static void Validate(MenuCommand menuCommand)
    {
        if (menuCommand.context is GameObject gameObject)
        {
            Validate(gameObject, true);
        }
    }

    private static void Validate(GameObject gameObject, bool ignoreInActive)
    {
        if (gameObject == null) return;
        var validators = gameObject.GetComponentsInChildren<IValidator>(ignoreInActive);
        var i = 0;
        foreach (var validator in validators)
        {
            i++;
            Debug.Log($"validate {i}/{validators.Length}");
            validator.Validate();
        }
    }
}