namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Defector : Objectifier
    {
        public bool Turned;
        public Faction Side;
        public bool Defect => ((Side == Faction.Intruder && ConstantVariables.LastImp) || (Side == Faction.Syndicate && ConstantVariables.LastSyn)) && !IsDead && !Turned;

        public Defector(PlayerControl player) : base(player)
        {
            Name = "Defector";
            Symbol = "ε";
            TaskText = () => "- Be the last one of your faction to switch sides";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Defector : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Defector;
            Type = LayerEnum.Defector;
            Hidden = !CustomGameOptions.DefectorKnows;
            Side = Player.GetFaction();

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void TurnSides()
        {
            Hidden = false;
            Turned = true;
            var crew = CustomGameOptions.DefectorFaction == DefectorFaction.Crew;
            var evil = CustomGameOptions.DefectorFaction == DefectorFaction.OpposingEvil;

            if (CustomGameOptions.DefectorFaction == DefectorFaction.Random)
            {
                var random = URandom.RandomRangeInt(0, 2);
                evil = random == 0;
                crew = random == 1;
            }

            var role = Role.GetRole(Player);

            if (crew)
            {
                role.Faction = Faction.Crew;
                role.FactionColor = Colors.Crew;
                role.IsCrewDefect = true;
                Color = Colors.Crew;
            }
            else if (evil)
            {
                if (Side == Faction.Intruder)
                {
                    role.Faction = Faction.Syndicate;
                    role.FactionColor = Colors.Syndicate;
                    Color = Colors.Syndicate;
                    role.IsSynDefect = true;
                }
                else if (Side == Faction.Intruder)
                {
                    role.Faction = Faction.Intruder;
                    role.FactionColor = Colors.Intruder;
                    Color = Colors.Intruder;
                    role.IsIntDefect = true;
                }
            }

            Side = role.Faction;
            role.RoleAlignment = role.RoleAlignment.GetNewAlignment(role.Faction);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Defect && !Turned)
            {
                TurnSides();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSides);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}