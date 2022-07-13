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

            public bool MoveRotatorToRotation(float time, float speed, float rotation, IMyMotorAdvancedStator hinge)
            {
                float dif = Math.Abs(SimplifyAngle(hinge.Angle) - rotation);
                if (SimplifyAngle(hinge.Angle) > rotation + 0.05f)
                {
                    if (dif < 3.12f)
                    {
                        if (dif > 0.01f * speed)
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
                        if (dif > 0.01f * speed)
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
                        if (dif > 0.01f * speed)
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
                        if (dif > 0.01f * speed)
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
