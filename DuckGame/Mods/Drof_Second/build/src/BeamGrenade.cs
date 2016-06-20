using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    class BeamGrenade : Gun
    {
        private float _timer = 1.2f;

        public bool _pin = true;

        private SpriteMap _sprite;

        public BeamGrenade(float xpos, float ypos) : base(xpos, ypos)
		{
            this._editorName = "Beam Grenade";
            this._bio = "Hellzone Grenade";
            this.ammo = 1;
            this._ammoType = new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(base.GetPath("BeamGrenade"), 16, 16, false);
            base.graphic = (this._sprite);
            this.center = (new Vec2(7f, 8f));
            this.collisionOffset = (new Vec2(-4f, -5f));
            this.collisionSize = (new Vec2(8f, 10f));
            base.bouncy = (0.4f);
            this.friction = 0.05f;
        }

        public override void Update()
        {
            base.Update();
            if (!this._pin)
            {
                this._timer -= 0.01f;
                if ((double)this._timer < 0.4)
                {
                    Level.Add(new ExplosionPart(this.position.x, this.position.y - 2f, true));
                    Level.Add(new VBeam(new Vec2(this.position.x, this.position.y + 3000f), new Vec2(this.position.x, this.position.y - 3000f)));
                    Graphics.flashAdd = (1.3f);
                    Layer.Game.darken = (1.3f);
                    SFX.Play("laserBlast", 1f, 0f, 0f, false);
                    base.level.RemoveThing(this);
                }
            }
        }

        public override void OnPressAction()
        {
            if (this._pin)
            {
                this._sprite.frame = (1);
                GrenadePin Pin = new GrenadePin(this.position.x, this.position.y);
                SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
                Level.Add(Pin);
                this._pin = false;
            }
        }
    }
}
