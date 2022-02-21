// Copyright (c) Dibertz Soft contributors (https://dibertz.com) and Conectividad (Bs.As.) Argentina.
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Godot;
using System;

public class Player : KinematicBody
{
    private float Speed = 0.85f;
    private Vector3 Direction;
    private Vector3 _velocity = Vector3.Zero;
    private int FallAcceleration = 35;

    public override void _PhysicsProcess(float delta)
    {

        Direction = Vector3.Zero;

        if (Input.IsActionPressed("ui_left"))
        {
            Direction.x -= 1f;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            Direction.x += 1f;
        }

        if (Input.IsActionPressed("ui_up"))
        {
            Direction.z -= 1f;
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            Direction.z += 1f;
        }

        if (Direction != Vector3.Zero)
        {
            Direction = Direction.Normalized();
            GetNode<Spatial>("Pivot").LookAt(Translation + Direction, Vector3.Up);

            _velocity.x = Direction.x * Speed;
            _velocity.z = Direction.z * Speed;

            //_velocity.y -= FallAcceleration * delta;
            _velocity = MoveAndSlide(_velocity, Vector3.Up);
        }
    }
}
