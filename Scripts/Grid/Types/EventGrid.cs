using Godot;

/// <summary>
/// Event trigger grid: manages events that automatically trigger when stepped on.
/// </summary>
public partial class EventGrid : PawnGrid
{
	public EventGrid(Node2D grid) : base(grid) { }
	public EventGrid() { }

	public override void _Ready()
	{
		InitializePawns(EventType);
	}

	/// <summary>
	/// Check if the target cell has an event entity, and trigger it if present.
	/// </summary>
	public void RequestEvent(Pawn pawn, Vector2I direction)
	{
		CellData cell = GetCellData(pawn.Position, direction);

		Pawn eventPawn = GetCellPawn(cell.Target);
		eventPawn?.TriggerEvent();
	}
}