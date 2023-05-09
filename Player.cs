using Godot;
using System;

using static LoopingRigidBody2D;

public class Player : LoopingRigidBody2D {
	private Vector2 thrust = new Vector2(0, -2);
	private float torque = 40.0F;
	private CollisionPolygon2D playerPolygon = new CollisionPolygon2D();

	private bool forward = false;
	private bool clockwise = false;
	private bool counterClockwise = false;

	public override void _Ready(){
		playerPolygon.Polygon = new Vector2[]{
			new Vector2(0, -25),
			new Vector2(-12.5f, 25),
			new Vector2(12.5f, 25)
		};
		AddChild(playerPolygon);
		this.GlobalPosition = GetViewportRect().Size / 2;
	}

	public override void _IntegrateForces(Physics2DDirectBodyState state){
		base._IntegrateForces(state);		
		if(forward) {
			state.ApplyCentralImpulse(thrust.Rotated(this.Rotation));
			forward = false;
		}
		if(clockwise) {
			state.ApplyTorqueImpulse(torque);
			clockwise = false;
		}
		if(counterClockwise) {
			state.ApplyTorqueImpulse(torque * -1.0F);
			counterClockwise = false;
		}
	}
	
	public override void _Draw(){
		DrawPolygon(playerPolygon.Polygon, new Color[] {new Color("FFFFFF")});
	}

	public void moveForward() {
		forward = true;
	}

	public void turnClockwise() {
		clockwise = true;
	}

	public void turnCounterClockwise() {
		counterClockwise = true;
	}
	
	private void OnAsteroidHitShip(Node body){
		body.QueueFree();
	}
}
