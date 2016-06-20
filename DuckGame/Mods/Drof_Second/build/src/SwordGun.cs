using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Drofdarb")]
    class SwordGun : Gun
    {
        public SwordGun(float xval, float yval) : base(xval, yval)
        {
            this.graphic = new Sprite(GetPath("swordGun"));
            this._editorName = "Sword Gun";
            this._bio = "'It's a SWORDSPLOSION!' ~ Mr. Torgue";
            this.ammo = 4;
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(27f, 14f);
            this._fireSound = "smg";
        }

        public override void Fire()
        {
            
        }

        public override void OnPressAction()
        {
            base.OnPressAction();
            if(this.ammo > 0)
            {
                this.ammo--;
                if(isServerForObject)
                {
                    Sword sword = new Sword(0, 0);
                    sword.position = Offset(new Vec2(4f, 1f));
                    sword.hSpeed = this.barrelVector.x * 7f;
                    sword.vSpeed = this.barrelVector.y * 7f;
                    SFX.Play("ting", 1f, 0f, 0f, false);

                    Level.Add((Thing)sword);
                }
            }
        }

    }
}
