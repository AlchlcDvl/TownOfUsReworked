namespace TownOfUsReworked.Utils;

public static class ButtonUtils
{
    public static void DisableButtons(this PlayerControl player)
    {
        var hud = HUD();
        player.GetButtons().ForEach(x => x.Disable());
        hud.SabotageButton.ToggleVisible(false);
        hud.ReportButton.ToggleVisible(false);
        hud.ImpostorVentButton.ToggleVisible(false);
        hud.UseButton.ToggleVisible(false);
        hud.PetButton.ToggleVisible(false);
        hud.AbilityButton.ToggleVisible(false);
    }

    public static IEnumerable<CustomButton> GetButtonsFromList(this PlayerControl player) => CustomButton.AllButtons.Where(x => x.Owner.Player == player);

    public static IEnumerable<CustomButton> GetButtons(this PlayerControl player)
    {
        if (player.Data.Role is LayerHandler handler)
            return handler.Buttons;

        return player.GetButtonsFromList();
    }

    public static void ResetButtons(this PlayerControl player)
    {
        if (player.Data.Role is LayerHandler handler)
            handler.ResetButtons();
    }

    public static void EnableButtons(this PlayerControl player)
    {
        var hud = HUD();
        player.GetButtons().ForEach(x => x.SetActive());
        player.GetRole()?.UpdateButtons();
        hud.KillButton.ToggleVisible(false);
        hud.UseButton.ToggleVisible(true);
        hud.PetButton.ToggleVisible(true);
        hud.SabotageButton.ToggleVisible(player.CanSabotage() && IsInGame());
        hud.ReportButton.ToggleVisible(!player.Is<Coward>() && !Meeting() && !player.HasDied() && IsInGame());
        hud.ImpostorVentButton.ToggleVisible(player.CanVent() && IsInGame());

        if (IsHnS())
            hud.AbilityButton.ToggleVisible(!CustomPlayer.Local.IsImpostor() && IsInGame());
        else
            hud.AbilityButton.ToggleVisible(!Meeting() && (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && IsInGame() && CustomPlayer.Local.HasDied());
    }

    public static void DisableAllButtons()
    {
        var hud = HUD();
        CustomButton.AllButtons.ForEach(x => x.Disable());
        hud.KillButton.ToggleVisible(false);
        hud.SabotageButton.ToggleVisible(false);
        hud.ReportButton.ToggleVisible(false);
        hud.ImpostorVentButton.ToggleVisible(false);
        hud.UseButton.ToggleVisible(false);
        hud.PetButton.ToggleVisible(false);
        hud.AbilityButton.ToggleVisible(false);
    }

    public static void SetDelay(this ActionButton button, float timer)
    {
        var ceil = Mathf.CeilToInt(timer);
        button.isCoolingDown = timer > 0f;
        button.graphic.transform.localPosition = button.position + (Vector3)(URandom.insideUnitCircle * 0.05f);
        button.cooldownTimerText.text = $"{ceil}";
        button.cooldownTimerText.color = UColor.white;
        button.cooldownTimerText.gameObject.SetActive(button.isCoolingDown);
        button.SetCooldownFill(ceil % 2 == 0 ? 1f : 0f);
    }

    // public static void DestroyButtons(this PlayerControl player) => player.GetButtons().ForEach(x => x.Destroy());

    public static bool CannotUse(this PlayerControl player) => player.onLadder || player.inVent || player.inMovingPlat || player.isKilling;

    public static float GetModifiedCooldown(this PlayerControl player, float cooldown, float difference = 0f, float factor = 1f)
    {
        var result = (cooldown * factor * player.GetMultiplier()) + difference + player.GetDifference();

        if (result <= 0f)
            result = 0f;

        return result;
    }

    private static float GetUnderdogChange(this PlayerControl player)
    {
        if (!player.Is<Underdog>())
            return 0f;

        if (Last(player))
            return -Underdog.UnderdogCdBonus;

        return Underdog.UnderdogIncreasedCd ? Underdog.UnderdogCdBonus : 0f;
    }

    private static float GetDifference(this PlayerControl player)
    {
        var result = 0f;
        result += player.GetUnderdogChange();
        return result;
    }

    private static float GetMultiplier(this PlayerControl player)
    {
        var num = 1f;
        var role = player.GetRole();

        if (role is Intruder { IsPromoted: true })
            num *= Godfather.GfPromotionCdDecrease;
        if (role is Syndicate { IsPromoted: true })
            num *= Rebel.RebPromotionCdDecrease;

        if (role.Diseased)
            num *= Diseased.DiseasedMultiplier;

        return num;
    }

    public static void Reset(CooldownType cooldown = CooldownType.Reset, PlayerControl player = null)
    {
        if (IsHnS())
            return;

        player ??= CustomPlayer.Local;
        player.GetButtons().ForEach(x => x.StartCooldown(cooldown));

        if (player.Is<Role>(out var role))
            role.Reset(cooldown == CooldownType.Meeting, cooldown == CooldownType.Start);
    }
}