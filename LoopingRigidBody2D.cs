using Godot;
using System;

public class LoopingRigidBody2D : RigidBody2D {
	//This is for looping objects to the other side of the screen when they go 
	//off the edge. Physics2DDirectBodyState lets you set the location of a 
	//RigidBody2D without breaking the physics engine.
	public override void _IntegrateForces(Physics2DDirectBodyState state){
		if (Position.x < 0)
			state.Transform = new Transform2D(Rotation, 
				new Vector2(GetViewportRect().Size.x, Position.y));
		
		if (Position.y < 0)
			state.Transform = new Transform2D(Rotation, 
				new Vector2(Position.x, GetViewportRect().Size.y));
		
		if (Position.x > GetViewportRect().Size.x)
			state.Transform = new Transform2D(Rotation, 
				new Vector2(0, Position.y));
		
		if (Position.y > GetViewportRect().Size.y)
			state.Transform = new Transform2D(Rotation, 
				new Vector2(this.Position.x, 0));
	}
}
