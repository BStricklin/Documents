using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    public class GurrenGrenade : GrenadeBase
    {
        public float xpos;
        public float ypos;
        public float rad;
        public float ang;
        public GurrenGrenade(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Gurren Grenade";
            _bio = "'My drills are the drills that will pierce all the Heavns' ~ Metal Kamina";
            sprite = new SpriteMap(GetPath("gurrenGrenade.png"), 16, 16, false);
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

            if (Network.isServer)
            {
                spawnDrills(8);
            }

            SFX.Play(GetPath("drillklang.wav"), 1f, 0.0f, 0.0f, false);

            Level.Remove(this);

            base.Explode();
        }

        public virtual void spawnDrills(int drills)
        {
            rad = 25;
            ang = (2*3.14159274f) / drills;
            for (int i = 0; i < drills; i++)
            {
                float speed = Rando.Float(3f, 5f);
                GurrenGrenadeDrill drill = new GurrenGrenadeDrill(0, 0);
                xpos = rad * Maths.FastCos(i * ang);
                ypos = rad * Maths.FastSin(i * ang);
                drill.position = Offset(new Vec2(xpos, ypos));
                drill.angle = ang;
                drill._hasFired = true;
                drill._readyForLaunch = true;
                drill._hasLaunched = true;
                drill._thrownAway = true;
                drill.setXspeed = 1.3f * Maths.FastCos(i * ang);
                drill.setYspeed = 2f * Maths.FastSin(i * ang);

                Level.Add(drill);
            }
        }
    }
}
