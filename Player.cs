using Godot;
using System;

using static LoopingRigidBody2D;

public class Player : LoopingRigidBody2D
{
	private Vector2 _thrust = new Vector2(0, -2);
	private float _torque = 40.0F;
	private CollisionPolygon2D playerPolygon = new CollisionPolygon2D();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//sets ship to center of screen
		playerPolygon.Polygon = new Vector2[]{
			new Vector2(0, -25),
			new Vector2(-12.5f, 25),
			new Vector2(12.5f, 25)
		};
		AddChild(playerPolygon);
		this.GlobalPosition = GetViewportRect().Size / 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		base._IntegrateForces(state);
		
		if(Input.IsActionPressed("ui_up"))
			state.ApplyCentralImpulse(_thrust.Rotated(this.Rotation));
		
		if(Input.IsActionPressed("ui_left"))
			state.ApplyTorqueImpulse(_torque * -1.0F);
		
		if(Input.IsActionPressed("ui_right"))
			state.ApplyTorqueImpulse(_torque);
	}
	
	public override void _Draw()
	{
		DrawPolygon(ship.Polygon, new Color[] {new Color("FFFFFF")});
	}
	
	private void OnAsteroidHitShip(Node body)
	{
		body.QueueFree();
	}
}
