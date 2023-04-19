using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Random = UnityEngine.Random;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class BountyHunter : NeutralRole
    {
        public PlayerControl TargetPlayer;
        public PlayerControl ClosestPlayer;
        public bool TargetKilled;
        public bool ColorHintGiven;
        public bool LetterHintGiven;
        public bool TargetFound;
        public DateTime LastChecked;
        public CustomButton GuessButton;
        public CustomButton HuntButton;
        public bool ButtonUsable => UsesLeft > 0;
        public bool Failed => (UsesLeft <= 0 && !TargetFound) || (!TargetKilled && (TargetPlayer?.Data.IsDead == true || TargetPlayer?.Data.Disconnected == true));
        public int UsesLeft;
        private static int lettersGiven;
        private static bool lettersExhausted;
        private static readonly List<string> letters = new();

        public BountyHunter(PlayerControl player) : base(player)
        {
            Name = "Bounty Hunter";
            StartText = "Find And Kill Your Target";
            Objectives = "- Find and kill your target";
            AbilitiesText = "- You can guess a player to be your bounty\n- Upon finding the bounty, you can kill them\n- After your bounty has been killed by you, you can kill others as " +
                "many times as you want\n- If your target dies not by your hands, you will become a <color=#678D36FF>Troll</color>";
            Color = CustomGameOptions.CustomNeutColors ? Colors.BountyHunter : Colors.Neutral;
            RoleType = RoleEnum.BountyHunter;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            UsesLeft = CustomGameOptions.BountyHunterGuesses;
            Type = LayerEnum.BountyHunter;
            TargetPlayer = null;
            GuessButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", Guess, true);
            HuntButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", Hunt);
        }

        public float CheckTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastChecked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BountyHunterCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnTroll()
        {
            var newRole = new Troll(Player);
            newRole.RoleUpdate(this);

            if (Player == PlayerControl.LocalPlayer)
                Utils.Flash(Colors.Troll);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            var targetName = TargetPlayer.name;
            var something = "";

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

                    LetterHintGiven = true;
                }
                else if (!ColorHintGiven)
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

                    something = $"Your target is a {colors[TargetPlayer.CurrentOutfit.ColorId]} color!";
                    ColorHintGiven = true;
                }
            }

            if (string.IsNullOrEmpty(something))
                return;

            //Ensures only the Bounty Hunter sees this
            if (HudManager.Instance && something != "")
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, something);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            GuessButton.Update("GUESS", CheckTimer(), CustomGameOptions.BountyHunterCooldown, UsesLeft, true, !TargetFound);
            HuntButton.Update("HUNT", CheckTimer(), CustomGameOptions.BountyHunterCooldown, true, TargetFound);

            if (Failed && !Player.Data.IsDead)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnTroll);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TurnTroll();
            }
        }

        public void Guess()
        {
            if (Utils.IsTooFar(Player, ClosestPlayer) || CheckTimer() != 0f)
                return;

            if (ClosestPlayer != TargetPlayer)
            {
                Utils.Flash(new Color32(255, 0, 0, 255));
                UsesLeft--;
            }
            else
            {
                TargetFound = true;
                Utils.Flash(new Color32(0, 255, 0, 255));
            }

            LastChecked = DateTime.UtcNow;
        }

        public void Hunt()
        {
            if (ClosestPlayer != TargetPlayer && !TargetKilled)
            {
                Utils.Flash(new Color32(255, 0, 0, 255));
                LastChecked = DateTime.UtcNow;
            }
            else if (ClosestPlayer == TargetPlayer && !TargetKilled)
            {
                var interact = Utils.Interact(Player, ClosestPlayer, true);

                if (!interact[3])
                    Utils.RpcMurderPlayer(Player, ClosestPlayer);

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.BountyHunterWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TargetKilled = true;
                LastChecked = DateTime.UtcNow;
            }
            else
            {
                var interact = Utils.Interact(Player, ClosestPlayer, true);

                if (interact[0] || interact[3])
                    LastChecked = DateTime.UtcNow;
                else if (interact[1])
                    LastChecked.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    LastChecked.AddSeconds(CustomGameOptions.VestKCReset);
            }
        }
    }
}