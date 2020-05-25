﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;

namespace EnhancedBattleTest
{
    public class MissionMenuView : MissionView
    {
        private MissionMenuVM _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;
        private BattleConfigBase _config;

        public bool IsActivated { get; set; }

        public MissionMenuView(BattleConfigBase config)
        {
            this._config = config;
            this.ViewOrderPriorty = 24;
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            this._gauntletLayer = null;
            this._dataSource?.OnFinalize();
            this._dataSource = null;
            this._movie = null;
            this._config = null;
        }

        public void ToggleMenu()
        {
            if (IsActivated)
                DeactivateMenu();
            else
                ActivateMenu();
        }

        public void ActivateMenu()
        {
            IsActivated = true;
            this._dataSource = new MissionMenuVM(_config, (side, tacticOption, isEnabled) =>
            {
                if (side == BattleSideEnum.None || side == BattleSideEnum.NumSides)
                    return false;
                if (_config.battleType == BattleType.SiegeBattle)
                {
                    Utility.DisplayLocalizedText("str_tactic_inchangable_in_siege");
                    return false;
                }
                var tacticArray = side == BattleSideEnum.Attacker
                    ? _config.attackerTacticOptions
                    : _config.defenderTacticOptions;
                int count = tacticArray.Sum(info =>
                    info.tacticOption == tacticOption ? (isEnabled ? 1 : 0) : info.isEnabled ? 1 : 0);
                if (count == 0)
                {
                    Utility.DisplayLocalizedText("str_at_least_one_tactic");
                    return false;
                }
                tacticArray.First(info => info.tacticOption == tacticOption).isEnabled = isEnabled;
                var team = side == BattleSideEnum.Attacker ? Mission.AttackerTeam : Mission.DefenderTeam;
                if (team == null)
                    return true;
                if (isEnabled)
                    TacticOptionHelper.AddTacticComponent(team, tacticOption, true);
                else
                    TacticOptionHelper.RemoveTacticComponent(team, tacticOption, true);
                return true;
            }, this.DeactivateMenu);
            this._gauntletLayer = new GauntletLayer(this.ViewOrderPriorty) { IsFocusLayer = true };
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();
            this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            this._movie = this._gauntletLayer.LoadMovie(nameof(MissionMenuView), _dataSource);
            this.MissionScreen.AddLayer(this._gauntletLayer);
            ScreenManager.TrySetFocus(this._gauntletLayer);
            PauseGame();
        }

        public void DeactivateMenu()
        {
            IsActivated = false;
            this._dataSource = null;
            this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
            this.MissionScreen.RemoveLayer(this._gauntletLayer);
            this._movie = null;
            this._gauntletLayer = null;
            UnpauseGame();
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (IsActivated)
            {
                if (this._gauntletLayer.Input.IsKeyReleased(InputKey.RightMouseButton) ||
                    this._gauntletLayer.Input.IsKeyReleased(InputKey.O) ||
                    this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
                    DeactivateMenu();
            }
            else if (this.Input.IsKeyReleased(InputKey.O))
                ActivateMenu();
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            base.OnAgentBuild(agent, banner);

            if (_config.changeCombatAI)
            {
                AgentStatModel.SetAgentAIStat(agent, agent.AgentDrivenProperties, _config.combatAI);
                agent.UpdateAgentProperties();
            }
        }

        private static bool _oldGameStatusDisabledStatus = false;

        private static void PauseGame()
        {
            MBCommon.PauseGameEngine();
            _oldGameStatusDisabledStatus = Game.Current.GameStateManager.ActiveStateDisabledByUser;
            Game.Current.GameStateManager.ActiveStateDisabledByUser = true;
        }

        private static void UnpauseGame()
        {
            MBCommon.UnPauseGameEngine();
            Game.Current.GameStateManager.ActiveStateDisabledByUser = _oldGameStatusDisabledStatus;
        }
    }
}
