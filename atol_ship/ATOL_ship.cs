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
            public string[] logs;
            private IMyShipController shipController;
            private IMyCockpit cockpit;
            private VTOL_thruster rightThruster;
            private VTOL_thruster leftThruster;
            private StatorController statorController;
            private IMyTextSurface mainCockpitPanel;
            private IMyTextSurface leftCockpitPanel;
            private IMyTextSurface rightCockpitPanel;
            public ATOL_ship(MyGridProgram grid, StatorController _statorController)
            {
                try
                {
                    isFuntional = true;
                    shipController = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
                    cockpit = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
                    mainCockpitPanel = cockpit.GetSurface(0) as IMyTextSurface;
                    leftCockpitPanel = cockpit.GetSurface(1) as IMyTextSurface;
                    rightCockpitPanel = cockpit.GetSurface(2) as IMyTextSurface;
                    if(mainCockpitPanel != null && leftCockpitPanel != null && rightCockpitPanel != null)
                    {
                        Color color = new Color(0, 100, 150);
                        mainCockpitPanel.BackgroundColor = color;
                        leftCockpitPanel.BackgroundColor = color;
                        rightCockpitPanel.BackgroundColor = color;
                    }
                    logs = new string[3];
                    logs[0] = "";
                    logs[1] = "";
                    logs[2] = "";
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

            public void FlyTo(Vector3 dierction,float speed)
            {
                if (isFuntional)
                {
                    dierction = Vector3.Normalize(dierction);
                  
                    if (dierction.X < 0)
                    {
                        leftThruster.Fly(dierction, speed);
                        rightThruster.Fly(Vector3.Zero, 0f);
                    }
                    else if (dierction.X > 0)
                    {
                        rightThruster.Fly(dierction, speed);
                        leftThruster.Fly(Vector3.Zero, 0f);
                    }
                    else
                    {
                        leftThruster.Fly(dierction, speed);
                        rightThruster.Fly(dierction, speed);
                    }
                }
            }

            public void Fly()
            {
                if (isFuntional)
                {
                    logs[0] = "";
                    if (cockpit.MoveIndicator != Vector3.Zero)
                    {
                        Vector3 dir = cockpit.MoveIndicator;
                        dir.X = -dir.X;
                        FlyTo(dir, 1f);

                    }
                    else if(cockpit.DampenersOverride && (float)cockpit.GetShipVelocities().LinearVelocity.LengthSquared() > (float)(Vector3.One.LengthSquared()))
                    {
                        Quaternion shipOrientation = Quaternion.CreateFromRotationMatrix(cockpit.WorldMatrix.GetOrientation());
                        logs[0] = shipOrientation.ToString() + "\n";

                        Vector3.Transform(cockpit.GetShipVelocities().LinearVelocity, shipOrientation);
                        shipOrientation.X = -shipOrientation.X;
                        shipOrientation.Z = -shipOrientation.Z;
                        FlyTo(cockpit.GetShipVelocities().LinearVelocity, 0);
                    }
                    else
                    {
                        leftThruster.Fly(Vector3.Zero, 0f);
                        rightThruster.Fly(Vector3.Zero, 0f);
                    }
                    logs[0] += leftThruster.logs[0] += rightThruster.logs[0];
                    logs[1] = leftThruster.logs[1];
                    logs[2] = rightThruster.logs[1];
                }
            }

            public void ResetThrusterRotation()
            {
                leftThruster.ResetStatorsRotations();
                rightThruster.ResetStatorsRotations();
            }

            public void DEBUG()
            {
                if ((mainCockpitPanel != null && leftCockpitPanel != null && rightCockpitPanel != null) && isFuntional)
                {
                    mainCockpitPanel.WriteText(logs[0]);
                    leftCockpitPanel.WriteText(logs[1]);
                    rightCockpitPanel.WriteText(logs[2]);
                    logs[0] = "";
                    logs[1] = "";
                    logs[2] = "";
                }
            }
        }
    }
}
