using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Example")]
    class TestGun : Pistol
    {
        public TestGun(float xval,float yval):base(xval,yval)
        {

        }

        public override void OnPressAction()
        {
            if(ammo > 0)
            DuckGame.DuckDebug.DuckDebug.Write("BANG!");
            base.OnPressAction();
        }
    }
}
