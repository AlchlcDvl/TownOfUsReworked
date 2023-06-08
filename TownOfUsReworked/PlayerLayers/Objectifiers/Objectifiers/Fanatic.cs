namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public bool Turned;
        public bool Betrayed;
        public Faction Side = Faction.Crew;
        public bool Betray => ((Side == Faction.Intruder && ConstantVariables.LastImp) || (Side == Faction.Syndicate && ConstantVariables.LastSyn)) && !IsDead && Turned && !Betrayed;

        public Fanatic(PlayerControl player) : base(player)
        {
            Name = "Fanatic";
            Symbol = "♠";
            TaskText = () => !Turned ? "- Get attacked by either an <color=#FF0000FF>Intruder</color> or a <color=#008000FF>Syndicate</color> to join their side" : (Side ==
                Faction.Intruder ? Role.IntrudersWinCon : (Side == Faction.Syndicate ? Role.SyndicateWinCon : "- You feel conflicted"));
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Fanatic : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Fanatic;
            Hidden = !CustomGameOptions.FanaticKnows && !Turned;
            Type = LayerEnum.Fanatic;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public void TurnFanatic(Faction faction)
        {
            var fanaticRole = Role.GetRole(Player);
            fanaticRole.Faction = faction;
            Turned = true;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Utils.Flash(Colors.Mystic);

            if (faction == Faction.Syndicate)
            {
                Color = Colors.Syndicate;
                fanaticRole.IsSynFanatic = true;
                fanaticRole.FactionColor = Colors.Syndicate;
                fanaticRole.Objectives = () => Role.SyndicateWinCon;
            }
            else if (faction == Faction.Intruder)
            {
                Color = Colors.Intruder;
                fanaticRole.IsIntFanatic = true;
                fanaticRole.FactionColor = Colors.Intruder;
                fanaticRole.Objectives = () => Role.IntrudersWinCon;
            }

            Side = faction;
            Hidden = false;
            fanaticRole.RoleAlignment = fanaticRole.RoleAlignment.GetNewAlignment(fanaticRole.Faction);
            Player.RegenTask();

            foreach (var snitch in Ability.GetAbilities<Snitch>(AbilityEnum.Snitch))
            {
                if (CustomGameOptions.SnitchSeesFanatic)
                {
                    if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && Local)
                        Role.LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, Colors.Snitch, 0));
                    else if (snitch.TasksDone && PlayerControl.LocalPlayer == snitch.Player)
                        Role.GetRole(snitch.Player).AllArrows.Add(PlayerId, new(snitch.Player, Colors.Snitch));
                }
            }

            foreach (var revealer in Role.GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (revealer.Revealed && CustomGameOptions.RevealerRevealsTraitor && Local)
                    Role.LocalRole.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Color));
            }
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);

            if (role.RoleType == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player) { Objectives = role.Objectives };
            betrayer.RoleUpdate(role);
            Betrayed = true;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Betray && Turned)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnFanaticBetrayer);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnBetrayer();
            }
        }
    }
}