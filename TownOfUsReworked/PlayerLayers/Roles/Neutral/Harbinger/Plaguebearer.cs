namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Plaguebearer : Neutral
    {
        public DateTime LastInfected;
        public List<byte> Infected = new();
        public bool CanTransform => CustomPlayer.AllPlayers.Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= Infected.Count || CustomGameOptions.PestSpawn;
        public CustomButton InfectButton;

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            StartText = () => "Spread Disease To Become Pestilence";
            AbilitiesText = () => "- You can infect players\n- When all players are infected, you will turn into <color=#424242FF>Pestilence</color>";
            Objectives = () => "- Infect everyone to become <color=#424242FF>Pestilence</color>\n- Kill off anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Plaguebearer : Colors.Neutral;
            RoleType = RoleEnum.Plaguebearer;
            RoleAlignment = RoleAlignment.NeutralHarb;
            Infected = new() { Player.PlayerId };
            Type = LayerEnum.Plaguebearer;
            InfectButton = new(this, "Infect", AbilityTypes.Direct, "ActionSecondary", Infect, Exception);
            InspectorResults = InspectorResults.SeeksToDestroy;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float InfectTimer()
        {
            var timespan = DateTime.UtcNow - LastInfected;
            var num = Player.GetModifiedCooldown(CustomGameOptions.InfectCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            if (Infected.Contains(source.PlayerId) && Infected.Contains(target.PlayerId))
                return;

            if (!Infected.Contains(source.PlayerId) && !Infected.Contains(target.PlayerId))
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Infect);
            writer.Write(PlayerId);

            if (Infected.Contains(source.PlayerId) || source.Is(RoleEnum.Plaguebearer))
            {
                Infected.Add(target.PlayerId);
                writer.Write(target.PlayerId);
            }
            else if (Infected.Contains(target.PlayerId) || target.Is(RoleEnum.Plaguebearer))
            {
                Infected.Add(source.PlayerId);
                writer.Write(source.PlayerId);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public void Infect()
        {
            if (Utils.IsTooFar(Player, InfectButton.TargetPlayer) || InfectTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, InfectButton.TargetPlayer);

            if (interact[3])
                RpcSpreadInfection(Player, InfectButton.TargetPlayer);

            if (interact[0])
                LastInfected = DateTime.UtcNow;
            else if (interact[1])
                LastInfected.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void TurnPestilence()
        {
            var newRole = new Pestilence(Player);
            newRole.RoleUpdate(this);

            if ((Local || CustomGameOptions.PlayersAlerted) && !IntroCutscene.Instance)
                Utils.Flash(Colors.Pestilence);

            if (CustomPlayer.Local.Is(RoleEnum.Seer) && !CustomGameOptions.PlayersAlerted && !IntroCutscene.Instance)
                Utils.Flash(Colors.Seer);
        }

        public bool Exception(PlayerControl player) => Infected.Contains(player.PlayerId);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            InfectButton.Update("INFECT", InfectTimer(), CustomGameOptions.InfectCd, true, !CanTransform);

            if (CanTransform && !IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnPestilence);
                writer.Write(CustomPlayer.Local.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnPestilence();
            }
        }
    }
}