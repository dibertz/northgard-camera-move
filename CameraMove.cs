// Copyright (c) Dibertz Soft contributors (https://dibertz.com) and Conectividad (Bs.As.) Argentina.
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Godot;
using System;

public class CameraMove : Camera
{
    private float Speed;
    [Export]
    public float InitSpeed = 6.54f;
    [Export]
    public float SpeedRatio = 0.016f;
    private int Border = 30;
    private Position3D Go;
    private Vector2 MarginLimit;
    private Vector2 MiddleScreen;
    private Vector2 Move;
    [Export]
    public float ZoomRatio = 12.0f;
    private Vector3 TargetPos;
    private float Trim = 2.55f;

    private Camera RTSCamera;
    private float SizeCamera;
    private float AverageZoom = 12;

    private bool JRPGCamera = false;
    private KinematicBody TargetToFollow; // Player Instance Required
    [Export]
    public float MapClamping = 9.0f;
    private Label UILabel;

    public override void _Ready()
    {
        RTSCamera = GetNode<Camera>("/root/SampleScene/TrackingCamera/Camera");
        Go = GetNode<Position3D>("/root/SampleScene/TrackingCamera");
        TargetToFollow = GetNode<KinematicBody>("/root/SampleScene/Player");
        UILabel = GetNode<Label>("/root/SampleScene/Debug/Label");
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
            if ((GetViewport().GetMousePosition().x < Border * 2) && (GetViewport().GetMousePosition().y < Border * 2) || (GetViewport().GetMousePosition().x < Border * 2) && (GetViewport().GetMousePosition().y > MarginLimit.y)
            || (GetViewport().GetMousePosition().y < Border * 2) && (GetViewport().GetMousePosition().x > MarginLimit.x) || (GetViewport().GetMousePosition().y > MarginLimit.y) && (GetViewport().GetMousePosition().x > MarginLimit.x))
            {
                Speed = Mathf.Lerp(Speed, SpeedRatio, delta * InitSpeed + 0.1f);
                if (((GetViewport().GetMousePosition().x < Border * 2) && (GetViewport().GetMousePosition().y < Border * 2) && (TargetPos.z > -MapClamping / 1.1)) || ((GetViewport().GetMousePosition().x > MarginLimit.x) && (GetViewport().GetMousePosition().y > MarginLimit.y) && (TargetPos.z < MapClamping / 1.1)))
                {
                    TargetPos += new Vector3((Move.y /  1.95f) * delta * Speed, .0f, (Move.y / 1.95f) * delta * Speed); // Top Corner Left [ \ ] Bottom Corner Right
                    Go.Translation = TargetPos;
                }
                else if (((GetViewport().GetMousePosition().x > MarginLimit.x) && (GetViewport().GetMousePosition().y < Border * 2) && (TargetPos.z > -MapClamping / 1.1)) || ((GetViewport().GetMousePosition().y > MarginLimit.y) && (GetViewport().GetMousePosition().x < Border * 2) && (TargetPos.z < MapClamping / 1.1)))
                {
                    TargetPos += new Vector3((-Move.y /  1.95f) * delta * Speed, .0f, (Move.y /  1.95f) * delta * Speed); // Top Corner Right [ / ] Bottom Corner Left
                    Go.Translation = TargetPos;
                }
                else
                {
                    Speed = 0;
                }
            }
            else if (((GetViewport().GetMousePosition().x < Border) && (TargetPos.x > -MapClamping)) || ((GetViewport().GetMousePosition().x > MarginLimit.x) && (TargetPos.x < MapClamping)))
            {
                Speed = Mathf.Lerp(Speed, SpeedRatio, delta * InitSpeed);
                TargetPos += new Vector3((Move.x / Trim) * delta * Speed, .0f, .0f);
                Go.Translation = TargetPos;
            }
            else if (((GetViewport().GetMousePosition().y < Border) && (TargetPos.z > -MapClamping)) || ((GetViewport().GetMousePosition().y > MarginLimit.y) && (TargetPos.z < MapClamping)))
            {
                Speed = Mathf.Lerp(Speed, SpeedRatio, delta * InitSpeed);
                TargetPos += new Vector3(.0f, .0f, Move.y * delta * Speed);
                Go.Translation = TargetPos;
            }
            else
            {
                Speed = 0;
            }

            UILabel.Text = "Speed: "+ Speed.ToString();

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
                    InitSpeed *= 2; SpeedRatio *= 2;
                }
            }
            else if (Input.IsActionJustReleased("ui_scroll_up")) // Zoom increment (+)
            {
                if (SizeCamera == AverageZoom)
                {
                    SizeCamera = AverageZoom / 2; // + : Camera[]Zoom x2
                    InitSpeed /= 2; SpeedRatio /= 2;
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
                RTSCamera.Size = Mathf.Lerp(RTSCamera.Size, SizeCamera, (ZoomRatio * delta) / 3);
            }
        }
        else
        {

            /*------------------------- -
                - RPG Camera Controller -
                ----------------------- -*/
            TargetPos = new Vector3(TargetToFollow.Translation.x, TargetToFollow.Translation.y, TargetToFollow.Translation.z);
            Go.Translation = Go.Translation.LinearInterpolate(TargetPos, delta * 4.0f);
        }
    }
}
