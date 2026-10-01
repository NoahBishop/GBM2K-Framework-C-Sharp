using Godot;

/// <summary>
/// 所有网格实体的抽象基类
/// 对应 pawn.gd
/// </summary>
public abstract partial class Pawn : Node2D
{
	public enum CellTypes { Actor = 0, Obstacle = 1, Event = 2 }

	[Export]
	public CellTypes Type { get; set; } = CellTypes.Actor;

	// 子类按需覆写，提供不同签名的触发方法
	public virtual void TriggerEvent() { }
	public virtual void TriggerEvent(Vector2I direction) { }
}