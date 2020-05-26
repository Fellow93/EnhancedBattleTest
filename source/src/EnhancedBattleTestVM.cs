﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace EnhancedBattleTest
{
    public class EnhancedBattleTestVM : ViewModel
    {
        private EnhancedBattleTestState _state;
        private BattleConfig _config;
        private List<SceneData> _scenes;
        private List<BasicCultureObject> _factionList;
        private List<BasicCharacterObject> _characterList;

        private MBBindingList<CustomBattleSiegeMachineVM> _attackerMeleeMachines;
        private MBBindingList<CustomBattleSiegeMachineVM> _attackerRangedMachines;
        private MBBindingList<CustomBattleSiegeMachineVM> _defenderMachines;

        private bool _isAttackerCustomMachineSelectionEnabled;
        private bool _isDefenderCustomMachineSelectionEnabled;

        public TextVM TitleText { get; }

        public SideVM PlayerSide { get; }
        public SideVM EnemySide { get; }
        public MapSelectionGroup MapSelectionGroup { get; }
        public BattleTypeSelectionGroup BattleTypeSelectionGroup { get; }

        [DataSourceProperty]
        public MBBindingList<CustomBattleSiegeMachineVM> AttackerMeleeMachines
        {
            get => this._attackerMeleeMachines;
            set
            {
                if (value == this._attackerMeleeMachines)
                    return;
                this._attackerMeleeMachines = value;
                this.OnPropertyChanged(nameof(AttackerMeleeMachines));
            }
        }

        [DataSourceProperty]
        public MBBindingList<CustomBattleSiegeMachineVM> AttackerRangedMachines
        {
            get => this._attackerRangedMachines;
            set
            {
                if (value == this._attackerRangedMachines)
                    return;
                this._attackerRangedMachines = value;
                this.OnPropertyChanged(nameof(AttackerRangedMachines));
            }
        }

        [DataSourceProperty]
        public MBBindingList<CustomBattleSiegeMachineVM> DefenderMachines
        {
            get => this._defenderMachines;
            set
            {
                if (value == this._defenderMachines)
                    return;
                this._defenderMachines = value;
                this.OnPropertyChanged(nameof(DefenderMachines));
            }
        }

        private IEnumerable<SiegeEngineType> GetAllDefenderRangedMachines()
        {
            yield return DefaultSiegeEngineTypes.Ballista;
            yield return DefaultSiegeEngineTypes.FireBallista;
            yield return DefaultSiegeEngineTypes.Catapult;
            yield return DefaultSiegeEngineTypes.FireCatapult;
        }

        private IEnumerable<SiegeEngineType> GetAllAttackerRangedMachines()
        {
            yield return DefaultSiegeEngineTypes.Ballista;
            yield return DefaultSiegeEngineTypes.FireBallista;
            yield return DefaultSiegeEngineTypes.Onager;
            yield return DefaultSiegeEngineTypes.FireOnager;
            yield return DefaultSiegeEngineTypes.Trebuchet;
        }

        private IEnumerable<SiegeEngineType> GetAllAttackerMeleeMachines()
        {
            yield return DefaultSiegeEngineTypes.Ram;
            yield return DefaultSiegeEngineTypes.SiegeTower;
        }

        private static SiegeEngineType GetSiegeWeaponType(SiegeEngineType siegeWeaponType)
        {
            if (siegeWeaponType == DefaultSiegeEngineTypes.Ladder)
                return DefaultSiegeEngineTypes.Ladder;
            if (siegeWeaponType == DefaultSiegeEngineTypes.Ballista)
                return DefaultSiegeEngineTypes.Ballista;
            if (siegeWeaponType == DefaultSiegeEngineTypes.FireBallista)
                return DefaultSiegeEngineTypes.FireBallista;
            if (siegeWeaponType == DefaultSiegeEngineTypes.Ram || siegeWeaponType == DefaultSiegeEngineTypes.ImprovedRam)
                return DefaultSiegeEngineTypes.Ram;
            if (siegeWeaponType == DefaultSiegeEngineTypes.SiegeTower)
                return DefaultSiegeEngineTypes.SiegeTower;
            if (siegeWeaponType == DefaultSiegeEngineTypes.Onager || siegeWeaponType == DefaultSiegeEngineTypes.Catapult)
                return DefaultSiegeEngineTypes.Onager;
            if (siegeWeaponType == DefaultSiegeEngineTypes.FireOnager || siegeWeaponType == DefaultSiegeEngineTypes.FireCatapult)
                return DefaultSiegeEngineTypes.FireOnager;
            return siegeWeaponType == DefaultSiegeEngineTypes.Trebuchet || siegeWeaponType == DefaultSiegeEngineTypes.Bricole ? DefaultSiegeEngineTypes.Trebuchet : siegeWeaponType;
        }

        public EnhancedBattleTestVM(EnhancedBattleTestState state, TextObject title)
        {
            _state = state;
            _config = new BattleConfig();
            _scenes = _state.Scenes;
            _factionList = Game.Current.ObjectManager.GetObjectTypeList<BasicCultureObject>().ToList();
            _scenes = _state.Scenes;
            _characterList = Game.Current.ObjectManager.GetObjectTypeList<BasicCharacterObject>().ToList();

            TitleText = new TextVM(title);


            PlayerSide = new SideVM(_config.PlayerTeamConfig, new TextObject("{=BC7n6qxk}PLAYER"));
            EnemySide = new SideVM(_config.EnemyTeamConfig, new TextObject("{=35IHscBa}ENEMY"));

            MapSelectionGroup = new MapSelectionGroup("",
                _scenes.Select(sceneData =>
                    new MapSelectionElement(sceneData.Name.ToString(), sceneData.IsSiegeMap,
                        sceneData.IsVillageMap)).ToList());
            BattleTypeSelectionGroup = new BattleTypeSelectionGroup(_config.BattleTypeConfig, MapSelectionGroup, OnPlayerTypeChange);

            InitializeSiegeMachines();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            TitleText.RefreshValues();
            PlayerSide.RefreshValues();
            EnemySide.RefreshValues();
            BattleTypeSelectionGroup.RefreshValues();
            MapSelectionGroup.RefreshValues();
        }


        public void SetActiveState(bool isActive)
        {

        }

        public void ExecuteBack()
        {
            _config = null;
            Game.Current.GameStateManager.PopState();
        }
        private void OnPlayerTypeChange(bool isCommander)
        {

        }

        private void InitializeSiegeMachines()
        {
            AttackerMeleeMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
            for (int index = 0; index < 3; ++index)
                this.AttackerMeleeMachines.Add(new CustomBattleSiegeMachineVM((SiegeEngineType)null, new Action<CustomBattleSiegeMachineVM>(this.OnMeleeMachineSelection)));
            this.AttackerRangedMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
            for (int index = 0; index < 4; ++index)
                this.AttackerRangedMachines.Add(new CustomBattleSiegeMachineVM((SiegeEngineType)null, new Action<CustomBattleSiegeMachineVM>(this.OnAttackerRangedMachineSelection)));
            this.DefenderMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
            for (int index = 0; index < 4; ++index)
                this.DefenderMachines.Add(new CustomBattleSiegeMachineVM((SiegeEngineType)null, new Action<CustomBattleSiegeMachineVM>(this.OnDefenderRangedMachineSelection)));
        }

        private void ExecuteDoneDefenderCustomMachineSelection()
        {
            this.IsDefenderCustomMachineSelectionEnabled = false;
        }

        private void ExecuteDoneAttackerCustomMachineSelection()
        {
            this.IsAttackerCustomMachineSelectionEnabled = false;
        }

        [DataSourceProperty]
        public bool IsAttackerCustomMachineSelectionEnabled
        {
            get => this._isAttackerCustomMachineSelectionEnabled;
            set
            {
                if (value == this._isAttackerCustomMachineSelectionEnabled)
                    return;
                this._isAttackerCustomMachineSelectionEnabled = value;
                this.OnPropertyChanged(nameof(IsAttackerCustomMachineSelectionEnabled));
            }
        }

        [DataSourceProperty]
        public bool IsDefenderCustomMachineSelectionEnabled
        {
            get => this._isDefenderCustomMachineSelectionEnabled;
            set
            {
                if (value == this._isDefenderCustomMachineSelectionEnabled)
                    return;
                this._isDefenderCustomMachineSelectionEnabled = value;
                this.OnPropertyChanged(nameof(IsDefenderCustomMachineSelectionEnabled));
            }
        }

        private void OnMeleeMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
        {
            List<InquiryElement> inquiryElements = new List<InquiryElement>();
            inquiryElements.Add(new InquiryElement((object)null, "Empty", (ImageIdentifier)null));
            foreach (SiegeEngineType attackerMeleeMachine in this.GetAllAttackerMeleeMachines())
                inquiryElements.Add(new InquiryElement((object)attackerMeleeMachine, attackerMeleeMachine.Name.ToString(), (ImageIdentifier)null));
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=MVOWsP48}Select a Melee Machine", (Dictionary<string, TextObject>)null).ToString(), string.Empty, inquiryElements, false, true, GameTexts.FindText("str_done", (string)null).ToString(), "", (Action<List<InquiryElement>>)(selectedElements => selectedSlot.SetMachineType(selectedElements.First<InquiryElement>().Identifier as SiegeEngineType)), (Action<List<InquiryElement>>)null, ""), false);
        }

        private void OnAttackerRangedMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
        {
            List<InquiryElement> inquiryElements = new List<InquiryElement>();
            inquiryElements.Add(new InquiryElement((object)null, "Empty", (ImageIdentifier)null));
            foreach (SiegeEngineType attackerRangedMachine in this.GetAllAttackerRangedMachines())
                inquiryElements.Add(new InquiryElement((object)attackerRangedMachine, attackerRangedMachine.Name.ToString(), (ImageIdentifier)null));
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=SLZzfNPr}Select a Ranged Machine", (Dictionary<string, TextObject>)null).ToString(), string.Empty, inquiryElements, false, true, GameTexts.FindText("str_done", (string)null).ToString(), "", (Action<List<InquiryElement>>)(selectedElements => selectedSlot.SetMachineType(selectedElements[0].Identifier as SiegeEngineType)), (Action<List<InquiryElement>>)null, ""), false);
        }

        private void OnDefenderRangedMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
        {
            List<InquiryElement> inquiryElements = new List<InquiryElement>();
            inquiryElements.Add(new InquiryElement((object)null, "Empty", (ImageIdentifier)null));
            foreach (SiegeEngineType defenderRangedMachine in this.GetAllDefenderRangedMachines())
                inquiryElements.Add(new InquiryElement((object)defenderRangedMachine, defenderRangedMachine.Name.ToString(), (ImageIdentifier)null));
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=SLZzfNPr}Select a Ranged Machine", (Dictionary<string, TextObject>)null).ToString(), string.Empty, inquiryElements, false, true, GameTexts.FindText("str_done", (string)null).ToString(), "", (Action<List<InquiryElement>>)(selectedElements => selectedSlot.SetMachineType(selectedElements[0].Identifier as SiegeEngineType)), (Action<List<InquiryElement>>)null, ""), false);
        }
    }
}
