using AnodyneSharp.Registry;
using AnodyneSharp.States;

namespace AnodyneArchipelago.Menu
{
    internal class MenuState : BaseMenuState
    {
        protected override void ChangeState()
        {
            if (_isNewGame)
            {
                GlobalState.GameState.SetState<IntroState>();
            }
            else
            {
                GlobalState.GameState.SetState<PlayState>();
            }
        }
    }
}
