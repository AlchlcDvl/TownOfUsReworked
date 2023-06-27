namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static class SaveLadderPlayer
    {
        public static void Prefix(PlayerPhysics __instance)
        {
            if (CustomPlayer.Local.Is(RoleEnum.Transporter))
                Role.GetRole<Transporter>(CustomPlayer.Local).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                Role.GetRole<Retributionist>(CustomPlayer.Local).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(RoleEnum.Warper))
                Role.GetRole<Warper>(CustomPlayer.Local).UnwarpablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);

            if (CustomPlayer.Local.Is(ModifierEnum.Astral))
                Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.Local.transform.position;
        }
    }

    [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.UsePlatform))]
    public static class SavePlatformPlayer
    {
        public static void Prefix()
        {
            if (CustomPlayer.Local.Is(RoleEnum.Transporter))
                Role.GetRole<Transporter>(CustomPlayer.Local).UntransportablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(RoleEnum.Retributionist))
                Role.GetRole<Retributionist>(CustomPlayer.Local).UntransportablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            else if (CustomPlayer.Local.Is(RoleEnum.Warper))
                Role.GetRole<Warper>(CustomPlayer.Local).UnwarpablePlayers.Add(CustomPlayer.Local.PlayerId, DateTime.UtcNow);
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetUninteractable);
                writer.Write(CustomPlayer.Local.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            if (CustomPlayer.Local.Is(ModifierEnum.Astral))
                Modifier.GetModifier<Astral>(CustomPlayer.Local).LastPosition = CustomPlayer.Local.transform.position;
        }
    }
}