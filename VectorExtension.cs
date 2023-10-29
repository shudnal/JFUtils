namespace JFUtils;

public static class VectorExtension
{
    public static Vector2 ToV2(this Vector3 vector3) => new(vector3.x, vector3.z);

    public static Vector3 ToV3(this Vector2 vector2) => new(vector2.x, 0, vector2.y);

    public static Vector3 RoundCords(this Vector3 vector3) => new((int)vector3.x, (int)vector3.y, (int)vector3.z);

    public static Vector2 RoundCords(this Vector2 vector3) => new((int)vector3.x, (int)vector3.y);

    public static Vector3 SetX(this ref Vector3 vector, float x)
    {
        vector.x = x;
        return vector;
    }

    public static Vector2 SetX(this ref Vector2 vector, float x)
    {
        vector.x = x;
        return vector;
    }

    public static Vector3 SetY(this ref Vector3 vector, float y)
    {
        vector.y = y;
        return vector;
    }

    public static Vector2 SetY(this ref Vector2 vector, float y)
    {
        vector.y = y;
        return vector;
    }

    public static Vector3 SetZ(this ref Vector3 vector, float z)
    {
        vector.z = z;
        return vector;
    }


    public static float DistanceXZ(this Vector3 pos, Vector3 otherPos) => Utils.DistanceXZ(pos, otherPos);

    public static float DistanceXZ(this Vector3 pos, Transform otherPos) => Utils.DistanceXZ(pos, otherPos.position);

    public static float DistanceXZ(this Vector3 pos, Component otherPos) =>
        Utils.DistanceXZ(pos, otherPos.transform.position);

    public static float DistanceXZ(this Vector3 pos, GameObject otherPos) =>
        Utils.DistanceXZ(pos, otherPos.transform.position);

    public static Vector2i GetZone(this Vector3 pos) => ZoneSystem.instance.GetZone(pos);
}