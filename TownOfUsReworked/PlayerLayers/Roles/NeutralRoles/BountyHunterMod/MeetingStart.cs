using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class StartMeetingPatch
    {
        private static int lettersGiven;
        private static bool lettersExhausted;
        private static readonly List<string> letters = new();

        public static void Prefix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.BountyHunter) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);
            var targetName = role.TargetPlayer.name;
            string something = "";

            if (!lettersExhausted)
            {
                var random = Random.RandomRangeInt(0, targetName.Length);
                var random2 = Random.RandomRangeInt(0, targetName.Length);
                var random3 = Random.RandomRangeInt(0, targetName.Length);

                if (lettersGiven <= targetName.Length - 3)
                {
                    while (random == random2 || random2 == random3 || random == random3 || letters.Contains($"{targetName[random]}") || letters.Contains($"{targetName[random2]}") ||
                        letters.Contains($"{targetName[random3]}"))
                    {
                        if (random == random2 || letters.Contains($"{targetName[random2]}"))
                            random2 = Random.RandomRangeInt(0, targetName.Length);

                        if (random2 == random3 || letters.Contains($"{targetName[random3]}"))
                            random3 = Random.RandomRangeInt(0, targetName.Length);

                        if (random == random3 || letters.Contains($"{targetName[random]}"))
                            random = Random.RandomRangeInt(0, targetName.Length);
                    }

                    something = $"Your target's name has the letters {targetName[random]}, {targetName[random2]} and {targetName[random3]} in it!";
                }
                else if (lettersGiven == targetName.Length - 2)
                {
                    while (random == random2 || letters.Contains($"{targetName[random]}") || letters.Contains($"{targetName[random2]}"))
                    {
                        if (letters.Contains($"{targetName[random2]}"))
                            random2 = Random.RandomRangeInt(0, targetName.Length);

                        if (letters.Contains($"{targetName[random]}"))
                            random = Random.RandomRangeInt(0, targetName.Length);

                        if (random == random2)
                            random = Random.RandomRangeInt(0, targetName.Length);
                    }

                    something = $"Your target's name has the letters {targetName[random]} and {targetName[random2]} in it!";
                }
                else if (lettersGiven == targetName.Length - 1)
                {
                    while (letters.Contains($"{targetName[random]}"))
                        random = Random.RandomRangeInt(0, targetName.Length);

                    something = $"Your target's name has the letter {targetName[random]} in it!";
                }
                else if (lettersGiven == targetName.Length)
                    lettersExhausted = true;

                if (!lettersExhausted)
                {
                    if (lettersGiven <= targetName.Length - 3)
                    {
                        letters.Add($"{targetName[random]}");
                        letters.Add($"{targetName[random2]}");
                        letters.Add($"{targetName[random3]}");
                        lettersGiven += 3;
                    }
                    else if (lettersGiven == targetName.Length - 2)
                    {
                        letters.Add($"{targetName[random]}");
                        letters.Add($"{targetName[random2]}");
                        lettersGiven += 2;
                    }
                    else if (lettersGiven == targetName.Length - 1)
                    {
                        letters.Add($"{targetName[random]}");
                        lettersGiven++;
                    }

                    role.LetterHintGiven = true;
                }
                else if (!role.ColorHintGiven)
                {
                    var colors = new Dictionary<int, string>
                    {
                        {0, "darker"},// red
                        {1, "darker"},// blue
                        {2, "darker"},// green
                        {3, "lighter"},// pink
                        {4, "lighter"},// orange
                        {5, "lighter"},// yellow
                        {6, "darker"},// black
                        {7, "lighter"},// white
                        {8, "darker"},// purple
                        {9, "darker"},// brown
                        {10, "lighter"},// cyan
                        {11, "lighter"},// lime
                        {12, "darker"},// maroon
                        {13, "lighter"},// rose
                        {14, "lighter"},// banana
                        {15, "darker"},// gray
                        {16, "darker"},// tan
                        {17, "lighter"},// coral
                        {18, "darker"},// watermelon
                        {19, "darker"},// chocolate
                        {20, "lighter"},// sky blue
                        {21, "lighter"},// beige
                        {22, "darker"},// magenta
                        {23, "lighter"},// turquoise
                        {24, "lighter"},// lilac
                        {25, "darker"},// olive
                        {26, "lighter"},// azure
                        {27, "darker"},// plum
                        {28, "darker"},// jungle
                        {29, "lighter"},// mint
                        {30, "lighter"},// chartreuse
                        {31, "darker"},// macau
                        {32, "darker"},// tawny
                        {33, "lighter"},// gold
                        {34, "lighter"},// panda
                        {35, "darker"},// contrast
                        {36, "lighter"},// chroma
                        {37, "darker"},// mantle
                        {38, "lighter"},// fire
                        {39, "lighter"},// galaxy
                        {40, "lighter"},// monochrome
                        {41, "lighter"},// rainbow
                    };

                    something = $"Your target is a {colors[role.TargetPlayer.CurrentOutfit.ColorId]} color!";
                    role.ColorHintGiven = true;
                }
            }

            if (string.IsNullOrEmpty(something))
                return;

            //Ensures only the Bounty Hunter sees this
            if (HudManager.Instance && something != "")
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, something);
        }
    }
}