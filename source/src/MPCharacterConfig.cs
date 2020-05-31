﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace EnhancedBattleTest
{
    public class MPCharacterConfig : CharacterConfig
    {
        [XmlIgnore]
        private string _characterId;
        public int SelectedFirstPerk;
        public int SelectedSecondPerk;
        public bool IsHero;
        public float FemaleRatio;

        public string CharacterId
        {
            get => _characterId;
            set
            {
                _characterId = value;
                HeroClass = Game.Current.ObjectManager.GetObject<MultiplayerClassDivisions.MPHeroClass>(_characterId);
                Character = new MPCharacter(HeroClass,
                    new MPGroup(HeroClass.ClassGroup, HeroClass.HeroCharacter.CurrentFormationClass).Info);
            }
        }

        [XmlIgnore]
        public override Character Character { get; protected set; }

        [XmlIgnore]
        public override BasicCharacterObject CharacterObject => IsHero ? HeroClass.HeroCharacter : HeroClass.TroopCharacter;

        [XmlIgnore]
        public MultiplayerClassDivisions.MPHeroClass HeroClass;

        public override CharacterConfig Clone()
        {
            var result = new MPCharacterConfig();
            result.CopyFrom(this);
            return result;
        }

        public override void CopyFrom(CharacterConfig other)
        {
            var mpOther = other as MPCharacterConfig;
            if (mpOther == null)
                return;
            CharacterId = mpOther.CharacterId;
            SelectedFirstPerk = mpOther.SelectedFirstPerk;
            SelectedSecondPerk = mpOther.SelectedSecondPerk;
            IsHero = mpOther.IsHero;
            FemaleRatio = mpOther.FemaleRatio;
        }

        public MPCharacterConfig()
        {
            CharacterId = Game.Current.ObjectManager.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>().First().StringId;
        }

        public MPCharacterConfig(string id, float femaleRatio = 0)
        {
            CharacterId = id;
            FemaleRatio = femaleRatio;
        }

    }
}
