using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    [HarmonyPatch]
    public class Ability : PlayerLayer
    {
        public static readonly List<Ability> AllAbilities = new();

        public Ability(PlayerControl player) : base(player)
        {
            Color = Colors.Ability;
            LayerType = PlayerLayerEnum.Ability;
            AllAbilities.Add(this);
        }

        public string TaskText = "- None.";
        public bool Hidden;

        public static Ability GetAbility(PlayerControl player) => AllAbilities.Find(x => x.Player == player);

        public static T GetAbility<T>(PlayerControl player) where T : Ability => GetAbility(player) as T;

        public static Ability GetAbility(PlayerVoteArea area) => GetAbility(Utils.PlayerByVoteArea(area));

        public static List<Ability> GetAbilities(AbilityEnum abilitytype) => AllAbilities.Where(x => x.AbilityType == abilitytype).ToList();

        public static List<T> GetAbilities<T>(AbilityEnum abilitytype) where T : Ability => GetAbilities(abilitytype).Cast<T>().ToList();
    }
}