using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Extension methods for common Unity types and operations.
/// </summary>
public static class ExtensionMethods
{
    #region Vector3 Extensions
    
    /// <summary>
    /// Set the X component of a Vector3
    /// </summary>
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }
    
    /// <summary>
    /// Set the Y component of a Vector3
    /// </summary>
    public static Vector3 WithY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }
    
    /// <summary>
    /// Set the Z component of a Vector3
    /// </summary>
    public static Vector3 WithZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
    
    /// <summary>
    /// Flatten Vector3 to XZ plane
    /// </summary>
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
    
    /// <summary>
    /// Get horizontal distance (ignoring Y)
    /// </summary>
    public static float HorizontalDistance(this Vector3 from, Vector3 to)
    {
        Vector3 flatFrom = from.Flatten();
        Vector3 flatTo = to.Flatten();
        return Vector3.Distance(flatFrom, flatTo);
    }
    
    #endregion
    
    #region Transform Extensions
    
    /// <summary>
    /// Reset transform to default values
    /// </summary>
    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    
    /// <summary>
    /// Set X position
    /// </summary>
    public static void SetX(this Transform transform, float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }
    
    /// <summary>
    /// Set Y position
    /// </summary>
    public static void SetY(this Transform transform, float y)
    {
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
    
    /// <summary>
    /// Set Z position
    /// </summary>
    public static void SetZ(this Transform transform, float z)
    {
        Vector3 pos = transform.position;
        pos.z = z;
        transform.position = pos;
    }
    
    /// <summary>
    /// Get all children recursively
    /// </summary>
    public static List<Transform> GetAllChildren(this Transform transform)
    {
        List<Transform> children = new List<Transform>();
        
        foreach (Transform child in transform)
        {
            children.Add(child);
            children.AddRange(child.GetAllChildren());
        }
        
        return children;
    }
    
    /// <summary>
    /// Destroy all children
    /// </summary>
    public static void DestroyAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    #endregion
    
    #region GameObject Extensions
    
    /// <summary>
    /// Get or add component
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
    
    /// <summary>
    /// Check if GameObject has component
    /// </summary>
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() != null;
    }
    
    /// <summary>
    /// Set layer recursively
    /// </summary>
    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetLayerRecursively(layer);
        }
    }
    
    #endregion
    
    #region Color Extensions
    
    /// <summary>
    /// Set alpha component of color
    /// </summary>
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
    
    /// <summary>
    /// Multiply color brightness
    /// </summary>
    public static Color MultipliedBy(this Color color, float multiplier)
    {
        return new Color(
            color.r * multiplier,
            color.g * multiplier,
            color.b * multiplier,
            color.a
        );
    }
    
    #endregion
    
    #region List Extensions
    
    /// <summary>
    /// Get random element from list
    /// </summary>
    public static T GetRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default(T);
        
        return list[Random.Range(0, list.Count)];
    }
    
    /// <summary>
    /// Shuffle list
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    #endregion
    
    #region Float Extensions
    
    /// <summary>
    /// Remap float from one range to another
    /// </summary>
    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
    
    /// <summary>
    /// Wrap float between min and max
    /// </summary>
    public static float Wrap(this float value, float min, float max)
    {
        float range = max - min;
        while (value < min) value += range;
        while (value > max) value -= range;
        return value;
    }
    
    #endregion
    
    #region Rigidbody Extensions
    
    /// <summary>
    /// Change Rigidbody direction while maintaining speed
    /// </summary>
    public static void ChangeDirection(this Rigidbody rb, Vector3 newDirection)
    {
        float speed = rb.velocity.magnitude;
        rb.velocity = newDirection.normalized * speed;
    }
    
    #endregion
}

