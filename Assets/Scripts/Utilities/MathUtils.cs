using UnityEngine;

/// <summary>
/// Math utility functions for game development.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Remap a value from one range to another
    /// </summary>
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
    
    /// <summary>
    /// Check if a value is approximately equal to another (with epsilon)
    /// </summary>
    public static bool Approximately(float a, float b, float epsilon = 0.0001f)
    {
        return Mathf.Abs(a - b) < epsilon;
    }
    
    /// <summary>
    /// Get a random point on a circle
    /// </summary>
    public static Vector2 RandomPointOnCircle(float radius)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }
    
    /// <summary>
    /// Get a random point inside a circle
    /// </summary>
    public static Vector2 RandomPointInCircle(float radius)
    {
        return Random.insideUnitCircle * radius;
    }
    
    /// <summary>
    /// Get a random point on a sphere surface
    /// </summary>
    public static Vector3 RandomPointOnSphere(float radius)
    {
        return Random.onUnitSphere * radius;
    }
    
    /// <summary>
    /// Get a random point inside a sphere
    /// </summary>
    public static Vector3 RandomPointInSphere(float radius)
    {
        return Random.insideUnitSphere * radius;
    }
    
    /// <summary>
    /// Calculate parabolic trajectory
    /// </summary>
    public static Vector3 CalculateParabolicPoint(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 mid = (start + end) / 2f;
        mid.y += height;
        
        Vector3 p0 = Vector3.Lerp(start, mid, t);
        Vector3 p1 = Vector3.Lerp(mid, end, t);
        
        return Vector3.Lerp(p0, p1, t);
    }
    
    /// <summary>
    /// Calculate velocity needed to reach target with ballistic arc
    /// </summary>
    public static Vector3 CalculateBallisticVelocity(Vector3 start, Vector3 target, float angle)
    {
        Vector3 direction = target - start;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);
        
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }
    
    /// <summary>
    /// Smooth damp angle (for rotation)
    /// </summary>
    public static float SmoothDampAngle(float current, float target, ref float velocity, float smoothTime)
    {
        return Mathf.SmoothDampAngle(current, target, ref velocity, smoothTime);
    }
    
    /// <summary>
    /// Wrap angle to -180 to 180 range
    /// </summary>
    public static float WrapAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            return angle - 360f;
        if (angle < -180f)
            return angle + 360f;
        return angle;
    }
    
    /// <summary>
    /// Get signed angle between two vectors
    /// </summary>
    public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
    {
        float angle = Vector3.Angle(from, to);
        float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(from, to)));
        return angle * sign;
    }
    
    /// <summary>
    /// Exponential decay for smooth transitions
    /// </summary>
    public static float ExponentialDecay(float a, float b, float decay, float deltaTime)
    {
        return b + (a - b) * Mathf.Exp(-decay * deltaTime);
    }
    
    /// <summary>
    /// Spring interpolation
    /// </summary>
    public static float Spring(float current, float target, ref float velocity, float springConstant, float dampingRatio, float deltaTime)
    {
        float force = (target - current) * springConstant;
        float damping = -velocity * dampingRatio;
        velocity += (force + damping) * deltaTime;
        current += velocity * deltaTime;
        return current;
    }
    
    /// <summary>
    /// Snap to grid
    /// </summary>
    public static Vector3 SnapToGrid(Vector3 position, float gridSize)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            Mathf.Round(position.z / gridSize) * gridSize
        );
    }
    
    /// <summary>
    /// Check if point is inside triangle (2D)
    /// </summary>
    public static bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
    {
        float areaABC = TriangleArea(a, b, c);
        float areaPBC = TriangleArea(point, b, c);
        float areaAPC = TriangleArea(a, point, c);
        float areaABP = TriangleArea(a, b, point);
        
        return Approximately(areaABC, areaPBC + areaAPC + areaABP);
    }
    
    /// <summary>
    /// Calculate triangle area (2D)
    /// </summary>
    public static float TriangleArea(Vector2 a, Vector2 b, Vector2 c)
    {
        return Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f);
    }
    
    /// <summary>
    /// Bounce vector off surface
    /// </summary>
    public static Vector3 Bounce(Vector3 velocity, Vector3 normal, float bounciness = 1f)
    {
        return Vector3.Reflect(velocity, normal) * bounciness;
    }
}

