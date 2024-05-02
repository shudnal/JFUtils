using TMPro;

namespace JFUtils.WithPatch;

[HarmonyPatch]
public static class GlobalRPCs
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start)), HarmonyPostfix]
    private static void Patch()
    {
        if (!ZRoutedRpc.instance.m_functions.ContainsKey("JFUtils_MoveZDO".GetStableHashCode()))
            ZRoutedRpc.instance.Register<ZDOID, Vector3>("JFUtils_MoveZDO", RPC_MoveZDO);
        if (!ZRoutedRpc.instance.m_functions.ContainsKey("JF_ShowText".GetStableHashCode()))
            ZRoutedRpc.instance.Register<ZPackage>("JF_ShowText", RPC_JF_ShowText);
    }

    private static void RPC_MoveZDO(long _, ZDOID zdoid, Vector3 position)
    {
        if (!ZDOMan.instance.m_objectsByID.TryGetValue(zdoid, out var zdo)) return;
        zdo.SetPosition(position);
    }

    private static void RPC_JF_ShowText(long sender, ZPackage pkg)
    {
        var inst = DamageText.instance;
        var mainCamera = Utils.GetMainCamera();
        if (!mainCamera || Hud.IsUserHidden()) return;
        var pos = pkg.ReadVector3();
        var msg = pkg.ReadString();
        float distance = Vector3.Distance(mainCamera.transform.position, pos);
        if (distance > inst.m_maxTextDistance) return;

        if (!msg.IsGood()) return;
        DamageText.WorldTextInstance worldTextInstance = new()
        {
            m_worldPos = pos + Random.insideUnitSphere * 0.5f,
            m_gui = Instantiate(inst.m_worldTextBase, inst.transform)
        };
        worldTextInstance.m_textField = worldTextInstance.m_gui.GetComponent<TMP_Text>();
        inst.m_worldTexts.Add(worldTextInstance);
        worldTextInstance.m_textField.color = Color.white;
        worldTextInstance.m_textField.fontSize =
            distance <= inst.m_smallFontDistance ? inst.m_largeFontSize : inst.m_smallFontSize;
        worldTextInstance.m_textField.text = msg.Localize();
        worldTextInstance.m_timer = 0.0f;
    }
}