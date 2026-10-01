using Godot;

/// <summary>
/// Character skin: controls walking animation, direction switching, and alternating left/right feet.
/// </summary>
public partial class CharacterSkin : Sprite2D
{
	private bool _switchWalk = false;

	private AnimationTree _animationTree;
	public float WalkLength { get; private set; }

	public override void _Ready()
	{
		_animationTree = GetNode<AnimationTree>("AnimationTree");

		var animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		WalkLength = (float)animPlayer.GetAnimation("walk_down").Length;
	}

	public void SetAnimationSpeed(float value)
	{
		_animationTree.Set("parameters/TimeScale/scale", value);
	}

	public void ToggleWalkSide()
	{
		_switchWalk = !_switchWalk;
	}

	public void PlayWalkAnimation()
	{
		var playback = _animationTree.Get("parameters/StateMachine/playback")
			.As<AnimationNodeStateMachinePlayback>();
		playback.Start(_switchWalk ? "Walk1" : "Walk0");
		_animationTree.Advance(0);
	}

	public void SetAnimationDirection(Vector2 inputDirection)
	{
		_animationTree.Set("parameters/StateMachine/Idle/blend_position", inputDirection);
		_animationTree.Set("parameters/StateMachine/Walk1/blend_position", inputDirection);
		_animationTree.Set("parameters/StateMachine/Walk0/blend_position", inputDirection);
	}
}