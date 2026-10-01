using Godot;

/// <summary>
/// Event entity that automatically triggers when stepped on.
/// </summary>
public partial class EventPawn : Pawn
{
	public override void TriggerEvent()
	{
		GD.Print("Event");
	}
}