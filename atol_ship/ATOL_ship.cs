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
            public string log;
            private IMyShipController shipController;
            private IMyCockpit cockpit;
            private VTOL_thruster rightThruster;
            private VTOL_thruster leftThruster;
            private StatorController statorController;
            private IMyTextSurface cockpitPanel;
            public ATOL_ship(MyGridProgram grid, StatorController _statorController)
            {
                try
                {
                    isFuntional = true;
                    shipController = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
                    cockpit = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
                    cockpitPanel = cockpit.GetSurface(0) as IMyTextSurface;
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
                    if (cockpit.MoveIndicator != Vector3.Zero)
                    {
                        leftThruster.Fly(cockpit.MoveIndicator, 0.1f);
                        rightThruster.Fly(cockpit.MoveIndicator, 0.1f);
                        log += leftThruster.log += rightThruster.log;
                    }
                    //else if (cockpit.DampenersOverride && (float)cockpit.GetShipVelocities().LinearVelocity.LengthSquared() > (float)(Vector3.One.LengthSquared()))
                   // {
                   //     leftThruster.Fly(cockpit.GetShipVelocities().LinearVelocity, 0f);
                   //     rightThruster.Fly(cockpit.GetShipVelocities().LinearVelocity, 0f);
                   //    
                   // }
                }
            }

            public void ResetThrusterRotation()
            {
                leftThruster.ResetStatorsRotations();
                rightThruster.ResetStatorsRotations();
            }

            public void DEBUG()
            {
                if (cockpitPanel != null && isFuntional)
                {
                    cockpitPanel.WriteText(log);
                    log = " ";
                    rightThruster.log = " ";
                    leftThruster.log = " ";
                }
            }
        }
    }
}
