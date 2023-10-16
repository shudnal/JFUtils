namespace JFUtils;

public static class MonoBehaviourExtension
{
    public static T SetActiveGO<T>(this T behaviour, bool flag) where T : Component
    {
        behaviour.gameObject.SetActive(flag);
        return behaviour;
    }

    public static T ToggleActiveGO<T>(this T behaviour) where T : Component
    {
        behaviour.gameObject.SetActive(!behaviour.gameObject.activeSelf);
        return behaviour;
    }
}