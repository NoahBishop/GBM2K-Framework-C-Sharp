using Godot;

/// <summary>
/// Grid manager (facade pattern): coordinates ActorGrid and EventGrid.
/// </summary>
public partial class GridManager : Node2D
{
	public const int Empty = -1;
	public const int Actor = 0;
	public const int Obstacle = 1;
	public const int Event = 2;

	private ActorGrid _actorGrid;
	private EventGrid _eventGrid;

	public override void _Ready()
	{
		_actorGrid = new ActorGrid(this);
		_eventGrid = new EventGrid(this);

		// call_deferred: defer adding child nodes until the next frame
		GetParent().CallDeferred(Node.MethodName.AddChild, _actorGrid);
		GetParent().CallDeferred(Node.MethodName.AddChild, _eventGrid);
	}

	public Vector2I RequestMove(Pawn pawn, Vector2I direction)
	{
		return _actorGrid.RequestMove(pawn, direction);
	}

	public Vector2I[] GetAStarPath(Pawn pawn, Vector2I endCell)
	{
		return _actorGrid.GetMovePath(pawn, endCell);
	}

	public void RequestActor(Pawn pawn, Vector2I direction)
	{
		_actorGrid.RequestActorEvent(pawn, direction);
	}

	public void RequestEvent(Pawn pawn, Vector2I direction)
	{
		_eventGrid.RequestEvent(pawn, direction);
	}
}