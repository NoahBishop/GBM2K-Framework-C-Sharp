using Godot;
using System.Collections.Generic;

/// <summary>
/// A* pathfinding NPC: cycles pathfinding movement between multiple target points.
/// </summary>
public partial class NpcAStar : Character
{
	[Export]
	public Godot.Collections.Array<Vector2I> Targets { get; set; } = new();

	[Export]
	public new float Speed { get; set; } = 2.0f;

	private bool _isStopped = false;
	private List<Vector2I> _path = new();
	private int _moveStep = 0;
	private int _moveMax = 0;
	private int _currTarget = 0;

	public override void _Process(double delta)
	{
		if (_isStopped) return;

		if (CanMove())
		{
			// When the path is empty, request a new path.
			if (_path.Count == 0)
			{
				Vector2I[] rawPath = Grid.GetAStarPath(this, Targets[_currTarget]);
				_path = new List<Vector2I>(rawPath);
				_moveStep = 0;
				_moveMax = _path.Count;
			}

			if (_moveStep >= _moveMax) return;

			Vector2I currentStep = _path[_moveStep];
			CharaSkin.SetAnimationDirection(currentStep);

			// Request movement; if blocked, clear the path and recalculate on the next frame.
			Vector2I targetPosition = Grid.RequestMove(this, currentStep);
			if (targetPosition == Vector2I.Zero)
			{
				_path.Clear();
				return;
			}
			MoveTo(targetPosition);

			// Step forward.
			_moveStep += 1;
			if (_moveStep >= _moveMax)
			{
				Wait();
				_path.Clear();
				// Equivalent to wrap(curr_target + 1, 0, 3).
				_currTarget = (_currTarget + 1) % Targets.Count;
			}
		}
	}

	private async void Wait()
	{
		_isStopped = true;
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		_isStopped = false;
	}

	public override void TriggerEvent(Vector2I direction)
	{
		if (!IsMoving)
		{
			CharaSkin.SetAnimationDirection(-direction); // Face the player.
			GD.Print("NPC AStar");
		}
	}
}