namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(Vent))]
public static class VentPatches
{
    [HarmonyPatch(nameof(Vent.CanUse))]
    public static bool Prefix(Vent __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse, ref float __result)
    {
        canUse = couldUse = false;

        if (IsHnS())
            return true;

        var num = float.MaxValue;
        var playerControl = pc.Object;
        var usable = __instance.TryCast<IUsable>();
        couldUse = GameManager.Instance.LogicUsables.CanUse(usable, playerControl) && pc.Role.CanUse(usable) && playerControl.CanVent();

        if (GameModifiers.NoVentingUncleanedVents)
        {
            var ventitaltionSystem = Ship().Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();

            if (ventitaltionSystem != null && ventitaltionSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
                couldUse = false;
        }

        canUse = couldUse;

        if (canUse)
        {
            var center = playerControl.Collider.bounds.center;
            var position = __instance.transform.position;
            num = Vector2.Distance(center, position);
            canUse &= num <= __instance.UsableDistance;

            if (!IsSubmerged() || __instance.Id != 14)
                canUse &= !PhysicsHelpers.AnythingBetween(playerControl.Collider, center, position, Constants.ShipOnlyMask, false);
        }

        __result = num;
        return false;
    }

    [HarmonyPatch(nameof(Vent.SetButtons))]
    public static bool Prefix(Vent __instance, bool enabled)
    {
        var player = CustomPlayer.Local;
        var role = player.GetRole();
        var flag = !player.IsMoving() && role switch
        {
            Jester => Jester.JesterVent && Jester.JestSwitchVent,
            Executioner => Executioner.ExeVent && Executioner.ExeSwitchVent,
            Survivor => Survivor.SurvVent && Survivor.SurvSwitchVent,
            Amnesiac => Amnesiac.AmneVent && Amnesiac.AmneSwitchVent,
            GuardianAngel => GuardianAngel.GAVent && GuardianAngel.GASwitchVent,
            Guesser => Guesser.GuessVent && Guesser.GuessSwitchVent,
            Troll => Troll.TrollVent && Troll.TrollSwitchVent,
            Actor => Actor.ActorVent && Actor.ActSwitchVent,
            _ => role ? !player.IsPostmortal() : true
        };

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
                        var ventilationSystem = Ship().Systems[SystemTypes.Ventilation].TryCast<VentilationSystem>();
                        var flag1 = ventilationSystem != null && ventilationSystem.IsVentCurrentlyBeingCleaned(vent.Id);
                        var gameObject = __instance.CleaningIndicators.Any() ? __instance.CleaningIndicators[i] : null;
                        __instance.ToggleNeighborVentBeingCleaned(flag1 || LocalBlocked(), buttonBehavior, gameObject);
                        var vector2 = vent.transform.position - __instance.transform.position;
                        var vector3 = vector2.normalized * (0.7f + __instance.spreadShift);
                        vector3.x *= Mathf.Sign(Ship().transform.localScale.x);
                        vector3.y -= 0.08f;
                        vector3.z = -10f;
                        buttonBehavior.transform.localPosition = vector3;
                        buttonBehavior.transform.LookAt2d(vent.transform);
                        var deg = __instance.spreadAmount * (vector.AngleSigned(vector2) > 0f ? 1 : -1);
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

    // Vent and kill shit
    // Yes thank you Discussions - AD
    [HarmonyPatch(nameof(Vent.SetOutline))]
    public static void Postfix(Vent __instance, bool mainTarget)
    {
        if (!CustomPlayer.Local.TryGetLayer<Role>(out var role) || Meeting() || !CustomPlayer.Local.CanVent())
            return;

        __instance.myRend.material.SetColor("_OutlineColor", role.Color);
        __instance.myRend.material.SetColor("_AddColor", mainTarget ? role.Color : UColor.clear);
    }

    [HarmonyPatch(nameof(Vent.Use)), HarmonyPrefix]
    public static bool UsePrefix(Vent __instance) => CheckMoveVent(__instance);

    [HarmonyPatch(nameof(Vent.TryMoveToVent)), HarmonyPrefix]
    public static bool TryMoveToVentPrefix(Vent otherVent) => CheckMoveVent(otherVent);

    private static bool CheckMoveVent(Vent vent)
    {
        if (NoPlayers() || !CustomPlayer.Local.CanVent() || LocalBlocked())
            return false;

        if (vent.IsBombed() && !CustomPlayer.Local.IsPostmortal() && CanAttack(AttackEnum.Powerful, CustomPlayer.Local.GetDefenseValue()))
        {
            RpcMurderPlayer(CustomPlayer.Local);
            Role.BastionBomb(vent, Bastion.BombRemovedOnKill);
            CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, vent);
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(Vent.UpdateArrows))]
    [HarmonyPatch(nameof(Vent.ToggleNeighborVentBeingCleaned))]
    public static bool Prefix() => MapPatches.CurrentMap != 3;

    [HarmonyPatch(nameof(Vent.EnterVent)), HarmonyPrefix]
    public static bool EnterVentPrefix(Vent __instance, PlayerControl pc)
    {
        if (!__instance.ExitVentAnim || !GameModifiers.HideVentAnims)
            return true;

        var truePosition = CustomPlayer.Local.GetTruePosition();
        var vector = pc.GetTruePosition() - truePosition;
        return vector.magnitude < CustomPlayer.Local.lightSource.viewDistance && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, vector.magnitude,
            Constants.ShipAndObjectsMask);
    }

    [HarmonyPatch(nameof(Vent.ExitVent)), HarmonyPrefix]
    public static bool ExitVentPrefix(Vent __instance, PlayerControl pc)
    {
        if (!__instance.ExitVentAnim || !GameModifiers.HideVentAnims)
            return true;

        var truePosition = CustomPlayer.Local.GetTruePosition();
        var vector = pc.GetTruePosition() - truePosition;
        return vector.magnitude < CustomPlayer.Local.lightSource.viewDistance && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, vector.magnitude,
            Constants.ShipAndObjectsMask);
    }
}

[HarmonyPatch(typeof(VentilationSystem), nameof(VentilationSystem.IsVentCurrentlyBeingCleaned))]
public static class GetCorrectResult
{
    public static bool Prefix(VentilationSystem __instance, int id, ref bool __result)
    {
        __result = __instance.PlayersCleaningVents.Any((x, y) => y == id && PlayerById(x).CanDoTasks());
        return false;
    }
}