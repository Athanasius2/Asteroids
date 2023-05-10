using Godot;
using System;
using System.Collections.Generic;

public class Main : Node2D {
	private PackedScene playerScene = GD.Load<PackedScene>("res://Player.tscn");
	private PackedScene asteroidScene = GD.Load<PackedScene>("res://Asteroid.tscn");
	private PackedScene bulletScene = GD.Load<PackedScene>("res://Bullet.tscn");

	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private List<Asteroid> asteroids = new List<Asteroid>();
	private Player player;
	private Vector2 bulletVelocity = new Vector2(0, -200);
	private float bulletsPerSecond = 2f;	
	private float secondsSinceLastFire = 0;
	private int numberOfStartingAsteroids = 2; 
	private int initStage = 1;	//starting stage of asteroids
	private int level = 0;
	private int score = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		rng.Randomize();
		player = (Player) playerScene.Instance();
		AddChild(player);
		addAsteroids(numberOfStartingAsteroids);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		secondsSinceLastFire += delta;
		if (Asteroid.newAsteroids.Count > 0) {
			addAsteroids(Asteroid.newAsteroids);
			Asteroid.newAsteroids.Clear();
		}
		if (asteroids.Count == 0) {
			nextLevel();
		}
		handleUserInput();
	}

	private void handleUserInput() {
		if(Input.IsActionPressed("ui_accept")) shootIfReady();
		if(Input.IsActionPressed("ui_up")) player.moveForward();
		if(Input.IsActionPressed("ui_right")) player.turnClockwise();
		if(Input.IsActionPressed("ui_left")) player.turnCounterClockwise();
	}

	private void addAsteroids(List<(Vector2 position , int stage)> newAsteroidPositions){
		foreach (var asteroidPosition in newAsteroidPositions){
			Asteroid newAsteroid = (Asteroid) asteroidScene.Instance();

			newAsteroid.GlobalPosition = asteroidPosition.position;
			newAsteroid.stage = asteroidPosition.stage;
			//Collision detection between player and asteroid.
			newAsteroid.Connect("body_entered", player, "OnAsteroidHitShip");
			//Remove ast from asteroids list when it is destroyed.
			newAsteroid.Connect("child_exiting_tree", this, "OnAsteroidExitTree");
			//Increment score when asteroid is destroyed.
			newAsteroid.Connect("child_exiting_tree", this, "score");
			asteroids.Add(newAsteroid);
			this.AddChild(newAsteroid);
		}
	}

	private void addAsteroids(int numberOfAsteroids) {
		List<(Vector2 pos, int stage)> newAsteroidPositions = new List<(Vector2, int)>();
		for (int i = 0; i < numberOfAsteroids; i++) {
			float xPosition = rng.RandfRange(0, GetViewportRect().Size.x);
			float yPosition = rng.RandfRange(0, GetViewportRect().Size.y);
			newAsteroidPositions.Add((new Vector2(xPosition, yPosition), initStage));
		}
		addAsteroids(newAsteroidPositions);
	}

	private void nextLevel(){
		level++;
		addAsteroids(level + numberOfStartingAsteroids);
	}

	private void shootIfReady() {
		if (secondsSinceLastFire > 1f/bulletsPerSecond) {
			addBullet();
			secondsSinceLastFire = 0;
		}
	}

	private void addBullet() {
		try {
			Bullet bullet = (Bullet) bulletScene.Instance();
  			bullet.Position = player.Position;
			bullet.LinearVelocity = bulletVelocity.Rotated(player.Rotation);
			collisionDetectionBulletAsteroids(bullet);
			AddChild(bullet);
		}
		catch (Exception e) {
			GD.Print("Game Over");
		}
	}

	private void collisionDetectionBulletAsteroids(Bullet bullet) {
		foreach (Asteroid ast in asteroids) {
			bullet.Connect("body_entered", ast, "OnBulletHitAsteroid");
			ast.Connect("body_entered", bullet, "OnAsteroidHitBullet");
		}
	}
	
	private void OnAsteroidExitTree(Node poly) {
		asteroids.Remove( (Asteroid) poly.GetParent());
	}
	
	private void score() {
		score += 1;
	}
}
