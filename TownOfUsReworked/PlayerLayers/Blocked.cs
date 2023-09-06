namespace TownOfUsReworked.PlayerLayers;

[HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
[HarmonyPriority(Priority.First)]
public static class PerformVent
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
[HarmonyPriority(Priority.First)]
public static class UseVent
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.TryMoveToVent))]
public static class MoveToVentPatch
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        if (!CustomPlayer.Local.CanVent())
            return false;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
[HarmonyPriority(Priority.First)]
public static class PerformReport
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        if (CustomPlayer.Local.Is(LayerEnum.Coward))
            return false;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
[HarmonyPriority(Priority.First)]
public static class PerformUse
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
[HarmonyPriority(Priority.First)]
public static class PerformSabotage
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(AdminButton), nameof(AdminButton.DoClick))]
[HarmonyPriority(Priority.First)]
public static class PerformAdmin
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        return LocalNotBlocked;
    }
}

[HarmonyPatch(typeof(PetButton), nameof(PetButton.DoClick))]
[HarmonyPriority(Priority.First)]
public static class PerformPet
{
    public static bool Prefix()
    {
        if (NoPlayers)
            return true;

        return LocalNotBlocked; //No petting for you lmao
    }
}

[HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
[HarmonyPriority(Priority.First)]
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
        if (IsEnded)
            return;

        if (!UseBlock && __instance.UseButton.isActiveAndEnabled)
        {
            UseBlock = new("UseBlock");
            UseBlock.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            UseBlock.transform.localScale *= 0.75f;
            UseBlock.transform.localPosition = new(0f, 0f, 5f);
            UseBlock.transform.SetParent(__instance.UseButton.transform);
        }

        if (UseBlock)
        {
            var pos = __instance.UseButton.transform.position;
            pos.z = -50f;
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
        }

        if (PetBlock)
        {
            var pos = __instance.PetButton.transform.position;
            pos.z = -50f;
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
        }

        if (SaboBlock)
        {
            var pos = __instance.SabotageButton.transform.position;
            pos.z = -50f;
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
        }

        if (VentBlock)
        {
            var pos = __instance.ImpostorVentButton.transform.position;
            pos.z = -50f;
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
        }

        if (ReportBlock)
        {
            var pos = __instance.ReportButton.transform.position;
            pos.z = -50f;
            ReportBlock.transform.position = pos;
            ReportBlock.SetActive(LocalBlocked && __instance.ReportButton.isActiveAndEnabled);
        }
    }
}