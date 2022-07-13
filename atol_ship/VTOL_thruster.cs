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
            public bool isFunctional;
            public string prefix;
            private IMyMotorAdvancedStator rotor;
            private IMyMotorAdvancedStator hinge;
            private IMyThrust thruster;
            private StatorController controller;
            public VTOL_thruster(string _prefix, MyGridProgram grid, StatorController _controller)
            {
                prefix = _prefix;
                isFunctional = true;
                controller = _controller;
                try
                {
                    rotor = grid.GridTerminalSystem.GetBlockWithName(prefix + " Rotor") as IMyMotorAdvancedStator;
                    hinge = grid.GridTerminalSystem.GetBlockWithName(prefix + " Hinge") as IMyMotorAdvancedStator;
                    thruster = grid.GridTerminalSystem.GetBlockWithName(prefix + " Thruster") as IMyThrust;

                    if (rotor == null || hinge == null || rotor == null)
                    {
                        isFunctional = false;
                    }
                }
                catch
                {
                    isFunctional = false;
                }
            }
            public void ResetStatorsRotations()
            {
                controller.MoveRotatorToRotation(2,0.1f,0,rotor); 
                controller.MoveRotatorToRotation(2, 0.1f, 0,hinge);
            }
        }
    }
}
