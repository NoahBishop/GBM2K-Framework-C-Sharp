using Godot;

/// <summary>
/// Obstacle entity that can be interacted with by pressing the confirm key while facing it.
/// </summary>
public partial class ObjectPawn : Pawn
{
	public override void TriggerEvent(Vector2I direction)
	{
		GD.Print("Object");
	}
}