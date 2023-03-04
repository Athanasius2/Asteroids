using Godot;
using System;
using static LoopingRigidBody2D;


public class Asteroid : LoopingRigidBody2D
{
	//destroy = true if an astroid was just destroyed
	public static bool destroy {get; set;} = false;
	//set to position of last destroyed asteroid
	public static Vector2 LastDestroyedPosition = new Vector2();
	public int stage {get; set;} 
	//Create random numbers to intialize this asteroid.
	RandomNumberGenerator rng = new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//So we don't get deterministic asteroids
		rng.Randomize();
		//random starting velocity
		this.LinearVelocity = 
			new Vector2(rng.RandfRange(-100, 100), rng.RandfRange(-100, 100));
		//random starting rotation
		this.Rotation = rng.RandfRange(-100, 100);
		//random angular velocity (how fast it spins)
		this.AngularVelocity = rng.RandfRange(-1, 1);
	}
	
	//Do physics on the asteroid every physics frame.
	public override void _IntegrateForces(Physics2DDirectBodyState state)
	{
		//if asteroid goes off edge, loop around.
		this.Loop(state);
	}

	//called when bullet and asteroid collide. Connection is created in Main.cs
	private void OnBulletHitAsteroid(Node body)
	{
		//Let Main know that an asteroid was destroyed
		Asteroid.destroy = true;
		//Save position of destroyed asteroid
		Asteroid.LastDestroyedPosition = body.GlobalPosition;
		body.QueueFree();
	}
}
