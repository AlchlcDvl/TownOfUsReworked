namespace TownOfUsReworked.Utils;

public static class ButtonUtils
{
    public static void DisableButtons(this PlayerControl player)
    {
        var hud = HUD()!;
        player.GetButtons().Do(x => x.Disable());
        hud.SabotageButton.ToggleVisible(false);
        hud.ReportButton.ToggleVisible(false);
        hud.ImpostorVentButton.ToggleVisible(false);
        hud.UseButton.ToggleVisible(false);
        hud.PetButton.ToggleVisible(false);
        hud.AbilityButton.ToggleVisible(false);
    }

    public static IEnumerable<CustomButton> GetButtonsFromList(this PlayerControl player) => CustomButton.AllButtons.Where(x => x.Owner.Player == player || (x.Owner is Crew crew &&
        crew.MimickedBy?.Player == player));

    public static IEnumerable<CustomButton> GetButtons(this PlayerControl player) => LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler)
        ? handler.Buttons : player.GetButtonsFromList();

    public static void ResetButtons(this PlayerControl player)
    {
        if (LayerHandler.Handlers.TryGetValue(player.PlayerId, out var handler))
            handler.ResetButtons();
    }

    public static void EnableButtons(this PlayerControl player)
    {
        var hud = HUD()!;
        LayerHandler.Handlers[player.PlayerId].UpdateButtons();
        hud.UseButton.ToggleVisible(true);
        hud.PetButton.ToggleVisible(true);
        var inGame = IsInGame();
        hud.SabotageButton.ToggleVisible(player.CanSabotage() && inGame);
        hud.ImpostorVentButton.ToggleVisible(player.CanVent() && inGame);
        var died = player.HasDied();
        hud.ReportButton.ToggleVisible(!player.Is<Coward>() && !Meeting() && !died && inGame);

        if (IsHnS())
            hud.AbilityButton.ToggleVisible(!player.IsImpostor() && inGame);
        else
            hud.AbilityButton.ToggleVisible(!Meeting() && (!player.Is<IGhosty>(out var ghost) || ghost.Caught) && inGame && died);
    }

    public static void DisableAllButtons()
    {
        var hud = HUD()!;
        CustomButton.AllButtons.ForEach(x => x.Disable());
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

        if (role is IPromoter { IsPromoted: true } promoter)
            num *= promoter.PromotionModifier;

        if (role.Handler.Diseased)
            num *= Diseased.DiseasedMultiplier;

        return num;
    }

    public static void Reset(CooldownType cooldown = CooldownType.Reset, PlayerControl? player = null)
    {
        if (IsHnS())
            return;

        player ??= LocalPlayer;
        player.GetButtons().Do(x => x.StartCooldown(cooldown));

        if (player.Is<Role>(out var role))
            role.Reset(cooldown == CooldownType.Meeting, cooldown == CooldownType.Start);
    }
}