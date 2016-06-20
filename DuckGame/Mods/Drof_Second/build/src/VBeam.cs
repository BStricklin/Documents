using DuckGame;
using System;
using System.Collections.Generic;

namespace MyMod
{
    internal class VBeam : Thing
    {
        public float _blast = 1f;

        private Vec2 _target;

        public VBeam(Vec2 pos, Vec2 target) : base(pos.x, pos.y, null)
        {
            this._target = target;
        }

        public override void Initialize()
        {
            Vec2 normalized = (this.position - this._target).Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
            Vec2 normalized2 = (this.position - this._target).Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
            Level.Add(new LaserLine(this.position, this._target - this.position, normalized, 4f, Color.White, 1f, 0.03f));
            Level.Add(new LaserLine(this.position, this._target - this.position, normalized2, 4f, Color.White, 1f, 0.03f));
            Level.Add(new LaserLine(this.position, this._target - this.position, normalized, 2.5f, Color.White, 2f, 0.03f));
            Level.Add(new LaserLine(this.position, this._target - this.position, normalized2, 2.5f, Color.White, 2f, 0.03f));
            float num = 64f;
            float num2 = 7f;
            float num3 = num / (num2 - 1f);
            Vec2 vec2_ = this.position + normalized * num / 2f;
            List<BlockGroup> list = new List<BlockGroup>();
            int index = 0;
            while ((double)index < (double)num2)
            {
                Vec2 vec2_2 = vec2_ + normalized2 * num3 * (float)index;
                foreach (PhysicsObject physicsObject in Level.CheckLineAll<PhysicsObject>(vec2_2, vec2_2 + this._target - this.position))
                {
                    physicsObject.Destroy(new DTIncinerate(this));
                    physicsObject._sleeping = false;
                    physicsObject.vSpeed = -2f;
                }
                foreach (BlockGroup blockGroup in Level.CheckLineAll<BlockGroup>(vec2_2, vec2_2 + this._target - this.position))
                {
                    if (blockGroup != null)
                    {
                        BlockGroup blockGroup2 = blockGroup;
                        List<Block> list2 = new List<Block>();
                        foreach (Block block in blockGroup2.blocks)
                        {
                        }
                        list.Add(blockGroup);
                    }
                }
                foreach (Block block in Level.CheckLineAll<Block>(vec2_2, vec2_2 + this._target - this.position))
                {
                    if (block is AutoBlock)
                    {
                        block.skipWreck = true;
                        block.shouldWreck = true;
                    }
                    else if (block is Door || block is VerticalDoor)
                    {
                        Level.Remove(block);
                        block.Destroy(new DTRocketExplosion(null));
                    }
                }
                index++;
            }
            foreach (BlockGroup blockGroup3 in list)
            {
                blockGroup3.Wreck();
            }
            if (Recorder.currentRecording != null)
            {
                Recorder.currentRecording.LogBonus();
            }
        }

        public override void Update()
        {
            this._blast = Maths.CountDown(this._blast, 0.05f, 0f);
            if ((double)this._blast < 0.0)
            {
                Level.Remove(this);
            }
        }

        public override void Draw()
        {
            double num = (double)Maths.NormalizeSection(this._blast, 0f, 0.2f);
            double num2 = (double)Maths.NormalizeSection(this._blast, 0.6f, 1f);
            double num3 = (double)this._blast;
            Vec2 normalized = (this.position - this._target).Rotate(Maths.DegToRad(-90f), Vec2.Zero).normalized;
            Vec2 normalized2 = (this.position - this._target).Rotate(Maths.DegToRad(90f), Vec2.Zero).normalized;
            float num4 = 64f;
            float num5 = 7f;
            float num6 = num4 / (num5 - 1f);
            Vec2 vec2 = this.position + normalized * num4 / 2f;
            int index = 0;
            while ((double)index < (double)num5)
            {
                Vec2 p = vec2 + normalized2 * num6 * (float)index;
                Graphics.DrawLine(p, p + this._target - this.position, Color.SkyBlue * (this._blast * 0.9f), 2f, 0.9f);
                index++;
            }
        }
    }
}
