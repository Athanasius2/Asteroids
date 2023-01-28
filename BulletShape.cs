using Godot;
using System;

public class BulletShape : CollisionShape2D
{
	public override void _Draw()
	{
		Color color = new Color(0xff, 0, 0);
		DrawCircle(this.Position, 5f, color);
	}
}
