namespace JFUtils;

public static class TransformExtension
{
    public static Transform FindChildByName(this Transform transform, string name) => Utils.FindChild(transform, name);

    public static float DistanceXZ(this Transform transform, Transform other) =>
        Utils.DistanceXZ(transform.position, other.position);

    public static float DistanceXZ(this Transform transform, Component otherGameObject) =>
        Utils.DistanceXZ(transform.position, otherGameObject.transform.position);

    public static float DistanceXZ(this Transform transform, GameObject otherGameObject) =>
        Utils.DistanceXZ(transform.position, otherGameObject.transform.position);

    public static float DistanceXZ(this Transform transform, Vector3 otherPos) =>
        Utils.DistanceXZ(transform.position, otherPos);
}