using UnityEngine;

public static class ComponentHelper
{

    //I add so many components, so I made this to keep track of them. It creates a new component if it isn't already there, but if it needs to create a new component it will.
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }
}
