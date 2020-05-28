﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using FaceGen = TaleWorlds.Core.FaceGen;

namespace EnhancedBattleTest
{
    public class MPCharacterConfigVM : CharacterConfigVM
    {
        private MPCharacterConfig _config = new MPCharacterConfig();
        private bool _isAttacker;

        public bool IsMultiplayer => true;
        public bool IsSinglePlayer => false;

        public CharacterViewModel Character { get; } = new CharacterViewModel(CharacterViewModel.StanceTypes.None);

        public SelectorVM<SelectorItemVM> FirstPerks { get; }
        public SelectorVM<SelectorItemVM> SecondPerks { get; }


        public TextVM IsHeroText { get; }
        public TextVM IsFemaleText { get; }
        public BoolVM IsHero { get; }
        public BoolVM IsFemale { get; }

        public MPCharacterConfigVM()
        {
            FirstPerks = new SelectorVM<SelectorItemVM>(0, null);
            SecondPerks = new SelectorVM<SelectorItemVM>(0, null);

            IsHeroText = new TextVM(GameTexts.FindText("str_ebt_is_hero"));
            IsFemaleText = new TextVM(GameTexts.FindText("str_ebt_is_female"));
            IsHero = new BoolVM(_config.IsHero);
            IsFemale = new BoolVM(_config.IsFemale);
            IsHero.OnValueChanged += b =>
            {
                _config.IsHero = b;
                SetCharacterToViewModel();
            };
            IsFemale.OnValueChanged += isFemale =>
            {
                _config.IsFemale = isFemale;
                SetCharacterToViewModel();
            };
        }

        public override void SetConfig(CharacterConfig config, bool isAttacker)
        {
            if (!(config is MPCharacterConfig mpConfig))
                return;
            _config = mpConfig;
            _isAttacker = isAttacker;
            FirstPerks.SelectedIndex = _config.SelectedFirstPerk;
            SecondPerks.SelectedIndex = _config.SelectedSecondPerk;
            IsHero.Value = _config.IsHero;
            IsFemale.Value = _config.IsFemale;
            SetCharacterToViewModel();
        }

        public override void SelectedCharacterChanged(Character character)
        {
            if (character == null)
                return;
            _config.CharacterId = character.StringId;
            _config.SelectedFirstPerk = 0;
            _config.SelectedSecondPerk = 0;
            _config.IsHero = IsHero.Value;
            _config.IsFemale = IsFemale.Value;
            SetPerks();
        }

        private void SetPerks()
        {
            if (!(_config.Character is MPCharacter mpCharacter))
                return;
            FirstPerks.Refresh(mpCharacter.HeroClass.GetAllAvailablePerksForListIndex(0).Select(perk => perk.Name), 0,
                vm =>
                {
                    if (vm.SelectedIndex < 0)
                        return;
                    _config.SelectedFirstPerk = vm.SelectedIndex;
                    SetCharacterToViewModel();
                });
            SecondPerks.Refresh(mpCharacter.HeroClass.GetAllAvailablePerksForListIndex(1).Select(perk => perk.Name), 0,
                vm =>
                {
                    if (vm.SelectedIndex < 0)
                        return;
                    _config.SelectedSecondPerk = vm.SelectedIndex;
                    SetCharacterToViewModel();
                });
        }

        private void SetCharacterToViewModel()
        {
            if (!(_config.Character is MPCharacter mpCharacter))
                return;
            var characterObject = _config.IsHero ? mpCharacter.HeroClass.HeroCharacter : mpCharacter.HeroClass.TroopCharacter;
            FillFrom(_isAttacker, characterObject);
        }

        private void FillFrom(bool isAttacker, BasicCharacterObject character, int seed = -1)
        {
            if (character.Culture != null)
            {
                Character.ArmorColor1 = Utility.ClothingColor1(character.Culture, isAttacker);
                Character.ArmorColor2 = Utility.ClothingColor2(character.Culture, isAttacker);
                Character.BannerCodeText = Utility.BannerFor(character.Culture, isAttacker).Serialize();
            }
            else
            {
                Character.ArmorColor1 = 0;
                Character.ArmorColor2 = 0;
                Character.BannerCodeText = "";
            }
            Character.CharStringId = character.StringId;
            Character.IsFemale = _config.IsFemale;
            var equipment = Utility.GetNewEquipmentsForPerks(_config.HeroClass, _config.IsHero,
                _config.SelectedFirstPerk, _config.SelectedSecondPerk, _config.IsHero);
            Character.EquipmentCode = equipment.CalculateEquipmentCode();
            Character.BodyProperties = null;    
            Character.BodyProperties = FaceGen.GetRandomBodyProperties(_config.IsFemale,
                character.GetBodyPropertiesMin(false), character.GetBodyPropertiesMax(),
                (int) equipment.HairCoverType, seed, character.HairTags, character.BeardTags,
                character.TattooTags).ToString();
            Character.MountCreationKey =
                MountCreationKey.GetRandomMountKey(equipment[10].Item, Common.GetDJB2(character.StringId));
        }
    }
}