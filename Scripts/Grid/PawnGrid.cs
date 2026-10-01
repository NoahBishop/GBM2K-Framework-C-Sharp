using Godot;
using System.Collections.Generic;

/// <summary>
/// Grid data layer base class: uses an invisible TileMapLayer to store collision/occupancy information.
/// </summary>
public partial class PawnGrid : TileMapLayer
{
	public const int Empty = -1;
	public const int ActorType = 0;
	public const int ObstacleType = 1;
	public const int EventType = 2;

	protected Node PawnGridNode;
	protected Dictionary<Vector2I, Pawn> PawnCoords = new();

	/// <summary>
	/// Constructor, replaces _init(grid).
	/// </summary>
	public PawnGrid(Node2D grid)
	{
		InitializeGridData(grid);
	}

	// Parameterless constructor required by Godot.
	public PawnGrid() { }

	private void InitializeGridData(Node2D grid)
	{
		// Load the TileSet used for collision.
		TileSet = GD.Load<TileSet>("uid://brjfac31s05jj");
		Enabled = false; // Not rendered, purely a logic layer.
		PawnGridNode = grid;
	}

	/// <summary>
	/// Read collision data from map tile layers and populate the grid.
	/// </summary>
	protected void InitializeCells(string nodeGroup, string customData)
	{
		var allLayers = GetTree().GetNodesInGroup(nodeGroup);
		foreach (Node node in allLayers)
		{
			if (node is not TileMapLayer layer) continue;

			var usedCells = layer.GetUsedCellsById();
			foreach (Vector2I cell in usedCells)
			{
				TileData tileData = layer.GetCellTileData(cell);
				int collId = tileData.GetCustomData(customData).AsInt32();
				SetCell(cell, collId, Vector2I.Zero);
			}
		}
	}

	/// <summary>
	/// Get the region of the largest layer in the map (for A*).
	/// </summary>
	protected Rect2I GetGridRegion(string nodeGroup)
	{
		Rect2I gridRegion = new();
		int currMaxCells = 0;

		var allLayers = GetTree().GetNodesInGroup(nodeGroup);
		foreach (Node node in allLayers)
		{
			if (node is not TileMapLayer layer) continue;

			Rect2I layerRegion = layer.GetUsedRect();
			int cellNum = layerRegion.Area;
			if (cellNum > currMaxCells)
			{
				gridRegion = layerRegion;
				currMaxCells = cellNum;
			}
		}

		return gridRegion;
	}

	/// <summary>
	/// Iterate through all child Pawns and register them to the grid by type.
	/// </summary>
	protected void InitializePawns(int pawnType)
	{
		foreach (Node child in PawnGridNode.GetChildren())
		{
			if (child is not Pawn pawn) continue;
			if ((int)pawn.Type != pawnType) continue;

			Vector2I mapPos = LocalToMap(pawn.Position);
			SetCell(mapPos, (int)pawn.Type, Vector2I.Zero);
			PawnCoords[mapPos] = pawn;
		}
	}

	/// <summary>
	/// Query start + direction → target cell information.
	/// Returns a data structure containing start, target, targetType.
	/// </summary>
	public CellData GetCellData(Vector2 start, Vector2I direction)
	{
		Vector2I cellStart = LocalToMap(start);
		Vector2I cellTarget = cellStart + direction;
		int cellTargetType = GetCellSourceId(cellTarget);

		return new CellData
		{
			Start = cellStart,
			Target = cellTarget,
			TargetType = cellTargetType
		};
	}

	/// <summary>
	/// Query the Pawn entity at a given coordinate.
	/// </summary>
	public Pawn GetCellPawn(Vector2I coordinates)
	{
		return PawnCoords.TryGetValue(coordinates, out Pawn pawn) ? pawn : null;
	}

	/// <summary>
	/// Update grid occupancy data: clear the old position, set the new position.
	/// </summary>
	public void UpdatePawnPos(Pawn.CellTypes pawnType, Vector2I cellStart, Vector2I cellTarget)
	{
		SetCell(cellTarget, (int)pawnType, Vector2I.Zero);
		SetCell(cellStart, Empty, Vector2I.Zero);

		// Synchronously update coordinate→entity mapping (skip obstacles not in the dictionary).
		if (PawnCoords.ContainsKey(cellStart))
		{
			PawnCoords[cellTarget] = PawnCoords[cellStart];
			PawnCoords.Remove(cellStart);
		}
	}
}


public struct CellData
{
	public Vector2I Start;
	public Vector2I Target;
	public int TargetType;
}