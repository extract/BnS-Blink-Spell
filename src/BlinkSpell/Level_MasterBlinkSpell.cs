using BS;
using System;
using UnityEngine;

namespace BlinkSpell
{
    public class Level_MasterBlinkSpell : LevelModule
    {
        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            initialized = true;
        }
        public override void Update(LevelDefinition levelDefinition)
        {
            if (!initialized) return;
            // Add code here.
        }
        public override void OnLevelUnloaded(LevelDefinition levelDefinition)
        {
            initialized = false;
        }
    }
}