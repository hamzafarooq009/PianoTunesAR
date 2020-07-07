using UnityEngine;

namespace Lean.Touch
{
	/// <summary>This component allows you to spawn a prefab at the specified world point.
	/// NOTE: To trigger the prefab spawn you must call the Spawn method on this component from somewhere.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSpawn")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Spawn")]
	public class LeanSpawn : MonoBehaviour
	{
		public enum SourceType
		{
			ThisTransform,
			Prefab
		}

		/// <summary>The prefab that this component can spawn.</summary>
		[Tooltip("The prefab that this component can spawn.")]
		public Transform Prefab;

		/// <summary>If you call <b>Spawn()</b>, where should the position come from?</summary>
		[Tooltip("If you call Spawn(), where should the position come from?")]
		public SourceType DefaultPosition;

		/// <summary>If you call <b>Spawn()</b>, where should the rotation come from?</summary>
		[Tooltip("If you call Spawn(), where should the rotation come from?")]
		public SourceType DefaultRotation;

		/// <summary>This will spawn <b>Prefab</b> at the current <b>Transform.position</b>.</summary>
		public void Spawn()
		{
			if (Prefab != null)
			{
				var position = DefaultPosition == SourceType.Prefab ? Prefab.position : transform.position;
				var rotation = DefaultRotation == SourceType.Prefab ? Prefab.rotation : transform.rotation;
				var clone    = Instantiate(Prefab, position, rotation);

				clone.gameObject.SetActive(true);
			}
		}

		/// <summary>This will spawn <b>Prefab</b> at the specified position in world space.</summary>
		public void Spawn(Vector3 position)
		{
			if (Prefab != null)
			{
				var rotation = DefaultRotation == SourceType.Prefab ? Prefab.rotation : transform.rotation;
				var clone    = Instantiate(Prefab, position, rotation);

				clone.gameObject.SetActive(true);
			}
		}
	}
}