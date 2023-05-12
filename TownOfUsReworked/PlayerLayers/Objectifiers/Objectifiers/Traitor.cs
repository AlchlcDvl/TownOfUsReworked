namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Traitor : Objectifier
    {
        public bool Turned;
        public Faction Side = Faction.Crew;
        public bool Betray => ((Side == Faction.Intruder && ConstantVariables.LastImp) || (Side == Faction.Syndicate && ConstantVariables.LastSyn)) && !IsDead;

        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            SymbolName = "♣";
            TaskText = "- Finish your tasks to switch sides to either <color=#FF0000FF>Intruders</color> or the <color=#008000FF>Syndicate</color>";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Traitor : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Traitor;
            Hidden = !CustomGameOptions.TraitorKnows && !Turned;
            Type = LayerEnum.Traitor;
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);

            if (role.RoleType == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player) { Objectives = role.Objectives };
            betrayer.RoleUpdate(role);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Betray && Turned)
            {
                TurnBetrayer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnTraitorBetrayer);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}