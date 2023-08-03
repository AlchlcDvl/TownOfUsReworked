namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static class SaveLadderPlayer
    {
        public static void Prefix(PlayerPhysics __instance)
        {
            try
            {
                if (CustomPlayer.Local.Is(RoleEnum.Transporter))
                    Role.GetRole<Transporter>(CustomPlayer.Local).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
                else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                    Role.GetRole<Retributionist>(CustomPlayer.Local).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
                else if (CustomPlayer.Local.Is(RoleEnum.Warper))
                    Role.GetRole<Warper>(CustomPlayer.Local).UnwarpablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
                else if (CustomPlayer.Local.Is(RoleEnum.PromotedRebel))
                    Role.GetRole<PromotedRebel>(CustomPlayer.Local).UnwarpablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
                else
                    CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local);
            }
            catch (Exception e)
            {
                LogSomething(e);
            }

            if (CustomPlayer.Local.Is(ModifierEnum.Astral))
                Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.LocalCustom.Position;
        }
    }

    [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
    public static class SavePlatformPlayer
    {
        public static void Prefix()
        {
            try
            {
                if (CustomPlayer.Local.Is(RoleEnum.Transporter))
                    Role.GetRole<Transporter>(CustomPlayer.Local).UntransportablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
                else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                    Role.GetRole<Retributionist>(CustomPlayer.Local).UntransportablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
                else if (CustomPlayer.Local.Is(RoleEnum.Warper))
                    Role.GetRole<Warper>(CustomPlayer.Local).UnwarpablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
                else if (CustomPlayer.Local.Is(RoleEnum.PromotedRebel))
                    Role.GetRole<PromotedRebel>(CustomPlayer.Local).UnwarpablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
                else
                    CallRpc(CustomRPC.Action, ActionsRPC.SetUninteractable, CustomPlayer.Local);
            }
            catch (Exception e)
            {
                LogSomething(e);
            }

            if (CustomPlayer.Local.Is(ModifierEnum.Astral))
                Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.LocalCustom.Position;
        }
    }
}