using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    class ReusableGrenade : Grenade
    {
        new public StateBinding _timerBinding = new StateBinding("_timer", -1, false);

        new public float _timer = 1.2f;

        private SpriteMap _sprite;

        private Duck _cookThrower;

        private float _cookTimeOnThrow;

        private bool _localDidExplode;

        private bool _didBonus;

        private bool _explosionCreated;


        public ReusableGrenade(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 1;
            this._editorName = "Reusable Grenade";
            this._bio = "Careful, they may decide to throw it back";
            this._ammoType = new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            _sprite = new SpriteMap(GetPath("reusableGrenade"), 16, 16, false);
            base.graphic = _sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 10f);

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            this._pin = false;
            this._localDidExplode = true;
            if (!this._explosionCreated)
            {
                Graphics.flashAdd = 1.3f;
                Layer.Game.darken = 1.3f;
            }
            this.CreateExplosion(pos);
        }

        new public void CreateExplosion(Vec2 pos)
        {
            if (!this._explosionCreated)
            {
                float x = pos.x;
                float num = pos.y - 2f;
                Level.Add(new ExplosionPart(x, num, true));
                int num2 = 6;
                if (Graphics.effectsLevel < 2)
                {
                    num2 = 3;
                }
                for (int i = 0; i < num2; i++)
                {
                    float deg = (float)i * 60f + Rando.Float(-10f, 10f);
                    float num3 = Rando.Float(12f, 20f);
                    ExplosionPart thing = new ExplosionPart(x + (float)(System.Math.Cos((double)Maths.DegToRad(deg)) * (double)num3), num - (float)(System.Math.Sin((double)Maths.DegToRad(deg)) * (double)num3), true);
                    Level.Add(thing);
                }
                this._explosionCreated = true;
                SFX.Play("explode", 1f, 0f, 0f, false);
                spawnGrenade();
            }
        }

        public override void Update()
        {
            base.Update();
            if (!this._pin)
            {
                this._timer -= 0.01f;
            }
            if (this._timer < 0.5f && this.owner == null && !this._didBonus)
            {
                this._didBonus = true;
                if (Recorder.currentRecording != null)
                {
                    Recorder.currentRecording.LogBonus();
                }
            }
            if (!this._localDidExplode && this._timer < 0f)
            {
                if (this._explodeFrames < 0)
                {
                    this.CreateExplosion(this.position);
                    this._explodeFrames = 4;
                }
                else
                {
                    this._explodeFrames--;
                    if (this._explodeFrames == 0)
                    {
                        float x = base.x;
                        float num = base.y - 2f;
                        Graphics.flashAdd = 1.3f;
                        Layer.Game.darken = 1.3f;
                        if (base.isServerForObject)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                float num2 = (float)i * 18f - 5f + Rando.Float(10f);
                                ATShrapnel aTShrapnel = new ATShrapnel();
                                aTShrapnel.range = 60f + Rando.Float(18f);
                                Bullet bullet = new Bullet(x + (float)(System.Math.Cos((double)Maths.DegToRad(num2)) * 6.0), num - (float)(System.Math.Sin((double)Maths.DegToRad(num2)) * 6.0), aTShrapnel, num2, null, false, -1f, false, true);
                                bullet.firedFrom = this;
                                this.firedBullets.Add(bullet);
                                Level.Add(bullet);
                            }
                            System.Collections.Generic.IEnumerable<Window> enumerable = Level.CheckCircleAll<Window>(this.position, 40f);
                            foreach (Window current in enumerable)
                            {
                                if (Level.CheckLine<Block>(this.position, current.position, current) == null)
                                {
                                    current.Destroy(new DTImpact(this));
                                }
                            }
                            this.bulletFireIndex += 20;
                            if (Network.isActive)
                            {
                                NMFireGun msg = new NMFireGun(this, this.firedBullets, this.bulletFireIndex, false, 4, false);
                                Send.Message(msg, NetMessagePriority.ReliableOrdered, null);
                                this.firedBullets.Clear();
                            }
                        }
                        Level.Remove(this);
                        base._destroyed = true;
                        this._explodeFrames = -1;

                    }
                }
            }
            if (base.prevOwner != null && this._cookThrower == null)
            {
                this._cookThrower = (base.prevOwner as Duck);
                this._cookTimeOnThrow = this._timer;
            }
            this._sprite.frame = (this._pin ? 0 : 1);
        }

        public void spawnGrenade()
        {
            ReusableGrenade grenade = new ReusableGrenade(0, 0);
            grenade.position = Offset(new Vec2(0, 0));
            SFX.Play("deepMachineGun", 1f, 0f, 0f, false);

            Level.Add((Thing)grenade);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSolidImpact(with, from);
        }

        public override void OnPressAction()
        {
            base.OnPressAction();
        }
    }
}
