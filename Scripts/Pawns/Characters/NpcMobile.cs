using Godot;

/// <summary>
/// Fixed-route patrol NPC: cycles movement according to a predefined direction sequence.
/// </summary>
public partial class NpcMobile : Character
{
	[Export]
	public Godot.Collections.Array<Vector2I> MovePattern { get; set; } = new();

	private int _moveStep = 0;
	private bool _isStopped = false;
	private int _moveMax;

	public override void _Ready()
	{
		base._Ready();
		_moveMax = MovePattern.Count;
	}

	public override void _Process(double delta)
	{
		if (_isStopped) return;

		if (CanMove())
		{
			Vector2I currentStep = MovePattern[_moveStep];

			if (currentStep != Vector2I.Zero)
			{
				CharaSkin.SetAnimationDirection(currentStep);

				// Request movement; if blocked, abort this frame.
				Vector2I targetPosition = Grid.RequestMove(this, currentStep);
				if (targetPosition == Vector2I.Zero) return;
				MoveTo(targetPosition);
			}
			else
			{
				Wait();
			}

			// Cycle the step index.
			_moveStep += 1;
			if (_moveStep >= _moveMax) _moveStep = 0;
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
			CharaSkin.SetAnimationDirection(-direction); // Face the player
			GD.Print("NPC Mobile");
		}
	}
}