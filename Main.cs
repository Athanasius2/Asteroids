using Godot;
using System;
using System.Collections.Generic;

public class Main : Node2D
{
	//When scenes are loaded from the disk, they are stored in the PackedScene
	//class, which we use to create new instances.
	public static PackedScene playerScene {get; private set;}
	public static PackedScene asteroidScene {get; private set;}
	public static PackedScene bulletScene {get; private set;}
	
	RandomNumberGenerator rng = new RandomNumberGenerator();
	
	//used to access instance of Player scene
	private Player player;
	//Used to set value of bullet.LinearVelocity when bullet is first created
	private Vector2 bulletVelocity = new Vector2(0, -200);
	//bullets per second
	private float fireRate = 2f;
	//keep track of time since last bullet fired in seconds
	private float timeSinceLastFire = 0;
	//number of asteroids to spawn on startup
	private int numInitAsteroids = 2;
		
	private static void LoadScenes()
	{
		playerScene = GD.Load<PackedScene>("res://Player.tscn");
		asteroidScene = GD.Load<PackedScene>("res://Asteroid.tscn");
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
	}

	private void addBullet()
	{
		Bullet bullet = (Bullet) bulletScene.Instance();
		
		//Every bullet's starting position will be at the player's current position
  		bullet.Position = player.Position;
		
		//Bullet velocity does NOT depend on the players velocity, but will 
		//always be the same no matter what.
		//Velocity vector should be rotated to match the player's rotation.
		bullet.LinearVelocity = bulletVelocity.Rotated(player.Rotation);
		
		//Send signals to Bullets and Asteroids when they collide
		foreach (Asteroid ast in Asteroid.asteroids){
			bullet.Connect("body_entered", ast, "OnBulletHitAsteroid");
			ast.Connect("body_entered", bullet, "OnAsteroidHitBullet");
		}
		//Add bullet to tree so it does stuff
		AddChild(bullet);
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Load our scenes so we can create instances of them later
		LoadScenes();
		//Make random numbers random
		rng.Randomize();
		//Create our one and only instance of Player
		player = (Player) playerScene.Instance();
		//Add it to the tree so it does stuff.
		this.AddChild(player);
		//Create Array of positions for asteroids
		List<Vector2> positions = new List<Vector2>();
		//Create random positions for asteroids
		for (int i = 0; i < numInitAsteroids; i++)
			positions.Add(new Vector2(
				rng.RandfRange(0, GetViewportRect().Size.x), 
					rng.RandfRange(0, GetViewportRect().Size.y)));
		//Add stage 1 asteroids using positions created
		Asteroid.addAsteroids(positions, 1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		//Keep track of how long it's been since the last bullet as been fired
		timeSinceLastFire += delta;
		
		foreach (Asteroid ast in Asteroid.newAsteroids) 
		{
			ast.Connect("body_entered", player, "OnAsteroidHitShip");
			AddChild(ast);
		}
		Asteroid.newAsteroids.Clear();
		
		//Enter and Space are the shoot buttons
		if(Input.IsActionPressed("ui_accept"))
		{
			//fireRate is bullets per second, so we divide 1 by fireRate to get
			//the ammount of time we should wait between bullets.
			if (timeSinceLastFire > 1f/fireRate)
			{
				addBullet();
				timeSinceLastFire = 0;
			}
		}
	}
}
