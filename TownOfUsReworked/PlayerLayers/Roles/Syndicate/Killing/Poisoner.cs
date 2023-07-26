namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Poisoner : Syndicate
    {
        public CustomButton PoisonButton;
        public CustomButton GlobalPoisonButton;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public float TimeRemaining;
        public bool Enabled;
        public bool Poisoned => TimeRemaining > 0f;
        public CustomMenu PoisonMenu;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Poisoner : Colors.Syndicate;
        public override string Name => "Poisoner";
        public override LayerEnum Type => LayerEnum.Poisoner;
        public override RoleEnum RoleType => RoleEnum.Poisoner;
        public override Func<string> StartText => () => "Delay A Kill To Decieve The <color=#8CFFFFFF>Crew</color>";
        public override Func<string> AbilitiesText => () => $"- You can poison players{(HoldsDrive ? " from afar" : "")}\n- Poisoned players will die after " +
            $"{CustomGameOptions.PoisonDuration}s\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.Unseen;

        public Poisoner(PlayerControl player) : base(player)
        {
            PoisonedPlayer = null;
            RoleAlignment = RoleAlignment.SyndicateKill;
            PoisonMenu = new(Player, Click, Exception1);
            PoisonButton = new(this, "Poison", AbilityTypes.Direct, "ActionSecondary", HitPoison, Exception1);
            GlobalPoisonButton = new(this, "GlobalPoison", AbilityTypes.Effect, "ActionSecondary", HitGlobalPoison);
        }

        public void Poison()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Meeting || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || IsDead)
                TimeRemaining = 0f;
        }

        public void PoisonKill()
        {
            if (!(PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || PoisonedPlayer.Is(RoleEnum.Pestilence)))
                RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

            PoisonedPlayer = null;
            Enabled = false;
            LastPoisoned = DateTime.UtcNow;
        }

        public float PoisonTimer()
        {
            var timespan = DateTime.UtcNow - LastPoisoned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.PoisonCd) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Interact(Player, player);

            if (interact[3] && !player.IsProtected() && !player.IsVesting() && !player.IsProtectedMonarch())
                PoisonedPlayer = player;
            else if (interact[0])
                LastPoisoned = DateTime.UtcNow;
            else if (interact[1] || player.IsProtected())
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (player.IsVesting())
                LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = PoisonedPlayer == null && HoldsDrive;
            GlobalPoisonButton.Update(flag ? "SET TARGET" : "POISON", PoisonTimer(), CustomGameOptions.PoisonCd, Poisoned, TimeRemaining, CustomGameOptions.PoisonDuration, true,
                HoldsDrive);
            PoisonButton.Update("POISON", PoisonTimer(), CustomGameOptions.PoisonCd, Poisoned, TimeRemaining, CustomGameOptions.PoisonDuration, true, !HoldsDrive);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (PoisonedPlayer != null && HoldsDrive && !Poisoned)
                    PoisonedPlayer = null;

                LogSomething("Removed a target");
            }
        }

        public bool Exception1(PlayerControl player) => player == PoisonedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public void HitPoison()
        {
            if (PoisonTimer() != 0f || Poisoned || HoldsDrive || IsTooFar(Player, PoisonButton.TargetPlayer))
                return;

            var interact = Interact(Player, PoisonButton.TargetPlayer);

            if (interact[3] && !PoisonButton.TargetPlayer.IsProtected() && !PoisonButton.TargetPlayer.IsVesting() && !PoisonButton.TargetPlayer.IsProtectedMonarch())
            {
                PoisonedPlayer = PoisonButton.TargetPlayer;
                CallRpc(CustomRPC.Action, ActionsRPC.Poison, this, PoisonedPlayer);
                TimeRemaining = CustomGameOptions.PoisonDuration;
                Poison();
            }
            else if (interact[1] || PoisonButton.TargetPlayer.IsProtected())
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[0])
                LastPoisoned = DateTime.UtcNow;
            else if (interact[2])
                LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void HitGlobalPoison()
        {
            if (PoisonTimer() != 0f || Poisoned || !HoldsDrive)
                return;

            if (PoisonedPlayer == null)
                PoisonMenu.Open();
            else
            {
                CallRpc(CustomRPC.Action, ActionsRPC.Poison, this, PoisonedPlayer);
                TimeRemaining = CustomGameOptions.PoisonDuration;
                Poison();
            }
        }
    }
}