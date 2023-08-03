namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Arsonist : Neutral
    {
        public CustomButton IgniteButton { get; set; }
        public CustomButton DouseButton { get; set; }
        public bool LastKiller => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.CrewKill) || x.Is(RoleAlignment.CrewAudit) || x.Is(RoleAlignment.NeutralPros) || x.Is(RoleAlignment.NeutralNeo) || (x.Is(RoleAlignment.NeutralKill) && x !=
            Player))) && CustomGameOptions.ArsoLastKillerBoost;
        public List<byte> Doused { get; set; }
        public DateTime LastDoused { get; set; }
        public DateTime LastIgnited { get; set; }

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Arsonist : Colors.Neutral;
        public override string Name => "Arsonist";
        public override LayerEnum Type => LayerEnum.Arsonist;
        public override RoleEnum RoleType => RoleEnum.Arsonist;
        public override Func<string> StartText => () => "PYROMANIAAAAAAAAAAAAAA";
        public override Func<string> Description => () => "- You can douse players in gasoline\n- Doused players can be ignited, killing them all at once\n- Players who interact with " +
            "you will get doused";
        public override InspectorResults InspectorResults => InspectorResults.SeeksToDestroy;
        public float DouseTimer => ButtonUtils.Timer(Player, LastDoused, CustomGameOptions.DouseCd);
        public float IgniteTimer => ButtonUtils.Timer(Player, LastIgnited, CustomGameOptions.IgniteCd);

        public Arsonist(PlayerControl player) : base(player)
        {
            Objectives = () => "- Burn anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            Doused = new();
            DouseButton = new(this, "ArsoDouse", AbilityTypes.Direct, "ActionSecondary", Douse, Exception);
            IgniteButton = new(this, "Ignite", AbilityTypes.Effect, "Secondary", Ignite);
        }

        public void Ignite()
        {
            if (IgniteTimer != 0f || Doused.Count == 0)
                return;

            foreach (var arso in GetRoles<Arsonist>(RoleEnum.Arsonist))
            {
                if (arso.Player != Player && !CustomGameOptions.ArsoIgniteAll)
                    continue;

                foreach (var playerId in arso.Doused)
                {
                    var player = PlayerById(playerId);

                    if (player?.Data.Disconnected == true || player.Data.IsDead || player.Is(RoleEnum.Pestilence) || player.IsProtected())
                        continue;

                    RpcMurderPlayer(Player, player, DeathReasonEnum.Ignited, false);
                }

                if (CustomGameOptions.IgnitionCremates)
                {
                    CallRpc(CustomRPC.Action, ActionsRPC.Burn, arso);

                    foreach (var body in AllBodies)
                    {
                        if (arso.Doused.Contains(body.ParentId) && PlayerById(body.ParentId).Data.IsDead)
                        {
                            Coroutines.Start(FadeBody(body));
                            _ = new Ash(body.TruePosition);
                        }
                    }
                }

                arso.Doused.Clear();
            }

            if (!LastKiller)
                LastIgnited = DateTime.UtcNow;

            if (CustomGameOptions.ArsoCooldownsLinked)
                LastDoused = DateTime.UtcNow;
        }

        public void Douse()
        {
            if (IsTooFar(Player, DouseButton.TargetPlayer) || DouseTimer != 0f || Doused.Contains(DouseButton.TargetPlayer.PlayerId))
                return;

            var interact = Interact(Player, DouseButton.TargetPlayer);

            if (interact[3])
                RpcSpreadDouse(Player, DouseButton.TargetPlayer);

            if (interact[0])
            {
                LastDoused = DateTime.UtcNow;

                if (CustomGameOptions.ArsoCooldownsLinked && !LastKiller)
                    LastIgnited = DateTime.UtcNow;
            }
            else if (interact[1])
            {
                LastDoused.AddSeconds(CustomGameOptions.ProtectKCReset);

                if (CustomGameOptions.ArsoCooldownsLinked && !LastKiller)
                    LastIgnited.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
        }

        public void RpcSpreadDouse(PlayerControl source, PlayerControl target)
        {
            if (!source.Is(RoleType) || Doused.Contains(target.PlayerId) || source != Player)
                return;

            Doused.Add(target.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.Douse, this, target);
        }

        public bool Exception(PlayerControl player) => Doused.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction
            is Faction.Intruder or Faction.Syndicate) || Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            DouseButton.Update("DOUSE", DouseTimer, CustomGameOptions.DouseCd);
            IgniteButton.Update("IGNITE", IgniteTimer, CustomGameOptions.IgniteCd, true, Doused.Count > 0);
        }
    }
}
