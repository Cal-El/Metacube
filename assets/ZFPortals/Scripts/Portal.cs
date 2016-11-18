using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

#if CROSS_PLATFORM_INPUT
using UnityStandardAssets.Characters.FirstPerson;
using MouseLook = UnityStandardAssets.Characters.FirstPerson.MouseLook;
#endif

namespace ZenFulcrum.Portal {

/**
 * World portals.
 * Based, in part, on the Unity Pro water asset (Water.cs), used with permission.
 */
[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Portal : MonoBehaviour {
	/** If true, hides things the shadow cameras in the editor. Normally you want this on. */
	public static bool hideObjects = true;

	protected static Vector3 entryFace = new Vector3(0, 0, -1);
	protected static Vector3 exitFace = new Vector3(0, 0, 1);

	[ReformattedTooltip(@"
		This it the place we are looking through and teleporting through to.

		You may use any transform, even an empty.

		When looking into a portal we look in to it from our the negative z axis. The
		image we see, peering out at the destination, is aligned with the destination's positive z axis.
	")]
	public Transform destination;

	[System.Serializable]
	public class PhysicsOptions {
		[ReformattedTooltip(@"
			Should we teleport rigidbodies and colliders that touch us over to the other side?

			This portal must have a trigger collider attached.

			When teleporting, the root-most rigidbody or collider will be used to help keep objects whole.

			Note that this does not handle moving objects with constraints as a whole.
		")]
		public bool teleportOnTouch = true;

		[ReformattedTooltip(@"
			Normally we want a seamless transition through the portal.

			Alter this to add an offset to objects that exit the portal (relative to the destination).
		")]
		public Vector3 teleportOffset = new Vector3(0, 0, 0);
	}
	public PhysicsOptions physicsOptions = new PhysicsOptions();

	/** Supported textures sizes. Feel free to add any you feel are needed. */
	public enum TextureSize {
		CameraFull = -100,//100%
		CameraHalf = -50,//50%
		CameraQuarter = -25,//25%
		CameraEighth = -12,//12%
		Static256x256 = 256,
		Static512x512 = 512,
		Static1024x1024 = 1024,
		Static2048x2048 = 2048,
	}

	[System.Serializable]
	public class RenderOptions {
		[ReformattedTooltip(@"
			How deeply can we recurse, rendering portals inside portals?

			More depth allow portals that can see portals to be rendered correctly, but requires the scene to be rendered
			more times to get the final image.
		")]
		public int maximumPortalDepth = 2;

		[ReformattedTooltip(@"
			What size of render texture should we use?

			Higher values look better, but take more time to render.

			This can be specified as a ratio of the viewing camera's size so the quality doesn't
			get ""worse"" as the resolution goes up.

			""CameraFull"" represents a render text sized exactly the same as the viewing camera; ""CameraHalf"" represents
			a texture half the size, and so forth.

			StaticNNNxNNN sizes use the given size, irrespective of the rendering camera's resolution.

			If you have a room full of destinations and quality doesn't matter too much, you should knock this down.

			If you want the player to be able to walk through a portal without realizing it, you should use CameraFull.
		")]
		public TextureSize textureSize = TextureSize.CameraFull;

		[ReformattedTooltip(@"Use an oblique clipping plane. Hides geometry behind portals, but breaks real time shadows in Unity.")]
		public bool useOblique = true;

		[ReformattedTooltip(@"
			When useOblique is on, amount to offset the clipping plane on the destination portal.

			Positive numbers add a gap between the exit portal and what is rendered.
			Negative numbers will include geometry *behind* the exit portal.

			For best results, make exit portal geometry invisible from behind and set this to a small negative number.
		")]
		public float clipPlaneOffset = 0f;

		[ReformattedTooltip(@"Which layers should be rendered when looking through a portal?")]
		public LayerMask renderLayers = -1;

		[ReformattedTooltip(@"What color should we render when we are at our recursion limit?")]
		public Color fallbackColor = Color.black;
	}
	[ReformattedTooltip(@"All render/performance tuning settings.")]
	public RenderOptions renderOptions = new RenderOptions();


	//End of useful user documentation.
	// ------------------------------------------------------

	/** Class for tracking information on each camera we render for. */
	protected class CameraInfo {
		/** The camera this struct is for. */
		public Camera srcCamera;
		/** Slave camera that shadows srcCamera from the other side of the portal. */
		public Camera portalCamera;

		/**
		 * Used to track if we are using the data.
		 * If not, we'll discard it.
		 */
		public bool renderedLastFrame = false;

		/** Since last time we were near this portal, have we been in front of it? */
		public bool nearFront = false;
	}

	/** Map of view camera => CameraInfo */
	protected Dictionary<Camera, CameraInfo> cameraInfo = new Dictionary<Camera, CameraInfo>();

	/** If we are recursively rendering portals, this will contain the camera just before the current render step. */
	protected static Camera lastRecursiveCamera = null;
	/** If we are recursively rendering portals, this will contain the portal just before the current render step. */
	protected static Portal lastRecursivePortal = null;

	protected Material portalMaterial = null;

	/** How many portals deep are we right now? (Recursion count) */
	protected static int currentPortalDepth = 0;

	/** List of items we have teleported recently and when they can teleport again (as compared against Time.time). */
	protected static Dictionary<Transform, float> recentTeleports = new Dictionary<Transform, float>();

	/**
	 * When an object gets near a portal, we need to track (and sometimes clone and display things).
	 * We keep track of that state in one of these.
	 */
	protected class PhysicalTrackingInfo {
		/** Root (for movement) and center (what exact point triggers a teleport) */
		public Transform root, center;
		public ShadowClone clone;

		/**
		 * How far away we were last (fixed) tick.
		 * (We don't want to teleport an item unless it's moving toward us.)
		 */
		public float lastDistance;

		/**
		 * True if the object origin has been in front of the portal at some recent point of time.
		 * 
		 * This is used to ensure that we don't teleport objects that came at us from behind.
		 */
		public bool hasBeenInFront = false;

		/**
		 * Time.fixedTime for the last time we processed this in a FixedUpdate. Needed because a root can have multiple
		 * colliders hitting the portal at once and we need to process the logic only once per fixed update.
		 */
		public float lastUpdatedFrame;

		/** 
		 * Number of colliders on the root object currently intersecting the portal. 
		 * By tracking this we can determine when the object truly stops touching the portal.
		 */
		public int collisionCount;
	}

	/** State for physical objects passing through. Tracks ShadowClones and information we need to know if a teleport is appropriate. */
	protected Dictionary<Transform, PhysicalTrackingInfo> physicalTrackingInfo = new Dictionary<Transform, PhysicalTrackingInfo>();

	public void Start() {
		if (!destination) {
			throw new System.ArgumentNullException("destination", "No destination set for portal!");
		}
	}

	public void OnEnable() {
		physicalTrackingInfo.Clear();
	}

	/**
	 * Each portal needs its own material.
	 * (If not, you couldn't have multiple portals on the screen at once.)
	 */
	protected void SetupMaterial() {
		var renderer = GetComponent<Renderer>();

		if (!renderer) {
			this.enabled = false;
			throw new System.InvalidOperationException("No renderer for portal.");
		}

		var mat = new Material(Resources.Load<Shader>("Portal"));
		mat.hideFlags = HideFlags.DontSave;
		portalMaterial = mat;
		if (!Application.isPlaying) renderer.sharedMaterial = mat;
		else renderer.material = mat;
	}

	/** Creates the objects we need, as needed. */
	protected CameraInfo CreateCameraObjects(Camera currentCamera)
	{
		//get the state for this particular camera
		CameraInfo camInfo;
		if (!cameraInfo.TryGetValue(currentCamera, out camInfo)) {
			//Debug.Log("Creating objects for camera " + currentCamera.GetInstanceID());
			camInfo = new CameraInfo();
			camInfo.srcCamera = currentCamera;
			cameraInfo[currentCamera] = camInfo;
		}


		//Camera for seeing through the portal
		if (!camInfo.portalCamera) {
			GameObject cameraObject = new GameObject("Portal Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			cameraObject.hideFlags = Portal.hideObjects ? HideFlags.HideAndDontSave : HideFlags.DontSave;
			camInfo.portalCamera = cameraObject.GetComponent<Camera>();
			camInfo.portalCamera.enabled = false;
			camInfo.portalCamera.transform.position = transform.position;
			camInfo.portalCamera.transform.rotation = transform.rotation;

			if (renderOptions.useOblique) {
				camInfo.portalCamera.useOcclusionCulling = false;//occlusion culling in Unity is broken, much more visibly so with a nonstandard near plane
			}
#if UNITY_5
			camInfo.portalCamera.gameObject.AddComponent<FlareLayer>();
#endif
		}

		if (!portalMaterial) {
			SetupMaterial();
		}

		return camInfo;
	}

	protected void CleanupCameraObjects(CameraInfo camInfo) {
		//Debug.Log("Deleting objects for camera " + camInfo.srcCamera.GetInstanceID());

		if (camInfo.portalCamera && camInfo.portalCamera.gameObject) DestroyImmediate(camInfo.portalCamera.gameObject);
	}

	public void OnDisable() {
		foreach (var kvp in cameraInfo) {
			CleanupCameraObjects(kvp.Value);
		}
		cameraInfo.Clear();

		//Remove any clones we have active.
		foreach (var kvp in physicalTrackingInfo) {
			var info = kvp.Value;
			if (info.clone) {
				Destroy(info.clone.gameObject);
				info.clone = null;
			}
		}
	}


	private List<Camera> nixList = new List<Camera>();
	public void Update() {
		//discard things we don't need because we aren't rendering them
		foreach (var kvp in cameraInfo) {
			CameraInfo camInfo = kvp.Value;

			if (!camInfo.renderedLastFrame) {
				//not used? free it up
				nixList.Add(camInfo.srcCamera);
				CleanupCameraObjects(camInfo);
			} else {
				//reset flag
				camInfo.renderedLastFrame = false;
			}
		}

		if (nixList.Count > 0) {
			foreach (Camera cam in nixList) {
				cameraInfo.Remove(cam);
			}

			nixList.Clear();
		}

	}


	public void LateUpdate() {
		/*
		 * During the course of Update(), things can be moved and shifted, including moving items
		 * "through" a portal. We don't get an OnTriggerStay call until after a frame renders, so
		 * in some cases where a camera is going through, we can get single-frame glitches where the 
		 * camera renders the wrong thing.
		 * Cope with this by proactively teleporting things in LateUpdate. (If we used normal Update
		 * things might still be moved around after our checks as other Updates are called.)
		 */

		foreach (var kvp in physicalTrackingInfo) {
			TrackForTeleport(kvp.Value);
		}
	}

	protected System.Collections.IEnumerator CooldownTeleports() {
		while (recentTeleports.Count != 0) {
			//Clear entries from our recent teleport list.
			foreach (var k in new List<Transform>(recentTeleports.Keys)) {
				var t = recentTeleports[k];

				//remove item if time has elapsed
				if (t < Time.time) {
					recentTeleports.Remove(k);
				}
			}

			yield return new WaitForFixedUpdate();
		}
	}

	/**
	 * This is called when it's known that the object will be rendered by a
	 * camera. We add a PortalRenderer to the camera so it can manage rendering the portals correctly.
	 */
	public void OnWillRenderObject() {
		var portalRenderer = Camera.current.GetComponent<PortalRenderer>();

		if (!portalRenderer) {
			//Add helper script so portals will render right.
			portalRenderer = Camera.current.gameObject.AddComponent<PortalRenderer>();
		}

		portalRenderer.PortalIsVisible(this);
	}

	/**
	 * This will render the slave camera to a texture as it would be seen by the given camera.
	 * Render results are placed in {result}. Pass this to AppearAsPreviouslyRendered to apply the rendered
	 * texture to our current model/appearance.
	 */
	internal void RenderSlaveCamera(Camera cam, RenderedFrame result) {
		//skip if not on or ready
		if (!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial || !GetComponent<Renderer>().enabled || !destination || !cam)
			return;

		CameraInfo trackingInfo = null;
		if (!cameraInfo.TryGetValue(cam, out trackingInfo)) {
			//Not actually being tracked? Just use a temp. Shouldn't happen if the colliders are set up right and traveling sane speeds.
			trackingInfo = new CameraInfo();
		}

		Vector3 entryFaceDirection = transform.rotation * entryFace;

		var isBehind = Vector3.Dot(transform.position - cam.transform.position, entryFaceDirection) >= 0;
		var isReallyNear = PortalMath.DistanceFromPointToPlane(transform.forward, transform.position, cam.transform.position) <= cam.nearClipPlane;

		//Depending on where we've been and are, we might or might not want to render. Keep reading.
		if (isReallyNear) {
			if (isBehind) {
				//we are just behind the portal
				if (trackingInfo.nearFront) {
					//We were in front of it earlier and can we can still see the portal (the portal mesh has more geometry behind the front plane),
					//Render.
					//(Usually this shouldn't happen, we should have been teleported by now.)
				} else {
					//We weren't just in front of the portal. Perhaps we teleported in from somewhere or walked up to a portal that's
					//invisible from behind while looking backwards.
					//Don't render.
					return;
				}
			} else {
				//We are in front of the portal (and rather close to boot).
				//Render.

				//Also set the "I was close to the front of the portal" flag so we know to keep rendering if we move behind it.
				trackingInfo.nearFront = true;
			}
		} else {
			//We are not close.

			//Reset this flag.
			trackingInfo.nearFront = false;

			if (isBehind) {
				//Don't render
				return;
			} else {
				//Render.
			}
		}

		//Note that, if we don't (try to) render the portal for a frame or so the tracking information will be deleted.
		//This covers the corner case where you are behind the portal with it rendering, then teleport away and back.
		//Without clearing the nearFront flag while we're away, we would incorrectly show the inside of the portal if we jumped back.

		//If we have a two-sided portal on the other end as our destination, the opposite face on the destination portal can sometimes
		//block the view as we look through.
		//Therefore, if we are the destination of the currently rendering portal, don't render at all.
		if (lastRecursivePortal && lastRecursivePortal.destination == this.transform) {
			return;
		}

		//Stop rendering if we are too recursively deep.
		if (currentPortalDepth + 1 > renderOptions.maximumPortalDepth) {
			result.renderOpaque = true;
			return;
		}

#if UNITY_EDITOR
		if (!Application.isPlaying && currentPortalDepth + 1 > 1) {
			//don't render more than one deep in the editor (todo: make this configurable)
			result.renderOpaque = true;
			return;
		}
#endif

		currentPortalDepth++;

		var lastLastRecursiveCamera = lastRecursiveCamera;
		var lastLastRecursivePortal = lastRecursivePortal;
		lastRecursiveCamera = cam;
		lastRecursivePortal = this;

		try {

			var camInfo = CreateCameraObjects(cam);

			var portalCamera = camInfo.portalCamera;

			UpdateCameraModes(cam, portalCamera);

			//Move the render target camera to where we'll be rendering from.
			TeleportRelativeToDestination(cam.transform, portalCamera.transform);

			//get the portal's plane
			var pos = destination.transform.position;
			var normal = destination.transform.rotation * exitFace;

			if (renderOptions.useOblique) {
				/*
				Normally, when you do a projection, the near and far (clipping) planes are perpendicular to the camera.

				They don't have to be, however, and here we take advantage of this fact to cull unwanted geometry.

				Here we set up an oblique projection matrix so that near clipping plane coincides with our portal plane.
				(Then shim it a bit, to avoid z-fighting.)
				This way the z-buffer will automatically clip out everything between the camera and portal.
				You'll only see things beyond the destination portal.
				 */
				Vector4 clipPlane = PortalMath.CameraSpacePlane(portalCamera, pos, normal, 1.0f, renderOptions.clipPlaneOffset);

				Matrix4x4 projection;
				if (currentPortalDepth > 1) {
					//If we have a regular projection matrix we can just go ahead and turn it into an oblique matrix.
					//But if we started with an oblique matrix (as happens when a portal renders a portal), re-obliquifying it
					//messes up the far clipping plane.
					//Instead, start with a fresh matrix for the camera and tweak that.

					//Note that we don't want to modify the src camera's matrix, just get a copy of what its normal matrix would be.
					//(Too bad Unity doesn't have an API to just fetch it.)
					//Also note: If we do this to a scene camera inside the Unity Editor (even though we put it back) the scene cameras might FREAK OUT.
					//(That's not a concern, however, because we only do this to slave cameras we generated.)
					var origMatrix = cam.projectionMatrix;//backup
					cam.ResetProjectionMatrix();
					projection = cam.projectionMatrix;//get what we need
					cam.projectionMatrix = origMatrix;//leave the original camera unmodified
				} else {
					projection = cam.projectionMatrix;
				}

				//how far is the camera on this side from the portal entrance?
				var cameraDistanceFromPortal = PortalMath.DistanceFromPointToPlane(transform.forward, transform.position, cam.transform.position);
				if (cameraDistanceFromPortal < cam.nearClipPlane * 3) {
					//When the camera's this close, the math we're using to construct the oblique matrix tends to break down and construct a matrix
					//with a far plane that intersects the original frustum.

					//If we're this close, we'll rely on the empty space that should be behind the portal actually being empty and just use a
					//regular near plan on our frustum.
				} else {
					if (portalCamera.orthographic)
						PortalMath.CalculateOrthographicObliqueMatrix(ref projection, clipPlane);
					else
						PortalMath.CalculatePerspectiveObliqueMatrix(ref projection, clipPlane);
				}

				//we don't use the normal near clip plane, but still need to tell culling algorithms about where we're looking
				//Never mind, occlusion culling is broken in Unity. //portalCamera.nearClipPlane = PortalMath.DistanceFromPointToPlane(destination.forward, destination.position, portalCamera.transform.position);//Vector3.Distance(portalCamera.transform.position, destination.transform.position);
				
				portalCamera.projectionMatrix = projection;
			}

			var renderTexture = result.CreateTexture(renderOptions, cam);

			portalCamera.cullingMask = renderOptions.renderLayers.value;
			portalCamera.targetTexture = renderTexture;


			var hideState = HideBehindPortal.HideObjects(this);
			portalCamera.Render();
			HideBehindPortal.RestoreObjects(hideState);


			// Debug.Log("portal texture (after render) is " + this+ "-"+camInfo.portalTexture.GetInstanceID());


			camInfo.renderedLastFrame = true;
		} finally {
			currentPortalDepth--;
			lastRecursiveCamera = lastLastRecursiveCamera;
			lastRecursivePortal = lastLastRecursivePortal;
		}
	}

	/** After calling RenderSlaveCamera on this frame, call this with the returned results to make the results of that render visible. */
	internal void AppearAsPreviouslyRendered(RenderedFrame renderedFrame) {
		if (!portalMaterial) SetupMaterial();

		if (renderedFrame.texture == null) {
			if (renderedFrame.renderOpaque) {
				//Nothing rendered, appear flat opaque.
				portalMaterial.SetVector("_Scale", new Vector4(0, 0, 1, 0));
				portalMaterial.SetVector("_Color", renderOptions.fallbackColor);
			} else {
				//don't appear
				portalMaterial.SetVector("_Scale", new Vector4(0, 0, -1, 0));
			}
		} else {
			//Set our texture to the rendered area beyond the portal
			portalMaterial.SetTexture("_PortalTex", renderedFrame.texture);
			portalMaterial.SetVector("_Scale", new Vector4(0, 0, 0, 1f));
		}
	}

	/**
	 * Given a starting position, teleports teleportee to the respective location on the portal exit.
	 * Omit teleportee to teleport the first object.
	 */
	public void TeleportRelativeToDestination(Transform startPos, Transform teleportee = null) {
		//take our position+rotation relative to the entrance and make it have the same position/rotation relative to the exit
		Quaternion reverseRotate = Quaternion.Inverse(transform.rotation);
		Vector3 relPos = reverseRotate * (startPos.position - transform.position);
		Quaternion relRotate = reverseRotate * startPos.rotation;

		if (!teleportee) teleportee = startPos;

        if (teleportee.GetComponent<CharacterMotorC>() != null) {
            Transform world = GameManager.GM.transform;
            world.position += transform.position - destination.position;

            Vector3 preRotLocalUp = transform.up;

            Vector3 crossForwardAxis = Vector3.Cross(destination.forward, transform.forward);
            float forwardAngle = Vector3.Angle(destination.forward, transform.forward);
            if (crossForwardAxis != Vector3.zero) {
                world.RotateAround(destination.position, crossForwardAxis, forwardAngle);
            } else if (forwardAngle > 179) {
                world.RotateAround(destination.position, transform.up, forwardAngle);
            }


            Vector3 crossUpAxis = Vector3.Cross(destination.up, preRotLocalUp);
            float upAngle = Vector3.Angle(destination.up, preRotLocalUp);
            if(crossUpAxis != Vector3.zero) {
               world.RotateAround(destination.position, crossUpAxis, upAngle);
            } else if(upAngle > 179){
              world.RotateAround(destination.position, destination.forward, upAngle);
            }
            teleportee.GetComponent<CharacterMotorC>().grounded = false;
            //teleportee.position -= relPos;
            //world.rotation = destination.rotation * relRotate;
            //teleportee.GetComponent<ModdedMouseLook>().xRot = (destination.rotation * relRotate).eulerAngles.y;
            //teleportee.GetComponent<CharacterMotorC>().movement.velocity = -((destination.rotation * relRotate) * teleportee.GetComponent<CharacterMotorC>().movement.velocity);
        } else {
            teleportee.position = destination.rotation * relPos + destination.position;
            teleportee.rotation = destination.rotation * relRotate;
        }
	}

	public Quaternion RotateRelativeToDestination(Quaternion start) {
		//"un"rotate against our rotation
		Quaternion relRotate = Quaternion.Inverse(transform.rotation) * start;
		//"re"rotate against the exit rotation
		return destination.rotation * relRotate;
	}

	public Vector3 RotateRelativeToDestination(Vector3 start) {
		Vector3 relRotate = Quaternion.Inverse(transform.rotation) * start;
		return destination.rotation * relRotate;
	}


	protected void UpdateCameraModes(Camera src, Camera dest) {
		if (!dest)
			return;

		//set portal camera to act like the current camera


		// This doesn't fully work in all Unity versions work for some reason (I haven't had a good reason to figure out why, either)
		// So we'll start with that, and then make sure things actually work.
		dest.CopyFrom(src);

		//NB: Render to the full frame, irrespective of what the source camera does:
		dest.rect = new Rect(0, 0, 1, 1);

		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox) {
			Skybox sky = src.GetComponent<Skybox>();
			Skybox mysky = dest.GetComponent<Skybox>();
			if (!sky || !sky.material) {
				mysky.enabled = false;
			} else {
				mysky.enabled = true;
				mysky.material = sky.material;
			}
		}

		// update other values to match current camera.
		// even if we are supplying custom camera & projection matrices,
		// some of values are used elsewhere (e.g. sky box uses far plane)
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	/**
	 * Finds the best transform parent for the given object to use when teleporting.
	 */
	protected Transform FindTeleportRoot(Transform obj) {
		/*
		 * Find the rigidbody/character controller we are part of.
		 * Failing that, find the root collider.
		 * Failing that, choose {obj}.
		 *
		 */

		Transform movementRoot = obj;
		var logicalRoot = (Component)obj.GetComponentInParent<Rigidbody>() ?? obj.GetComponentInParent<CharacterController>();

		if (logicalRoot) movementRoot = logicalRoot.transform;
		else {
			Transform current = obj.parent;

			while (current) {
				Component component = current.GetComponent<Rigidbody>();
				if (!component) component = current.GetComponent<Collider>();

				if (component) movementRoot = component.transform;

				current = current.parent;
			}
		}

		return movementRoot;
	}

	public void OnTriggerEnter(Collider otherCollider) {
		//Debug.Log(FrameStamp + "OnTriggerEnter " + otherCollider.name + " at " + otherCollider.transform.position.y);

		var root = FindTeleportRoot(otherCollider.transform);


		PhysicalTrackingInfo trackingInfo;

		if (physicalTrackingInfo.TryGetValue(root.transform, out trackingInfo)) {
			//+1 collision, also make sure we can look up the info by out transform
			trackingInfo.collisionCount += 1;
			physicalTrackingInfo[otherCollider.transform] = trackingInfo;
			return;
		}

		//If there's a camera on the object, make sure we teleport based on the camera center, not the movement center
		var center = root;
		var childCamera = root.GetComponentInChildren<Camera>();
		if (childCamera) center = childCamera.transform;

		trackingInfo = new PhysicalTrackingInfo {
			root = root,
			center = center,
			collisionCount = 1
		};
		physicalTrackingInfo[root.transform] = trackingInfo;
		physicalTrackingInfo[otherCollider.transform] = trackingInfo;

		// var inFront = Vector3.Dot(other.transform.positionhasBeenInFront)

		// trackingInfo.lastDistance = PortalMath.DistanceFromPointToPlane(transform.position, transform.forward, other.transform.position);

		//mimic the object on the other side
		trackingInfo.clone = ShadowClone.Create(root.transform, this);
	}


	public void OnTriggerStay(Collider otherCollider) {
		//Debug.Log(FrameStamp + "OnTriggerStay " + otherCollider.name + " at " + otherCollider.transform.position.y);

		PhysicalTrackingInfo trackingInfo;
		if (!physicalTrackingInfo.TryGetValue(otherCollider.transform, out trackingInfo)) {
			Debug.LogWarning("Collider staying, but there was no tracking info", otherCollider);
			return;
		}

		// ReSharper disable once CompareOfFloatsByEqualityOperator (We're looking for an exact bit-for-bit value with no math applied)
		if (trackingInfo.lastUpdatedFrame == Time.fixedTime) {
			//already processed this frame;
			return;
		}

		TrackForTeleport(trackingInfo);


		trackingInfo.lastUpdatedFrame = Time.fixedTime;
	}

	public void OnTriggerExit(Collider otherCollider) {
		//Debug.Log(FrameStamp + "OnTriggerExit " + otherCollider.name + " at " + otherCollider.transform.position.y);

		PhysicalTrackingInfo trackingInfo;
		if (!physicalTrackingInfo.TryGetValue(otherCollider.transform, out trackingInfo)) {
			Debug.LogWarning("Collider exited, but there was no tracking info", otherCollider);
			return;
		}

		trackingInfo.collisionCount -= 1;

		if (trackingInfo.collisionCount > 0) {
			//we aren't free yet, part of us is still in the portal.
			return;
		}

		if (trackingInfo.clone) { //(note: clone might be null -or- a destroyed object here)
			Object.Destroy(trackingInfo.clone.gameObject);
		}

		foreach (var item in physicalTrackingInfo.Where(x => x.Value == trackingInfo).ToList()) {
			physicalTrackingInfo.Remove(item.Key);
		}
	}

	/**
	 * Called once per Update AND once per FixedUpdate, this checks to see if an
	 * object near the portal has gone through it and whisks it away if it has.
	 */
	protected void TrackForTeleport(PhysicalTrackingInfo info) {

		//What side of the portal is it on?
		var pos = transform.InverseTransformPoint(info.center.position);
		var isInFront = pos.z <= 0;

		info.hasBeenInFront = info.hasBeenInFront || isInFront;

		//If it was once in front of us and now it's behind we should look into teleporting it.
		if (info.hasBeenInFront && !isInFront) {
			var obj = info.root;

			//Is teleporting something we should do?
			if (!Application.isPlaying || !physicsOptions.teleportOnTouch || !destination) {
				return;
			}

			//Have we recently teleported? (Unity will call OnTriggerStay an additional frame sometimes, even after
			//an item has been teleported away.)
			if (recentTeleports.ContainsKey(obj)) {
				return;
			}

			TeleportObjectThroughPortal(obj);
		}
	}

	const int teleportCooldownFrames = 2;

	/** Teleports the given item to the other side of the portal, altering rotation, position, speed, etc. */
	public void TeleportObjectThroughPortal(Transform obj) {
		//Debug.Log(FrameStamp + "Teleport " + obj.name);
		Debug.DrawRay(obj.transform.position, obj.transform.forward, Color.blue, 1);

		var teleportController = obj.GetComponent<TeleportController>();

		if (teleportController) {
			var doIt = teleportController.BeforeTeleport(this);
			if (!doIt) return;
		}

		//If we have a ShadowClone for this object (we probably do), swap the location of the object and the clone.
		//This helps cover the one-frame gap before the return portal (if there is one) creates its own clone to cover the shape.
		PhysicalTrackingInfo trackingInfo = null;
		if (physicalTrackingInfo.TryGetValue(obj, out trackingInfo) && trackingInfo.clone) {
			var clone = trackingInfo.clone;
			clone.DestroyNextFixedFrame();
			clone.transform.position = obj.position;
			clone.transform.rotation = obj.rotation;

			//Now that we're teleporting, remove the "been in front of this portal" flag.
			trackingInfo.hasBeenInFront = false;
		}

		//teleport to other side
		TeleportRelativeToDestination(obj.transform);

		Debug.DrawRay(obj.transform.position, obj.transform.forward, Color.green, 1);

		//Don't teleport this again for a few fixed updates
		recentTeleports[obj] = teleportCooldownFrames * Time.fixedDeltaTime + Time.fixedTime;
		StartCoroutine(CooldownTeleports());//kick off job to remove this "hot" teleport

		//also add teleport offset, if any
		obj.transform.position += destination.rotation * physicsOptions.teleportOffset;


		//Deal with any other state that has to handle being teleported.
		RotateComponents(obj);
		//todo: dissolve RotateComponents into a bunch of auto-added TeleportControllers

		if (teleportController) {
			teleportController.AfterTeleport(this);
		}
	}

	private void RotateComponents(Transform obj) {
		/*
		 *
		 * 
		 * 
		 * 
		 * Need to customize a teleport?
		 * Don't add code here. Attach a TeleportController instead.
		 * 
		 * 
		 * 
		 * 
		 * 
		 */


		var rigidbody = obj.GetComponent<Rigidbody>();
		if (rigidbody) {
			rigidbody.velocity = RotateRelativeToDestination(rigidbody.velocity);
			rigidbody.angularVelocity = RotateRelativeToDestination(rigidbody.angularVelocity);
		}

		//Since some of these are soft dependencies, or need to edit private fields (ewwww...) reflection is used
		//to fetch and twiddle some types.

		//Unity 4.x FPS
		var characterMotor = obj.GetComponent("CharacterMotor");
		if (characterMotor) {
			var movementField = characterMotor.GetType().GetField("movement");
			var movement = movementField.GetValue(characterMotor);

			// This does: characterMotor.movement.velocity = RotateRelativeToDestination(characterMotor.movement.velocity);
			var velocityField = movement.GetType().GetField("velocity");
			Vector3 velocity = (Vector3)velocityField.GetValue(movement);
			velocityField.SetValue(movement, RotateRelativeToDestination(velocity));

			// This does: characterMotor.movement.frameVelocity = RotateRelativeToDestination(characterMotor.movement.frameVelocity);
			var frameVelocityField = movement.GetType().GetField("frameVelocity");
			Vector3 frameVelocity = (Vector3)frameVelocityField.GetValue(movement);
			frameVelocityField.SetValue(movement, RotateRelativeToDestination(frameVelocity));
		}

#if CROSS_PLATFORM_INPUT

		//Unity 5.x FPS
		var fpc = obj.GetComponent<FirstPersonController>();
		if (fpc) {
			//Reflection to access private fields! Oh noes! Look away!
			//..the public API lacks access to what we need to do our jobs, however. :-(

			var lookField = fpc.GetType().GetField("m_MouseLook", BindingFlags.NonPublic | BindingFlags.Instance);
			var mouseLook = (UnityStandardAssets.Characters.FirstPerson.MouseLook)lookField.GetValue(fpc);

			var cameraField = fpc.GetType().GetField("m_Camera", BindingFlags.NonPublic | BindingFlags.Instance);
			var fpcCamera = (Camera)cameraField.GetValue(fpc);

			//Reset look script to new location.
			mouseLook.Init(obj, fpcCamera.transform);
		}

		//Unity 5.x RB FPS
		var rbFPS = obj.GetComponent<RigidbodyFirstPersonController>();
		if (rbFPS) {
			var mouseLook = rbFPS.mouseLook;
			//Reset look script to new location.
			mouseLook.Init(obj, rbFPS.cam.transform);
		}

#endif

	}

	private static string FrameStamp {
		get { return "" + Time.frameCount + " (" + Time.realtimeSinceStartup + "s): "; }
	}
}

/** Utility methods for Portal.TextureSize */
public static class TextureSizeFuncs {
	public static void GetTextureSize(this Portal.TextureSize size, Camera camera, out int w, out int h) {
		int sizeValue = (int)size;
		if (sizeValue > 0) {
			//static size
            w = sizeValue;
            h = sizeValue;
		} else {
			//fraction of camera size
			w = -(int)(camera.pixelWidth * sizeValue) / 100;
			h = -(int)(camera.pixelHeight * sizeValue) / 100;
		}
	}
}

}
