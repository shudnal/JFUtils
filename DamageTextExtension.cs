namespace JFUtils;

[PublicAPI]
public static class DamageTextExtension
{
    public static void ShowText(this DamageText instance, Vector3 pos, string msg)
    {
        ZPackage zpackage = new ZPackage();
        zpackage.Write(pos);
        zpackage.Write(msg);
        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "JF_ShowText", zpackage);
    }
}