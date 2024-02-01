namespace JFUtils;

[Serializable]
public class BezierCurve
{
    public Vector3[] Points;

    public BezierCurve() { Points = new Vector3[4]; }

    public BezierCurve(Vector3[] Points) { this.Points = Points; }

    public Vector3 StartPosition => Points[0];

    public Vector3 EndPosition => Points[3];

    // Equations from: https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    public Vector3 GetSegment(float Time)
    {
        Time = Clamp01(Time);
        var time = 1 - Time;
        return time * time * time * Points[0]
               + 3 * time * time * Time * Points[1]
               + 3 * time * Time * Time * Points[2]
               + Time * Time * Time * Points[3];
    }

    public Vector3[] GetSegments(int Subdivisions)
    {
        var segments = new Vector3[Subdivisions];

        for (var i = 0; i < Subdivisions; i++)
        {
            var time = (float)i / Subdivisions;
            segments[i] = GetSegment(time);
        }

        return segments;
    }
}