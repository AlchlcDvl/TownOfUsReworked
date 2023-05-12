namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ClimbLadder))]
    public static class SaveLadderPlayer
    {
        public static void Prefix(PlayerPhysics __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                Role.GetRole<Retributionist>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Warper))
                Role.GetRole<Warper>(PlayerControl.LocalPlayer).UnwarpablePlayers.Add(__instance.myPlayer.PlayerId, DateTime.UtcNow);
        }
    }

    [HarmonyPatch(typeof(MovingPlatformBehaviour), nameof(MovingPlatformBehaviour.Use), new Type[] {})]
    public static class SavePlatformPlayer
    {
        public static void Prefix()
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
                Role.GetRole<Retributionist>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Warper))
                Role.GetRole<Warper>(PlayerControl.LocalPlayer).UnwarpablePlayers.Add(PlayerControl.LocalPlayer.PlayerId, DateTime.UtcNow);
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetUninteractable);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}