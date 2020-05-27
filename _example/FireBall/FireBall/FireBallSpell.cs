using BS;
using System;
using UMA.Examples;
using UMA.PoseTools;
using UnityEngine;

namespace FireBall
{
    public class FireBallSpell : LevelModule
    {
        private ItemData itemData;

        public string itemID = "FireBallSpellItem";

        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            initialized = true;
            itemData = Catalog.current.GetData<ItemData>(itemID, true);
        }
        
        public override void Update(LevelDefinition levelDefinition)
        {

            Player player = Player.local;
            
            if (!initialized || player == null || player.body == null)
            {
                return;
            }

            var rightHand = (player.body.handRight != null);
            var leftHand = (player.body.handLeft != null);
            
            if (rightHand && !leftHand) HandleCast(player.body.handRight, null);
            if (!rightHand && leftHand) HandleCast(player.body.handLeft, null);
            if (rightHand && leftHand) HandleCast(player.body.handRight, player.body.handLeft);

        }

        private void HandleCast(BodyHand hand, BodyHand secondaryHand)
        {
            // check if both hands are empty and close to eachother,
            // in that case use dualCasting.
            
            var mainHandCanCast = VerifyCanCast(hand);
            if (!mainHandCanCast) return;
            var secondaryHandCanCast = false;
            if (secondaryHand != null)
                secondaryHandCanCast = VerifyCanCast(secondaryHand);

            // Attempt dual casting, otherwise just cast normal spell
            if (secondaryHandCanCast && TryDualCastSpell(hand, secondaryHand)) 
                return;

            CastSpell(hand);
        }

        private bool TryDualCastSpell(BodyHand hand, BodyHand secondaryHand)
        {
            if (Vector3.Distance(hand.transform.position, secondaryHand.transform.position) > 0.5) return false;
            var controlHand = PlayerControl.GetHand(hand.side);
            var secondaryControlHand = PlayerControl.GetHand(secondaryHand.side);

            if (!(controlHand.gripPressed || secondaryControlHand.gripPressed)) return false; // Need to be creative here...
            
            if (!(controlHand.usePressed || secondaryControlHand.usePressed)) return false;
            

            var item = itemData.Spawn();
            item.gameObject.AddComponent<FireBallScript>();
            item.gameObject.GetComponent<FireBallScript>().isDualCast = true;
            var distance = 0f;
            item.transform.position = Vector3.Lerp(hand.transform.position, secondaryHand.transform.position, 0.5f) + Player.local.body.transform.forward * distance;
            item.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            hand.telekinesis.StartTargeting(item.GetMainHandle(hand.side));
            hand.telekinesis.TryCatch();
            secondaryHand.telekinesis.StartTargeting(item.GetMainHandle(hand.side));
            secondaryHand.telekinesis.TryCatch();

            item.gameObject.GetComponentInChildren<Collider>().isTrigger = false;
            return true;
        }

        private void CastSpell(BodyHand hand)
        {
            var controlHand = PlayerControl.GetHand(hand.side);
            if (!controlHand.usePressed) return;
            if (!controlHand.gripPressed) return; // Need to be creative here...

            var item = itemData.Spawn();
            item.gameObject.AddComponent<FireBallScript>();
            item.gameObject.GetComponent<FireBallScript>().castHand = hand;
            var distance = 0.5f;
            item.transform.position = hand.caster.transform.position - hand.transform.up * distance; // Spawn object distance meters infront of hand
            item.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            hand.telekinesis.StartTargeting(item.GetMainHandle(hand.side));
            hand.telekinesis.TryCatch();

            item.gameObject.GetComponentInChildren<Collider>().isTrigger = false;
            
        }

        private bool VerifyCanCast(BodyHand hand)
        {
            var caster = hand.caster;
            if (caster == null || caster.currentSpell == null)
            {
                return false;
            }

            if (caster.currentSpell.name != "FireBallSpell(Clone)")
            {
                GameObject.Destroy(caster.currentSpell.gameObject);
                return false;
            }

            if (hand.interactor.grabbedHandle != null) // or if only grabbed fireball
            {
                return false;
            }
            if (hand.telekinesis.catchedHandle != null)
            {
                return false;
            }
            return true;
        }

        public override void OnLevelUnloaded(LevelDefinition levelDefinition)
        {
            initialized = false;
        }
    }
}
