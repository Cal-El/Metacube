using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


namespace ZenFulcrum.Portal { 

/**
 * Mimics the appearance of an object.
 * 
 * When an object straddles a portal, the polygons will normally only be rendered on one side, causing visual glitches.
 * To combat this, we use Tobirama Senju's technique, automatically, to create something that looks 
 * identical to the original, but on the other side of the portal, causing a seamless appearance.
 */
public class ShadowClone : MonoBehaviour {
	private const bool debug = false;

	private static int id;

	/** 
	 * Creates and returns a new ShadowClone that will mimic the appearance of the given object on the other side of 
	 * the given portal. 
	 */
	public static ShadowClone Create(Transform realObject, Portal portal) {
		if (!Application.isPlaying) {
			Debug.LogWarning("FIXME: Attempting to create ShadowClone in edit mode");
			return null;
		}

		var name = realObject.name + " Portal Clone for " + portal.name + " " + (++id);

		var cloneGO = new GameObject(name);

		cloneGO.hideFlags = HideFlags.NotEditable;
		//note: don't add the "don't save" flag; we don't run in edit mode and setting the "don't save" flag really confuses the editor in this case.

		var clone = cloneGO.AddComponent<ShadowClone>();
		clone.portal = portal;
		clone.realObject = realObject.gameObject;
		clone.transform.localScale = realObject.lossyScale;

		//Also clone current position (while building the object tree) until we Update() to our real location.
		clone.transform.position = realObject.position;
		clone.transform.rotation = realObject.rotation;

		clone.cloneMapping[realObject.transform] = clone.transform;
		clone.CloneChildren(realObject.gameObject, cloneGO);
		clone.CopyLook(realObject.gameObject, cloneGO);

		return clone;
	}
	

	
	public Portal portal;
	public GameObject realObject;

	/** Map of original transform => cloned transform for us and all our children. */ 
	protected Dictionary<Transform, Transform> cloneMapping = new Dictionary<Transform, Transform>();


	/**
	 * Copies the appearance (and nothing else!) of an existing GO to another blank GO.
	 */
	protected void CopyLook(GameObject src, GameObject dest) {
		if (!src.GetComponent<Renderer>()) return;

		var origMR = src.GetComponent<MeshRenderer>();
		if (origMR) {
			var cloneMR = dest.AddComponent<MeshRenderer>();
			CloneSettings(origMR, cloneMR);
		}

		var origMF = src.GetComponent<MeshFilter>();
		if (origMF) {
			var cloneMF = dest.AddComponent<MeshFilter>();
			cloneMF.mesh = origMF.mesh;
		}

		var origSMR =  src.GetComponent<SkinnedMeshRenderer>();
		if (origSMR) {
			var cloneSMR = dest.AddComponent<SkinnedMeshRenderer>();

			//Swap real bones for clone bones
			var bones = new Transform[origSMR.bones.Length];
			var i = 0;
			foreach (var bone in origSMR.bones) bones[i++] = cloneMapping[bone];
			cloneSMR.bones = bones;

			cloneSMR.quality = origSMR.quality;
			cloneSMR.sharedMesh = origSMR.sharedMesh;
			cloneSMR.updateWhenOffscreen = origSMR.updateWhenOffscreen;

			CloneSettings(origSMR, cloneSMR);
		}
	}

	protected void CloneSettings(Renderer src, Renderer dest) {
#if UNITY_5
		dest.shadowCastingMode = src.shadowCastingMode;
		dest.reflectionProbeUsage = src.reflectionProbeUsage;
#else
		dest.castShadows = origMR.castShadows;
#endif

		dest.receiveShadows = src.receiveShadows;
		dest.useLightProbes = src.useLightProbes;
		dest.probeAnchor = src.probeAnchor;
		dest.materials = (Material[])src.materials.Clone();

		//foreach (var mat in cloneMR.materials) mat.color = Color.green;//for debugging
	}

	protected void CloneChildren(GameObject src, GameObject dest) {
		foreach (Transform srcChild in src.transform) {
			var destChild = new GameObject(srcChild.name + " Portal Clone");
			destChild.hideFlags = HideFlags.NotEditable;
			destChild.transform.parent = dest.transform;

			destChild.transform.localScale = srcChild.localScale;
			destChild.transform.position = srcChild.position;
			destChild.transform.rotation = srcChild.rotation;

			cloneMapping[srcChild.transform] = destChild.transform;
			CloneChildren(srcChild.gameObject, destChild);
		}

		foreach (Transform srcChild in src.transform) {
			CopyLook(srcChild.gameObject, cloneMapping[srcChild.transform].gameObject);
		}
	}

	public void Update() {
		if (!portal || !realObject) {
			enabled = false;
			return;
		}

		portal.TeleportRelativeToDestination(realObject.transform, transform);

		//Have our children follow their originals
		foreach (var kvp in cloneMapping) {
			if (kvp.Value == transform || !kvp.Value) continue;

			kvp.Value.localRotation = kvp.Key.localRotation;
			kvp.Value.localPosition = kvp.Key.localPosition;
			kvp.Value.localScale = kvp.Key.localScale;
		}

	}

	/** Causes this clone to be destroyed next fixed frame. */
	public void DestroyNextFixedFrame() {
		Destroy(this.gameObject, Time.fixedDeltaTime + .001f);
		//don't move in the meantime
		portal = null;
	}

}

}
