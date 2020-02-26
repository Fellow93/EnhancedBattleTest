﻿using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace EnhancedBattleTest
{
    class SwitchTeamLogic : MissionLogic
    {
        public delegate void SwitchTeamDelegate();
        
        public event SwitchTeamDelegate PreSwitchTeam;
        public event SwitchTeamDelegate PostSwitchTeam;
        

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            
            if (this.Mission.InputManager.IsKeyPressed(InputKey.Numpad5))
                this.SwapTeam();
        }

        public void SwapTeam()
        {
            var targetAgent = !Utility.IsAgentDead(this.Mission.PlayerEnemyTeam.PlayerOrderController.Owner)
                ? this.Mission.PlayerEnemyTeam.PlayerOrderController.Owner
                : this.Mission.PlayerEnemyTeam.Leader;
            if (targetAgent == null)
                return;
            if (!Utility.IsPlayerDead()) // MainAgent may be null because of free camera mode.
            {
                this.Mission.MainAgent.Controller = Agent.ControllerType.AI;
                this.Mission.MainAgent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
            }

            PreSwitchTeam?.Invoke();
            this.Mission.PlayerTeam = this.Mission.PlayerEnemyTeam;
            targetAgent.Controller = Agent.ControllerType.Player;
            PostSwitchTeam?.Invoke();
        }
    }
}