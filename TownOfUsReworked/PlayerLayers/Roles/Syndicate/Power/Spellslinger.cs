namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Spellslinger : Syndicate
    {
        public CustomButton SpellButton;
        public List<byte> Spelled = new();
        public DateTime LastSpelled;
        public int SpellCount;

        public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Spellslinger : Colors.Syndicate;
        public override string Name => "Spellslinger";
        public override LayerEnum Type => LayerEnum.Spellslinger;
        public override RoleEnum RoleType => RoleEnum.Spellslinger;
        public override Func<string> StartText => () => "Place the <color=#8CFFFFFF>Crew</color> Under A Curse";
        public override Func<string> AbilitiesText => () => $"- You can place a spell on players\n- When all non-{FactionColorString} players are spelled the game ends in a " +
            $"{FactionColorString} win{(HoldsDrive ? "\n- Your spells don't trigger interaction sensitive roles and your cooldown does not increase" : "")}\n{CommonAbilities}";
        public override InspectorResults InspectorResults => InspectorResults.SeeksToDestroy;

        public Spellslinger(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.SyndicatePower;
            Spelled = new();
            SpellCount = 0;
            SpellButton = new(this, "Spell", AbilityTypes.Direct, "Secondary", HitSpell, Exception1);
        }

        public float SpellTimer()
        {
            var timespan = DateTime.UtcNow - LastSpelled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SpellCooldown, SpellCount * CustomGameOptions.SpellCooldownIncrease) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        public void Spell(PlayerControl player)
        {
            if (player.Is(Faction) || Spelled.Contains(player.PlayerId))
                return;

            Spelled.Add(player.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.Spell, this, player);

            if (!HoldsDrive)
                SpellCount++;
            else
                SpellCount = 0;
        }

        public void HitSpell()
        {
            if (SpellTimer() != 0f || IsTooFar(Player, SpellButton.TargetPlayer))
                return;

            if (HoldsDrive)
            {
                Spell(SpellButton.TargetPlayer);
                LastSpelled = DateTime.UtcNow;
            }
            else
            {
                var interact = Interact(Player, SpellButton.TargetPlayer);

                if (interact[3])
                    Spell(SpellButton.TargetPlayer);

                if (interact[0])
                    LastSpelled = DateTime.UtcNow;
                else if (interact[1])
                    LastSpelled.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
        }

        public bool Exception1(PlayerControl player) => Spelled.Contains(player.PlayerId) || player.Is(Faction);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SpellButton.Update("SPELL", SpellTimer(), CustomGameOptions.SpellCooldown, SpellCount * CustomGameOptions.SpellCooldownIncrease);
        }
    }
}