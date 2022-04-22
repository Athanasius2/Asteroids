using Godot;
using System;
using static LoopingRigidBody2D;


public class Asteroid : LoopingRigidBody2D
{
	//Create random numbers to intialize this asteroid.
	RandomNumberGenerator rng = new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//So we don't get deterministic asteroids
		rng.Randomize();
		//Start the asteroid somewhere within the range of the viewport.
		//Might have to start handling this in Main.cs so we know where to start
		//the the smaller asteroids that the bigger ones break into.
		this.GlobalPosition = 
			new Vector2(rng.RandfRange(0, GetViewportRect().Size.x), 
				rng.RandfRange(0, GetViewportRect().Size.y));
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
		//Destroy this asteroid.
		body.QueueFree();
	}
}
