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
        public class ATOL_ship
        {
            public bool isFuntional;
            public float percentageSpeed;
            private IMyShipController shipController;
            private IMyCockpit cockpit;
            private VTOL_thruster rightThruster;
            private VTOL_thruster leftThruster;
            private StatorController statorController;

            public ATOL_ship(MyGridProgram grid, StatorController _statorController)
            {
                try
                {
                    isFuntional = true;
                    shipController = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
                    cockpit = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
                    if (cockpit == null)
                    {
                        isFuntional = false;
                    }
                    else
                    {
                        statorController = _statorController;
                        rightThruster = new VTOL_thruster("Right", grid, statorController, Vector3.Right);
                        leftThruster = new VTOL_thruster("Left", grid, statorController, Vector3.Left);
                        if(!rightThruster.isFunctional || !leftThruster.isFunctional)
                        {
                            isFuntional = false;
                        }
                    }
              
                }
                catch
                {
                    isFuntional = false;
                }
            }

            public void Fly()
            {
                if (isFuntional)
                {
                    leftThruster.Fly(cockpit.MoveIndicator, 0.1f);
                    rightThruster.Fly(cockpit.MoveIndicator, 0.1f);
                }
            }

            public void ResetThrusterRotation()
            {
                leftThruster.ResetStatorsRotations();
                rightThruster.ResetStatorsRotations();
            }

            public string DEBUG()
            {
                if (isFuntional)
                    return cockpit.MoveIndicator.ToString();
                else
                    return null;
            }
        }
    }
}
