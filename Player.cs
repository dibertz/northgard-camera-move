// Copyright (c) Dibertz Soft contributors (https://dibertz.com) and Conectividad (Bs.As.) Argentina.
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Godot;
using System;

public class Player : MeshInstance
{
    private float Speed = 5.0f;
    private float XMove;
    private float YMove;

    public override void _Process(float delta)
    {

        /*------------------- -
            - Move Controller -
            ----------------- -*/

        // -----------------------------------------------------------------
        
        if (Input.GetActionRawStrength("ui_left") > 0)
        {
            XMove += Input.GetActionRawStrength("ui_left") * Speed * delta;
        }
        else if  (Input.GetActionRawStrength("ui_right") > 0)
        {
            XMove -= Input.GetActionRawStrength("ui_right") * Speed * delta;
        }

        // -----------------------------------------------------------------

        if (Input.GetActionRawStrength("ui_up") > 0)
        {
            YMove -= Input.GetActionRawStrength("ui_up") * Speed * delta;
        }
        else if  (Input.GetActionRawStrength("ui_down") > 0)
        {
            YMove += Input.GetActionRawStrength("ui_down") * Speed * delta;
        }
        Translation = new Vector3(YMove, Translation.y, XMove);

        // -----------------------------------------------------------------

    }
}
