using BS;
using System;
using UnityEngine;

namespace BlinkSpell
{
    public class Spell_Blink : Spell
    {
        SpellCasterHand spellCasterHand;
        public override void Load(SpellData.Instance spellDataInstance, SpellCasterHand handCaster)
        {
            spellCasterHand = handCaster;   
            base.Load(spellDataInstance, handCaster);
        }

        public override void Fire(bool active)
        {
            // Just experimenting
            if (!spellCasterHand.caster.currentMidSpell)
                DoSpell();
            else
                AimSpell();
            //base.Fire(active);
        }

        public override void Unload()
        {
            base.Unload();
        }

        public void Teleport()
        {
            //spellCasterHand.bodyHand.body.player
        }

        public void AimSpell()
        {

        }

        public void DoSpell()
        {

        }
    }
}