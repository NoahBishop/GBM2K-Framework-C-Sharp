using Godot;
using System.Collections.Generic;

/// <summary>
/// Player controller: handles input priority, movement requests, and event triggering.
/// </summary>
public partial class Player : Character
{
	private static readonly Dictionary<string, Vector2I> Movements = new()
	{
		{ "ui_up",    Vector2I.Up },
		{ "ui_left",  Vector2I.Left },
		{ "ui_right", Vector2I.Right },
		{ "ui_down",  Vector2I.Down },
	};

	private readonly List<string> _inputHistory = new();
	private Vector2I _curDirection = Vector2I.Down;

	public override void _Process(double delta)
	{
		InputPriority();

		if (CanMove())
		{
			// Press confirm key → interact with the NPC/object in front
			if (Input.IsActionJustPressed("ui_accept"))
			{
				Grid.RequestActor(this, _curDirection);
			}

			Vector2I inputDirection = SetDirection();

			if (inputDirection != Vector2I.Zero)
			{
				_curDirection = inputDirection;
				CharaSkin.SetAnimationDirection(inputDirection);

				// Request movement
				Vector2I targetPosition = Grid.RequestMove(this, inputDirection);
				if (targetPosition != Vector2I.Zero)
				{
					MoveTo(targetPosition);
				}
			}
		}
	}

	/// <summary>
	/// Input priority system: the most recently pressed direction key takes priority.
	/// </summary>
	private void InputPriority()
	{
		foreach (var direction in Movements.Keys)
		{
			if (Input.IsActionJustReleased(direction))
			{
				int index = _inputHistory.IndexOf(direction);
				if (index != -1) _inputHistory.RemoveAt(index);
			}

			if (Input.IsActionJustPressed(direction))
			{
				_inputHistory.Add(direction);
			}
		}
	}

	/// <summary>
	/// Calculate the final movement direction based on input history (ensures only four directions).
	/// </summary>
	private Vector2I SetDirection()
	{
		Vector2I direction = Vector2I.Zero;

		if (_inputHistory.Count > 0)
		{
			foreach (var input in _inputHistory)
			{
				direction += Movements[input];
			}

			string last = _inputHistory[^1]; // The last pressed key

			switch (last)
			{
				case "ui_right":
				case "ui_left":
					if (direction.X != 0) direction.Y = 0;
					break;
				case "ui_up":
				case "ui_down":
					if (direction.Y != 0) direction.X = 0;
					break;
			}
		}

		return direction;
	}

	/// <summary>
	/// Override move completion callback: first check for events underfoot, then call base logic.
	/// </summary>
	protected override void OnMoveTweenDone()
	{
		Grid.RequestEvent(this, Vector2I.Zero); // Check if there is an event underfoot
		base.OnMoveTweenDone();
	}

	public override void SetTalking(bool talkState)
	{
		base.SetTalking(talkState);
		if (IsTalking) _inputHistory.Clear();
	}
}