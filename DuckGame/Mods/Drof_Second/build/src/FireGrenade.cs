using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DuckGame;


namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    public class FireGrenade : GrenadeBase
    {
        public FireGrenade(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Fire Grenade";
            _bio = "A standard grenade...but like with fire";
            sprite = new SpriteMap(GetPath("fireGrenade.png"), 16, 16, false);
            graphic = sprite;
            center = new Vec2(7f, 7f);
            collisionOffset = new Vec2(-4f, -5f);
            collisionSize = new Vec2(8f, 12f);
        }
        
        public override void Initialize()
        {
            base.Initialize();
        }
        
        /// <summary>
        /// Explode the grenade
        /// </summary>
        public override void Explode()
        {
            QuickFlash();
            
            if(Network.isServer)
            {
                DestroyWindowsInRadius(40f);
                FireExplosion(40);
            }
            
            SFX.Play(GetPath("fireGrenadeExplode.wav"), 1f, 0.0f, 0.0f, false);
            
            Level.Remove(this);
            
            base.Explode();
        }

        /// <summary>
        /// Create a explosion of fire
        /// </summary>
        /// <param name="fireAmount">Amount of fires</param>
        public virtual void FireExplosion(int fireAmount)
        {
            for(int i = 0; i < fireAmount; i++)
            {
                float speed = Rando.Float(3f, 5f);
                
                Level.Add(SmallFire.New(x, y - 2f, Rando.Float(-speed, speed), Rando.Float(-speed, speed + 2f), false, (MaterialThing)null, true, (Thing)this, false));
            }
        }
    }
}
