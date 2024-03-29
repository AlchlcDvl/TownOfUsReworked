namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
public static class PerformVent
{
    public static bool Prefix()
    {
        if (NoPlayers || IsLobby)
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
public static class PerformReport
{
    public static bool ReportPressed;

    public static bool Prefix()
    {
        ReportPressed = true;

        if (NoPlayers || IsLobby)
            return true;

        if (CustomPlayer.Local.Is(LayerEnum.Coward))
            return false;

        return LocalNotBlocked;
    }

    public static void Postfix() => ReportPressed = false;
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
public static class ReportClosest
{
    public static bool Prefix()
    {
        if (NoPlayers || IsLobby)
            return true;

        if (CustomPlayer.Local.Is(LayerEnum.Coward))
            return false;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
public static class ReportDeadBody
{
    public static bool Prefix()
    {
        if (NoPlayers || IsLobby)
            return true;

        if (CustomPlayer.Local.Is(LayerEnum.Coward))
            return false;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
public static class PerformUse
{
    public static bool Prefix()
    {
        if (NoPlayers || IsLobby)
            return true;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
public static class PerformSabotage
{
    public static bool Prefix()
    {
        if (NoPlayers || IsLobby)
            return true;

        return LocalNotBlocked && CustomPlayer.Local.CanSabotage();
    }
}

[HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
public static class PerformAdmin
{
    public static bool Prefix()
    {
        if (NoPlayers || IsLobby)
            return true;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
public static class PerformPet
{
    public static bool Prefix()
    {
        if (NoPlayers || IsLobby)
            return true;

        return LocalNotBlocked; // No petting for you lmao
    }
}

[HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
public static class PerformKill
{
    public static bool Prefix() => false;
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class Blocked
{
    private static GameObject UseBlock;
    private static GameObject PetBlock;
    private static GameObject SaboBlock;
    private static GameObject VentBlock;
    private static GameObject ReportBlock;

    public static void Postfix(HudManager __instance)
    {
        if (!CustomPlayer.Local || IsLobby)
            return;

        if (!UseBlock && __instance.UseButton.isActiveAndEnabled)
        {
            UseBlock = new("UseBlock");
            UseBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            UseBlock.transform.localScale *= 0.75f;
            UseBlock.transform.localPosition = new(0f, 0f, 5f);
            UseBlock.transform.SetParent(__instance.UseButton.transform);
            __instance.UseButton.GetComponent<PassiveButton>().HoverSound = SoundEffects["Hover"];
            __instance.UseButton.GetComponent<PassiveButton>().ClickSound = SoundEffects["Click"];
        }

        if (UseBlock)
        {
            var pos = __instance.UseButton.transform.position;
            pos.z = 50f;
            UseBlock.transform.position = pos;
            UseBlock.SetActive(LocalBlocked && __instance.UseButton.isActiveAndEnabled);
        }

        if (!PetBlock && __instance.PetButton.isActiveAndEnabled)
        {
            PetBlock = new("PetBlock");
            PetBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            PetBlock.transform.localScale *= 0.75f;
            PetBlock.transform.localPosition = new(0f, 0f, 5f);
            PetBlock.transform.SetParent(__instance.PetButton.transform);
            __instance.PetButton.GetComponent<PassiveButton>().HoverSound = SoundEffects["Hover"];
            __instance.PetButton.GetComponent<PassiveButton>().ClickSound = SoundEffects["Click"];
        }

        if (PetBlock)
        {
            var pos = __instance.PetButton.transform.position;
            pos.z = 50f;
            PetBlock.transform.position = pos;
            PetBlock.SetActive(LocalBlocked && __instance.PetButton.isActiveAndEnabled);
        }

        if (!SaboBlock && __instance.SabotageButton.isActiveAndEnabled)
        {
            SaboBlock = new("SaboBlock");
            SaboBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            SaboBlock.transform.localScale *= 0.75f;
            SaboBlock.transform.localPosition = new(0f, 0f, 5f);
            SaboBlock.transform.SetParent(__instance.SabotageButton.transform);
            __instance.SabotageButton.GetComponent<PassiveButton>().HoverSound = SoundEffects["Hover"];
            __instance.SabotageButton.GetComponent<PassiveButton>().ClickSound = SoundEffects["Click"];
        }

        if (SaboBlock)
        {
            var pos = __instance.SabotageButton.transform.position;
            pos.z = 50f;
            SaboBlock.transform.position = pos;
            SaboBlock.SetActive(LocalBlocked && __instance.SabotageButton.isActiveAndEnabled);
        }

        if (!VentBlock && __instance.ImpostorVentButton.isActiveAndEnabled)
        {
            VentBlock = new("VentBlock");
            VentBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            VentBlock.transform.localScale *= 0.75f;
            VentBlock.transform.localPosition = new(0f, 0f, 5f);
            VentBlock.transform.SetParent(__instance.ImpostorVentButton.transform);
            __instance.ImpostorVentButton.GetComponent<PassiveButton>().HoverSound = SoundEffects["Hover"];
            __instance.ImpostorVentButton.GetComponent<PassiveButton>().ClickSound = SoundEffects["Click"];
        }

        if (VentBlock)
        {
            var pos = __instance.ImpostorVentButton.transform.position;
            pos.z = 50f;
            VentBlock.transform.position = pos;
            VentBlock.SetActive(LocalBlocked && __instance.ImpostorVentButton.isActiveAndEnabled);
        }

        if (!ReportBlock && __instance.ReportButton.isActiveAndEnabled)
        {
            ReportBlock = new("ReportBlock");
            ReportBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            ReportBlock.transform.localScale *= 0.75f;
            ReportBlock.transform.localPosition = new(0f, 0f, 5f);
            ReportBlock.transform.SetParent(__instance.ReportButton.transform);
            __instance.ReportButton.GetComponent<PassiveButton>().HoverSound = SoundEffects["Hover"];
            __instance.ReportButton.GetComponent<PassiveButton>().ClickSound = SoundEffects["Click"];
        }

        if (ReportBlock)
        {
            var pos = __instance.ReportButton.transform.position;
            pos.z = 50f;
            ReportBlock.transform.position = pos;
            ReportBlock.SetActive(LocalBlocked && __instance.ReportButton.isActiveAndEnabled);
        }

        __instance.KillButton.SetTarget(null);
        __instance.KillButton.gameObject.SetActive(false);

        if (__instance.ImpostorVentButton.currentTarget == null || LocalBlocked)
            __instance.ImpostorVentButton.SetDisabled();
        else
            __instance.ImpostorVentButton.SetEnabled();

        __instance.ImpostorVentButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "VENT";
        __instance.ImpostorVentButton.gameObject.SetActive((CustomPlayer.Local.CanVent() || CustomPlayer.Local.inVent) && !(Map && Map.IsOpen) && !ActiveTask);
        var closestDead = CustomPlayer.Local.GetClosestBody(maxDistance: CustomGameOptions.ReportDistance);

        if (closestDead == null || CustomPlayer.Local.CannotUse())
            __instance.ReportButton.SetDisabled();
        else
            __instance.ReportButton.SetEnabled();

        __instance.ReportButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "REPORT";

        if (CustomPlayer.Local.closest == null || LocalBlocked)
            __instance.UseButton.SetDisabled();
        else
            __instance.UseButton.SetEnabled();

        __instance.UseButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "USE";

        if (LocalBlocked || CustomPlayer.Local.CannotUse())
            __instance.PetButton.SetDisabled();
        else
            __instance.PetButton.SetEnabled();

        __instance.PetButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "PET";

        if (CustomPlayer.Local.CannotUse() || !CustomPlayer.Local.CanSabotage())
            __instance.SabotageButton.SetDisabled();
        else
            __instance.SabotageButton.SetEnabled();

        __instance.SabotageButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "SABOTAGE";
        __instance.SabotageButton.gameObject.SetActive(CustomPlayer.Local.CanSabotage() && !(Map && Map.IsOpen) && !ActiveTask);

        if (!IsInGame)
            __instance.AbilityButton.gameObject.SetActive(false);
        else if (IsHnS)
            __instance.AbilityButton.gameObject.SetActive(!CustomPlayer.Local.IsImpostor());
        else
            __instance.AbilityButton.gameObject.SetActive(!Meeting && (!CustomPlayer.Local.IsPostmortal() || CustomPlayer.Local.Caught()) && CustomPlayer.LocalCustom.Dead);
    }
}