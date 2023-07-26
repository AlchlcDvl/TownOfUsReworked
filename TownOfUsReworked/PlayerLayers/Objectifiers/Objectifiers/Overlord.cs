namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Overlord : Objectifier
    {
        public bool IsAlive => !(IsDead || Disconnected);

        public override Color32 Color => ClientGameOptions.CustomObjColors ? Colors.Overlord : Colors.Objectifier;
        public override string Name => "Overlord";
        public override string Symbol => "β";
        public override LayerEnum Type => LayerEnum.Overlord;
        public override ObjectifierEnum ObjectifierType => ObjectifierEnum.Overlord;
        public override Func<string> TaskText => () => $"- Stay alive for {CustomGameOptions.OverlordMeetingWinCount} rounds";

        public Overlord(PlayerControl player) : base(player) => Hidden = !CustomGameOptions.OverlordKnows && !IsDead;

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Hidden && IsDead)
                Hidden = false;
        }
    }
}