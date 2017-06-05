using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Messages
{
    public enum GameplayTypes
    {
        Connect,
        Disconnect,
        Movement,
        Promotion,
        Turn,
        Finish,
        Surrender,
        NONE
    }
}
