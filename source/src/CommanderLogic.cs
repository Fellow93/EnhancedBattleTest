﻿using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.MountAndBlade;

namespace EnhancedBattleTest
{
    class CommanderLogic : MissionLogic
    {
        protected override void OnAgentControllerChanged(Agent agent)
        {
            if (agent == this.Mission.MainAgent)
            {
                if (agent.Controller == Agent.ControllerType.Player)
                    Utility.SetPlayerAsCommander();
                else
                    Utility.CancelPlayerCommander();
            }
        }
    }
}
