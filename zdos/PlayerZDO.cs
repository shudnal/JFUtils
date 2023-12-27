namespace JFUtils.zdos;

[PublicAPI]
public class PlayerZDO : CustomZDO
{
    private static readonly int prefabHash = "Player".GetStableHashCode();
    public PlayerZDO(ZDO zdo) : base(zdo) { }

    public override int GetPrefab() => prefabHash;

    public string GetPlayerName(bool noSpaces = false)
    {
        var result = GetZdo().GetString(ZDOVars.s_playerName);
        if (noSpaces) result = result.Replace(" ", "");
        return result.IsGood() ? result : "Unknown";
    }

    public void TeleportTo(Vector3 position, bool distantTeleport = false) =>
        TeleportTo(position, Quaternion.identity, distantTeleport);

    public void TeleportTo(Vector3 position, Quaternion rotation, bool distantTeleport = false)
    {
        ZRoutedRpc.instance.InvokeRoutedRPC(GetZdo().GetOwner(), GetZdo().m_uid, "RPC_TeleportTo", position, rotation,
            distantTeleport);
    }
}