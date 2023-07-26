namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Concealer : Syndicate
    {
        public CustomButton ConcealButton;
        public bool Enabled;
        public DateTime LastConcealed;
        public float TimeRemaining;
        public bool Concealed => TimeRemaining > 0f;
        public PlayerControl ConcealedPlayer;
        public CustomMenu ConcealMenu;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate;
        public override string Name => "Concealer";
        public override LayerEnum Type => LayerEnum.Concealer;
        public override RoleEnum RoleType => RoleEnum.Concealer;
        public override Func<string> StartText => () => "Turn The <color=#8CFFFFFF>Crew</color> Invisible For Some Chaos";
        public override Func<string> AbilitiesText => () => $"- You can turn {(HoldsDrive ? "everyone" : "a player")} invisible\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.Unseen;

        public Concealer(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicateDisrup;
            ConcealMenu = new(Player, Click, Exception1);
            ConcealedPlayer = null;
            ConcealButton = new(this, "Conceal", AbilityTypes.Effect, "Secondary", HitConceal);
        }

        public void Conceal()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (HoldsDrive)
                Utils.Conceal();
            else
                Invis(ConcealedPlayer, CustomPlayer.Local.Is(Faction.Syndicate));

            if (Meeting || (ConcealedPlayer == null && !HoldsDrive))
                TimeRemaining = 0f;
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;

            if (HoldsDrive)
                DefaultOutfitAll();
            else
                DefaultOutfit(ConcealedPlayer);

            ConcealedPlayer = null;
        }

        public float ConcealTimer()
        {
            var timespan = DateTime.UtcNow - LastConcealed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ConcealCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Interact(Player, player);

            if (interact[3])
                ConcealedPlayer = player;
            else if (interact[0])
                LastConcealed = DateTime.UtcNow;
            else if (interact[1])
                LastConcealed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void HitConceal()
        {
            if (ConcealTimer() != 0f || Concealed)
                return;

            if (HoldsDrive)
            {
                TimeRemaining = CustomGameOptions.ConcealDuration;
                Conceal();
                CallRpc(CustomRPC.Action, ActionsRPC.Conceal, this);
            }
            else if (ConcealedPlayer == null)
                ConcealMenu.Open();
            else
            {
                TimeRemaining = CustomGameOptions.ConcealDuration;
                CallRpc(CustomRPC.Action, ActionsRPC.Conceal, this, ConcealedPlayer);
                Conceal();
            }
        }

        public bool Exception1(PlayerControl player) => player == ConcealedPlayer || player == Player || (player.Is(Faction) && !CustomGameOptions.ConcealMates);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = ConcealedPlayer == null && !HoldsDrive;
            ConcealButton.Update(flag ? "SET TARGET" : "CONCEAL", ConcealTimer(), CustomGameOptions.ConcealCooldown, Concealed, TimeRemaining, CustomGameOptions.ConcealDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (ConcealedPlayer != null && !HoldsDrive && !Concealed)
                    ConcealedPlayer = null;

                LogSomething("Removed a target");
            }
        }
    }
}