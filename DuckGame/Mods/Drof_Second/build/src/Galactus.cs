using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    [BaggedProperty("isSuperWeapon", true)]
    class Galactus : Gun
    {

        private float _timer = 1.2f;

        public bool _pin = true;

        private SpriteMap _sprite;

        public Galactus(float xpos, float ypos) : base(xpos, ypos)
		{
            this._editorName = "Galactus";
            this._bio = "You may not know it, but you're world already belongs to me";
            this.ammo = 1;
            this._ammoType = new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(base.GetPath("galactus"), 16, 16, false);
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
                    terraform();
                    Graphics.flashAdd = (1.3f);
                    Layer.Game.darken = (1.3f);
                    SFX.Play("littleGun", 1f, 0f, 0f, false);
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

        public void terraform()
        {

            foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(this.position, 500f))
            {
                if (!(materialThing is Duck))
                {
                    //materialThing.onFire = true;
                    materialThing.Destroy(new DTRocketExplosion(this));
                    materialThing.vSpeed = (-20f);
                }

            }
        }
    }
}
