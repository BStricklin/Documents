using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    public class Drill : Gun
    {
        public StateBinding _hasFiredStateBinding = new StateBinding("_hasFired", -1, false);

        public StateBinding _launcherStateBinding = new StateFlagBinding(new string[]
        {
            "_readyForLaunch",
            "_hasLaunched",
            "_goingUp"
        });

        public StateBinding _thrownAwayStateBinding = new StateBinding("_thrownAway", -1, false);

        public StateBinding _animCycleStateBinding = new StateBinding("_animCycle", -1, false);

        public StateBinding _crackednessStateBinding = new StateBinding("_crackedness", -1, false);

        public StateBinding _savedDirStateBinding = new StateBinding("_savedDir", -1, false);

        public StateBinding _savedDuckStateBinding = new StateBinding("_savedDuck", -1, false);

        public StateBinding netSFX_drillklangStateBinding = new NetSoundBinding("netSFX_drillklang");

        public StateBinding netSFX_explodeStateBinding = new NetSoundBinding("netSFX_explode");

        public StateBinding netSFX_missileStateBinding = new NetSoundBinding("netSFX_missile");

        public StateBinding netSFX_clashStateBinding = new NetSoundBinding("netSFX_clash");

        public NetSoundEffect netSFX_drillklang = new NetSoundEffect(new string[]
        {
            "drillklang"
        });

        public NetSoundEffect netSFX_explode = new NetSoundEffect(new string[]
        {
            "explode"
        })
        {
            volume = 0.2f
        };

        public NetSoundEffect netSFX_missile = new NetSoundEffect(new string[]
        {
            "missile"
        });

        public NetSoundEffect netSFX_clash = new NetSoundEffect(new string[]
        {
            "chainsawClash"
        })
        {
            volume = 0.4f
        };

        public bool _hasFired;

        public bool _readyForLaunch;

        public bool _hasLaunched;

        public bool _goingUp;

        public bool _thrownAway;

        public int _animCycle;

        public int _crackedness;

        public int _cooldown;

        public float _savedDir;

        public Duck _savedDuck;

        private List<MaterialThing> thingsHit = new List<MaterialThing>();

        private float minHSpeed;

        private SpriteMap sprite;

        public Drill(float x, float y) : base(x, y)
        {
            this._editorName = "Drill";
            this._bio = "'My drill is the drill that will pierce the Heavns' ~ Kamina";
            this.sprite = new SpriteMap(GetPath("Drill"), 25, 20, false);
            base.graphic = this.sprite;
            this.center = new Vec2(7f, 10f);
            this._collisionSize = new Vec2(14f, 10f);
            this._collisionOffset = new Vec2(-7f, -5f);
            this._holdOffset = new Vec2(3f, 2f);
            this.ammo = 120;
            this.weight = 7.5f;
            this._hasTrigger = false;
            this.physicsMaterial = PhysicsMaterial.Default;
            this._hasFired = false;
            this._readyForLaunch = false;
            this._hasLaunched = false;
            this._goingUp = false;
            this._thrownAway = false;
            this._animCycle = 0;
            this._crackedness = 0;
        }

        public override void Terminate()
        {
            this.thingsHit.Clear();
            base.Terminate();
        }

        public override void OnPressAction()
        {
            if (!this._hasFired)
            {
                if (Network.isActive && base.isServerForObject)
                {
                    this.netSFX_drillklang.Play(1f, 0f);
                }
                else
                {
                    SFX.Play(GetPath("drillklang"), 1f, 0f, 0f, false);
                }
                this._animCycle = 0;
                this._hasFired = true;
                this._thrownAway = false;
                base.OnPressAction();
            }
        }

        private void StateChange(int i)
        {
            this._crackedness = i;
            this._animCycle += 5;
        }

        public override void Thrown()
        {
            if (this._readyForLaunch)
            {
                this.DrillLaunch();
                return;
            }
            if (this._hasFired && this._crackedness != 3)
            {
                this._readyForLaunch = true;
                this.DrillLaunch();
                return;
            }
            this._hasFired = false;
        }

        public override void Update()
        {
            if (base.duck != null)
            {
                this._savedDuck = base.duck;
            }
            this._savedDir = (float)this.offDir;
            this.BrokenCheck();
            if (base.duck != null && base.duck.inputProfile.Pressed("GRAB", false) && this._hasFired && this._crackedness != 3)
            {
                this._readyForLaunch = true;
            }
            if ((!this._hasFired || this.owner == null) && this._crackedness == 3 && !this._hasLaunched)
            {
                this._animCycle = 21;
            }
            else if ((!this._hasFired || this.owner == null) && !this._hasLaunched)
            {
                this._animCycle = 0;
            }
            if (this._hasLaunched)
            {
                if (base.isServerForObject && (base.x > Level.current.bottomRight.x + 200f || base.x < Level.current.topLeft.x - 200f))
                {
                    Level.Remove(this);
                }
                if (this._goingUp)
                {
                    if (this._vSpeed < 0.8f)
                    {
                        this._vSpeed += 0.35f;
                    }
                    else
                    {
                        this._goingUp = false;
                    }
                }
                else if (this._vSpeed > -0.8f)
                {
                    this._vSpeed -= 0.35f;
                }
                else
                {
                    this._goingUp = true;
                }
                if (Math.Abs(this._hSpeed) < Math.Abs(this.minHSpeed))
                {
                    this._hSpeed = this.minHSpeed;
                }
                if (Math.Abs(this._hSpeed) < 6.3f)
                {
                    this._hSpeed = MathHelper.Lerp(this._hSpeed, 6.3f * this._savedDir, 0.16f);
                }
                else
                {
                    this._hSpeed = MathHelper.Lerp(this._hSpeed, 6.3f * this._savedDir, 0.24f);
                }
                Level.Add(SmallSmoke.New(base.x, base.y + Rando.Float(-2f, 2f)));
            }
            if (this.ammo <= 80 && this.ammo > 40 && this._crackedness != 1)
            {
                this.StateChange(1);
            }
            else if (this.ammo <= 40 && this.ammo > 0 && this._crackedness != 2)
            {
                this.StateChange(2);
            }
            if (this.owner != null && this._animCycle >= 5 && this._animCycle < 21)
            {
                this._collisionSize = new Vec2(22f, 12f);
                this._collisionOffset = new Vec2(-11f, -6f);
            }
            else
            {
                this._collisionSize = new Vec2(14f, 10f);
                this._collisionOffset = new Vec2(-7f, -5f);
            }
            if (this._hasFired && (base.duck != null || this._hasLaunched))
            {
                if (this._crackedness != 3)
                {
                    foreach (BlockGroup current in Level.CheckCircleAll<BlockGroup>(new Vec2(base.x + (float)this.offDir * 8f, base.y), 16f))
                    {
                        this.GigaDrillBreak(current);
                    }
                    foreach (MaterialThing current2 in Level.CheckRectAll<MaterialThing>(base.topLeft, base.bottomRight - new Vec2(0f, this._hasLaunched ? 0f : 4f)))
                    {
                        if (!this.thingsHit.Contains(current2) && !(current2 is Drill) && current2 != this && current2 != base.duck && (this._savedDuck == null || current2 != this._savedDuck) && !(current2 is Equipment) && (!(current2 is Gun) || current2 is Drill))
                        {
                            if (current2 is Window || current2 is Duck)
                            {
                                this.PierceTheHeavens(new DTImpale(this), current2);
                            }
                            else if (current2 is Door)
                            {
                                this.PierceTheHeavens(new DTRocketExplosion(this), current2);
                            }
                            else if (current2 is PhysicsObject && !(current2 is Duck) && current2 != this && current2 != this.owner && !(current2 is Hat) && !(current2 is Gun))
                            {
                                this.PierceTheHeavens(new DTRocketExplosion(this), current2);
                            }
                            else if (current2 is Block)
                            {
                                this.GigaDrillBreak(current2);
                            }
                        }
                    }
                    this._animCycle++;
                    if (this._crackedness == 0 && this._animCycle > 7)
                    {
                        this._animCycle = 5;
                    }
                    else if (this._crackedness == 1)
                    {
                        if ((this._animCycle >= 5 && this._animCycle < 10) || this._animCycle > 12)
                        {
                            this._animCycle = 10;
                        }
                    }
                    else if (this._crackedness == 2 && ((this._animCycle >= 5 && this._animCycle < 15) || this._animCycle > 17))
                    {
                        this._animCycle = 15;
                    }
                    if (this.owner != null)
                    {
                        if (Math.Abs(this.owner.hSpeed) < 5.8f)
                        {
                            this.owner.hSpeed = MathHelper.Lerp(this.owner._hSpeed, (float)this.owner.offDir * 5.8f, 0.24f);
                        }
                        else
                        {
                            this.owner.hSpeed = MathHelper.Lerp(this.owner._hSpeed, (float)this.owner.offDir * 5.8f, 0.32f);
                        }
                    }
                }
                else
                {
                    this._animCycle++;
                    if (this._animCycle >= 5)
                    {
                        this._animCycle = 20;
                    }
                }
            }
            this.sprite.frame = this._animCycle;
            base.Update();
        }

        private void DrillLaunch()
        {
            this._hasLaunched = true;
            if (Network.isActive && base.isServerForObject)
            {
                this.netSFX_missile.Play(0.8f, 0f);
            }
            else
            {
                SFX.Play("missile", 0.8f, 0f, 0f, false);
            }
            this.minHSpeed = 1.3f * this._savedDir;
            if (this.owner != null)
            {
                this.owner._hSpeed += -8f * this._savedDir;
            }
            this.canPickUp = false;
        }

        private bool BrokenCheck()
        {
            if (this.ammo <= 0 && this._crackedness == 2)
            {
                Level.Add(new ExplosionPart(base.x + Rando.Float(-1f, 1f), base.y + Rando.Float(-1f, 1f), false));
                for (int i = 0; i < 4; i++)
                {
                    Level.Add(new ExplosionPart(base.x + Rando.Float(-16f, 16f), base.y + Rando.Float(-16f, 16f), false));
                }
                if (Network.isActive && base.isServerForObject)
                {
                    this.netSFX_explode.Play(1f, 0f);
                }
                else
                {
                    SFX.Play("explode", 0.2f, 0f, 0f, false);
                }
                this._crackedness = 3;
                if (this._hasLaunched)
                {
                    Level.Remove(this);
                }
                return true;
            }
            if (this.ammo <= 0 || this._crackedness == 3)
            {
                if (this._hasLaunched)
                {
                    Level.Remove(this);
                }
                return true;
            }
            return false;
        }

        private void PierceTheHeavens(DestroyType destroyType, MaterialThing materialThing)
        {
            this.thingsHit.Add(materialThing);
            if (materialThing is Door)
            {
                Level.Add(new ExplosionPart(materialThing.x + Rando.Float(-1f, 1f), materialThing.y + Rando.Float(-1f, 1f), false));
                SFX.Play("explode", 0.2f, 0f, 0f, false);
                SFX.Play("chainsawClash", 0.4f, 0f, 0f, false);
                this.ammo -= 7;
                Level.Remove(materialThing);
                this.Knockback();
                return;
            }
            if (materialThing.Destroy(destroyType))
            {
                Level.Add(new ExplosionPart(materialThing.x + Rando.Float(-1f, 1f), materialThing.y + Rando.Float(-1f, 1f), false));
                SFX.Play("explode", 0.3f, 0f, 0f, false);
                SFX.Play("chainsawClash", 0.6f, 0f, 0f, false);
                this.ammo -= 7;
                this.Knockback();
            }
        }

        private void GigaDrillBreak(MaterialThing block)
        {
            if (block is BlockGroup)
            {
                BlockGroup blockGroup = block as BlockGroup;
                blockGroup.Wreck();
                return;
            }
            if (block is AutoBlock)
            {
                this.thingsHit.Add(block);
                Level.Add(new GlobalExplosion(block.x + Rando.Float(-1f, 1f), block.y + Rando.Float(-1f, 1f)));
                this.netSFX_explode.Play(1f, 0f);
                this.netSFX_clash.Play(1f, 0f);
                this.ammo -= 8;
                foreach (MaterialThing current in Level.CheckCircleAll<MaterialThing>(block.position, 16f))
                {
                    if (current is BlockGroup)
                    {
                        BlockGroup blockGroup2 = current as BlockGroup;
                        blockGroup2.Wreck();
                    }
                    else if (current is PhysicsObject && current != ((this._savedDuck == null) ? null : this._savedDuck) && current != this && current != ((base.duck == null) ? null : base.duck))
                    {
                        if (block.isLocal && block.owner == null)
                        {
                            Thing.Fondle(current, DuckNetwork.localConnection);
                        }
                        ((PhysicsObject)current).sleeping = false;
                        current.vSpeed = -2f;
                    }
                }
                this.Knockback();
                HashSet<ushort> hashSet = new HashSet<ushort>();
                hashSet.Add((block as AutoBlock).blockIndex);
                ((Block)block).skipWreck = true;
                ((Block)block).shouldWreck = true;
                if (Network.isActive && this.isLocal)
                {
                    Send.Message(new NMDestroyBlocks(hashSet));
                    return;
                }
            }
            else if (this._hasLaunched)
            {
                this.ammo = 0;
                while (this._crackedness < 2)
                {
                    this.StateChange(this._crackedness + 1);
                }
                this.BrokenCheck();
            }
        }

        public void Knockback()
        {
            if (base.duck == null)
            {
                return;
            }
            if (base.duck.sliding)
            {
                base.duck.sliding = false;
            }
            base.duck.hSpeed = -3.5f * (float)base.duck.offDir;
            base.duck.vSpeed = -0.2f;
        }

        public override void CheckIfHoldObstructed()
        {
            if (this.owner == null)
            {
                return;
            }
            base.duck.holdObstructed = false;
        }

        public override void Fire()
        {
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._hasLaunched && this.impactPowerH >= 1f && with is Duck && with != this._savedDuck && ((this.offDir > 0 && from == ImpactedFrom.Right) || (this.offDir < 0 && from == ImpactedFrom.Left)))
            {
                ((Duck)with).Kill(new DTImpale(with));
            }
            base.OnImpact(with, from);
        }
    }
}
