using Godot;
using System;

public class BulletShape : CollisionShape2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	
	public override void _Draw()
	{
		Color color = new Color(0xff, 0, 0);
		DrawCircle(this.Position, 5f, color);
	}
	
	public override void _Ready()
	{
		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
