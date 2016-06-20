using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class GlobalExplosion : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position", 2147483647);

        private bool _created;

        private SpriteMap _sprite;

        private int _smokeFrame;

        private bool _smoked;

        public GlobalExplosion(float xpos, float ypos) : base(xpos, ypos, null)
        {
            this._sprite = new SpriteMap("explosion", 64, 64, false);
            this._sprite.AddAnimation("explode", 1f, false, new int[]
            {
                0,
                0,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10
            });
            this._sprite.SetAnimation("explode");
            base.graphic = this._sprite;
            this._sprite.speed = 0.4f + Rando.Float(0.2f);
            base.xscale = 0.5f + Rando.Float(0.5f);
            base.yscale = base.xscale;
            this.center = new Vec2(32f, 32f);
            this._smokeFrame = Rando.Int(1, 3);
            base.depth = 1f;
            this.vSpeed = Rando.Float(-0.2f, -0.4f);
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            if (!this._created)
            {
                this._created = true;
            }
            if (this._sprite.frame > this._smokeFrame && !this._smoked)
            {
                int num = (Graphics.effectsLevel == 2) ? Rando.Int(1, 4) : 1;
                for (int i = 0; i < num; i++)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(base.x + Rando.Float(-5f, 5f), base.y + Rando.Float(-5f, 5f));
                    smallSmoke.vSpeed = Rando.Float(0f, -0.5f);
                    smallSmoke.xscale = (smallSmoke.yscale = Rando.Float(0.2f, 0.7f));
                    Level.Add(smallSmoke);
                }
                this._smoked = true;
            }
            base.y += (float)((double)this.vSpeed);
            if (!this._sprite.finished)
            {
                return;
            }
            Level.Remove(this);
        }
    }
}
