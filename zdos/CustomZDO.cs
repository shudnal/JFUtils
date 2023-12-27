namespace JFUtils.zdos;

[PublicAPI]
public abstract class CustomZDO
{
    private ZDO zdo;
    public virtual ZDO GetZdo() => zdo;
    public abstract int GetPrefab();

    protected CustomZDO(ZDO zdo)
    {
        if (zdo.GetPrefab() != GetPrefab())
            throw new UnityException($"Wrong prefab in ZDO {zdo} creating custom ZDO instance. "
                                     + $"Expected: {GetPrefab()}, got: {zdo.GetPrefab()}");
        this.zdo = zdo;
    }

    public override string ToString() { return $"Zdo: {zdo}, prefab: {GetPrefab()}"; }
}