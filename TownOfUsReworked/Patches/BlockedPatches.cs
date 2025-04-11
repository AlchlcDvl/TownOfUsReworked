namespace TownOfUsReworked.Patches;

[HarmonyPatch]
public static class BlockPatches
{
    [HarmonyPatch(typeof(UseButton))]
    [HarmonyPatch(typeof(PetButton))]
    [HarmonyPatch(typeof(AdminButton))]
    [HarmonyPatch("DoClick")]
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        var blocked = LocalBlocked();

        if (blocked)
            BlockExposed = true;

        return !blocked;
    }
}

[HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
public static class PerformVent
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        var blocked = LocalBlocked();

        if (blocked)
            BlockExposed = true;

        return !blocked;
    }
}

[HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
public static class PerformReport
{
    public static bool ReportPressed;

    public static bool Prefix() => ReportPressed = IsInGame() && CustomPlayer.Local.GetClosestBody(maxDistance: CustomPlayer.Local.lightSource.viewDistance);

    public static void Postfix() => ReportPressed = false;
}

[HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
public static class PerformSabotage
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        if (!CustomPlayer.Local.CanSabotage())
            return false;

        switch (LocalBlocked())
        {
            case true:
            {
                BlockExposed = true;
                break;
            }
            case false when !CustomPlayer.Local.inVent && GameManager.Instance.SabotagesEnabled():
            {
                HUD().ToggleMapVisible(new() { Mode = MapOptions.Modes.Sabotage });
                break;
            }
        }

        return false;
    }
}

[HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
public static class PerformKill
{
    public static bool Prefix() => IsHnS();
}

[HarmonyPatch(typeof(HudManager))]
public static class Blocked
{
    private static GameObject UseBlock;
    private static GameObject PetBlock;
    private static GameObject SaboBlock;
    private static GameObject VentBlock;
    private static GameObject ReportBlock;

    public static GameObject BlockPrefab;

    [HarmonyPatch(nameof(HudManager.Start)), HarmonyPostfix]
    public static void StartPostfix(HudManager __instance)
    {
        if (!BlockPrefab)
        {
            BlockPrefab = new GameObject("BlockPrefab").DontDestroy().DontUnload();
            BlockPrefab.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            BlockPrefab.SetActive(false);
        }

        if (!ReportBlock)
            ReportBlock = __instance.ReportButton.SetBlock();

        if (!UseBlock)
            UseBlock = __instance.UseButton.SetBlock();

        if (!PetBlock)
            PetBlock = __instance.PetButton.SetBlock();

        if (!SaboBlock)
            SaboBlock = __instance.SabotageButton.SetBlock();

        if (!VentBlock)
            VentBlock = __instance.ImpostorVentButton.SetBlock();
    }

    [HarmonyPatch(nameof(HudManager.Update)), HarmonyPostfix]
    public static void UpdatePostfix(HudManager __instance)
    {
        if (!CustomPlayer.Local || IsLobby() || !Ship())
            return;

        if (CustomPlayer.Local.Data?.Role is LayerHandler handler)
            handler.UpdateHud(__instance);
        else
            return;

        UseBlock.SetActive(BlockExposed);
        PetBlock.SetActive(BlockExposed);
        SaboBlock.SetActive(BlockExposed);
        VentBlock.SetActive(BlockExposed);
        ReportBlock.SetActive(BlockExposed);

        if (!__instance.ImpostorVentButton.currentTarget || BlockExposed)
            __instance.ImpostorVentButton.SetDisabled();
        else
            __instance.ImpostorVentButton.SetEnabled();

        __instance.ImpostorVentButton.buttonLabelText.text = BlockExposed ? "BLOCKED" : "VENT";
        __instance.ImpostorVentButton.ToggleVisible((CustomPlayer.Local.CanVent() || CustomPlayer.Local.inVent) && !(Map() && Map().IsOpen) && !ActiveTask());
        var closestDead = handler.CustomModifier is Shy ? null : CustomPlayer.Local.GetClosestBody(maxDistance: CustomPlayer.Local.lightSource.viewDistance);

        if (!closestDead || CustomPlayer.Local.CannotUse() || BlockExposed)
            __instance.ReportButton.SetDisabled();
        else
            __instance.ReportButton.SetEnabled();

        __instance.ReportButton.buttonLabelText.SetText(BlockExposed ? "BLOCKED" : "REPORT");

        if (CustomPlayer.Local.closest == null || BlockExposed)
            __instance.UseButton.SetDisabled();
        else
            __instance.UseButton.SetEnabled();

        __instance.UseButton.buttonLabelText.text = BlockExposed ? "BLOCKED" : "USE";

        if (CustomPlayer.Local.CannotUse() || BlockExposed)
            __instance.PetButton.SetDisabled();
        else
            __instance.PetButton.SetEnabled();

        __instance.PetButton.buttonLabelText.text = BlockExposed ? "BLOCKED" : "PET";

        if (CustomPlayer.Local.CannotUse() || !CustomPlayer.Local.CanSabotage() || BlockExposed)
            __instance.SabotageButton.SetDisabled();
        else
            __instance.SabotageButton.SetEnabled();

        __instance.SabotageButton.buttonLabelText.text = BlockExposed ? "BLOCKED" : "SABOTAGE";
        __instance.SabotageButton.ToggleVisible(CustomPlayer.Local.CanSabotage() && !(Map() && Map().IsOpen) && !ActiveTask());

        if (!IsInGame() || IsLobby())
            __instance.AbilityButton.ToggleVisible(false);
        else if (IsHnS())
            __instance.AbilityButton.ToggleVisible(!CustomPlayer.Local.IsImpostor());
        else
            __instance.AbilityButton.ToggleVisible(!Meeting() && (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && CustomPlayer.Local.HasDied());

        __instance.FullScreen.enabled = true;
        __instance.FullScreen.gameObject.SetActive(true);
    }

    private static GameObject SetBlock(this ActionButton button)
    {
        var passive = button.GetComponent<PassiveButton>();
        passive.HoverSound = GetAudio("Hover");
        passive.ClickSound = GetAudio("Click");
        var block = UObject.Instantiate(BlockPrefab, button.transform);
        block.transform.SetLocalZ(-5f);
        return block;
    }
}