using Godot;
using System.Collections.Generic;

/// <summary>
/// Actor collision grid: manages movement collision, A* pathfinding, and NPC interaction.
/// </summary>
public partial class ActorGrid : PawnGrid
{
	private readonly AStarGrid2D _astar = new();

	public ActorGrid(Node2D grid) : base(grid) { }
	public ActorGrid() { }

	public override void _Ready()
	{
		InitializeCells("Tilemap", "coll_type");
		InitializePawns(ActorType);
		InitializeAStar();
	}

	private void InitializeAStar()
	{
		_astar.Region = GetGridRegion("Tilemap");
		_astar.CellSize = TileSet.TileSize;
		_astar.Offset = (Vector2)TileSet.TileSize / 2.0f;
		_astar.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
		_astar.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Manhattan;
		_astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		_astar.Update();
	}

	/// <summary>
	/// Core: request movement. If the target cell is empty, approve it; otherwise reject it.
	/// </summary>
	public Vector2I RequestMove(Pawn pawn, Vector2I direction)
	{
		CellData cell = GetCellData(pawn.Position, direction);

		if (cell.TargetType == Empty)
		{
			UpdatePawnPos(pawn.Type, cell.Start, cell.Target);
			return (Vector2I)MapToLocal(cell.Target);
		}

		return Vector2I.Zero; // Blocked
	}

	/// <summary>
	/// Calculate an A* path and return a sequence of directions.
	/// </summary>
	public Vector2I[] GetMovePath(Pawn pawn, Vector2I endCell)
	{
		UpdateAStarGrid();

		Vector2I startPoint = LocalToMap(pawn.Position);
		_astar.SetPointSolid(startPoint, false); // Temporarily make the start point walkable
		_astar.SetPointSolid(endCell, false);    // Temporarily make the end point walkable

		Godot.Collections.Array<Vector2I> path = _astar.GetIdPath(startPoint, endCell);
		return IdToDirPath(path);
	}

	/// <summary>
	/// Convert a coordinate path into a direction sequence.
	/// [(5,3), (6,3), (6,4)] → [RIGHT, DOWN]
	/// </summary>
	private Vector2I[] IdToDirPath(Godot.Collections.Array<Vector2I> cellPath)
	{
		var pathMoves = new List<Vector2I>();
		for (int i = 0; i < cellPath.Count - 1; i++)
		{
			Vector2I move = (Vector2I)(cellPath[i + 1] - cellPath[i]);
			pathMoves.Add(move);
		}
		return pathMoves.ToArray();
	}

	/// <summary>
	/// Interaction request when the confirm key is pressed while facing an NPC.
	/// </summary>
	public void RequestActorEvent(Pawn pawn, Vector2I direction)
	{
		CellData cell = GetCellData(pawn.Position, direction);

		if (cell.TargetType == ActorType)
		{
			Pawn eventPawn = GetCellPawn(cell.Target);
			eventPawn?.TriggerEvent(direction);
		}
	}

	/// <summary>
	/// Refresh the A* grid: mark all occupied cells as impassable.
	/// </summary>
	private void UpdateAStarGrid()
	{
		foreach (Vector2I pos in GetUsedCells())
		{
			_astar.SetPointSolid(pos);
		}
	}
}