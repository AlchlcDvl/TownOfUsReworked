namespace TownOfUsReworked.PlayerLayers.Abilities
{
    [HarmonyPatch]
    public class Ability : PlayerLayer
    {
        public static readonly List<Ability> AllAbilities = new();
        public static Ability LocalAbility => GetAbility(PlayerControl.LocalPlayer);

        public Ability(PlayerControl player) : base(player)
        {
            if (GetAbility(player))
                GetAbility(player).Player = null;

            Color = Colors.Ability;
            LayerType = PlayerLayerEnum.Ability;
            AllAbilities.Add(this);
        }

        public Func<string> TaskText = () => "- None";
        public bool Hidden;

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);

            foreach (var assassin in GetAbilities<Assassin>(AbilityEnum.Assassin))
            {
                assassin.HideButtons();
                assassin.OtherButtons.Clear();
            }

            foreach (var swapper in GetAbilities<Swapper>(AbilityEnum.Swapper))
            {
                swapper.Actives.Clear();
                swapper.MoarButtons.Clear();
                swapper.Swap1 = null;
                swapper.Swap2 = null;
            }

            foreach (var pol in GetAbilities<Politician>(AbilityEnum.Politician))
            {
                pol.ExtraVotes.Clear();

                if (pol.VoteBank < 0)
                    pol.VoteBank = 0;

                pol.VotedOnce = false;

                if (!pol.CanKill)
                    pol.VoteBank++;
            }
        }

        public static Ability GetAbility(PlayerControl player) => AllAbilities.Find(x => x.Player == player);

        public static T GetAbility<T>(PlayerControl player) where T : Ability => GetAbility(player) as T;

        public static Ability GetAbility(PlayerVoteArea area) => GetAbility(Utils.PlayerByVoteArea(area));

        public static List<Ability> GetAbilities(AbilityEnum abilitytype) => AllAbilities.Where(x => x.AbilityType == abilitytype).ToList();

        public static List<T> GetAbilities<T>(AbilityEnum abilitytype) where T : Ability => GetAbilities(abilitytype).Cast<T>().ToList();
    }
}