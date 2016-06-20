using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    class FireSword : Sword
    {
        private SpriteMap _swordSwing;
        private SpriteMap _sprite;
        private bool isCharging;
        public float currentCharge;
        public float chargeTime = 100f;

        public FireSword(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this._bio = "Power of love obtained by confessing love for Ramona Flowers";
            this._sprite = new SpriteMap(GetPath("fireSword"), 8, 23, false);
            base.graphic = this._sprite;
            this.center = new Vec2(4f, 21f);
            this.collisionOffset = new Vec2(-2f, -16f);
            this.collisionSize = new Vec2(4f, 18f);
            this._barrelOffsetTL = new Vec2(4f, 1f);
           
            this._fireSound = "smg";
            this._fullAuto = true;

            this._fireWait = 1f;
            this._kickForce = 6f;
            this._holdOffset = new Vec2(-4f, 4f);
            this.weight = 0.9f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._swordSwing = new SpriteMap(GetPath("swordSwiping"), 32, 32, false);
            this._swordSwing.AddAnimation("swing", 0.6f, false, new int[]
            {
                0,
                1,
                1,
                2
            });
            this._swordSwing.currentAnimation = "swing";
            this._swordSwing.speed = 0f;
            this._swordSwing.center = new Vec2(9f, 25f);
            this._bouncy = 0.5f;
            this._impactThreshold = 0.3f;
        }

        public override void OnPressAction()
        {
            this.isCharging = true;
            base.OnPressAction();
        }

        public override void Update()
        {
            if (this.isCharging)
            {
                this.currentCharge += 1f;
            }
            else
            {
                this.currentCharge = 0f;
            }
            if (this.currentCharge < 25f)
            {
                this._sprite.frame = 1;
            }
            else if (this.currentCharge < 50f)
            {
                this._sprite.frame = 2;
            }
            else if (this.currentCharge < 75f)
            {
                this._sprite.frame = 3;
            }
            else if (this.currentCharge < chargeTime)
            {
                this._sprite.frame = 4;
            }
            else
            {
                this._sprite.frame = 5;
            }
            base.Update();
        }

        public override void OnReleaseAction()
        {
            
            if (this.currentCharge > this.chargeTime)
            {
                this.isCharging = false;
                SFX.Play("netGunFire", 0.5f, -0.4f + Rando.Float(0.2f), 0f, false);
                base.ApplyKick();
                if (!this.receivingPress && base.isServerForObject)
                {
                    Vec2 vec = this.Offset(base.barrelOffset);
                    FlareGun temp = new FlareGun(0, 0);
                    Flare flare = new Flare(vec.x, vec.y, temp, 4);
                    Flare flare2 = new Flare(vec.x, vec.y + 6, temp, 4);
                    Flare flare3 = new Flare(vec.x, vec.y + 11, temp, 4);
                    base.Fondle(flare);
                    base.Fondle(flare2);
                    base.Fondle(flare3);
                    Vec2 vec2 = Maths.AngleToVec(base.barrelAngle + Rando.Float(-0.2f, 0.2f));
                    flare.hSpeed = vec2.x * 14f;
                    flare.vSpeed = vec2.y * 14f;
                    Vec2 vec3 = Maths.AngleToVec(base.barrelAngle + Rando.Float(-0.2f, 0.2f));
                    flare2.hSpeed = vec3.x * 14f;
                    flare2.vSpeed = vec3.y * 14f;
                    Vec2 vec4 = Maths.AngleToVec(base.barrelAngle + Rando.Float(-0.2f, 0.2f));
                    flare3.hSpeed = vec4.x * 14f;
                    flare3.vSpeed = vec4.y * 14f;

                    Level.Add(flare);
                    Level.Add(flare2);
                    Level.Add(flare3);
                    return;
                }
            }
            this.isCharging = false;
            base.OnReleaseAction();
        }

    }
}
