namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Fanatic : Objectifier
    {
        public bool Turned;
        public bool Betrayed;
        public Faction Side = Faction.Crew;
        public bool Betray => ((Side == Faction.Intruder && LastImp) || (Side == Faction.Syndicate && LastSyn)) && !IsDead && Turned && !Betrayed;

        public override Color32 Color
        {
            get
            {
                if (Turned)
                {
                    if (Side == Faction.Syndicate)
                        return Colors.Syndicate;
                    else if (Side == Faction.Intruder)
                        return Colors.Intruder;
                    else
                        return ClientGameOptions.CustomObjColors ? Colors.Fanatic : Colors.Objectifier;
                }
                else
                    return ClientGameOptions.CustomObjColors ? Colors.Fanatic : Colors.Objectifier;
            }
        }
        public override string Name => "Fanatic";
        public override string Symbol => "♠";
        public override LayerEnum Type => LayerEnum.Fanatic;
        public override ObjectifierEnum ObjectifierType => ObjectifierEnum.Fanatic;
        public override Func<string> TaskText => () => !Turned ? "- Get attacked by either an <color=#FF0000FF>Intruder</color> or a <color=#008000FF>Syndicate</color> to join their side" :
            "";
        public override bool Hidden => !CustomGameOptions.FanaticKnows && !Turned && !IsDead;

        public Fanatic(PlayerControl player) : base(player) {}

        public void TurnFanatic(Faction faction)
        {
            var fanaticRole = Role.GetRole(Player);
            fanaticRole.Faction = faction;
            Turned = true;

            if (CustomPlayer.Local.Is(RoleEnum.Mystic) || CustomPlayer.Local.Is(faction))
                Flash(Colors.Mystic);

            if (faction == Faction.Syndicate)
            {
                fanaticRole.IsSynFanatic = true;
                fanaticRole.FactionColor = Colors.Syndicate;
                fanaticRole.Objectives = () => Role.SyndicateWinCon;
            }
            else if (faction == Faction.Intruder)
            {
                fanaticRole.IsIntFanatic = true;
                fanaticRole.FactionColor = Colors.Intruder;
                fanaticRole.Objectives = () => Role.IntrudersWinCon;
            }

            Side = faction;
            fanaticRole.RoleAlignment = fanaticRole.RoleAlignment.GetNewAlignment(fanaticRole.Faction);

            foreach (var snitch in Ability.GetAbilities<Snitch>(AbilityEnum.Snitch))
            {
                if (CustomGameOptions.SnitchSeesFanatic)
                {
                    if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && Local)
                        Role.LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, Colors.Snitch));
                    else if (snitch.TasksDone && CustomPlayer.Local == snitch.Player)
                        Role.GetRole(snitch.Player).AllArrows.Add(PlayerId, new(snitch.Player, Colors.Snitch));
                }
            }

            foreach (var revealer in Role.GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (revealer.Revealed && CustomGameOptions.RevealerRevealsTraitor && Local)
                    Role.LocalRole.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Color));
            }

            if (CustomPlayer.Local.Is(RoleEnum.Mystic) && !Local)
                Flash(Colors.Mystic);

            if (Local || CustomPlayer.Local.Is(faction))
                Flash(Colors.Fanatic);
        }

        public void TurnBetrayer()
        {
            var role = Role.GetRole(Player);
            Betrayed = true;

            if (role.RoleType == RoleEnum.Betrayer)
                return;

            var betrayer = new Betrayer(Player) { Objectives = role.Objectives };
            betrayer.RoleUpdate(role);

            if (Local)
                Flash(Colors.Betrayer);

            if (CustomPlayer.Local.Is(RoleEnum.Seer))
                Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Betray && Turned)
            {
                CallRpc(CustomRPC.Change, TurnRPC.TurnFanaticBetrayer, this);
                TurnBetrayer();
            }
        }
    }
}