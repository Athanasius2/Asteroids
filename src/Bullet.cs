using Godot;
using System;
using static LoopingRigidBody2D;

public class Bullet : LoopingRigidBody2D {
	private float age = 0;
	private float MAX_AGE = 2f;
	private CollisionShape2D shape = new CollisionShape2D();

	public override void _Ready() {
		shape.Shape = new CircleShape2D();
		((CircleShape2D) shape.Shape).Radius = 2.5f;
		AddChild(shape);
	}

	public override void _Process(float delta){
		age += delta;
		if (age > MAX_AGE) QueueFree();
	}

	public override void _Draw(){
		Color color = new Color(0xff, 0, 0);
		DrawCircle(shape.Position, ((CircleShape2D) shape.Shape).Radius, color);
	}
	
	private void OnAsteroidHitBullet(Node body){
		body.QueueFree();
	}
}
