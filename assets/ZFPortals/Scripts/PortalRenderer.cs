using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ZenFulcrum.Portal { 

/**
 * This script is automatically attached to any cameras rendering portals. YOu do not need to add it yourself.
 * It helps manage recursive portal rendering when rendering with render textures.
 *
 * Specifically, if we have two visible portals and they can see each other, we cannot apply "this is what 
 * the portal looks like" texture until they have both been rendered. If we apply the texture immediately, 
 * other portals that can see us will "see" the destination from the wrong direction.
 *
 * In OnWillRenderObject we don't know what other portals there are to be seen, so we defer to the camera 
 * to handle getting things done in the right order.
 *
 * Keep reading below for a more detailed description.
 */
[ExecuteInEditMode]
public class PortalRenderer : MonoBehaviour {
	/*
	During OnWillRenderObject, visible portals notify us they will be rendered.

	We subrender each individually. Note that while they are subrendering they might repeat this entire 
	process with a different PortalRenderer.
	This might change the visible texture on other portals, therefore we need to render and collect the 
	results first and NOT apply the "result" texture to the portals as we go.

	After all the renders are completed, we apply the "final" textures for each portal and then render.
	 */

	/**
	 * Portals that have rendered in this scene.
	 */
	private List<Portal> renderedPortals = new List<Portal>();
	/**
	 * Rendered results from portals in our current scene.
	 */
	private List<RenderedFrame> renderedPortalData = new List<RenderedFrame>();

	/** The portal script will call this method in OnWillRenderObject while the scene is being culled. */
	public void PortalIsVisible(Portal portal) {
		//render the portal right now and save the result
		var renderResult = RenderedFrame.Get();
		portal.RenderSlaveCamera(GetComponent<Camera>(), renderResult);

		//in a minute we'll apply the results to the portal so it will look right when we render
		renderedPortals.Add(portal);
		renderedPortalData.Add(renderResult);
	}

	public void OnPreRender() {
		// all the portals are rendered, swap out the textures so they look as desired for the main render
		var i = 0;
		foreach (var portal in renderedPortals) {
			portal.AppearAsPreviouslyRendered(renderedPortalData[i++]);
		}		
	}

	public void OnPostRender() {
		// all rendering is done this frame, free up textures for later use
		foreach (var portalFrameData in renderedPortalData) {
			portalFrameData.Dispose();
		}

		renderedPortals.Clear();
		renderedPortalData.Clear();
	}
}

}
