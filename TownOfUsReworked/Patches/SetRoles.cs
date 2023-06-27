namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    public static class SetRoles
    {
        public static void Postfix()
        {
            Utils.LogSomething("RPC SET ROLE");
            RoleGen.ResetEverything();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Start, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Utils.LogSomething("Cleared Variables");
            RoleGen.BeginRoleGen();
        }
    }
}