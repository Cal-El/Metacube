using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.Portal {

/**
 * Contains data for one frame for a given camera.
 */
internal class RenderedFrame {
	private static List<RenderedFrame> pool = new List<RenderedFrame>();

	public static RenderedFrame Get() {
		if (pool.Count > 0) {
			var c = pool[pool.Count - 1];
			pool.RemoveAt(pool.Count - 1);
			return c;
		} else {
			return new RenderedFrame();
		}
	}

#if false

	private static List<RenderTexture> texturePool = new List<RenderTexture>();
	private static RenderTexture GetTexture(int w, int h, int aa) {
		//try to find something from the pool
		foreach (var pTex in texturePool) {
			if (!pTex) {
				//Unity Destroy'd this. Maybe they switched scenes in the editor?
				texturePool.Remove(pTex);
				return GetTexture(w, h, aa);
			}

			if (pTex.width == w && pTex.height == h && pTex.antiAliasing == aa) {
				texturePool.Remove(pTex);
				return pTex;
			}
		}

		var tex = new RenderTexture(
			w, h, 16,
			RenderTextureFormat.Default, RenderTextureReadWrite.Default
		);
		tex.antiAliasing = aa;
		return tex;
	}

	private static void ReturnTexture(RenderTexture texture) {
		texture.DiscardContents();
		texturePool.Add(texture);
	}

#else

	private static RenderTexture GetTexture(int w, int h, int aa) {
		return RenderTexture.GetTemporary(w, h, 16, RenderTextureFormat.Default, RenderTextureReadWrite.Default, aa);
	}

	private static void ReturnTexture(RenderTexture texture) {
		RenderTexture.ReleaseTemporary(texture);
	}

#endif

	/** Texture to use when we are rendered. */
	public RenderTexture texture = null;
	/** If true, use the "fallback" flat opaque color. */
	public bool renderOpaque = false;

	private RenderedFrame() {}

	public void Reset() {
		if (texture) {
			ReturnTexture(texture);
			texture = null;
		}
		texture = null;
		renderOpaque = false;
	}


	public RenderTexture CreateTexture(Portal.RenderOptions renderOptions, Camera targetCamera) {
		if (texture) {
			ReturnTexture(texture);
			texture = null;
		}

		//Portal render texture
		//Get the desired size of the RT
		int portalW, portalH;
		renderOptions.textureSize.GetTextureSize(targetCamera, out portalW, out portalH);

		var aa = QualitySettings.antiAliasing;
		//So, http://docs.unity3d.com/ScriptReference/QualitySettings-antiAliasing.html 
		//says 0, 2, 4, or 8 and http://docs.unity3d.com/ScriptReference/RenderTexture.GetTemporary.html
		//says 1, 2, 4, or 8
		if (aa == 0) aa = 1;

		texture = GetTexture(portalW, portalH, aa);

		if (!texture) Debug.LogWarning("Insufficient video memory for new render texture");

		texture.filterMode = renderOptions.textureSize == Portal.TextureSize.CameraFull ? FilterMode.Point : FilterMode.Bilinear;

		return texture;
	}

	public void Dispose() {
		Reset();
		pool.Add(this);
	}

}

}
