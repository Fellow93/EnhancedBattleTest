﻿using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace EnhancedBattleTest
{
    public class EnhancedBattleTestPartyController
    {
        public static BattleConfig BattleConfig;

        public static MobileParty PlayerParty;
        public static MobileParty EnemyParty;


        public static void Initialize()
        {
            PlayerParty = MobileParty.Create("enhanced_battle_test_player_party");
            PlayerParty.Name = new TextObject("{=sSJSTe5p}Player Party");
            EnemyParty = MobileParty.Create("enemy_battle_test_enemy_party");
            EnemyParty.Name = new TextObject("{=0xC75dN6}Enemy Party");
        }

        public static void OnGameEnd()
        {
            PlayerParty.RemoveParty();
            PlayerParty = null;
            EnemyParty.RemoveParty();
            EnemyParty = null;
        }
    }
}
