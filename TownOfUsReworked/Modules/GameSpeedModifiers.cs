namespace TownOfUsReworked.Modules;

public sealed class OxySabSpeedModifier : ISpeedModifier
{
    public void ModifySpeed(PlayerControl player, ref float result)
    {
        if (Ship()?.Systems?.TryGetValue(SystemTypes.LifeSupp, out var life) == true && life.TryCast<LifeSuppSystemType>(out var lifeSuppSystemType) && lifeSuppSystemType!.IsActive && !player.HasDied())
            result *= Mathf.Lerp(100f, BetterSabotages.LowestOxySpeed, lifeSuppSystemType.Countdown / lifeSuppSystemType.LifeSuppDuration) / 100f;
    }
}

public sealed class BodyDraggingModifier : ISpeedModifier
{
    public void ModifySpeed(PlayerControl player, ref float result)
    {
        if (player.HasDied())
            return;

        if (DeadBodyHandler.Dragging.Contains(player.PlayerId))
            result *= Janitor.DragModifier;
    }
}