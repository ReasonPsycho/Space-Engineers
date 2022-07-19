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
            public string[] logs;
            private IMyMotorAdvancedStator rotorX;
            private IMyMotorAdvancedStator rotorY;
            private IMyMotorAdvancedStator rotorZ;
            private IMyThrust thruster;
            private StatorController statorController;
            private Vector3 rotation;
            public VTOL_thruster(string _prefix, MyGridProgram grid, StatorController _statorController, Vector3 _rotation)
            {
                logs = new string[2];
                logs[0] = "";
                logs[1] = "";
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
            static float AngleBetween(Vector3 u, Vector3 v) // EVEN MORE LOVE http://james-ramsden.com/angle-between-two-vectors/
            {
                double toppart = 0;
                toppart += u.X * v.X;
                toppart += u.Y * v.Y;
                toppart += u.Z * v.Z;
                double u2 = 0; //u squared
                double v2 = 0; //v squared

                u2 += u.X * u.X;
                v2 += v.X * v.X;
                u2 += u.Y * u.Y;
                v2 += v.Y * v.Y;
                u2 += u.Z * u.Z;
                v2 += v.Z * v.Z;

                double bottompart = 0;
                bottompart = Math.Sqrt(u2 * v2);

                double rtnval = Math.Acos(toppart / bottompart);
                rtnval *= 360.0 / (2 * Math.PI);
                return (float) rtnval;
            }
            public static Quaternion AngleAxis(float aAngle, Vector3 aAxis) // LOVE https://answers.unity.com/questions/1668856/whats-the-source-code-of-quaternionfromtorotation.html
            {
                aAxis.Normalize();
                float rad = aAngle * 0.5f;
                aAxis *= (float)Math.Sin(rad);
                return new Quaternion(new Vector3(aAxis.X, aAxis.Y, aAxis.Z), (float)Math.Cos(rad));
            }
            public static Quaternion FromToRotation(Vector3 aFrom, Vector3 aTo)
            {
                Vector3 axis = Vector3.Cross(aFrom, aTo);
                float angle = AngleBetween(aFrom, aTo);
                return AngleAxis(angle, Vector3.Normalize(axis));
            }
            public bool FaceThrusterToDirection(Vector3 direction)
            {
                logs[0] = ""; 
                logs[0] += prefix + " VTOL thruster is rotating to a: \n";
                logs[0] += "direction: " + direction.ToString() + "\n";
                bool isReady = true;
                if (direction == Vector3.Zero)
                {
                    ResetStatorsRotations();
                    return false;
                }
                else
                {
                    Quaternion rotateTo = Quaternion.Zero;
                    if (direction == Vector3.Forward) // Just face backwards
                    {

                    }else if (direction != Vector3.Backward) 
                    {
                        Vector3.Transform(direction, FromToRotation(Vector3.Forward, rotation));
                        rotateTo = FromToRotation(Vector3.Forward, direction);
                        if(direction.Z < 0) // reverse x
                        {
                            rotateTo.X = -rotateTo.X;
                        }
                    }
                    else // Just face forward
                    {
                        rotateTo = new Quaternion(0, -(float)Math.PI/2, 0, 0);
                    }

                    logs[0] += "transformed direction: " + direction.ToString() + "\n";
                    if(rotation == Vector3.Right) // Really just spaghetti, but hey as long it works! TODO fix rotating depending of dierectiuon
                    {
                        rotateTo.X = -rotateTo.X;
                        if(direction.Z == -1)
                        {
                            rotateTo.Y = (float)Math.PI;
                        }
                    }
                    logs[0] += "rotateTo quaternion: " + rotateTo.ToString() + ".\n";
                    logs[1] = "";                  
                    if (!statorController.MoveRotorToRotation(6f, 0.1f, rotateTo.X*2, rotorX))
                    {
                        isReady = false;
                        logs[1] += statorController.log;
                    }
                    if (!statorController.MoveRotorToRotation(6f, 0.1f, rotateTo.Y*2, rotorY))
                    {
                        isReady = false;
                        logs[1] += statorController.log;
                    }
                    if (!statorController.MoveRotorToRotation(6f, 0.1f, rotateTo.Z*2, rotorZ))
                    {
                        isReady = false;
                        logs[1] += statorController.log;
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
                        thruster.Enabled = true;
                        thruster.ThrustOverridePercentage = percentageSpeed;
                    }
                    else
                    {
                        thruster.Enabled = false;
                    }
                }
            }

            public void ResetStatorsRotations()
            {
                statorController.MoveRotorToRotation(4f, 0.1f, 0f, rotorX);
                statorController.MoveRotorToRotation(4f, 0.1f, 0f, rotorY);
                statorController.MoveRotorToRotation(4f, 0.1f, 0f, rotorZ);
            }
        }
    }
}
