using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DuckGame;


namespace MyMod.src
{
    public abstract class GrenadeBase : Gun
    {
        public StateBinding TimerBinding {
            get;
            set;
        }
        public StateBinding HasPinBinding
        {
            get;
            set;
        }
        public bool HasPin
        {
            get
            {
                return hasPin;
            }
            set
            {
                if(HasPin && !value)
                {
                    CreatePinParticle();
                }

                hasPin = value;
            }
        }
        public float Timer
        {
            get;
            set;
        }
        public bool HasExploded {
            get;
            protected set;
        }
        //Not sure what sets this but it was on the normal grenade
        public bool pullOnImpact
        {
            get;
            set;
        }
        protected SpriteMap sprite;
        bool hasPin;

        public GrenadeBase(float xval, float yval) : base(xval, yval)
        {
            TimerBinding = new StateBinding("Timer", -1, false);
            HasPinBinding = new StateBinding("HasPin", -1, false);
            HasPin = true;
            Timer = 1.2f;

            _editorName = "Grenade base";
            _bio = "You should not see this item ingame.";
            _type = "gun";
            bouncy = 0.4f;
            friction = 0.05f;
            ammo = 1;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Create a flash that lasts for a second
        /// </summary>
        public virtual void QuickFlash()
        {
            Graphics.flashAdd = 1.3f;
            Layer.Game.darken = 1.3f;
        }

        /// <summary>
        /// Explode the grenade
        /// </summary>
        public virtual void Explode()
        {
            HasExploded = true;
        }

        /// <summary>
        /// Destroy windows in a radius
        /// </summary>
        /// <param name="radius">Radius</param>
        public virtual void DestroyWindowsInRadius(float radius)
        {
            foreach(Window window in Level.CheckCircleAll<Window>(position, radius))
            {
                if(Level.CheckLine<Block>(position, window.position, window) == null)
                {
                    window.Destroy(new DTImpact(this));
                }
            }
        }

        /// <summary>
        /// Create the pin particle
        /// </summary>
        protected virtual void CreatePinParticle()
        {
            GrenadePin grenadePin = new GrenadePin(x, y);
            grenadePin.hSpeed = -offDir * Rando.Float(1.5f, 2f);
            grenadePin.vSpeed = -2f;
            Level.Add(grenadePin);

            SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
        }

        protected virtual void UpdateTimer()
        {
            if(!HasPin)
            {
                if(Timer > 0)
                {
                    Timer -= 0.01f;
                }
                else
                {
                    if(!HasExploded)
                    {
                        Explode();
                    }
                }
            }
        }

        protected virtual void UpdateFrame()
        {
            //If grenade has pin, then frame 0, else frame 1
            sprite.frame = HasPin ? 0 : 1;
        }

        /// <summary>
        /// Called all the time
        /// </summary>
        public override void Update()
        {
            base.Update();
            
            UpdateTimer();
            UpdateFrame();
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if(pullOnImpact)
            {
                OnPressAction();
            }

            base.OnSolidImpact(with, from);
        }

        /// <summary>
        /// Called when a duck uses the item
        /// </summary>
        public override void OnPressAction()
        {
            if(HasPin)
            {
                HasPin = false;
            }
        }
    }
}
