namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Ability : PlayerLayer
    {
        public static readonly List<Ability> AllAbilities = new();
        public static Ability LocalAbility => GetAbility(CustomPlayer.Local);

        public override Color32 Color => Colors.Ability;
        public override PlayerLayerEnum LayerType => PlayerLayerEnum.Ability;

        public virtual AbilityEnum AbilityType => AbilityEnum.None;
        public virtual Func<string> TaskText => () => "- None";

        public Ability(PlayerControl player) : base(player)
        {
            if (GetAbility(player))
                GetAbility(player).Player = null;

            AllAbilities.Add(this);
        }

        public bool Hidden;

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            GetLayers<Assassin>(LayerEnum.Assassin).ForEach(x => x.AssassinMenu.HideButtons());

            foreach (var swapper in GetAbilities<Swapper>(AbilityEnum.Swapper))
            {
                swapper.SwapMenu.HideButtons();
                swapper.Swap1 = null;
                swapper.Swap2 = null;
            }

            foreach (var pol in GetAbilities<Politician>(AbilityEnum.Politician))
            {
                pol.DestroyAbstain();
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

        public static Ability GetAbility(PlayerVoteArea area) => GetAbility(PlayerByVoteArea(area));

        public static List<Ability> GetAbilities(AbilityEnum abilitytype) => AllAbilities.Where(x => x.AbilityType == abilitytype).ToList();

        public static List<T> GetAbilities<T>(AbilityEnum abilitytype) where T : Ability => GetAbilities(abilitytype).Cast<T>().ToList();
    }
}