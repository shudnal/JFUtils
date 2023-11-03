using JetBrains.Annotations;
using static Minimap;

namespace JFUtils;

public static class MinimapExtension
{
    public static void ForceUpdateLocationPins(this Minimap minimap)
    {
        minimap.m_updateLocationsTimer = -1;
        minimap.UpdateLocationPins(0);
    }

    [CanBeNull] public static PinData GetClosestPin(this Minimap minimap, Vector3 pos, float radius,
        Func<PinData, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        PinData result = null;
        var maxDistance = float.MaxValue;
        foreach (Minimap.PinData pin in minimap.m_pins)
        {
            var distance = pos.DistanceXZ(pin.m_pos);
            if (distance < radius && distance < maxDistance && predicate(pin))
            {
                result = pin;
                maxDistance = distance;
            }
        }

        return result;
    }
}