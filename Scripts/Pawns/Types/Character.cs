using Godot;

/// <summary>
/// Base class for movable characters, providing common capabilities such as movement, animation, and state locking.
/// </summary>
public partial class Character : Pawn
{
	[Export]
	public float Speed { get; set; } = 1.5f;

	protected Tween MoveTween;
	protected bool IsMoving = false;
	protected bool IsTalking = false;

	protected CharacterSkin CharaSkin;
	protected GridManager Grid;

	public override void _Ready()
	{
		// @onready var chara_skin = $Skin
		CharaSkin = GetNode<CharacterSkin>("Skin");
		// @onready var Grid = get_parent()
		Grid = GetParent<GridManager>();
	}

	public bool CanMove()
	{
		return !(IsMoving || IsTalking);
	}

	public void MoveTo(Vector2 targetPosition)
	{
		CharaSkin.SetAnimationSpeed(Speed);
		CharaSkin.PlayWalkAnimation();

		MoveTween = CreateTween();
		MoveTween.Finished += OnMoveTweenDone;
		MoveTween.TweenProperty(this, "position", targetPosition,
			CharaSkin.WalkLength / Speed);
		IsMoving = true;
	}

	protected virtual void OnMoveTweenDone()
	{
		MoveTween.Kill();
		CharaSkin.ToggleWalkSide();
		IsMoving = false;
	}

	public virtual void SetTalking(bool talkState)
	{
		IsTalking = talkState;
	}
}