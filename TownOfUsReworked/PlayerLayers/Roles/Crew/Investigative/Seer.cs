namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Seer : Crew
    {
        public DateTime LastSeered { get; set; }
        public bool ChangedDead => !AllRoles.Any(x => x.Player != null && !x.IsDead && !x.Disconnected && (x.RoleHistory.Count > 0 || x.Is(RoleEnum.Amnesiac) || x.Is(RoleEnum.Thief) ||
            x.Player.Is(ObjectifierEnum.Traitor) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Godfather) || x.Is(RoleEnum.Mafioso) || x.Is(RoleEnum.Shifter) || x.Is(RoleEnum.Guesser) ||
            x.Is(RoleEnum.Rebel) || x.Is(RoleEnum.Mystic) || (x.Is(RoleEnum.Seer) && x != this) || x.Is(RoleEnum.Sidekick) || x.Is(RoleEnum.GuardianAngel) || x.Is(RoleEnum.Executioner) ||
            x.Is(RoleEnum.BountyHunter) || x.Player.Is(ObjectifierEnum.Fanatic)));
        public CustomButton SeerButton { get; set; }
        public float Timer => ButtonUtils.Timer(Player, LastSeered, CustomGameOptions.SeerCooldown);

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Seer : Colors.Crew;
        public override string Name => "Seer";
        public override LayerEnum Type => LayerEnum.Seer;
        public override RoleEnum RoleType => RoleEnum.Seer;
        public override Func<string> StartText => () => "You Can See People's Histories";
        public override Func<string> Description => () => "- You can investigate players to see if their roles have changed\n- If all players whose roles changed have died, you will " +
            "become a <color=#FFCC80FF>Sheriff</color>";
        public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

        public Seer(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewInvest;
            SeerButton = new(this, "Seer", AbilityTypes.Direct, "ActionSecondary", See);
        }

        public void TurnSheriff()
        {
            var role = new Sheriff(Player);
            role.RoleUpdate(this);

            if (Local)
                Flash(Colors.Sheriff);
        }

        public void See()
        {
            if (Timer != 0f || IsTooFar(Player, SeerButton.TargetPlayer))
                return;

            var interact = Interact(Player, SeerButton.TargetPlayer);

            if (interact[3])
            {
                if (GetRole(SeerButton.TargetPlayer).RoleHistory.Count > 0 || SeerButton.TargetPlayer.IsFramed())
                    Flash(new(255, 0, 0, 255));
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastSeered = DateTime.UtcNow;
            else if (interact[1])
                LastSeered.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SeerButton.Update("SEE", Timer, CustomGameOptions.SeerCooldown);

            if (ChangedDead && !IsDead)
            {
                CallRpc(CustomRPC.Change, TurnRPC.TurnSheriff, this);
                TurnSheriff();
            }
        }
    }
}