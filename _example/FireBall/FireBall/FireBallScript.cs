using System;
using System.Collections.Generic;
using System.Timers;
using BS;
using UnityEngine;

namespace FireBall
{
    class FireBallScript : MonoBehaviour
    {
        //protected EffectBurn effectBurn;
        //protected EffectShock effectShock;
        protected Timer chargeTimer;
        protected List<ParticleSystem> particleSystems;
        protected Vector3 maxScale = new Vector3(1, 1, 1);
        protected Vector3 maxDualScale = new Vector3(2, 2, 2);
        public bool initialGrip = true;
        private bool bothPressed = true;
        public bool isDualCast = false;
        private double spellChargeTime = 2;
        public double spellDespawnTime = 5;
        private double currentTime = 0;
        private double grabTimeSpan = 0.5; // 0.5 seconds between two button presses to be taken as "similtaneus"
        private readonly int MILLISECONDS = 1000;

        public BodyHand castHand { get; internal set; }

        public void Awake()
        {
            this.gameObject.GetComponent<Item>().OnTeleGrabEvent += OnTeleGrab;
            this.gameObject.GetComponent<Item>().OnTeleUnGrabEvent += OnTeleUnGrab;
            //this.GetComponent<Item>().OnHeldActionEvent += OnHeldAction;
            particleSystems = new List<ParticleSystem>(gameObject.GetComponentsInChildren<ParticleSystem>());
            var t = new Timer(grabTimeSpan * MILLISECONDS);
            t.AutoReset = false;
            t.Elapsed += GrabTimeElapsed;
            t.Start();

            Debug.Log("Beast has awake");

            

        }
        protected void GrabTimeElapsed(object sender, ElapsedEventArgs e)
        {
            Debug.Log("GRAB TIME ELAPSED");
            // Check if hands are using this handle
            var bodyHandL = Player.local.body.handLeft;
            var bodyHandR = Player.local.body.handRight;
            var mainHandleL = gameObject.GetComponent<Item>().GetMainHandle(Side.Left);
            var mainHandleR = gameObject.GetComponent<Item>().GetMainHandle(Side.Right);
            Debug.Log("bHL: " + bodyHandL + ". mHL: " + mainHandleL);
            Debug.Log("bHR: " + bodyHandR + ". mHR: " + mainHandleR);
            if (!(bodyHandL.telekinesis.catchedHandle == mainHandleL && bodyHandR.telekinesis.catchedHandle == mainHandleR))
            {
                bothPressed = false;
                return;
            } 
            
            var controlHand = PlayerControl.GetHand(Side.Left);
            var secondaryControlHand = PlayerControl.GetHand(Side.Right);

            if (!(controlHand.usePressed && secondaryControlHand.usePressed)) bothPressed = false;
            Debug.Log("bothPressed = " + bothPressed);
        }
        public void LateUpdate()
        {
            var bodyHandL = Player.local.body.handLeft;
            var bodyHandR = Player.local.body.handRight;
            var mainHandleL = gameObject.GetComponent<Item>().GetMainHandle(Side.Left);
            var mainHandleR = gameObject.GetComponent<Item>().GetMainHandle(Side.Right);
            if (!(bodyHandL.telekinesis.catchedHandle == mainHandleL || bodyHandR.telekinesis.catchedHandle == mainHandleR))
                return;
            var bothHands = false;
            if((bodyHandL.telekinesis.catchedHandle == mainHandleL && bodyHandR.telekinesis.catchedHandle == mainHandleR))
            {
                bothHands = true;
            }
            Debug.Log("isDualCast=" + isDualCast + ". initialGrip=" + initialGrip);
            if (initialGrip && isDualCast && !bothHands)
            {
                initialGrip = false;
                
            }

            if (isDualCast && bothHands && initialGrip)
            {
                // Assume both are pressed until the timer expired.
                //if (!bothPressed) initialGrip = false;
                
                float size = Vector3.Distance(bodyHandL.transform.position, bodyHandR.transform.position);
                Debug.Log("bHL=" + bodyHandL.transform.position + ". bHR=" + bodyHandR.transform.position + ". distance=" + size);
                transform.localScale = size * Vector3.one;
                //float dist = (float)(Math.Sqrt(Vector3.Distance(bodyHands[0].transform.position, bodyHands[1].transform.position)) * Math.Sqrt(transform.localScale.magnitude) / 3f);
                transform.position = Vector3.Lerp(bodyHandL.transform.position, bodyHandR.transform.position, 0.5f);// + dist * -(bodyHands[0].transform.up.normalized + bodyHands[1].transform.up.normalized);
                return;
            }
            
            if (chargeTimer != null && chargeTimer.Enabled && initialGrip && !isDualCast)
            {
                currentTime += Time.deltaTime * MILLISECONDS; // To milliseconds
                
                
                if (currentTime < spellChargeTime * MILLISECONDS)
                {
                    transform.localScale = Vector3.Lerp(Vector3.zero, maxScale, (float)(currentTime / (spellChargeTime * MILLISECONDS)));
                    var distance = 0.5f;
                    transform.position = castHand.transform.position - castHand.transform.forward * distance;
                }
            }
        }

        protected void Charge()
        {
            //if (chargeTimer != null || chargeTimer.Enabled) return;
            //if (isFullCharge) return;
            chargeTimer = new Timer()
            {
                AutoReset = false,
                Interval = spellChargeTime * MILLISECONDS,
                Enabled = true
            };
            chargeTimer.Elapsed += FullCharge;
            
        }

        protected void FullCharge(object sender, ElapsedEventArgs e)
        {
            chargeTimer.Stop();
        }

        public void OnTeleGrab(Handle handle, Telekinesis teleGrabber)
        {
            Charge();
        }
        public void OnTeleUnGrab(Handle handle, Telekinesis teleGrabber)
        {

            gameObject.GetComponent<Item>().Despawn(5);
                
                
            if (chargeTimer == null) return;
            chargeTimer.Stop();
            chargeTimer.Interval = spellChargeTime * MILLISECONDS; // Reset timer.
            

        }

        public void OnHeldAction(Interactor interactor, Handle handle, Interactable.Action action)
        {
            // Could do something cool here.
        }
    }
}
