namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Traitor : Objectifier
    {
        public bool Turned;
        public bool Betrayed;
        public Faction Side = Faction.Crew;
        public bool Betray => ((Side == Faction.Intruder && ConstantVariables.LastImp) || (Side == Faction.Syndicate && ConstantVariables.LastSyn)) && !IsDead && Turned && !Betrayed;

        public Traitor(PlayerControl player) : base(player)
        {
            Name = "Traitor";
            Symbol = "♣";
            TaskText = () => !Turned ? "- Finish your tasks to switch sides to either <color=#FF0000FF>Intruders</color> or the <color=#008000FF>Syndicate</color>" : (Side ==
                Faction.Intruder ? Role.IntrudersWinCon : (Side == Faction.Syndicate ? Role.SyndicateWinCon : "- You feel conflicted"));
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Traitor : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Traitor;
            Hidden = !CustomGameOptions.TraitorKnows && !Turned;
            Type = LayerEnum.Traitor;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
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

        public void TurnTraitor()
        {
            var traitorRole = Role.GetRole(Player);

            var IntrudersAlive = PlayerControl.AllPlayerControls.Count(x => x.Is(Faction.Intruder) && !x.Data.IsDead && !x.Data.Disconnected);
            var SyndicateAlive = PlayerControl.AllPlayerControls.Count(x => x.Is(Faction.Syndicate) && !x.Data.IsDead && !x.Data.Disconnected);

            var turnIntruder = false;
            var turnSyndicate = false;

            if (IntrudersAlive > 0 && SyndicateAlive > 0)
            {
                var random = URandom.RandomRangeInt(0, 100);

                if (IntrudersAlive == SyndicateAlive)
                {
                    turnIntruder = random < 50;
                    turnSyndicate = random >= 50;
                }
                else if (IntrudersAlive > SyndicateAlive)
                {
                    turnIntruder = random < 25;
                    turnSyndicate = random >= 25;
                }
                else if (IntrudersAlive < SyndicateAlive)
                {
                    turnIntruder = random < 75;
                    turnSyndicate = random >= 75;
                }
            }
            else if (IntrudersAlive > 0 && SyndicateAlive == 0)
                turnIntruder = true;
            else if (SyndicateAlive > 0 && IntrudersAlive == 0)
                turnSyndicate = true;
            else
                return;

            if (turnIntruder)
            {
                traitorRole.Faction = Faction.Intruder;
                Color = Colors.Intruder;
                traitorRole.IsIntTraitor = true;
                traitorRole.FactionColor = Colors.Intruder;
                traitorRole.Objectives = () => Role.IntrudersWinCon;
            }
            else if (turnSyndicate)
            {
                traitorRole.Faction = Faction.Syndicate;
                traitorRole.IsSynTraitor = true;
                Color = Colors.Syndicate;
                traitorRole.FactionColor = Colors.Syndicate;
                traitorRole.Objectives = () => Role.SyndicateWinCon;
            }

            Side = traitorRole.Faction;
            Turned = true;
            Hidden = false;
            traitorRole.RoleAlignment = traitorRole.RoleAlignment.GetNewAlignment(traitorRole.Faction);
            Player.RegenTask();

            foreach (var snitch in Ability.GetAbilities<Snitch>(AbilityEnum.Snitch))
            {
                if (CustomGameOptions.SnitchSeesTraitor)
                {
                    if (snitch.TasksLeft <= CustomGameOptions.SnitchTasksRemaining && PlayerControl.LocalPlayer == Player)
                        Role.LocalRole.AllArrows.Add(snitch.PlayerId, new(Player, Colors.Snitch, 0));
                    else if (snitch.TasksDone && PlayerControl.LocalPlayer == snitch.Player)
                        Role.GetRole(snitch.Player).AllArrows.Add(Player.PlayerId, new(snitch.Player, Colors.Snitch));
                }
            }

            foreach (var revealer in Role.GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (revealer.Revealed && CustomGameOptions.RevealerRevealsTraitor && Player == PlayerControl.LocalPlayer)
                    Role.LocalRole.AllArrows.Add(revealer.PlayerId, new(Player, revealer.Color));
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && Player != PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Mystic);

            if (Player == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Is(traitorRole.Faction))
                Utils.Flash(Colors.Traitor);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (Betray && Turned)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnTraitorBetrayer);
                writer.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnBetrayer();
            }
        }
    }
}