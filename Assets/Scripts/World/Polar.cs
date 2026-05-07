using UnityEngine;

public class Polar
{
    public float rad;
    public float dist;

    public static Polar Zero { get { return new Polar(0, 0); } }
    public static float CircleRad { get { return Mathf.PI * 2; } }

    public Polar(float rad, float dist)
    {
        this.rad = rad;
        this.dist = dist;
    }

    public Vector2 ToCartesian()
    {
        float x = Mathf.Cos(rad) * dist;
        float y = Mathf.Sin(rad) * dist;

        return new Vector2(x, y);
    }
    public Vector2Int ToCartesianInt()
    {
        Vector2 pos = ToCartesian();
        return new Vector2Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.y)
            );
    }

    public Vector3 ToCartesian3()
    {
        Vector2 v = ToCartesian();

        return new Vector3(v.x, 0, v.y);
    }

    public static Polar operator +(Polar a, Polar b)
    {
        Vector2 a2 = a.ToCartesian();
        Vector2 b2 = b.ToCartesian();
        Polar polar = FromVector2(a2 + b2);
        return polar;
    }


    public static Polar FromVector2(Vector2 v) { 
    
        float dist = Mathf.Sqrt(v.x * v.x + v.y * v.y);
        float rad = Mathf.Atan2(v.y, v.x);
        return new Polar(rad, dist);
    }
}