using Godot;

/// <summary>
/// Stationary NPC: does not move on its own; can be interacted with when the confirm key is pressed while facing it.
/// </summary>
public partial class Npc : Character
{
	public override void TriggerEvent(Vector2I direction)
	{
		if (!IsMoving)
		{
			CharaSkin.SetAnimationDirection(-direction); // Face the player
			GD.Print("NPC");
		}
	}
}