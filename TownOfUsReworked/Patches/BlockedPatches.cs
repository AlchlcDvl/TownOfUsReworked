namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
public static class PerformVent
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked;
    }
}

[HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
public static class PerformReport
{
    public static bool ReportPressed;

    public static bool Prefix()
    {
        ReportPressed = true;

        if (NoPlayers() || IsLobby())
            return true;

        if (CustomPlayer.Local.Is(LayerEnum.Coward))
            return false;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked;
    }

    public static void Postfix() => ReportPressed = false;
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
public static class ReportClosest
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        if (CustomPlayer.Local.Is(LayerEnum.Coward))
            return false;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
public static class ReportDeadBody
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        if (CustomPlayer.Local.Is(LayerEnum.Coward))
            return false;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked;
    }
}

[HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
public static class PerformUse
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked;
    }
}

[HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
public static class PerformSabotage
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        if (blocked && CustomPlayer.Local.CanSabotage() && !CustomPlayer.Local.inVent && GameManager.Instance.SabotagesEnabled())
            HUD().ToggleMapVisible(new() { Mode = MapOptions.Modes.Sabotage });

        return false;
    }
}

[HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
public static class PerformAdmin
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked;
    }
}

[HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
public static class PerformPet
{
    public static bool Prefix()
    {
        if (NoPlayers() || IsLobby())
            return true;

        var blocked = LocalNotBlocked();

        if (!blocked)
            Blocked.BlockExposed = true;

        return blocked; // No petting for you lmao
    }
}

[HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
public static class PerformKill
{
    public static bool Prefix() => IsHnS();
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class Blocked
{
    private static GameObject UseBlock;
    private static GameObject PetBlock;
    private static GameObject SaboBlock;
    private static GameObject VentBlock;
    private static GameObject ReportBlock;

    public static bool BlockExposed;

    public static void Postfix(HudManager __instance)
    {
        if (!CustomPlayer.Local || IsLobby())
            return;

        if (CustomPlayer.LocalCustom.Data.Role is LayerHandler handler)
            handler.UpdateHud(__instance);

        if (!UseBlock && __instance.UseButton.isActiveAndEnabled)
        {
            UseBlock = new("UseBlock");
            UseBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            UseBlock.transform.localScale *= 0.75f;
            UseBlock.transform.localPosition = new(0f, 0f, 5f);
            UseBlock.transform.SetParent(__instance.UseButton.transform);
            var passive = __instance.UseButton.GetComponent<PassiveButton>();
            passive.HoverSound = GetAudio("Hover");
            passive.ClickSound = GetAudio("Click");
        }

        if (UseBlock)
        {
            var pos = __instance.UseButton.transform.position;
            pos.z = 50f;
            UseBlock.transform.position = pos;
            UseBlock.SetActive(BlockIsExposed() && __instance.UseButton.isActiveAndEnabled && BlockIsExposed());
        }

        if (!PetBlock && __instance.PetButton.isActiveAndEnabled)
        {
            PetBlock = new("PetBlock");
            PetBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            PetBlock.transform.localScale *= 0.75f;
            PetBlock.transform.localPosition = new(0f, 0f, 5f);
            PetBlock.transform.SetParent(__instance.PetButton.transform);
            var passive = __instance.PetButton.GetComponent<PassiveButton>();
            passive.HoverSound = GetAudio("Hover");
            passive.ClickSound = GetAudio("Click");
        }

        if (PetBlock)
        {
            var pos = __instance.PetButton.transform.position;
            pos.z = 50f;
            PetBlock.transform.position = pos;
            PetBlock.SetActive(BlockIsExposed() && __instance.PetButton.isActiveAndEnabled);
        }

        if (!SaboBlock && __instance.SabotageButton.isActiveAndEnabled)
        {
            SaboBlock = new("SaboBlock");
            SaboBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            SaboBlock.transform.localScale *= 0.75f;
            SaboBlock.transform.localPosition = new(0f, 0f, 5f);
            SaboBlock.transform.SetParent(__instance.SabotageButton.transform);
            var passive = __instance.SabotageButton.GetComponent<PassiveButton>();
            passive.HoverSound = GetAudio("Hover");
            passive.ClickSound = GetAudio("Click");
        }

        if (SaboBlock)
        {
            var pos = __instance.SabotageButton.transform.position;
            pos.z = 50f;
            SaboBlock.transform.position = pos;
            SaboBlock.SetActive(BlockIsExposed() && __instance.SabotageButton.isActiveAndEnabled);
        }

        if (!VentBlock && __instance.ImpostorVentButton.isActiveAndEnabled)
        {
            VentBlock = new("VentBlock");
            VentBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            VentBlock.transform.localScale *= 0.75f;
            VentBlock.transform.localPosition = new(0f, 0f, 5f);
            VentBlock.transform.SetParent(__instance.ImpostorVentButton.transform);
            var passive = __instance.ImpostorVentButton.GetComponent<PassiveButton>();
            passive.HoverSound = GetAudio("Hover");
            passive.ClickSound = GetAudio("Click");
        }

        if (VentBlock)
        {
            var pos = __instance.ImpostorVentButton.transform.position;
            pos.z = 50f;
            VentBlock.transform.position = pos;
            VentBlock.SetActive(BlockIsExposed() && __instance.ImpostorVentButton.isActiveAndEnabled);
        }

        if (!ReportBlock && __instance.ReportButton.isActiveAndEnabled)
        {
            ReportBlock = new("ReportBlock");
            ReportBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            ReportBlock.transform.localScale *= 0.75f;
            ReportBlock.transform.localPosition = new(0f, 0f, 5f);
            ReportBlock.transform.SetParent(__instance.ReportButton.transform);
            var passive = __instance.ReportButton.GetComponent<PassiveButton>();
            passive.HoverSound = GetAudio("Hover");
            passive.ClickSound = GetAudio("Click");
        }

        if (ReportBlock)
        {
            var pos = __instance.ReportButton.transform.position;
            pos.z = 50f;
            ReportBlock.transform.position = pos;
            ReportBlock.SetActive(BlockIsExposed() && __instance.ReportButton.isActiveAndEnabled);
        }

        if (!IsHnS())
        {
            __instance.KillButton.SetTarget(null);
            __instance.KillButton.gameObject.SetActive(false);
        }

        if (!__instance.ImpostorVentButton.currentTarget || BlockIsExposed())
            __instance.ImpostorVentButton.SetDisabled();
        else
            __instance.ImpostorVentButton.SetEnabled();

        __instance.ImpostorVentButton.buttonLabelText.text = BlockIsExposed() ? "BLOCKED" : "VENT";
        __instance.ImpostorVentButton.gameObject.SetActive((CustomPlayer.Local.CanVent() || CustomPlayer.Local.inVent) && !(Map() && Map().IsOpen) && !ActiveTask());
        var closestDead = CustomPlayer.Local.GetClosestBody(maxDistance: GameSettings.ReportDistance);

        if (!closestDead || CustomPlayer.Local.CannotUse())
            __instance.ReportButton.SetDisabled();
        else
            __instance.ReportButton.SetEnabled();

        __instance.ReportButton.buttonLabelText.text = BlockIsExposed() ? "BLOCKED" : "REPORT";

        if (CustomPlayer.Local.closest == null || BlockIsExposed())
            __instance.UseButton.SetDisabled();
        else
            __instance.UseButton.SetEnabled();

        __instance.UseButton.buttonLabelText.text = BlockIsExposed() ? "BLOCKED" : "USE";

        if (BlockIsExposed() || CustomPlayer.Local.CannotUse())
            __instance.PetButton.SetDisabled();
        else
            __instance.PetButton.SetEnabled();

        __instance.PetButton.buttonLabelText.text = BlockIsExposed() ? "BLOCKED" : "PET";

        if (CustomPlayer.Local.CannotUse() || !CustomPlayer.Local.CanSabotage())
            __instance.SabotageButton.SetDisabled();
        else
            __instance.SabotageButton.SetEnabled();

        __instance.SabotageButton.buttonLabelText.text = BlockIsExposed() ? "BLOCKED" : "SABOTAGE";
        __instance.SabotageButton.gameObject.SetActive(CustomPlayer.Local.CanSabotage() && !(Map() && Map().IsOpen) && !ActiveTask());

        if (!IsInGame() || IsLobby())
            __instance.AbilityButton.gameObject.SetActive(false);
        else if (IsHnS())
            __instance.AbilityButton.gameObject.SetActive(!CustomPlayer.Local.IsImpostor());
        else
            __instance.AbilityButton.gameObject.SetActive(!Meeting() && (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && CustomPlayer.LocalCustom.Dead);
    }
}