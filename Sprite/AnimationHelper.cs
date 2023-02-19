using System;
using Assets.Scripts.Renderer.Sprite;
using static UnityEngine.EventSystems.EventTrigger;

public class AnimationHelper {
	public static bool IsLoopingMotion(SpriteMotion motion) {
		switch (motion) {
			case SpriteMotion.Walk:
			case SpriteMotion.Freeze1:
			case SpriteMotion.Freeze2:
			case SpriteMotion.Standby:
			case SpriteMotion.Idle:
				return true;
			case SpriteMotion.Casting:
			case SpriteMotion.Dead:
			case SpriteMotion.Sit:
			case SpriteMotion.Attack1:
			case SpriteMotion.Attack2:
			case SpriteMotion.Attack3:
			case SpriteMotion.Hit:
			case SpriteMotion.PickUp:
			case SpriteMotion.Special:
			case SpriteMotion.Performance1:
			case SpriteMotion.Performance2:
			case SpriteMotion.Performance3:
				return false;
		}

		return false;
	}

	public static int GetMotionIdForSprite(SpriteMotion motion, bool isMonster) {
		if (isMonster) {
			return motion switch {
				SpriteMotion.Walk => 1 * 8,
				SpriteMotion.Attack1 => 2 * 8,
				SpriteMotion.Attack2 => 2 * 8,
				SpriteMotion.Attack3 => 2 * 8,
				SpriteMotion.Hit => 3 * 8,
				SpriteMotion.Dead => 4 * 8,
				_ => 0,
			};
		}

		return motion switch {
			SpriteMotion.Walk => 1 * 8,
			SpriteMotion.Sit => 2 * 8,
			SpriteMotion.PickUp => 3 * 8,
			SpriteMotion.Standby => 4 * 8,
			SpriteMotion.Attack1 => 5 * 8,
			SpriteMotion.Hit => 6 * 8,
			SpriteMotion.Freeze1 => 7 * 8,
			SpriteMotion.Dead => 8 * 8,
			SpriteMotion.Freeze2 => 9 * 8,
			SpriteMotion.Attack2 => 10 * 8,
			SpriteMotion.Attack3 => 11 * 8,
			SpriteMotion.Casting => 12 * 8,
			_ => 0,
		};
	}
}

