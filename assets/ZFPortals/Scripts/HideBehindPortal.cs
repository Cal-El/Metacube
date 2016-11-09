using UnityEngine;
using System.Collections.Generic;

namespace ZenFulcrum.Portal {

/** 
 * Sometimes you need features that don't work with oblique frustums. 
 * When you turn oblique frustums off, things behind portals will appear.
 * Attach this script to any Renderers that you would like to be conditionally hid 
 * when its origin is behind the currently rendering portal.
 */
[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class HideBehindPortal : MonoBehaviour {
	private static HashSet<HideBehindPortal> allHiders = new HashSet<HideBehindPortal>();
	private static Stack<State> statePool = new Stack<State>();

	/**
	 * Previous positions for the hidden items.
	 * Note that we don't use renderer.enabled because Unity has a fit if we enable a renderer 
	 * while a camera is currently rendering.
	 * (For the render in question, we don't actually care if new items appear because we'll put everything back before
	 * *that* render actually takes place, but let's keep those logs clean, folks.)
	 */
	private class State : Dictionary<HideBehindPortal, Vector3> {}

	/** 
	 * Call this to hide all hideable objects that are behind the given portal. 
	 * Call RestoreObjects when done.
	 * Returns an opaque object to pass to RestoreObjects. 
	 * (Use the returned value to call RestoreObjects exactly once.)
	 */
	public static object HideObjects(Portal portal) {
		var state = statePool.Count > 0 ? statePool.Pop() : new State();

		var invisiblePos = new Vector3(float.MaxValue / 2f, float.MaxValue / 2f, float.MaxValue / 2f);

		foreach (var hider in allHiders) {
			if (!hider.enabled || !hider.renderer.enabled) continue;

			//save for later
			state[hider] = hider.transform.localPosition;

			//change visibility for now
			hider.transform.localPosition = hider.IsBehindExit(portal) ? invisiblePos : hider.usualLocalPosition;
		}

		return state;
	}

	/**
	 * Restores the enabled/disabled state of renderers to a previous state, as returned by HideObjects.
	 */
	public static void RestoreObjects(object hideState) {
		var state = (State)hideState;

		foreach (var kvp in state) {
			//restore earlier state
			kvp.Key.transform.localPosition = kvp.Value;
		}

		state.Clear();
		statePool.Push(state);
	}


	private new Renderer renderer;
	/** True if our renderer is normally enabled. */
	private Vector3 usualLocalPosition;

	protected void OnEnable() {
		allHiders.Add(this);
	}

	protected void OnDisable() {
		allHiders.Remove(this);
	}

	protected void Awake() {
		renderer = GetComponent<Renderer>();
	}

	protected void LateUpdate() {
		usualLocalPosition = transform.localPosition;
	}

	/** Returns true if we are behind the exit of the given portal. */
	public bool IsBehindExit(Portal portal) {
		var portalForward = portal.destination.transform.forward;
		var portalToUs = (transform.position - portal.destination.transform.position).normalized;

		return Vector3.Dot(portalForward, portalToUs) <= 0;
	}
}

}
