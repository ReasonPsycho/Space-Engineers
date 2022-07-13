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
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set RuntimeInfo.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public float RangeFinder(IMyCameraBlock c, float maxRange, float angle)
        {
            var info = c.Raycast(maxRange, angle, 0);
            if (info.Type != 0)
            {
                panelText += "RangeFinder: " + Vector3.Distance(c.GetPosition(), info.HitPosition.Value).ToString() + "\n";
                return Vector3.Distance(c.GetPosition(), info.HitPosition.Value);
            }
            else
            {
                return maxRange;
            }
        }

        public void RotateTheAnkleAndMainBody(IMyMotorAdvancedStator m, IMyMotorAdvancedStator a, Vector3 mI, Vector2 rI)
        {
            if (mI.X == 0)
            {
                if (rI.X == 0)
                {
                    a.SetValueFloat("Velocity", 0f);
                    m.SetValueFloat("Velocity", 0f);
                }
                else
                {
                    m.SetValueFloat("Velocity", rI.Y);
                }
            }
            else
            {
                if (rI.X == 0)
                {
                    a.SetValueFloat("Velocity", mI.X * 10f);
                    m.SetValueFloat("Velocity", -mI.X * 10f);
                }
                else
                {
                    a.SetValueFloat("Velocity", mI.X * 10f);
                    m.SetValueFloat("Velocity", (rI.Y / 10f + mI.X) * 10f);
                }
            }
        }

        public void ResetRotorRotation(IMyMotorAdvancedStator r)
        {
            if (r.Angle <= 3.14f)
            {
                if (r.Angle < 0.4f)
                {
                    r.SetValueFloat("Velocity", 2f);

                }
                else
                {
                    r.SetValueFloat("Velocity", 10f);

                }
            }
            else if (r.Angle > 3.14f)
            {
                if (r.Angle > 5.8f)
                {
                    r.SetValueFloat("Velocity", -2f);
                }
                else
                {
                    r.SetValueFloat("Velocity", -10f);
                }
            }

            if (r.Angle > 6.17f || r.Angle < 0.05f)
            {
                r.SetValueFloat("Velocity", 0f);
            }
        }

        public bool MoveHingeToRotation(float time, float speed, float rotation, IMyMotorAdvancedStator hinge)
        {
            float dif = Math.Abs(hinge.Angle - rotation);
            if (hinge.Angle > rotation + 0.01f)
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
            else if (hinge.Angle < rotation - 0.01f)
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
                hinge.SetValueFloat("Velocity", 0f);
                return true;
            }
        }


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
                panelText += "Dif: " + dif.ToString() + "\n";
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

        public void CorrectAnklePosition(IMyMotorAdvancedStator h, IMyCameraBlock f, IMyCameraBlock b)
        {
            var fInfo = f.Raycast(4f, -75f, 0);
            var bInfo = b.Raycast(4f, -75f, 0);
            if (fInfo.Type == 0 && bInfo.Type == 0)
            {
                return;
            }
            else if (fInfo.Type == 0)
            {
                h.SetValueFloat("Velocity", -4f);
            }
            else if (bInfo.Type == 0)
            {
                h.SetValueFloat("Velocity", 4f);
            }
            else
            {
                var difPosF = Vector3.Distance(f.GetPosition(), fInfo.HitPosition.Value);
                var difPosB = Vector3.Distance(b.GetPosition(), bInfo.HitPosition.Value);
                panelText += "Anlke Fdist: " + difPosF.ToString() + "\n";
                panelText += "Anlke Bdist: " + difPosB.ToString() + "\n";
                if (Math.Abs(difPosF - difPosB) < 0.0001f)
                {
                    h.SetValueFloat("Velocity", 0f);
                }
                else if (difPosF > difPosB)
                {
                    h.SetValueFloat("Velocity", -6f);
                }
                else
                {
                    h.SetValueFloat("Velocity", 6f);
                }
            }
        }

        float speed = 0f;
        float timer = 6f;

        bool isRotatingForward = true;
        bool isHalfwayThere = false;
        bool isLocked = false;

        float frontHipRotation = 0.8f;
        float frontKneeRotation = -0.8f;
        float frontAnkleRotation = 0f;

        float halfwayHipRotation = 0.4f;
        float halfwayKneeRotation = -1f;
        float halfwayAnkleRotation = 0f;

        float halfwayBackHipRotation = 0f;
        float halfwayBackKneeRotation = 0f;
        float halfwayBackAnkleRotation = 0f;

        float backHipRotation = -0.8f;
        float backKneeRotation = 0.6f;
        float backAnkleRotation = 0f;

        float backRotationToAdd = 1f;
        string panelText;
        public void Move(Vector3 moveIndicator, Vector2 rotationIndicator, List<IMyMotorAdvancedStator> hs, List<IMyMotorAdvancedStator> rs, List<IMyLandingGear> lgs, List<IMyCameraBlock> cams, List<IMyGyro> g)
        {
            if (-moveIndicator.Z > 0)
            {
                if (timer > 5)
                {
                    speed++;
                    timer = 0f;
                }
                else
                {
                    timer += 0.01f;
                }
            }
            else if (-moveIndicator.Z < 0)
            {
                if (timer > 5)
                {
                    speed--;
                    timer = 0f;
                }
                else
                {
                    timer += 0.01f;
                }
            }
            else
            {
                timer = 6f;
            }

            if (speed > 0)
            {
                var dif = (RangeFinder(cams[6], 15f, -80f) - RangeFinder(cams[7], 15f, -80f));
                panelText += "Range Diffrence: " + dif.ToString() + "\n";
                for (int i = 0; i < g.Count; i++)
                {
                    g[i].GyroOverride = true;
                    g[i].Pitch = -dif;
                }

                if (isRotatingForward)
                {
                    RotateTheAnkleAndMainBody(rs[2], rs[1], moveIndicator, rotationIndicator);
                    ResetRotorRotation(rs[0]);
                    if (isHalfwayThere)
                    {
                        float addedRotation = 0;
                        for (int i = 3; i < 6; i++)
                        {
                            if (!lgs[i].AutoLock)
                            {
                                lgs[i].ApplyAction("Autolock");
                            }
                            if (lgs[i].IsLocked)
                            {
                                addedRotation = 0;
                                isRotatingForward = false;
                                isHalfwayThere = false;
                                for (int j = 0; j < 3; j++)
                                {
                                    if (lgs[j].AutoLock)
                                    {
                                        lgs[j].ApplyAction("Autolock");
                                    }
                                    lgs[j].ApplyAction("Unlock");
                                }
                            }
                        }
                        if (MoveRotatorToRotation(1f, 8f * speed, -frontHipRotation, hs[0]))
                        {
                            var kneeInfo = cams[2].Raycast(3f, -60f, 0);
                            if (kneeInfo.Type != 0 || RangeFinder(cams[0], 10f, -30f) < 2f)
                            {
                                addedRotation = -backRotationToAdd;
                                MoveHingeToRotation(1f, 6f * speed, frontAnkleRotation, hs[2]);
                            }
                            else
                            {
                                addedRotation = backRotationToAdd;
                                CorrectAnklePosition(hs[2], cams[0], cams[1]);
                            }
                        }
                        else
                        {
                            MoveHingeToRotation(1f, 6f * speed, frontAnkleRotation, hs[2]);
                        }
                        // Front
                        MoveHingeToRotation(1f, 8f * speed, frontKneeRotation, hs[1]);
                        //Back
                        MoveRotatorToRotation(1f, 3f * speed, backHipRotation - addedRotation, hs[3]);
                        MoveHingeToRotation(1f, 2f * speed, backKneeRotation + addedRotation, hs[4]);
                        MoveHingeToRotation(1f, 2f * speed, backAnkleRotation + addedRotation, hs[5]);
                    }
                    else
                    {
                        if (MoveRotatorToRotation(1f, 8f * speed, -halfwayHipRotation, hs[0]))
                        {
                            isHalfwayThere = true;
                        }
                        //Halfway
                        MoveHingeToRotation(1f, 8f * speed, halfwayKneeRotation, hs[1]);
                        MoveHingeToRotation(1f, 5f * speed, halfwayAnkleRotation, hs[2]);
                        //Halfwayback
                        MoveRotatorToRotation(1f, 8f * speed, halfwayBackHipRotation, hs[3]);
                        MoveHingeToRotation(1f, 4f * speed, halfwayBackKneeRotation, hs[4]);
                        MoveHingeToRotation(1f, 4f * speed, halfwayBackAnkleRotation, hs[5]);

                    }
                }
                else
                {
                    RotateTheAnkleAndMainBody(rs[2], rs[0], moveIndicator, rotationIndicator);
                    ResetRotorRotation(rs[1]);
                    if (isHalfwayThere)
                    {
                        float addedRotation = 0;

                        for (int i = 0; i < 3; i++)
                        {
                            if (!lgs[i].AutoLock)
                            {
                                lgs[i].ApplyAction("Autolock");
                            }
                            if (lgs[i].IsLocked)
                            {
                                addedRotation = 0;
                                isRotatingForward = true;
                                isHalfwayThere = false;
                                for (int j = 3; j < 6; j++)
                                {
                                    if (lgs[j].AutoLock)
                                    {
                                        lgs[j].ApplyAction("Autolock");
                                    }
                                    lgs[j].ApplyAction("Unlock");
                                }
                            }
                        }

                        if (MoveRotatorToRotation(1f, 8f * speed, frontHipRotation, hs[3]))
                        {
                            var kneeInfo = cams[5].Raycast(3f, -60f, 0);
                            if (kneeInfo.Type != 0 || RangeFinder(cams[3], 10f, -30f) < 2f)
                            {
                                addedRotation = -backRotationToAdd;
                                MoveHingeToRotation(1f, 6f * speed, frontAnkleRotation, hs[5]);
                            }
                            else
                            {
                                addedRotation = backRotationToAdd;
                                CorrectAnklePosition(hs[5], cams[3], cams[4]);
                            }
                        }
                        else
                        {
                            MoveHingeToRotation(1f, 6f * speed, frontAnkleRotation, hs[5]);
                        }
                        //Back
                        MoveRotatorToRotation(1f, 3f * speed, -backHipRotation + addedRotation, hs[0]);
                        MoveHingeToRotation(1f, 2f * speed, backKneeRotation + addedRotation, hs[1]);
                        MoveHingeToRotation(1f, 2f * speed, backAnkleRotation + addedRotation, hs[2]);
                        //Front
                        MoveHingeToRotation(1f, 8f * speed, frontKneeRotation, hs[4]);

                    }
                    else
                    {
                        if (MoveRotatorToRotation(1f, 8f * speed, halfwayHipRotation, hs[3]))
                        {
                            isHalfwayThere = true;
                        }
                        //Halfway
                        MoveHingeToRotation(1f, 8f * speed, halfwayKneeRotation, hs[4]);
                        MoveHingeToRotation(1f, 5f * speed, halfwayAnkleRotation, hs[5]);
                        //halfwayback
                        MoveRotatorToRotation(1f, 8f * speed, -halfwayBackHipRotation, hs[0]);
                        MoveHingeToRotation(1f, 4f * speed, halfwayBackKneeRotation, hs[1]);
                        MoveHingeToRotation(1f, 4f * speed, halfwayBackAnkleRotation, hs[2]);
                    }
                }
            }
            else if (speed < 0)
            {
                if (isRotatingForward)
                {
                    if (MoveHingeToRotation(1f, 5f, 0.3f, hs[0]))
                    {
                        isRotatingForward = false;
                    }
                    MoveHingeToRotation(1f, 2.5f, 0.15f, hs[1]);
                }
                else
                {
                    if (MoveHingeToRotation(1f, 5f, -0.6f, hs[0]))
                    {
                        isRotatingForward = true;
                    }
                    MoveHingeToRotation(1f, 5f, 0.6f, hs[1]);
                }
            }
            else
            {
                for (int i = 0; i < g.Count; i++)
                {
                    g[i].GyroOverride = false;
                }

                bool isLeftLegLocked = false;
                bool isRightLegLocked = false;

                for (int i = 3; i < 6; i++)
                {
                    if (lgs[i].IsLocked)
                    {
                        isLeftLegLocked = true;
                        break;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    if (lgs[i].IsLocked)
                    {
                        isRightLegLocked = true;
                        break;
                    }
                }

                if ((isLeftLegLocked && isRightLegLocked) || isLeftLegLocked)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        lgs[i].ApplyAction("Unlock");
                        if (lgs[i].AutoLock)
                        {
                            lgs[i].ApplyAction("Autolock");
                        }
                    }
                    for (int i = 3; i < 6; i++)
                    {
                        if (!lgs[i].AutoLock)
                        {
                            lgs[i].ApplyAction("Autolock");
                        }
                    }
                    if (!isHalfwayThere)
                    {
                        //Back
                        MoveRotatorToRotation(1f, 3f, backHipRotation, hs[3]);
                        MoveHingeToRotation(1f, 3f, backKneeRotation, hs[4]);
                        MoveHingeToRotation(1f, 3f, backAnkleRotation, hs[5]);
                        //Front
                        MoveRotatorToRotation(1f, 3f, -frontHipRotation, hs[0]);
                        MoveHingeToRotation(1f, 3f, frontKneeRotation, hs[1]);
                        MoveHingeToRotation(1f, 3f, frontAnkleRotation, hs[2]);
                    }
                    else
                    {
                        //Bakc
                        MoveRotatorToRotation(1f, 3f, -backHipRotation, hs[0]);
                        MoveHingeToRotation(1f, 3f, backKneeRotation, hs[1]);
                        MoveHingeToRotation(1f, 3f, backAnkleRotation, hs[2]);
                        //Front
                        MoveRotatorToRotation(1f, 3f, frontHipRotation, hs[3]);
                        MoveHingeToRotation(1f, 3f, frontKneeRotation, hs[4]);
                        MoveHingeToRotation(1f, 3f, frontAnkleRotation, hs[5]);
                    }

                    ResetRotorRotation(rs[1]);
                    RotateTheAnkleAndMainBody(rs[2], rs[0], moveIndicator, rotationIndicator);

                }
                else
                {
                    for (int i = 3; i < 6; i++)
                    {
                        lgs[i].ApplyAction("Unlock");
                        if (lgs[i].AutoLock)
                        {
                            lgs[i].ApplyAction("Autolock");
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        if (!lgs[i].AutoLock)
                        {
                            lgs[i].ApplyAction("Autolock");
                        }
                    }
                    if (!isHalfwayThere)
                    {
                        //Bakc
                        MoveRotatorToRotation(1f, 3f, -backHipRotation, hs[0]);
                        MoveHingeToRotation(1f, 3f, backKneeRotation, hs[1]);
                        MoveHingeToRotation(1f, 3f, backAnkleRotation, hs[2]);
                        //Front
                        MoveRotatorToRotation(1f, 3f, frontHipRotation, hs[3]);
                        MoveHingeToRotation(1f, 3f, frontKneeRotation, hs[4]);
                        MoveHingeToRotation(1f, 3f, frontAnkleRotation, hs[5]);
                    }
                    else
                    {
                        //Back
                        MoveRotatorToRotation(1f, 3f, backHipRotation, hs[3]);
                        MoveHingeToRotation(1f, 3f, backKneeRotation, hs[4]);
                        MoveHingeToRotation(1f, 3f, backAnkleRotation, hs[5]);
                        //Front
                        MoveRotatorToRotation(1f, 3f, -frontHipRotation, hs[0]);
                        MoveHingeToRotation(1f, 3f, frontKneeRotation, hs[1]);
                        MoveHingeToRotation(1f, 3f, frontAnkleRotation, hs[2]);
                    }
                    ResetRotorRotation(rs[0]);
                    RotateTheAnkleAndMainBody(rs[2], rs[1], moveIndicator, rotationIndicator);
                }
            }
        }

        public void Main()
        {
            panelText = "";
            float speedModifere;
            float m = 0;

            var controller = GridTerminalSystem.GetBlockWithName("Control Seat") as IMyShipController;
            var cockpit = GridTerminalSystem.GetBlockWithName("Control Seat") as IMyCockpit;

            var panel = cockpit.GetSurface(0) as IMyTextSurface;
            panelText = speed + "\n" + "isRotatingForward: " + isRotatingForward.ToString() + "\n" + "isHalfwayThere " + isHalfwayThere.ToString() + "\n";

            m = controller.MoveIndicator.X * 10;
            speedModifere = controller.MoveIndicator.Z;

            List<IMyGyro> gyros = new List<IMyGyro>();
            List<IMyMotorAdvancedStator> hinges = new List<IMyMotorAdvancedStator>();
            List<IMyLandingGear> landingGears = new List<IMyLandingGear>();
            List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
            List<IMyMotorAdvancedStator> rotators = new List<IMyMotorAdvancedStator>();

            var hHingeR = GridTerminalSystem.GetBlockWithName("Hip Rotor Right") as IMyMotorAdvancedStator;
            hinges.Add(hHingeR); // i 
            var kHingeR = GridTerminalSystem.GetBlockWithName("Knee Hinge Right") as IMyMotorAdvancedStator;
            hinges.Add(kHingeR); // i + 1
            var aHingeR = GridTerminalSystem.GetBlockWithName("Ankle Hinge Right") as IMyMotorAdvancedStator;
            hinges.Add(aHingeR); // i + 2

            var hHingeL = GridTerminalSystem.GetBlockWithName("Hip Rotor Left") as IMyMotorAdvancedStator;
            hinges.Add(hHingeL); // 2i
            var kHingeL = GridTerminalSystem.GetBlockWithName("Knee Hinge Left") as IMyMotorAdvancedStator;
            hinges.Add(kHingeL); // 2i + 1
            var aHingeL = GridTerminalSystem.GetBlockWithName("Ankle Hinge Left") as IMyMotorAdvancedStator;
            hinges.Add(aHingeL); // 2i + 2

            var legRotorR = GridTerminalSystem.GetBlockWithName("Right Leg Rotor") as IMyMotorAdvancedStator;
            rotators.Add(legRotorR);
            var legRotorL = GridTerminalSystem.GetBlockWithName("Left Leg Rotor") as IMyMotorAdvancedStator;
            rotators.Add(legRotorL);
            var rotor = GridTerminalSystem.GetBlockWithName("Main Rotor") as IMyMotorAdvancedStator;
            rotators.Add(rotor);

            var fFeetCameraR = GridTerminalSystem.GetBlockWithName("Feet Right Front Camera") as IMyCameraBlock;
            cameras.Add(fFeetCameraR);
            var bFeetCameraR = GridTerminalSystem.GetBlockWithName("Feet Right Back Camera") as IMyCameraBlock;
            cameras.Add(bFeetCameraR);
            var kneeCameraR = GridTerminalSystem.GetBlockWithName("Knee Right Camera") as IMyCameraBlock;
            cameras.Add(kneeCameraR);

            var fFeetCameraL = GridTerminalSystem.GetBlockWithName("Feet Left Front Camera") as IMyCameraBlock;
            cameras.Add(fFeetCameraL);
            var bFeetCameraL = GridTerminalSystem.GetBlockWithName("Feet Left Back Camera") as IMyCameraBlock;
            cameras.Add(bFeetCameraL);
            var kneeCameraL = GridTerminalSystem.GetBlockWithName("Knee Left Camera") as IMyCameraBlock;
            cameras.Add(kneeCameraL);

            var fCrouchCamera = GridTerminalSystem.GetBlockWithName("Front Crouch Camera") as IMyCameraBlock;
            cameras.Add(fCrouchCamera);
            var bCrouchCamera = GridTerminalSystem.GetBlockWithName("Back Crouch Camera") as IMyCameraBlock;
            cameras.Add(bCrouchCamera);

            for (int i = 0; i < 6; i++)
            {
                landingGears.Add(GridTerminalSystem.GetBlockWithName("Landing Gear " + i.ToString()) as IMyLandingGear);
            }

            for (int i = 0; i < 4; i++)
            {
                gyros.Add(GridTerminalSystem.GetBlockWithName("Hip Gyroscope " + i.ToString()) as IMyGyro);
            }

            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].EnableRaycast = true;
            }

            Move(controller.MoveIndicator, controller.RotationIndicator, hinges, rotators, landingGears, cameras, gyros);

            panelText += "Hinges positions: " + "\n";
            for (int i = 0; i < hinges.Count; i++)
            {
                panelText += hinges[i].CustomName + " - Angle: " + Math.Round(hinges[i].Angle, 4).ToString() + "\n";
            }

            for (int i = 0; i < rotators.Count; i++)
            {
                panelText += rotators[i].CustomName + " - Angle: " + Math.Round(rotators[i].Angle, 4).ToString() + "\n";
            }
            panelText += controller.RotationIndicator.ToString();
            Echo(panelText);
            panel.WriteText(panelText, false);
        }
    }
}
