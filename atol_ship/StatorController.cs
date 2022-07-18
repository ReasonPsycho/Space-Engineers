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
        public class StatorController
        {
            public string log;
            public float SimplifyAngle(float angle)
            {
                if (angle < 3.12f)
                {
                    return angle;
                }
                else
                {
                    return angle - 6.24f;
                }
            }

            public bool MoveRotorToRotation(float speedMultiplayer, float precision, float rotation, IMyMotorAdvancedStator rotor)
            {
               
                float distance = Math.Abs(rotor.Angle - rotation);
                float speed = (float)Math.Max(Math.Log(Math.Abs(distance) + 3 + precision) * speedMultiplayer, 0.1f); //https://www.desmos.com/calculator/w7lbvg2ghb?lang=pl\
                log += "Name: " +rotor.CustomName  + ",distance: " + distance.ToString();
                if (distance < precision / 2)
                {
                    rotor.SetValueFloat("Velocity", 0);
                    return true;
                }
                float angle1 = rotation;
                float angle2 = rotor.Angle;

                if (angle1 < 0)
                    angle1 += (float)Math.PI*2;

                if (angle2 < 0)
                    angle2 += (float)Math.PI * 2;

                log += ",angle1: " + rotor.CustomName + ",angle2: " + angle1.ToString();
                if (angle2 > angle1 && angle2 - angle1 <= (float)Math.PI)   //go clockwise
                {
                    rotor.SetValueFloat("Velocity", -speed);
                }

                else if (angle2 > angle1 && angle2 - angle1 > Math.PI) //go counter clockwise
                {
                    rotor.SetValueFloat("Velocity", speed);
                }
 
                else if (angle1 > angle2 && angle1 - angle2 <= Math.PI)  //go counter clockwise
                {
                    rotor.SetValueFloat("Velocity", speed);
                }
 
                else if (angle1 > angle2 && angle1 - angle2 > Math.PI)   //go clockwise
                {
                    rotor.SetValueFloat("Velocity", -speed);
                }

                log += ",velocity: " + rotor.TargetVelocityRad.ToString();
                log = "";
                return false;
            }

                public bool MoveHingdeToRotation(float speed, float precision, float  rotation, IMyMotorAdvancedStator hinge)
            {
                float dif = Math.Abs(SimplifyAngle(hinge.Angle) - rotation);
                if (SimplifyAngle(hinge.Angle) > rotation + 0.05f)
                {
                    if (dif < 3.12f)
                    {
                        if (dif > precision * speed)
                        {
                            hinge.SetValueFloat("Velocity", -speed);
                        }
                        else
                        {
                            hinge.SetValueFloat("Velocity", Math.Min((-0.5f * speed), 4f));
                        }
                        return false;

                    }
                    else
                    {
                        if (dif > precision * speed)
                        {
                            hinge.SetValueFloat("Velocity", speed);
                        }
                        else
                        {
                            hinge.SetValueFloat("Velocity", Math.Min((0.5f * speed), 4f));
                        }
                    }
                    return false;
                }
                else if (SimplifyAngle(hinge.Angle) < rotation - 0.05f)
                {
                    if (dif < 3.12f)
                    {
                        if (dif > precision * speed)
                        {
                            hinge.SetValueFloat("Velocity", speed);
                        }
                        else
                        {
                            hinge.SetValueFloat("Velocity", Math.Min((0.5f * speed), 4f));
                        }
                        return false;
                    }
                    else
                    {
                        if (dif > precision * speed)
                        {
                            hinge.SetValueFloat("Velocity", -speed);
                        }
                        else
                        {
                            hinge.SetValueFloat("Velocity", Math.Min((-0.5f * speed), 4f));
                        }
                        return false;
                    }
                }
                else
                {
                    hinge.SetValueFloat("Velocity", 0f);
                    return true;
                }
            }
        }
    }
}
