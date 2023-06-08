namespace TownOfUsReworked.Data
{
    [HarmonyPatch]
    public static class Colors
    {
        //Layer Colors
        public static Color32 Role => new(255, 215, 0, 255); //#FFD700FF
        public static Color32 Modifier => new(128, 128, 128, 255); //#7F7F7FFF
        public static Color32 Ability => new(255, 153, 0, 255); //#FF9900FF
        public static Color32 Layer => new(38, 132, 193, 255); //#2684C1FF
        public static Color32 Objectifier => new(221, 88, 91, 255); //#DD585BFF
        public static Color32 Faction => new(0, 230, 109, 255); //#00E66DFF
        public static Color32 SubFaction => new(32, 77, 66, 255); //#204D42FF

        //Faction Colors
        public static Color32 Crew => new(140, 255, 255, 255); //#8CFFFFFF
        public static Color32 Neutral => new(179, 179, 179, 255); //#B3B3B3FF
        public static Color32 Intruder => new(255, 0, 0, 255); //#FF0000FF
        public static Color32 Syndicate => new(0, 128, 0, 255); //#008000FF
        public static Color32 Other => new(128, 128, 0, 255); //#808000FF

        //Subfaction Colors
        public static Color32 Undead => new(123, 137, 104, 255); //#7B8968FF
        public static Color32 Cabal => new(87, 86, 87, 255); //#575657FF
        public static Color32 Reanimated => new(230, 16, 138, 255); //#E6108AFF
        public static Color32 Sect => new(249, 149, 252, 255); //#F995FCFF
        public static Color32 Infector => new(167, 197, 150, 255); //#A7C596FF

        //Crew Colors
        public static Color32 Mayor => new(112, 79, 168, 255); //#704FA8FF
        public static Color32 Vigilante => new(255, 255, 0, 255); //#FFFF00FF
        public static Color32 Engineer => new(255, 166, 10, 255); //#FFA60AFF
        public static Color32 Medic => new(0, 102, 0, 255); //#006600FF
        public static Color32 Sheriff => new(255, 204, 128, 255); //#FFCC80FF
        public static Color32 Altruist => new(102, 0, 0, 255); //#660000FF
        public static Color32 Veteran => new(153, 128, 64, 255); //#998040FF
        public static Color32 Tracker => new(0, 153, 0, 255); //#009900FF
        public static Color32 Transporter => new(0, 238, 255, 255); //#00EEFFFF
        public static Color32 Medium => new(166, 128, 255, 255); //#A680FFFF
        public static Color32 Coroner => new(77, 153, 230, 255); //#4D99E6FF
        public static Color32 Operative => new(167, 209, 179, 255); //#A7D1B3FF
        public static Color32 Detective => new(77, 77, 255, 255); //#4D4DFFFF
        public static Color32 Shifter => new(223, 133, 31, 255); //#DF851FFF
        public static Color32 VampireHunter => new(192, 192, 192, 255); //#C0C0C0FF
        public static Color32 Escort => new(128, 51, 51, 255); //#803333FF
        public static Color32 Inspector => new(126, 60, 100, 255); //#7E3C64FF
        public static Color32 Revealer => new(211, 211, 211, 255); //#D3D3D3FF
        public static Color32 Mystic => new(112, 142, 239, 255); //#708EEFFF
        public static Color32 Retributionist => new(141, 15, 140, 255); //#8D0F8CFF
        public static Color32 Chameleon => new(84, 17, 248, 255); //#5411F8FF
        public static Color32 Seer => new(113, 54, 138, 255); //#71368AFF
        public static Color32 Monarch => new(255, 0, 78, 255); //#FF004EFF
        public static Color32 Dictator => new(0, 203, 151, 255); //#00CB97FF

        //Neutral Colors
        public static Color32 Jester => new(247, 179, 218, 255); //#F7B3DAFF
        public static Color32 Executioner => new(204, 204, 204, 255); //#CCCCCCFF
        public static Color32 Glitch => new(0, 255, 0, 255); //#00FF00FF
        public static Color32 Arsonist => new(238, 118, 0, 255); //#EE7600FF
        public static Color32 Amnesiac => new(34, 255, 255, 255); //#22FFFFFF
        public static Color32 Survivor => new(221, 221, 0, 255); //#DDDD00FF
        public static Color32 GuardianAngel => new(255, 255, 255, 255); //#FFFFFFFF
        public static Color32 Plaguebearer => new(207, 254, 97, 255); //#CFFE61FF
        public static Color32 Pestilence => new(66, 66, 66, 255); //#424242FF
        public static Color32 Werewolf => new(159, 112, 58, 255); //#9F703AFF
        public static Color32 Cannibal => new(140, 64, 5, 255); //#8C4005FF
        public static Color32 Juggernaut => new(161, 43, 86, 255); //#A12B56FF
        public static Color32 Dracula => new(172, 138, 0, 255); //#AC8A00FF
        public static Color32 Murderer => new(111, 123, 234, 255); //#6F7BEAFF
        public static Color32 SerialKiller => new(51, 110, 255, 255); //#336EFFFF
        public static Color32 Cryomaniac => new(100, 45, 234, 255); //#642DEAFF
        public static Color32 Thief => new(128, 255, 0, 255); //#80FF00FF
        public static Color32 Troll => new(103, 141, 54, 255); //#678D36FF
        //public static Color32 Pirate => new(237, 194, 64, 255); //#EDC240FF
        public static Color32 Jackal => new(69, 7, 106, 255); //#45076AFF
        public static Color32 Phantom => new(102, 41, 98, 255); //#662962FF
        public static Color32 Necromancer => new(191, 95, 255, 255); //#BF5FFFFF
        public static Color32 Whisperer => new(45, 106, 165, 255); //#2D6AA5FF
        public static Color32 Guesser => new(238, 229, 190, 255); //#EEE5BEFF
        public static Color32 Actor => new(0, 172, 194, 255); //#00ACC2FF
        public static Color32 BountyHunter => new(181, 30, 57, 255); //#B51E39FF
        public static Color32 Betrayer => new(17, 128, 106, 255); //#11806AFF

        //Intruder Colors
        public static Color32 Consigliere => new(255, 255, 153, 255); //#FFFF99FF
        public static Color32 Grenadier => new(133, 170, 91, 255); //#85AA5BFF
        public static Color32 Morphling => new(187, 69, 176, 255); //#BB45B0FF
        public static Color32 Wraith => new(92, 79, 117, 255); //#5C4F75FF
        public static Color32 Camouflager => new(55, 138, 192, 255); //#378AC0FF
        public static Color32 Janitor => new(38, 71, 162, 255); //#2647A2FF
        public static Color32 Miner => new(170, 118, 50, 255); //#AA7632FF
        public static Color32 Blackmailer => new(2, 167, 162, 255); //#02A752FF
        public static Color32 Disguiser => new(64, 180, 255, 255); //#40B4FFFF
        public static Color32 Consort => new(128, 23, 128, 255); //#801780FF
        public static Color32 Teleporter => new(147, 149, 147, 255); //#939593FF
        public static Color32 Godfather => new(64, 76, 8, 255); //#404C08FF
        public static Color32 Mafioso => new(100, 0, 255, 255); //#6400FFFF
        public static Color32 Ambusher => new(43, 210, 156, 255); //#2BD29CFF
        public static Color32 Ghoul => new(241, 196, 15, 255); //#F1C40FFF
        public static Color32 Enforcer => new(0, 86, 67, 255); //#005643FF

        //Syndicate Colors
        public static Color32 Warper => new(140, 113, 64, 255); //#8C7140FF
        public static Color32 Framer => new(0, 255, 255, 255); //#00FFFFFF
        public static Color32 Rebel => new(255, 252, 206, 255); //#FFFCCEFF
        public static Color32 Sidekick => new(151, 156, 159, 255); //#979C9FFF
        public static Color32 Concealer => new(192, 37, 37, 255); //#C02525FF
        public static Color32 Shapeshifter => new(45, 255, 0, 255); //#2DFF00FF
        public static Color32 Bomber => new(201, 204, 63, 255); //#C9CC3FFF
        public static Color32 Poisoner => new(181, 0, 76, 255); //#B5004CFF
        public static Color32 Crusader => new(223, 122, 232, 255); //#DF7AE8FF
        public static Color32 Banshee => new(230, 126, 34, 255); //#E67E22FF
        public static Color32 Collider => new(179, 69, 255, 255); //#B345FFFF
        public static Color32 Stalker => new(126, 77, 0, 255); //#7E4D00FF
        public static Color32 Spellslinger => new(0, 40, 245, 255); //#0028F5FF
        public static Color32 Drunkard => new(255, 121, 0, 255); //#FF7900FF
        public static Color32 TimeKeeper => new(55, 105, 254, 255); //#3769FEFF
        public static Color32 Silencer => new(170, 180, 62, 255); //#AAB43EFF

        //Modifier Colors
        public static Color32 Bait => new(0, 179, 179, 255); //#00B3B3FF
        public static Color32 Coward => new(69, 107, 168, 255); //#456BA8FF
        public static Color32 Diseased => new(55, 77, 30, 255); //#374D1EFF
        public static Color32 Drunk => new(117, 128, 0, 255); //#758000FF
        public static Color32 Dwarf => new(255, 128, 128, 255); //#FF8080FF
        public static Color32 Giant => new(255, 179, 77, 255); //#FFB34DFF
        public static Color32 Volatile => new(255, 166, 10, 255); //#FFA60AFF
        public static Color32 Flincher => new(128, 179, 255, 255); //#80B3FFFF
        public static Color32 VIP => new(220, 238, 133, 255); //#DCEE85FF
        public static Color32 Shy => new(16, 2, 197, 255); //#1002C5FF
        public static Color32 Professional => new(134, 11, 122, 255); //#860B7AFF
        public static Color32 Indomitable => new(45, 229, 190, 255); //#2DE5BEFF
        public static Color32 Astral => new(97, 43, 239, 255); //#612BEFFF
        public static Color32 Yeller => new(246, 170, 183, 255); //#F6AAB7FF

        //Ability Colors
        public static Color32 Assassin => new(7, 55, 99, 255); //#073763FF
        public static Color32 Torch => new(255, 255, 153, 255); //#FFFF99FF
        public static Color32 Tunneler => new(233, 30, 99, 255); //#E91E63FF
        public static Color32 ButtonBarry => new(230, 0, 255, 255); //#E600FFFF
        public static Color32 Tiebreaker => new(153, 230, 153, 255); //#99E699FF
        public static Color32 Snitch => new(212, 174, 55, 255); //#D4AF37FF
        public static Color32 Underdog => new(132, 26, 127, 255); //#841A7FFF
        public static Color32 Insider => new(38, 252, 251, 255); //#26FCFBFF
        public static Color32 Radar => new(255, 0, 128, 255); //#FF0080FF
        public static Color32 Multitasker => new(255, 128, 77, 255); //#FF804DFF
        public static Color32 Ruthless => new(33, 96, 221, 255); //#2160DDFF
        public static Color32 Ninja => new(168, 67, 0, 255); //#A84300FF
        public static Color32 Politician => new(204, 163, 204, 255); //#CCA3CCFF
        public static Color32 Swapper => new(102, 230, 102, 255); //#66E666FF

        //Objectifier Colors
        public static Color32 Lovers => new(255, 102, 204, 255); //#FF66CCFF
        public static Color32 Traitor => new(55, 13, 67, 255); //#370D43FF
        public static Color32 Rivals => new(61, 45, 44, 255); //#3D2D2CFF
        public static Color32 Fanatic => new(103, 141, 54, 255); //#678D36FF
        public static Color32 Taskmaster => new(171, 171, 255, 255); //#ABABFFFF
        public static Color32 Overlord => new(0, 128, 128, 255); //#008080FF
        public static Color32 Corrupted => new(69, 69, 255, 255); //#4545FFFF
        public static Color32 Allied => new(69, 69, 169, 255); //#4545A9FF
        public static Color32 Mafia => new(0, 238, 255, 255); //#00EEFFFF
        public static Color32 Defector => new(225, 200, 73, 255); //#E1C849FF

        //Other
        public static Color32 Stalemate => new(239, 230, 230, 255); //#E6E6E6FF
        public static Color32 Alignment => new(29, 124, 242, 255); //#1D7CF2FF
        public static Color32 Status => new(155, 89, 182, 255); //#9B59B6FF
        public static Color32 Clear => new(0, 0, 0, 0); //#00000000
        public static Color32 Objectives => new(177, 72, 226, 255); //#B148E2FF
        public static Color32 Attributes => new(236, 28, 69, 255); //#EC1C45FF
        public static Color32 Abilities => new(32, 102, 148, 255); //#206694FF
        public static Color32 What => new(102, 151, 255, 255); //#6697FFFF

        //Color Storage
        //#dcee85 #6c29ab #800000 #808000 #008000 #800080 #000080 #e74c3c #992d22 #00FFFD #917ac0 #Eac1d2 #286e58 #db4f20 #abd432 #2e3b97 #ffd100 #fffcce #40b4ff #a82626 #8ff731 #942b3b
        //#4e4e4e #fffead #1abc9c #2ecc71 #1f8b4c #3498db #ad1457 #c27c0e #ffd2fb #805bc4 #95a5a6 #979c9f #888888 #ff7272 #f25ff1 #FF00FF #916640 #1AFF74 #2672FF #8637C2 #e7dae2 #9B7038
        //#6a1515 #569d29 #f1612b #7d86e1 #EC62A5 #78c689 #fccc52 #6b2d2a #FCBA03 #ff351f #F8CD46 #FF4D00 #7EFBC2 #4d4d4d #38b553 #0000FF #0000A7 #f25e75 #5865F2 #0437EF #7FFF00 #FB9327
        //#1E300B #06DF0C #F3A6D3

        //Symbol Storage
        //⟡ ☆ ♡ ♧ ♤ ▶ ❥ ✔ Γ ι ν σ τ ψ Ψ ✧ ¢
    }
}