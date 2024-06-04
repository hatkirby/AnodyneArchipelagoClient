using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnodyneArchipelago.Menu
{
    internal class MenuState : BaseMenuState
    {
        protected override void ChangeState()
        {
            ChangeStateEvent(_isNewGame ? AnodyneSharp.AnodyneGame.GameState.Intro : AnodyneSharp.AnodyneGame.GameState.Game);
        }
    }
}
