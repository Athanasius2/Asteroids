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
	// (1 / fireRate) is the ammount of time in seconds to wait between each 
	//bullet being fired
	private float fireRate = 2f;
	//keep track of time since last bullet fired in seconds
	private float timeSinceLastFire = 0;
	//number of asteroids to spawn on startup
	private int numInitAsteroids = 2;
	
	//Keep track of which groups I'm using
	//Going to remove groups eventually because they don't work on individual
	//instance, but EVERY instance of a node, which is not what we want
	//String asteroidsGroup = "asteroids";
	//String bulletsGroup = "bullets";
		
	private static void LoadScenes()
	{
		playerScene = GD.Load<PackedScene>("res://Player.tscn");
		asteroidScene = GD.Load<PackedScene>("res://Asteroid.tscn");
		bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");
	}

	private void addBullet()
	{
		Bullet bullet = (Bullet) bulletScene.Instance();
		
		//Every bullet's starting position will be at the player's 
		//current position
  		bullet.Position = player.Position;
		
		//Bullet velocity does NOT depend on the players velocity, but will 
		//always be the same no matter what.
		//Velocity vector should be rotated to match the player's rotation.
		bullet.LinearVelocity = bulletVelocity.Rotated(player.Rotation);
		
		//Iterate through every Asteroid in the tree
		foreach (Asteroid ast in Asteroid.asteroids){
			//when the bullet collides with the asteroid, call 
			//OnBulletHitAsteroid() in Asteroid.cs, which destroys ast
			bullet.Connect("body_entered", ast, "OnBulletHitAsteroid");
			
			//this line is supposed to call OnAsteroidHitBullet() in Bullet.cs
			//to destroy the bullet, but the bullet gets destroyed even when
			//this line is commented out. I have no idea why this is happening,
			//but I DO know that OnAsteroidHitBullet() is not getting called
			//ast.Connect("body_entered", bullet, "OnAsteroidHitBullet");
		}
		
		//We have a "bullets" just like we have an "asteroids" group.
		//bullet.AddToGroup(bulletsGroup);
		//add to tree so it does stuff
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
		
		foreach (Asteroid ast in Asteroid.newAsteroids) AddChild(ast);
		Asteroid.newAsteroids.Clear();
		
		//Is true if "ui_accept" action is being pressed (usually activated
		//by Space or Enter keys).  I should probably create unique actions
		//for gameplay that aren't related to ui actions, because I think those
		//are more for menus and such.
		if(Input.IsActionPressed("ui_accept"))
		{
			//fireRate is bullets per second, so we divide 1 by fireRate to get
			//the ammount of time we should wait between bullets.
			if (timeSinceLastFire > 1f/fireRate)
			{
				//Enough time has passed, so we call addBullet()
				addBullet();
				//Since we just fired a bullet, the time since we last fired is 
				//now 0.
				timeSinceLastFire = 0;
			}
		}
	}
}
