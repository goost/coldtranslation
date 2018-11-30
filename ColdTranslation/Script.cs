using PS4MacroAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ColdTranslation
{
    public class Script : ScriptBase
    {
        public DialogBoxForm DialogBox { get; }

        public Script()
        {
            Config.Name = "Cold Translation";
            Config.LoopDelay = 10;
            Config.EnableCapture = false;
            ScriptForm = DialogBox = new DialogBoxForm();


        }

        public override void Start()
        {
            DialogBox.Start();

        }

        public override void OnStopped()
        {
            DialogBox.OnStopped();
        }


        public override void Update()
        {
            if (DialogBox.Advance)
            {
                Press(new DualShockState() { Circle = true }, 10);
            }
            else
            {
                ClearButtons();
            }
        }
    }
}
