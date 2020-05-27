using BS;
using System;
using Action = BS.Action;

namespace FireBall
{
    class ActionBurn : Action
    {
        // Not used.
        public override void Init(Creature creature)
        {
            base.Init(creature);
            creature.brain.underSpellFire = true;
            
            creature.ClearActions();
            if (base.CanRun()) base.Run(); // If this means run as in sprint instead of run away I am disappointed.
            else
            {
                ActionAttackMelee actionAttackMelee = new ActionAttackMelee();
                creature.TryAction(actionAttackMelee); // trying to spread the fire to the player? idk
            }
            
        }
    }
}
