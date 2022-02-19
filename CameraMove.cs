// Copyright (c) Dibertz Soft contributors (https://dibertz.com) and Conectividad (Bs.As.) Argentina.
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Godot;
using System;

public class CameraMove : Camera
{

    private float Speed;
    [Export]
    public float MinSpeed = 0.017f;
    [Export]
    public float MaxSpeed = 0.02f;
    private int Border = 10;
    private Position3D Go;
    private Vector2 MarginLimit;
    private Vector2 MiddleScreen;
    private Vector2 Move;
    [Export]
    public float Acceleration = 15.0f;
    private Vector3 TargetPos;
    private float Trim = 1.95f;

    private Camera RTSCamera;
    private float SizeCamera;
    private float AverageZoom = 12;

    private bool JRPGCamera = true;
    public MeshInstance TargetToFollow; // Player Instance Required

    public override void _Ready()
    {
        RTSCamera = GetNode<Camera>("/root/SampleScene/TrackingCamera/Camera");
        Go = GetNode<Position3D>("/root/SampleScene/TrackingCamera");
        TargetToFollow = GetNode<MeshInstance>("/root/SampleScene/Player");
        MarginLimit = new Vector2(GetViewport().GetVisibleRect().Size.x - Border, GetViewport().GetVisibleRect().Size.y - Border);
        MiddleScreen = new Vector2(GetViewport().GetVisibleRect().Size.x / 2, GetViewport().GetVisibleRect().Size.y / 2);
        SizeCamera = AverageZoom;
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("ui_camera"))
        {
            JRPGCamera = JRPGCamera ? false : true;
        }

        if (!JRPGCamera)
        {
            Move = GetViewport().GetMousePosition() - MiddleScreen;
            Move.Normalized();

            /*------------------------- -
                - Move Mouse Controller -
                ----------------------- -*/
            if ((GetViewport().GetMousePosition().x < Border) && (GetViewport().GetMousePosition().y < Border) || (GetViewport().GetMousePosition().x < Border) && (GetViewport().GetMousePosition().y > MarginLimit.y)
            || (GetViewport().GetMousePosition().y < Border) && (GetViewport().GetMousePosition().x > MarginLimit.x) || (GetViewport().GetMousePosition().y > MarginLimit.y) && (GetViewport().GetMousePosition().x > MarginLimit.x))
            {
                Speed = Mathf.Lerp(Speed, MaxSpeed, delta * Acceleration);
                if ((GetViewport().GetMousePosition().x < Border) && (GetViewport().GetMousePosition().y < Border) || (GetViewport().GetMousePosition().x > MarginLimit.x) && (GetViewport().GetMousePosition().y > MarginLimit.y))
                {
                    TargetPos += new Vector3(Move.x * delta * Speed, .0f, .0f);
                    Go.Translation = TargetPos;

                }
                else if ((GetViewport().GetMousePosition().x > MarginLimit.x) && (GetViewport().GetMousePosition().y < Border) || (GetViewport().GetMousePosition().y > MarginLimit.y) && (GetViewport().GetMousePosition().x < Border))
                {
                    TargetPos += new Vector3(.0f, .0f, -Move.x * delta * Speed);
                    Go.Translation = TargetPos;
                }
            }
            else if ((GetViewport().GetMousePosition().x < Border) || (GetViewport().GetMousePosition().x > MarginLimit.x))
            {
                Speed = Mathf.Lerp(Speed, MaxSpeed, delta * Acceleration);
                TargetPos += new Vector3((Move.x / Trim) * delta * Speed, .0f, (-Move.x / Trim) * delta * Speed);
                Go.Translation = TargetPos;
            }
            else if ((GetViewport().GetMousePosition().y < Border) || (GetViewport().GetMousePosition().y > MarginLimit.y))
            {
                Speed = Mathf.Lerp(Speed, MaxSpeed, delta * Acceleration);
                TargetPos += new Vector3(.0f, -Move.y * delta * Speed, .0f);
                Go.Translation = TargetPos;
            }
            else
            {
                Speed = Mathf.Lerp(Speed, MinSpeed, delta * Acceleration);
            }

            /*------------------------- -
                - Zoom Mouse Controller -
                ----------------------- -*/
            if (Input.IsActionJustReleased("ui_scroll_down")) // Zoom decrement (-)
            {
                if (SizeCamera == AverageZoom)
                {
                    SizeCamera = AverageZoom * 1.75f; // - : Camera[]Zoom x3
                }
                else if (SizeCamera == AverageZoom / 2)
                {
                    SizeCamera = AverageZoom; // Normal : Camera[]Zoom x1
                    MinSpeed *= 2; MaxSpeed *= 2;
                }
            }
            else if (Input.IsActionJustReleased("ui_scroll_up")) // Zoom increment (+)
            {
                if (SizeCamera == AverageZoom)
                {
                    SizeCamera = AverageZoom / 2; // + : Camera[]Zoom x2
                    MinSpeed /= 2; MaxSpeed /= 2;
                }
                else if ((SizeCamera < AverageZoom) && (SizeCamera > AverageZoom / 2))
                {
                    SizeCamera = AverageZoom; // - : Camera[]Zoom x1
                }
                else if (SizeCamera == AverageZoom * 1.75f)
                {
                    SizeCamera = AverageZoom; // + : Camera[]Zoom x1
                }
            }
            else
            {
                RTSCamera.Size = Mathf.Lerp(RTSCamera.Size, SizeCamera, (Acceleration * delta) / 3);
            }
        }
        else{

            /*------------------------- -
                - RPG Camera Controller -
                ----------------------- -*/
            TargetPos = new Vector3(TargetToFollow.Translation.x, TargetToFollow.Translation.y, TargetToFollow.Translation.z);
            Go.Translation = Go.Translation.LinearInterpolate(TargetPos, delta * 4.0f);
        }
    }
}
