using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.Classes;
using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    [HarmonyPatch]
    public class Modifier : PlayerLayer
    {
        public static readonly List<Modifier> AllModifiers = new();
        public static Modifier LocalModifier => GetModifier(PlayerControl.LocalPlayer);

        public Modifier(PlayerControl player) : base(player)
        {
            Color = Colors.Modifier;
            LayerType = PlayerLayerEnum.Modifier;
            AllModifiers.Add(this);
        }

        public string TaskText = "- None";
        public bool Hidden;

        public static Modifier GetModifier(PlayerControl player) => AllModifiers.Find(x => x.Player == player);

        public static T GetModifier<T>(PlayerControl player) where T : Modifier => GetModifier(player) as T;

        public static Modifier GetModifier(PlayerVoteArea area) => GetModifier(Utils.PlayerByVoteArea(area));

        public static List<Modifier> GetModifiers(ModifierEnum modifierType) => AllModifiers.Where(x => x.ModifierType == modifierType).ToList();

        public static List<T> GetModifiers<T>(ModifierEnum modifierType) where T : Modifier => GetModifiers(modifierType).Cast<T>().ToList();
    }
}