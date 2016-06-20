using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    public class GurrenGrenadeDrill : Drill
    {
        private List<MaterialThing> thingsHit = new List<MaterialThing>();

        private SpriteMap sprite;
        public float setXspeed;
        public float setYspeed;
        private float minHSpeed;

        public float SetXspeed
        {
            get
            {
                return setXspeed;
            }
            set
            {
                this.setXspeed = value;
            }
        }

        public float SetYspeed
        {
            get
            {
                return setYspeed;
            }
            set
            {
                this.setYspeed = value;
            }
        }

        public GurrenGrenadeDrill(float x, float y) : base(x, y)
        {
            this._editorName = "Gurren Grenade Drill";
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

            this.setXspeed = 1.3f;
            this.setYspeed = 0;
        }

        public override void Update()
        {
            base.Update();
            this._hSpeed = setXspeed;
            this._vSpeed = setYspeed;
        }

        private void StateChange(int i)
        {
            this._crackedness = i;
            this._animCycle += 5;
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
    }
}
