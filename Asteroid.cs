using Godot;
using System;
using System.Collections.Generic;
using static LoopingRigidBody2D;


public class Asteroid : LoopingRigidBody2D
{
	//This is how OnBulletHitAsteroid will tell Main to create new asteroids
	public static List<(Vector2 position, int stage)> newAsteroids 
	{
		//newAsteroids should be cleared when it is accessed
		get { var ret = new List<(Vector2, int)>(newAsteroids); newAsteroids.Clear(); return ret; }
		private set { newAsteroids = value; }
	}
	public int stage { set; get; }
	private RandomNumberGenerator rng = new RandomNumberGenerator(); 
	
	static Asteroid()
	{
		newAsteroids = new List<(Vector2, int)>(); 
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng.Randomize();
		this.LinearVelocity = 
			new Vector2(rng.RandfRange(-100, 100), rng.RandfRange(-100, 100));
		this.Rotation = rng.RandfRange(-100, 100);
		this.AngularVelocity = rng.RandfRange(-1, 1);
	}

	//called when bullet and asteroid collide. Connection is created in Main.cs
	private void OnBulletHitAsteroid(Node body)
	{
		//have to make sure that body == this, because EVERY instance
		//of Asteroid will call this when any individual instance is hit
		if (body == this)
		{
			//Create two new asteroids in place of the one we just hit
			if (this.stage < 3)
			{
				GD.Print(this);
				newAsteroids.Add((new Vector2( ((RigidBody2D) body).GlobalPosition ), stage + 1));
 				newAsteroids.Add((new Vector2( ((RigidBody2D) body).GlobalPosition ), stage + 1));
			}
			//destroy asteroid we just hit
			body.QueueFree();
		}
	}
}
