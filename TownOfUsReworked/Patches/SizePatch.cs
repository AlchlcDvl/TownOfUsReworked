namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SizePatch
    {
        public static void Postfix()
        {
            if (ConstantVariables.IsLobby)
            {
                foreach (var player in PlayerControl.AllPlayerControls.ToArray())
                    player.transform.localScale = new(0.7f, 0.7f, 1f);

                return;
            }

            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
                player.transform.localScale = !(player.Data.IsDead || player.Data.Disconnected) ? player.GetAppearance().SizeFactor : new(0.7f, 0.7f, 1f);

            foreach (var body in Utils.AllBodies)
                body.transform.localScale = Utils.PlayerByBody(body).GetAppearance().SizeFactor;
        }
    }
}