namespace TownOfUsReworked.Patches.Gameplay;

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

        if (couldUse && VentingOptions.NoVentingUncleanedVents && Ship().Systems[SystemTypes.Ventilation].TryCast<VentilationSystem>(out var ventilationSystem) &&
            ventilationSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
        {
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
        // Fix for dlekS and other things
        if (LocalPlayer.IsMoving() || (LocalPlayer.Is<Role>(out var role) && (!role.CanVent || !role.CanSwitchVents)))
            return false;

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
                    var ship = Ship();
                    var ventilationSystem = ship.Systems[SystemTypes.Ventilation].TryCast<VentilationSystem>();
                    var gameObject = __instance.CleaningIndicators.Any() ? __instance.CleaningIndicators[i] : null;
                    __instance.ToggleNeighborVentBeingCleaned(ventilationSystem?.IsVentCurrentlyBeingCleaned(vent.Id) == true || LocalBlocked(), buttonBehavior, gameObject);
                    var vector2 = vent.transform.position - __instance.transform.position;
                    var vector3 = vector2.normalized * (0.7f + __instance.spreadShift);
                    vector3.x *= Mathf.Sign(ship.transform.localScale.x);
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

        return false;
    }

    // Vent and kill shit
    // Yes, thank you Discussions - AD
    [HarmonyPatch(nameof(Vent.SetOutline))]
    public static void Postfix(Vent __instance, bool mainTarget)
    {
        if (!LocalPlayer.Is<Role>(out var role) || Meeting() || !LocalPlayer.CanVent())
            return;

        __instance.myRend.material.SetColor(OutlineColor, role.Color);
        __instance.myRend.material.SetColor(AddColor, mainTarget ? role.Color : UColor.clear);
    }

    [HarmonyPatch(nameof(Vent.Use)), HarmonyPrefix]
    public static bool UsePrefix(Vent __instance) => CheckMoveVent(__instance);

    [HarmonyPatch(nameof(Vent.TryMoveToVent)), HarmonyPrefix]
    public static bool TryMoveToVentPrefix(Vent otherVent) => CheckMoveVent(otherVent);

    private static bool CheckMoveVent(Vent vent)
    {
        if (NoPlayers() || !LocalPlayer.CanVent() || LocalBlocked())
            return false;

        if (!vent.IsBombed() || LocalPlayer.Is<IGhosty>() || !CanAttack(AttackEnum.Powerful, LocalPlayer.GetDefenseValue()))
            return true;

        LocalPlayer.RpcSuicide();
        Role.BastionBomb(vent, Bastion.BombRemovedOnKill);
        CallRpc(CustomRPC.Misc, MiscRPC.BastionBomb, vent);
        return false;
    }

    [HarmonyPatch(nameof(Vent.EnterVent)), HarmonyPrefix]
    public static bool EnterVentPrefix(Vent __instance, PlayerControl pc) => EnterExitVentPrefix(pc, __instance.EnterVentAnim);

    [HarmonyPatch(nameof(Vent.ExitVent)), HarmonyPrefix]
    public static bool ExitVentPrefix(Vent __instance, PlayerControl pc) => EnterExitVentPrefix(pc, __instance.ExitVentAnim);

    private static bool EnterExitVentPrefix(PlayerControl pc, AnimationClip clip)
    {
        if (!clip || !VentingOptions.HideVentAnims)
            return true;

        var truePosition = LocalPlayer.GetTruePosition();
        var vector = pc.GetTruePosition() - truePosition;
        return vector.magnitude < LocalPlayer.lightSource.viewDistance && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask);
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

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoEnterVent))]
public static class VanillaVentWalkToFix
{
    public static bool Prefix(PlayerPhysics __instance, int id, ref IIEnumerator __result)
    {
        __result = CoEnterVent(__instance, id).WrapToIl2Cpp();
        return false;
    }

    private static IEnumerator CoEnterVent(PlayerPhysics __instance, int id)
    {
        if (Meeting())
            yield break;

        var vent = VentById(id);
        __instance.myPlayer.NetTransform.SetPaused(true);

        if (__instance.myPlayer.AmOwner)
            __instance.inputHandler.enabled = true;

        __instance.myPlayer.walkingToVent = true;
        __instance.myPlayer.moveable = false;
        var iEnum = __instance.WalkPlayerTo(vent.transform.position + vent.Offset);
        var time = 0f;

        while (iEnum.MoveNext())
        {
            time += Time.deltaTime;

            if (time >= 2f)
                yield break;
        }

        __instance.myPlayer.inVent = true;
        DebugAnalytics.Instance.Analytics.VentUsed(__instance.myPlayer.Data);
        vent.EnterVent(__instance.myPlayer);
        __instance.myPlayer.cosmetics.AnimateSkinEnterVent();
        yield return __instance.Animations.CoPlayEnterVentAnimation(vent.NumFramesUntilPlayerDisappears);
        __instance.myPlayer.cosmetics.AnimateSkinIdle();
        __instance.Animations.PlayIdleAnimation();
        __instance.myPlayer.Visible = false;
        __instance.myPlayer.walkingToVent = false;
        __instance.myPlayer.currentRoleAnimations.ForEach(an => an.ToggleRenderer(false));

        if (__instance.myPlayer.AmOwner)
        {
            VentilationSystem.Update(VentilationSystem.Operation.Enter, id);
            __instance.inputHandler.enabled = false;
        }

        __instance.logger.Debug($"Player {__instance.myPlayer.PlayerId} entered vent {id}");
    }
}