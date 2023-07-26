namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    public static class SetRoles
    {
        public static void Postfix()
        {
            LogSomething("RPC SET ROLE");
            RoleGen.ResetEverything();
            CallRpc(CustomRPC.Misc, MiscRPC.Start);
            LogSomething("Cleared Variables");
            RoleGen.BeginRoleGen();
        }
    }
}