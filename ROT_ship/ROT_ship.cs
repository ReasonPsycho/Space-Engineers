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
        public class ROT_ship
        {
            public bool isFuntional;
            public string[] logs;
            public float speed;
            private IMyShipController shipController;
            private IMyCockpit cockpit;
            private ROT_thruster rightThruster;
            private ROT_thruster leftThruster;
            private StatorController statorController;
            private IMyTextSurface mainCockpitPanel;
            private IMyTextSurface leftCockpitPanel;
            private IMyTextSurface rightCockpitPanel;
            private MyCommandLine commandLine;
            public ROT_ship(MyGridProgram grid, StatorController _statorController, MyCommandLine _commandLine)
            {
                try
                {
                    isFuntional = true;
                    shipController = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
                    cockpit = grid.GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
                    mainCockpitPanel = cockpit.GetSurface(0) as IMyTextSurface;
                    leftCockpitPanel = cockpit.GetSurface(1) as IMyTextSurface;
                    rightCockpitPanel = cockpit.GetSurface(2) as IMyTextSurface;
                    commandLine = _commandLine;
                    if (mainCockpitPanel != null && leftCockpitPanel != null && rightCockpitPanel != null)
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
                        rightThruster = new ROT_thruster("Right", grid, statorController, Vector3.Right);
                        leftThruster = new ROT_thruster("Left", grid, statorController, Vector3.Left);
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

            public void FlyTo(Vector3 dierction,float speed,float rotataionSpeed)
            {
                if (isFuntional)
                {
                    dierction = Vector3.Normalize(dierction);
                  
                    if (dierction.X < 0)
                    {
                        leftThruster.Fly(dierction, speed, rotataionSpeed);
                        rightThruster.Fly(Vector3.Zero, 0f, rotataionSpeed/2);
                    }
                    else if (dierction.X > 0)
                    {                    
                        rightThruster.Fly(dierction, speed, rotataionSpeed);
                        leftThruster.Fly(Vector3.Zero, 0f, rotataionSpeed/2);
                    }
                    else
                    {
                        leftThruster.Fly(dierction, speed, rotataionSpeed);
                        rightThruster.Fly(dierction, speed, rotataionSpeed);
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
                        FlyTo(dir, speed, 6f);

                    }
                    else if(cockpit.DampenersOverride && (float)cockpit.GetShipVelocities().LinearVelocity.LengthSquared() > (float)(Vector3.One.LengthSquared()))
                    {
                        Quaternion shipOrientation = Quaternion.CreateFromRotationMatrix(cockpit.WorldMatrix.GetOrientation());
                        logs[0] = shipOrientation.ToString() + "\n";

                        Vector3 dir = Vector3.Transform(cockpit.GetShipVelocities().LinearVelocity, shipOrientation);
                        dir.Normalize();
                        logs[0] = (dir).ToString() + "\n";

                        shipOrientation.X = -shipOrientation.X;
                        shipOrientation.Z = -shipOrientation.Z;
                        FlyTo(dir, 0,2f);
                    }
                    else
                    {
                        leftThruster.Fly(Vector3.Left, 0f,3f);
                        rightThruster.Fly(Vector3.Right, 0f, 3f);
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

            public void ShipCommands()
            {

                // Argument no. 1 is the speed
                if (commandLine.Argument(1) == "speed")
                {
                    float value;
                    
                    if(float.TryParse(commandLine.Argument(2),out value))
                    {
                        speed = value/100;
                    }                  
                }
            }
        }
    }
}
