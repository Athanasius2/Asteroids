using Godot;
using System;
using System.Collections.Generic;
using static LoopingRigidBody2D;


public class Asteroid : LoopingRigidBody2D
{
	//Keep track of asteroids we have created
	public static List<Asteroid> asteroids {get; private set;} = new List<Asteroid>();
	//Main will use this list to add newly created asteroids to Tree
	public static List<Asteroid> newAsteroids {get; private set;} = new List<Asteroid>();
	public int stage {get; private set;} 
	//Create random numbers to intialize this asteroid.
	RandomNumberGenerator rng = new RandomNumberGenerator();
	
	//int number is the number of asteroids we are adding
	//int stage is the asteroid stage, 1 being the first and biggest stage,
	// 3 being the smallest and final stage.
	//When an asteroid is shot, it is destroyed and replaced by two asteroids
	// of the proceeding stage. 
	public static void addAsteroids(List<Vector2> positions, int stage)
	{
		foreach (Vector2 pos in positions)
		{
			//Use the Asteroid scene to create a new instance.
			Asteroid ast = (Asteroid) Main.asteroidScene.Instance();

			//set position and stage before adding to tree
			ast.GlobalPosition = pos;
			ast.stage = stage;
			//Add to newAsteroids so Main can add to Tree.
			newAsteroids.Add(ast);
		}
	}
	
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
		asteroids.Add(this);
	}

	//make sure to remove Asteroids from list before they get destroyed
	public override void _ExitTree()
	{
		Asteroid.asteroids.Remove(this);
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
				List<Vector2> positions = new List<Vector2>();
				positions.Add(new Vector2( ((RigidBody2D)body).GlobalPosition ));
 				positions.Add(new Vector2( ((RigidBody2D)body).GlobalPosition ));
				addAsteroids(positions, this.stage + 1);
			}
			//destroy asteroid we just hit
			body.QueueFree();
		}
	}
}
