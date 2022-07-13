using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class VTOL_thruster
        {
            bool isFunctional;
            string prefix;
            IMyMotorAdvancedStator rotor;
            IMyMotorAdvancedStator hinge;
            IMyThrust thruster;

            public VTOL_thruster(string _prefix, MyGridProgram grid)
            {
                prefix = _prefix;
                isFunctional = true;
                try
                {
                    rotor = grid.GridTerminalSystem.GetBlockWithName(prefix + " Rotor") as IMyMotorAdvancedStator;
                    hinge = grid.GridTerminalSystem.GetBlockWithName(prefix + " Hinge") as IMyMotorAdvancedStator;
                    thruster = grid.GridTerminalSystem.GetBlockWithName(prefix + " Thruster") as IMyThrust;
                }
                catch
                {
                    isFunctional = false;
                }
            }
        }
    }
}
