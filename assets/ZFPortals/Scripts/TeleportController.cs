using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.Portal {


/**
 * Override this script and attach it to your GameObject to control/be notified of
 * teleports.
 */
public class TeleportController : MonoBehaviour {

	/** 
	 * Called before an object teleports.
	 * Return true to jump, false to not.
	 * May be called multiple times if jump is canceled.
	 */
	public virtual bool BeforeTeleport(Portal portal) {
		return true;
	}

	/** 
	 * Called after an object is teleported. 
	 * If your object has internal direction/rotation state, call 
	 * portal.RotateRelativeToDestination (and sometimes TeleportRelativeToDestination)
	 * to update the state to correctly reflect our new location and orientation.
	 */
	public virtual void AfterTeleport(Portal portal) {}


}

}
