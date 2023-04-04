using Godot;
using System;
using System.Collections.Generic;
using static LoopingRigidBody2D;


public class Asteroid : LoopingRigidBody2D
{
	//This is how OnBulletHitAsteroid will tell Main to create new asteroids
	public static List<(Vector2 position, int stage)> newAsteroids = new List<(Vector2, int)>();
	
	public int stage { set; get; }
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private CollisionPolygon2D AsteroidPolygon = new CollisionPolygon2D();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng.Randomize();
		LinearVelocity = 
			new Vector2(rng.RandfRange(-100, 100), rng.RandfRange(-100, 100));
		Rotation = rng.RandfRange(-100, 100);
		AngularVelocity = rng.RandfRange(-1, 1);

		AsteroidPolygon.Polygon = CreatePolygon();
		AddChild(AsteroidPolygon);
	}

	public override void _Draw()
	{
		DrawPolygon(AsteroidPolygon.Polygon, new Color[] {new Color("FFFFFF")});
	}

	//TODO implement procedurally generated polygons
	private Vector2[] CreatePolygon()
	{
		int scale;
		if (stage == 1)
			scale = 50;
		else if (stage == 2)
			scale = 35;
		else
			scale = 20;
		
		var poly = new Vector2[]
		{
			new Vector2(-scale, -scale),
			new Vector2(scale, -scale),
			new Vector2(scale, scale),
			new Vector2(-scale, scale)
		};
		return poly;
	}

	//called when bullet and asteroid collide. Connection is created in Main.cs
	private void OnBulletHitAsteroid(Node body)
	{
		//have to make sure that body == this, because EVERY instance
		//of Asteroid will call this when any individual instance is hit
		if (body == this)
		{
			//Create two new asteroids in place of the one we just hit
			if (stage < 3)
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
