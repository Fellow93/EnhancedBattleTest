﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace EnhancedBattleTest
{
    public class EnhancedCustomBattleTroopSupplier : IMissionTroopSupplier
    {
        private bool _anyTroopRemainsToBeSupplied = true;
        private readonly EnhancedCustomBattleCombatant _customBattleCombatant;
        private PriorityQueue<float, BasicCharacterObject> _characters;
        private int _numAllocated;
        private int _numWounded;
        private int _numKilled;
        private int _numRouted;
        private readonly bool _isPlayerSide;

        public EnhancedCustomBattleTroopSupplier(EnhancedCustomBattleCombatant customBattleCombatant, bool isPlayerSide)
        {
            this._customBattleCombatant = customBattleCombatant;
            this._isPlayerSide = isPlayerSide;
            this.ArrangePriorities();
        }

        private void ArrangePriorities()
        {
            this._characters = new PriorityQueue<float, BasicCharacterObject>((IComparer<float>)new GenericComparer<float>());
            int[] numArray = new int[8];
            for (int i = 0; i < 8; i++)
                numArray[i] = this._customBattleCombatant.Characters.Count<BasicCharacterObject>((Func<BasicCharacterObject, bool>)(character => character.CurrentFormationClass == (FormationClass)i));
            int num = 1000;
            foreach (BasicCharacterObject character in this._customBattleCombatant.Characters)
            {
                FormationClass currentFormationClass = character.CurrentFormationClass;
                this._characters.Enqueue(character.IsHero ? (float)num-- : (float)(numArray[(int)currentFormationClass] / ((IEnumerable<int>)numArray).Sum()), character);
                --numArray[(int)currentFormationClass];
            }
        }

        public IEnumerable<IAgentOriginBase> SupplyTroops(
          int numberToAllocate)
        {
            List<BasicCharacterObject> basicCharacterObjectList = this.AllocateTroops(numberToAllocate);
            EnhancedCustomBattleAgentOrigin[] battleAgentOriginArray = new EnhancedCustomBattleAgentOrigin[basicCharacterObjectList.Count];
            this._numAllocated += basicCharacterObjectList.Count;
            for (int rank = 0; rank < battleAgentOriginArray.Length; ++rank)
            {
                UniqueTroopDescriptor uniqueNo = new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed);
                battleAgentOriginArray[rank] = new EnhancedCustomBattleAgentOrigin(this._customBattleCombatant, basicCharacterObjectList[rank], this, this._isPlayerSide, rank, uniqueNo);
            }
            if (battleAgentOriginArray.Length < numberToAllocate)
                this._anyTroopRemainsToBeSupplied = false;
            return (IEnumerable<IAgentOriginBase>)battleAgentOriginArray;
        }

        private List<BasicCharacterObject> AllocateTroops(int numberToAllocate)
        {
            if (numberToAllocate > this._characters.Count)
                numberToAllocate = this._characters.Count;
            List<BasicCharacterObject> basicCharacterObjectList = new List<BasicCharacterObject>();
            for (int index = 0; index < numberToAllocate; ++index)
                basicCharacterObjectList.Add(this._characters.DequeueValue());
            return basicCharacterObjectList;
        }

        public void OnTroopWounded()
        {
            ++this._numWounded;
        }

        public void OnTroopKilled()
        {
            ++this._numKilled;
        }

        public void OnTroopRouted()
        {
            ++this._numRouted;
        }

        public int NumActiveTroops
        {
            get
            {
                return this._numAllocated - (this._numWounded + this._numKilled + this._numRouted);
            }
        }

        public int NumRemovedTroops
        {
            get
            {
                return this._numWounded + this._numKilled + this._numRouted;
            }
        }

        public bool AnyTroopRemainsToBeSupplied
        {
            get
            {
                return this._anyTroopRemainsToBeSupplied;
            }
        }
    }
}
