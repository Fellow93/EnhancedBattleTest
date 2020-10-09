﻿using System.Xml.Serialization;
using EnhancedBattleTest.Data;
using EnhancedBattleTest.Multiplayer.Config;
using EnhancedBattleTest.SinglePlayer.Config;
using TaleWorlds.Core;

namespace EnhancedBattleTest.Config
{
    [XmlInclude(typeof(MPCharacterConfig))]
    [XmlInclude(typeof(SPCharacterConfig))]
    public abstract class CharacterConfig
    {
        [XmlIgnore]
        public abstract Character Character { get; protected set; }

        public abstract BasicCharacterObject CharacterObject { get; }

        public abstract CharacterConfig Clone();
        public abstract void CopyFrom(CharacterConfig other);

        public static CharacterConfig Create(bool isMultiplayer)
        {
            return isMultiplayer ? (CharacterConfig)new MPCharacterConfig() : new SPCharacterConfig();
        }

        public static CharacterConfig Create(bool isMultiplayer, string id, float femaleRatio = 0)
        {
            return isMultiplayer ? (CharacterConfig)new MPCharacterConfig(id, femaleRatio) : new SPCharacterConfig(id, femaleRatio);
        }
    }
}
