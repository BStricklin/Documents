using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    class GenocideWorldEnder : Gun
    {
        public StateBinding _fireAngleState = new StateBinding("_fireAngle", -1, false);

        public StateBinding _aimAngleState = new StateBinding("_aimAngle", -1, false);

        public StateBinding _aimingState = new StateBinding("_aiming", -1, false);

        public float _fireAngle;

        public float _aimAngle;

        public override float angle
        {
            get
            {
                return base.angle + this._aimAngle;
            }
            set
            {
                this._angle = value;
            }
        }

        public GenocideWorldEnder(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Human Extinction Attack";
            _bio = "Ends the Match";
            this.ammo = 3;
            this._ammoType = (AmmoType)new ATPlasmaBlaster();
            this._ammoType.range = 50000f;
            this._ammoType.accuracy = 0.010f;
            this._ammoType.penetration = 500f;
            this._ammoType.bulletSpeed = 20f;
            this._numBulletsPerFire = 1000;
            this._ammoType.bulletThickness = 0.6f;
            this._ammoType.affectedByGravity = true;
           
            this.weight = 4f;
            this.bouncy = 0.2f;
            this.gravMultiplier = 0.5f;
            this._ammoType.bulletColor = Color.DarkRed;
            this._bulletColor = Color.Gold;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("genocideWorldEnder"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._barrelOffsetTL = new Vec2(20f, 15f);
            this._fireSound = GetPath("tinylittlegun");
            this._kickForce = 999f;
            this.laserSight = false;
            this._laserOffsetTL = new Vec2(9f, 8.5f);
        }

        public override void Fire()
        {
            base.Fire();
        }

        public override void Update()
        {
            base.Update();
            this._fireAngle = 90f;

            if (this.owner != null)
            {
                this._aimAngle = -Maths.DegToRad(this._fireAngle);
                if (this.offDir < 0)
                {
                    this._aimAngle = -this._aimAngle;
                }
            }
        }

    }
}
