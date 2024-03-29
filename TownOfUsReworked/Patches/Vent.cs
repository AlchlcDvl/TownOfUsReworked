namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
public static class VentCanUsePatch
{
    public static void Postfix(Vent __instance, ref GameData.PlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
    {
        var num = float.MaxValue;
        var playerControl = pc.Object;

        if (IsNormal)
            couldUse = playerControl.CanVent();
        else if (IsHnS)
            couldUse = !pc.IsImpostor();

        var ventitaltionSystem = Ship.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

        if (ventitaltionSystem != null && ventitaltionSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
            couldUse = false;

        canUse = couldUse;

        if (canUse)
        {
            var center = playerControl.Collider.bounds.center;
            var position = __instance.transform.position;
            num = Vector2.Distance(center, position);
            canUse &= num <= __instance.UsableDistance;

            if (__instance.Id != 14 || !IsSubmerged())
                canUse &= !PhysicsHelpers.AnythingBetween(playerControl.Collider, center, position, Constants.ShipOnlyMask, false);
        }

        __result = num;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
public static class EnterVentPatch
{
    public static bool Prefix(Vent __instance, ref bool enabled)
    {
        var player = CustomPlayer.Local;
        var flag = !player.IsMoving();

        if (player.Is(LayerEnum.Jester) && CustomGameOptions.JesterVent)
            flag = CustomGameOptions.JestVentSwitch;
        else if (player.Is(LayerEnum.Executioner) && CustomGameOptions.ExeVent)
            flag = CustomGameOptions.ExeVentSwitch;
        else if (player.Is(LayerEnum.Survivor) && CustomGameOptions.SurvVent)
            flag = CustomGameOptions.SurvVentSwitch;
        else if (player.Is(LayerEnum.Amnesiac) && CustomGameOptions.AmneVent)
            flag = CustomGameOptions.AmneVentSwitch;
        else if (player.Is(LayerEnum.GuardianAngel) && CustomGameOptions.GAVent)
            flag = CustomGameOptions.GAVentSwitch;
        else if (player.Is(LayerEnum.Guesser) && CustomGameOptions.GuessVent)
            flag = CustomGameOptions.GuessVentSwitch;
        else if (player.Is(LayerEnum.Troll) && CustomGameOptions.TrollVent)
            flag = CustomGameOptions.TrollVentSwitch;
        else if (player.Is(LayerEnum.Actor) && CustomGameOptions.ActorVent)
            flag = CustomGameOptions.ActVentSwitch;
        else if (player.IsPostmortal())
            flag = false;

        // Fix for dlekS
        if (flag)
        {
            Vector2 vector;

            if (__instance.Right && __instance.Left)
                vector = ((__instance.Right.transform.position + __instance.Left.transform.position) / 2f) - __instance.transform.position;
            else
                vector = Vector2.zero;

            for (var i = 0; i < __instance.Buttons.Length; i++)
            {
                var buttonBehavior = __instance.Buttons[i];

                if (enabled)
                {
                    var vent = __instance.NearbyVents[i];

                    if (vent)
                    {
                        var ventilationSystem = Ship.Systems[SystemTypes.Ventilation].TryCast<VentilationSystem>();
                        var flag1 = ventilationSystem != null && ventilationSystem.IsVentCurrentlyBeingCleaned(vent.Id);
                        var gameObject = __instance.CleaningIndicators.Any() ? __instance.CleaningIndicators[i] : null;
                        __instance.ToggleNeighborVentBeingCleaned(flag1 || LocalBlocked, buttonBehavior, gameObject);
                        var vector2 = vent.transform.position - __instance.transform.position;
                        var vector3 = vector2.normalized * (0.7f + __instance.spreadShift);
                        vector3.x *= Mathf.Sign(Ship.transform.localScale.x);
                        vector3.y -= 0.08f;
                        vector3.z = -10f;
                        buttonBehavior.transform.localPosition = vector3;
                        buttonBehavior.transform.LookAt2d(vent.transform);
                        var deg = vector.AngleSigned(vector2) > 0f ? __instance.spreadAmount : -__instance.spreadAmount;
                        vector3 = vector3.RotateZ(deg);
                        buttonBehavior.transform.localPosition = vector3;
                        buttonBehavior.transform.Rotate(0f, 0f, deg);
                    }

                    buttonBehavior.gameObject.SetActive(vent);
                }
                else
                    buttonBehavior.gameObject.SetActive(false);
            }
        }

        return false;
    }
}

// Vent and kill shit
// Yes thank you Discussions - AD
[HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
public static class SetVentOutlinePatch
{
    public static void Postfix(Vent __instance, ref bool mainTarget)
    {
        var active = CustomPlayer.Local && !Meeting && CustomPlayer.Local.CanVent();

        if (!Role.LocalRole || !active)
            return;

        __instance.myRend.material.SetColor("_OutlineColor", Role.LocalRole.Color);
        __instance.myRend.material.SetColor("_AddColor", mainTarget ? Role.LocalRole.Color : UColor.clear);
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
public static class UseVent
{
    public static bool Prefix(Vent __instance)
    {
        if (NoPlayers || !CustomPlayer.Local.CanVent() || LocalBlocked)
            return false;

        if (__instance.IsBombed() && !CustomPlayer.Local.IsPostmortal() && CanAttack(AttackEnum.Powerful, CustomPlayer.Local.GetDefenseValue()))
        {
            RpcMurderPlayer(CustomPlayer.Local);
            Role.BastionBomb(__instance, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, __instance);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.TryMoveToVent))]
public static class MoveToVentPatch
{
    public static bool Prefix(ref Vent otherVent)
    {
        if (NoPlayers || !CustomPlayer.Local.CanVent() || LocalBlocked)
            return false;

        if (otherVent.IsBombed() && !CustomPlayer.Local.IsPostmortal() && CanAttack(AttackEnum.Powerful, CustomPlayer.Local.GetDefenseValue()))
        {
            RpcMurderPlayer(CustomPlayer.Local);
            Role.BastionBomb(otherVent, CustomGameOptions.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, otherVent);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.UpdateArrows))]
public static class FixdlekSVents1
{
    public static bool Prefix(Vent __instance, ref VentilationSystem ventSystem)
    {
        if (__instance != Vent.currentVent || ventSystem == null)
            return false;

        for (var i = 0; i < __instance.NearbyVents.Length; i++)
        {
            var vent = __instance.NearbyVents[i];

            if (vent)
            {
                __instance.ToggleNeighborVentBeingCleaned(ventSystem.IsVentCurrentlyBeingCleaned(vent.Id) || LocalBlocked, __instance.Buttons[i], __instance.CleaningIndicators.Any() ?
                    __instance.CleaningIndicators[i] : null);
            }
        }

        return false;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.ToggleNeighborVentBeingCleaned))]
public static class FixdlekSVents2
{
    public static bool Prefix(ref bool ventBeingCleaned, ref ButtonBehavior b, ref GameObject c)
    {
        b.enabled = !ventBeingCleaned;
        c?.SetActive(ventBeingCleaned);
        return false;
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
public static class HideVentAnims1
{
    public static bool Prefix(Vent __instance, ref PlayerControl pc)
    {
        if (!__instance.ExitVentAnim || !CustomGameOptions.HideVentAnims)
            return true;

        var truePosition = CustomPlayer.Local.GetTruePosition();
        var vector = pc.GetTruePosition() - truePosition;
        return vector.magnitude < CustomPlayer.Local.lightSource.viewDistance && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, vector.magnitude,
            Constants.ShipAndObjectsMask);
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
public static class HideVentAnims2
{
    public static bool Prefix(Vent __instance, ref PlayerControl pc)
    {
        if (!__instance.ExitVentAnim || !CustomGameOptions.HideVentAnims)
            return true;

        var truePosition = CustomPlayer.Local.GetTruePosition();
        var vector = pc.GetTruePosition() - truePosition;
        return vector.magnitude < CustomPlayer.Local.lightSource.viewDistance && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, vector.magnitude,
            Constants.ShipAndObjectsMask);
    }
}