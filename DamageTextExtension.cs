namespace JFUtils;

[PublicAPI]
public static class DamageTextExtension
{
    public static void ShowText(this DamageText instance, Vector3 pos, string msg, string webColor = "#FFFFFF")
    {
        ZPackage zpackage = new ZPackage();
        zpackage.Write(pos);
        zpackage.Write(msg);
        zpackage.Write(webColor);
        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "JF_ShowText", zpackage);
    }
}