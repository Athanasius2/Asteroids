using Godot;
using System;

public class AsteroidPolygon : Polygon2D
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure we are drawing the exact shape of the collision Polygon
		this.Polygon = ((CollisionPolygon2D) this.GetParent()).Polygon;
	}
}
