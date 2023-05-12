namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Cryomaniac : NeutralRole
    {
        public CustomButton FreezeButton;
        public CustomButton DouseButton;
        public CustomButton KillButton;
        public List<byte> Doused;
        public bool FreezeUsed;
        public DateTime LastDoused;
        public DateTime LastKilled;
        public bool LastKiller => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || x.Is(RoleAlignment.NeutralNeo) || (x.Is(RoleAlignment.NeutralKill) && x !=
            Player)));
        public int DousedAlive => Doused.Count(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);

        public Cryomaniac(PlayerControl player) : base(player)
        {
            Name = "Cryomaniac";
            StartText = "Who Likes Ice Cream?";
            AbilitiesText = "- You can douse players in coolant\n- Doused players can be frozen, which kills all of them at once at the start of the next meeting\n- People who " +
                "interact with you will also get doused";
            Objectives = "- Freeze anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Cryomaniac : Colors.Neutral;
            RoleType = RoleEnum.Cryomaniac;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            Doused = new();
            Type = LayerEnum.Cryomaniac;
            DouseButton = new(this, "CryoDouse", AbilityTypes.Direct, "ActionSecondary", Douse, Exception);
            FreezeButton = new(this, "CryoFreeze", AbilityTypes.Effect, "Secondary", Freeze);
            KillButton = new(this, "CryoKill", AbilityTypes.Direct, "Tertiary", Kill, Exception);
            InspectorResults = InspectorResults.SeeksToDestroy;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDoused;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DouseCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DouseCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Kill()
        {
            if (Utils.IsTooFar(Player, KillButton.TargetPlayer) || KillTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, KillButton.TargetPlayer, true);

            if (interact[0] || interact[3])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
        {
            if (!source.Is(RoleType) || Doused.Contains(target.PlayerId))
                return;

            Doused.Add(target.PlayerId);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.FreezeDouse);
            writer.Write(PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var cryo in GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
            {
                if (cryo.FreezeUsed)
                {
                    foreach (var player in cryo.Doused)
                    {
                        var player2 = Utils.PlayerById(player);

                        if (player2.Data.IsDead || player2.Data.Disconnected || player2.Is(RoleEnum.Pestilence) || player2.IsProtected())
                            continue;

                        Utils.RpcMurderPlayer(Player, player2, DeathReasonEnum.Frozen);
                    }

                    cryo.Doused.Clear();
                    cryo.FreezeUsed = false;
                }
            }
        }

        public bool Exception(PlayerControl player) => !Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction
            is Faction.Intruder or Faction.Syndicate) || player == Player.GetOtherLover() || player == Player.GetOtherRival() || (player.Is(ObjectifierEnum.Mafia) &&
            Player.Is(ObjectifierEnum.Mafia));

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            DouseButton.Update("DOUSE", DouseTimer(), CustomGameOptions.DouseCd);
            KillButton.Update("KILL", KillTimer(), CustomGameOptions.DouseCd, true, LastKiller);
            FreezeButton.Update("FREEZE", 0, 1, true, DousedAlive > 0 && !FreezeUsed);
        }

        public void Douse()
        {
            if (Utils.IsTooFar(Player, DouseButton.TargetPlayer) || DouseTimer() != 0f || Doused.Contains(DouseButton.TargetPlayer.PlayerId))
                return;

            var interact = Utils.Interact(Player, DouseButton.TargetPlayer, LastKiller);

            if (interact[3])
                RpcSpreadDouse(Player, DouseButton.TargetPlayer);

            if (interact[0])
                LastDoused = DateTime.UtcNow;
            else if (interact[1])
                LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastDoused.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void Freeze()
        {
            if (DousedAlive <= 0 || FreezeUsed)
                return;

            FreezeUsed = true;
        }
    }
}
