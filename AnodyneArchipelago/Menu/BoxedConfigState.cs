using AnodyneSharp.States;
using AnodyneSharp.States.MenuSubstates;

namespace AnodyneArchipelago.Menu
{
    internal class BoxedConfigState : State
    {
        private ConfigSubstate _substate = new(true);

        public BoxedConfigState()
        {
            _substate.GetControl();
        }

        public override void Update()
        {
            if (_substate.Exit)
            {
                Exit = true;
            }

            _substate.Update();
            _substate.HandleInput();
        }

        public override void DrawUI()
        {
            _substate.DrawUI();
        }
    }
}
