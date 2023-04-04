using Godot;
using System;
using System.Collections.Generic;


public class Main : Node2D
{
	/* When scenes are loaded from the disk, they are stored in the PackedScene
	class, which we use to create new instances. */
	private PackedScene playerScene = GD.Load<PackedScene>("res://Player.tscn");
	private PackedScene asteroidScene = GD.Load<PackedScene>("res://Asteroid.tscn");
	private PackedScene bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
	
	RandomNumberGenerator rng = new RandomNumberGenerator();
	
	//Keep track of asteroids we have created so we can connect them with bullets
	private List<Asteroid> asteroids = new List<Asteroid>();

	private Player player;
	private Vector2 bulletVelocity = new Vector2(0, -200);
	private float fireRate = 2f;	//bullets per second
	private float secondsSinceLastFire = 0;
	private int numInitAsteroids = 2; //number of asteroids to spawn on startup
	private int initStage = 1;	//starting stage of asteroids
	private int level = 0; //what level the player has reached in the game

	/// <summary>
	/// Create new Asteroids and add them to newAsteroids list
	/// </summary>
	/// <param name="asteroids">List of tuples containing the positions and stages 
	/// of asteroids to be created</param>
	private void addAsteroids(List<(Vector2 position , int stage)> newAsteroids)
	{
		foreach (var a in newAsteroids)
		{
			Asteroid ast = (Asteroid) asteroidScene.Instance();

			ast.GlobalPosition = a.position;
			ast.stage = a.stage;
			//destroy player when ast hits it
			ast.Connect("body_entered", player, "OnAsteroidHitShip");
			//remove ast from asteroids list when it is destroyed
			ast.Connect("child_exiting_tree", this, "OnAsteroidExitTree");
			asteroids.Add(ast);
			this.AddChild(ast);
		}
	}

	/// <summary>
	/// Create stage one Asteroids at random positions
	/// </summary>
	/// <param name="n">Number of asteroids to create</param>
	private void addAsteroids(int n)
	{
		//Create List of positions for asteroids
		List<(Vector2 pos, int stage)> newAsteroids = new List<(Vector2, int)>();
		//Create random positions for asteroids
		for (int i = 0; i < n; i++)
			newAsteroids.Add((new Vector2( 
				rng.RandfRange(0, GetViewportRect().Size.x), 
					rng.RandfRange(0, GetViewportRect().Size.y)), initStage));
		
		addAsteroids(newAsteroids);
	}

	/// <summary>
	/// Create a bullet at player's position and connect it with all the asteroids
	/// </summary>
	private void addBullet()
	{
		try
		{
			Bullet bullet = (Bullet) bulletScene.Instance();
  			bullet.Position = player.Position;
			bullet.LinearVelocity = bulletVelocity.Rotated(player.Rotation);
			//Send signals to Bullets and Asteroids when they collide
			foreach (Asteroid ast in asteroids)
			{
				bullet.Connect("body_entered", ast, "OnBulletHitAsteroid");
				ast.Connect("body_entered", bullet, "OnAsteroidHitBullet");
			}
			AddChild(bullet);
		}
		catch (Exception e) 
		{
			GD.Print("Game Over");
		}
		
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng.Randomize();
		player = (Player) playerScene.Instance();
		AddChild(player);
		addAsteroids(numInitAsteroids);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		//Keep track of how long it's been since the last bullet as been fired
		secondsSinceLastFire += delta;
		if (Asteroid.newAsteroids.Count > 0)
		{
			addAsteroids(Asteroid.newAsteroids);
			Asteroid.newAsteroids.Clear();
		}
		//check if all the asteroids have been destroyed
		if (asteroids.Count == 0)
		{
			level++;
			addAsteroids(level + numInitAsteroids);
		}
		
		//Enter and Space are the shoot buttons
		if(Input.IsActionPressed("ui_accept"))
		{
			//fireRate is bullets per second, so we divide 1 by fireRate to get
			//the ammount of time we should wait between bullets.
			if (secondsSinceLastFire > 1f/fireRate)
			{
				addBullet();
				secondsSinceLastFire = 0;
			}
		}
	}

	/// <summary>
	/// When asteroid is destroyed, remove it's reference from astroids list
	/// </summary>
	/// <param name="poly">Child CollisionPolygon2D of the Asteroid that was destroyed</param>
	private void OnAsteroidExitTree(Node poly)
	{
		//I was originally expecting the input to be an Asteroid, but it is a CollisionPolygon2d instead
		//I don't know why
		asteroids.Remove( (Asteroid) poly.GetParent());
	}
}
