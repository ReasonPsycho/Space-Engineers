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
            private IMyMotorAdvancedStator rotorX;
            private IMyMotorAdvancedStator rotorY;
            private IMyMotorAdvancedStator rotorZ;
            private IMyThrust thruster;
            private StatorController statorController;
            private Vector3 rotation;
            public VTOL_thruster(string _prefix, MyGridProgram grid, StatorController _statorController, Vector3 _rotation)
            {
                prefix = _prefix;
                isFunctional = true;
                statorController = _statorController;
                rotation = _rotation;
                try
                {
                    rotorX = grid.GridTerminalSystem.GetBlockWithName(prefix + " Rotor X") as IMyMotorAdvancedStator;
                    rotorY = grid.GridTerminalSystem.GetBlockWithName(prefix + " Rotor Y") as IMyMotorAdvancedStator;
                    rotorZ = grid.GridTerminalSystem.GetBlockWithName(prefix + " Rotor Z") as IMyMotorAdvancedStator;
                    thruster = grid.GridTerminalSystem.GetBlockWithName(prefix + " Thruster") as IMyThrust;

                    if (rotorX == null || rotorY == null || rotorZ == null || thruster == null)
                    {
                        isFunctional = false;
                    }
                }
                catch
                {
                    isFunctional = false;
                }
            }

            public bool FaceThrusterToDirection(Vector3 direction)
            {
                bool isReady = true;
                if (direction == Vector3.Zero)
                {
                    ResetStatorsRotations();
                    return false;
                }
                else
                {
                    Vector3.Transform(direction, Quaternion.CreateFromTwoVectors(Vector3.Forward, rotation));
                    Quaternion rotateTo = Quaternion.CreateFromTwoVectors(Vector3.Forward, direction);

                    if (!statorController.MoveRotatorToRotation(1f, 0.001f, rotateTo.X, rotorX))
                    {
                        isReady = false;
                    }
                    if (!statorController.MoveRotatorToRotation(1f, 0.001f, rotateTo.Y, rotorY))
                    {
                        isReady = false;
                    }
                    if (!statorController.MoveRotatorToRotation(1f, 0.001f, rotateTo.Z, rotorZ))
                    {
                        isReady = false;
                    }
                }
                return isReady;


            }

            public void Fly(Vector3 direction, float percentageSpeed)
            {
                if (isFunctional)
                {
                    if (FaceThrusterToDirection(direction))
                    {
                        thruster.ThrustOverridePercentage = percentageSpeed;
                    }
                    else
                    {
                        thruster.ThrustOverridePercentage = 0;
                    }
                }
            }

            public void ResetStatorsRotations()
            {
                statorController.MoveRotatorToRotation(1f, 0.001f, 0, rotorX);
                statorController.MoveRotatorToRotation(1f, 0.001f, 0, rotorY);
                statorController.MoveRotatorToRotation(1f, 0.001f, 0, rotorZ);
            }
        }
    }
}
