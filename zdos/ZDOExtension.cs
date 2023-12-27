#nullable enable
using JFUtils.zdos;

namespace JFUtils;

[PublicAPI]
public static class ZDOExtension
{
    public static PlayerZDO? AsPlayerZdo(this ZDO zdo)
    {
        var prefab = zdo.GetPrefab();
        if (!zdo.IsValid() || prefab < 1) return null;
        return new PlayerZDO(zdo);
    }

    public static void MoveTo(this ZDO zdo, Vector3 position)
    {
        if (!zdo.IsValid()) return;
        //if (zdo.IsOwner()) zdo.SetPosition(position);
        //else 
        ZRoutedRpc.instance.InvokeRoutedRPC("JFUtils_MoveZDO", zdo.m_uid, position);
    }
}