﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace EnhancedBattleTest
{
    public class MPAgentOrigin : EnhancedBattleTestAgentOrigin
    {
        public override BasicCharacterObject Troop => MPCharacter.Character;
        public override FormationClass FormationIndex => MPCharacter.FormationIndex;

        public MPSpawnableCharacter MPCharacter { get; }

        public MPAgentOrigin(MPCombatant combatant, MPSpawnableCharacter character, MPTroopSupplier troopSupplier, BattleSideEnum side, int rank = -1, UniqueTroopDescriptor uniqueNo = default)
            : base(combatant, troopSupplier, side, rank, uniqueNo)
        {
            MPCharacter = character;
        }
    }
}
