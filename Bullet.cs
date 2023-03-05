using Godot;
using System;
using static LoopingRigidBody2D;

public class Bullet : LoopingRigidBody2D
{
	//keep track of the age of the bullet so we know when to delete it.
	private float age = 0;
	private float MAX_AGE = 2f;
	
	public override void _Ready()
	{

	}

	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		this.Loop(state);
		//GD.Print(this.LinearVelocity);
	}
	
	public override void _Process(float delta)
	{
		age += delta;
		if (age > MAX_AGE) QueueFree();
	}
	
	//
	private void OnAsteroidHitBullet(Node body)
	{
		GD.Print("Bullet destroyed");
		body.QueueFree();
	}
}
