using Godot;
using System;

using static LoopingRigidBody2D;

public class Player : LoopingRigidBody2D
{
	private Vector2 _thrust = new Vector2(0, -2);
	private float _torque = 40.0F;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//sets ship to center of screen
		this.GlobalPosition = GetViewportRect().Size / 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		this.Loop(state);
		
		if(Input.IsActionPressed("ui_up"))
			state.ApplyCentralImpulse(_thrust.Rotated(this.Rotation));
		
		if(Input.IsActionPressed("ui_left"))
			state.ApplyTorqueImpulse(_torque * -1.0F);
		
		if(Input.IsActionPressed("ui_right"))
			state.ApplyTorqueImpulse(_torque);
	}
	
	public override void _Process(float delta)
	{
	}
	
	private void OnAsteroidHitShip(Node body)
	{
		GD.Print("Boom!!!");
		body.QueueFree();
	}
}
