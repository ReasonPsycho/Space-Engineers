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

            public bool CheckIfPassesThrought(float from, float to,float dierection,float upperLimit = float.MaxValue, float lowerLimit = float.MaxValue)
            {
                float distance = from - to;
                if(dierection < 0) 
                {
                    if(from - distance < 0) //Check if it's gonna do full rotation    // it can go throught here buuuut limit can't so it makes it easier
                    {
                        if(from <= lowerLimit) // if it's below the limit
                        {
                            if(to <= lowerLimit) // if it's below we good
                            {
                                return false;
                            }
                            else  // in any other scenerio we damned
                            {
                                return true;
                            }
                         
                        }
                    }
                    else
                    {
                        if (from >= upperLimit) // if it's above the limit
                        {
                            if(to >= upperLimit) // if it's above we good
                            {
                                return false;
                            }else // in any other scenerio we damned
                            {
                                return true;
                            }
                       
                        }
                        //else if (from < upperLimit)// if it's below it we good                     
                    }
                }
                else
                {
                    if (from + distance > Math.PI*2) //Check if it's gonna do full rotation // it can go throught here buuuut limit can so it makes it easier
                    {
                        if (from >= upperLimit) // if it's belowe the limit
                        {
                            if (to >= upperLimit) // if it's below we good
                            {
                                return false;
                            }
                            else // in any other scenerio we damned
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (from <= lowerLimit) // if it's belowe the limit
                        {
                            if (to <= upperLimit) // if it's below we good
                            {
                                return false;
                            }
                            else // in any other scenerio we damned
                            {
                                return true;
                            }
                        }
                        //else if (from < upperLimit)// if it's below it we good    
                    }
                }
                return false;
            }
            public bool MoveRotorToRotation(float speedMultiplayer, float precision, float rotation, IMyMotorAdvancedStator rotor,bool rotorLock = true)
            {
                float upperLimit = rotor.UpperLimitRad;
                float lowerLimit = rotor.LowerLimitRad;

                if (upperLimit < 0)
                    upperLimit += (float)Math.PI * 2;

                if (lowerLimit < 0)
                    lowerLimit += (float)Math.PI * 2;

                if (lowerLimit > rotation && upperLimit < rotation) // Check if we even get there
                {
                    float distanceA = Math.Abs(rotation - lowerLimit); 
                    float distanceB = Math.Abs(rotation - upperLimit);
                    if (distanceA > distanceB) // if not just go to closer limit
                    {
                        rotation = upperLimit;
                    }
                    else
                    {
                        rotation = lowerLimit;
                    }
                }



                float angle1 = rotation;
                float angle2 = rotor.Angle;

                if (angle1 < 0)
                    angle1 += (float)Math.PI * 2;

                if (angle2 < 0)
                    angle2 += (float)Math.PI * 2;

                float distance = Math.Abs(angle2 - angle1);
                float speed = (float)Math.Max(Math.Log(Math.Abs(distance) + 2 - speedMultiplayer / 10 -precision) * speedMultiplayer, 0.1f ); //https://www.desmos.com/calculator/8rntlz6osp?lang=pl\
                log = "";
                log += "Name: " +rotor.CustomName + "\n"  + "distance: " + distance.ToString() + "\n";
                if (distance < precision/2)
                {
                    if (rotor.CustomData == "Being locked.")
                    {
                        rotor.SetValueFloat("Velocity", 0);
                        rotor.RotorLock = rotorLock;
                        rotor.CustomData = "Is locked.";
                        return false;
                    }
                    else
                    {
                        if(rotor.CustomData == "Is locked.")
                        {
                            return true;
                        }
                        else
                        {
                            rotor.SetValueFloat("Velocity", 0);
                            rotor.RotorLock = rotorLock;
                            rotor.CustomData = "Being locked.";
                            return false;
                        }                       
                    }                              
                }
                else
                {
                    rotor.RotorLock = false;
                    rotor.CustomData = "";
                }
               

                log += "angle1: " + angle1.ToString() + "\nangle2: " + angle2.ToString() + "\n";
                if (angle2 > angle1 && angle2 - angle1 <= (float)Math.PI)   //go clockwise
                {
                    if (!CheckIfPassesThrought(angle2, angle1, -1, upperLimit, lowerLimit))
                    {
                        rotor.SetValueFloat("Velocity", -speed);
                    }
                    else
                    {
                        rotor.SetValueFloat("Velocity", speed); // that isn't correct realy but hey it works
                    }
                }

                else if (angle2 > angle1 && angle2 - angle1 > Math.PI) //go counter clockwise
                {
                    if (!CheckIfPassesThrought(angle2, angle1, 1, upperLimit, lowerLimit))
                    {
                        rotor.SetValueFloat("Velocity", speed);
                    }
                    else
                    {
                        rotor.SetValueFloat("Velocity", -speed); // that isn't correct realy but hey it works
                    }
                }
 
                else if (angle1 > angle2 && angle1 - angle2 <= Math.PI)  //go counter clockwise
                {
                    if (!CheckIfPassesThrought(angle2, angle1, 1, upperLimit, lowerLimit))
                    {
                        rotor.SetValueFloat("Velocity", speed);
                    }
                    else
                    {
                        rotor.SetValueFloat("Velocity", -speed); // that isn't correct realy but hey it works
                    }
                }
 
                else if (angle1 > angle2 && angle1 - angle2 > Math.PI)   //go clockwise
                {
                    if (!CheckIfPassesThrought(angle2, angle1, -1, upperLimit, lowerLimit))
                    {
                        rotor.SetValueFloat("Velocity", -speed);
                    }
                    else
                    {
                        rotor.SetValueFloat("Velocity", speed); // that isn't correct realy but hey it works
                    }
                }

                log += "velocity: " + rotor.TargetVelocityRad.ToString() + "\n";
                log += "locked:" + rotor.RotorLock.ToString() + "\n";
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
