using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZenFulcrum.Portal { 

/**
 * Assorted math operations for dealing with clipping planes for portals.
 */
public class PortalMath {

	/** Extended sign: returns -1, 0 or 1 based on sign of a */
	public static float Sign(float a) {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
	}

	/** Given position/normal of the plane, calculates plane in camera space. */
	public static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign, float clipPlaneOffset) {
		Vector3 offsetPos = pos + normal * clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	/**
	 * Adjusts the given perspective projection matrix so that near plane is the given clipPlane.
	 * See:
	 * Lengyel, Eric. “Oblique View Frustum Depth Projection and Clipping”. Journal of Game Development, Vol. 1, No. 2 (2005), Charles River Media, pp. 5–16.
	 * http://www.terathon.com/code/oblique.html
	 */
	public static void CalculatePerspectiveObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane) {
		Vector4 q = projection.inverse * new Vector4(
			(Sign(clipPlane.x) + projection[8]) / projection[0],
			(Sign(clipPlane.y) + projection[9]) / projection[5],
			1.0f,//reverse this since we aren't mirroring - we're looking beyond
			(1.0f + projection[10]) / projection[14]
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));

		projection[2] = c.x;
		projection[6] = c.y;
		projection[10] = c.z + 1;
		projection[14] = c.w;
	}

	/**
	 * Adjusts the given projection matrix so that near plane is the given clipPlane
	 * clipPlane is given in camera space. See article in Game Programming Gems 5 and
	 * http://aras-p.info/texts/obliqueortho.html
	 */
	public static void CalculateOrthographicObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane) {
		Vector4 q = projection.inverse * new Vector4(
			Sign(clipPlane.x),
			Sign(clipPlane.y),
			1.0f,
			1.0f
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot (clipPlane, q)));
		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}

	/**
	 * Given a camera and a position-independent ray, determines the vanishing point of the ray and how much to scale it.
	 * Returns a vector of center (x and y in texture space) and how much to scale (w)
	 * 
	 * TODO: This is not complete.
	 */
	public static Vector4 GetVanishingPointAndScale(Camera prevCamera, Camera currentCamera, Portal portal) {
		var ret = new Vector4(0, 0, 0, 1);

		//get vector relative to camera
		var dirInCameraSpace = currentCamera.transform.InverseTransformDirection(portal.transform.forward);

		//do some math to determine the vanishing point (derived somewhat empirically, this may not be exact)
		ret.x = (1 / currentCamera.aspect) * dirInCameraSpace.x / dirInCameraSpace.z;
		ret.y = dirInCameraSpace.y / dirInCameraSpace.z;

		// ret *= .75f;

		//determine how much to scale by
		var prevCameraToPortalDistance = Vector3.Distance(portal.transform.position, prevCamera.transform.position);
		var cameraToPortalDistance = Vector3.Distance(portal.transform.position, currentCamera.transform.position);
		ret.w = prevCameraToPortalDistance / cameraToPortalDistance;
		// ret.w = Mathf.Cos(prevCamera.fieldOfView * Mathf.Deg2Rad) / cameraToPortalDistance;
		// ret.w = Mathf.Cos(prevCamera.fieldOfView * Mathf.Deg2Rad);


		//move results (relative to zero) into texture space
		ret.x += 0.5f;
		ret.y += 0.5f;
		return ret;
	}

	/** Returns the distance from the nearest point on the given plane and the given point. */
	public static float DistanceFromPointToPlane(Vector3 planeNormal, Vector3 pointOnPlane, Vector3 point) {
		//center everything on pointOnPlane
		point = point - pointOnPlane;

		//project the point onto the normal (http://docs.unity3d.com/ScriptReference/Vector3.Project.html)
		var projection = Vector3.Project(point, planeNormal);

		return projection.magnitude;
	}
}

}
