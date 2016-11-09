 _  _  _             _     _    ______                        _
(_)(_)(_)           | |   | |  (_____ \             _        | |
 _  _  _  ___   ____| | __| |   _____) )__   ____ _| |_ _____| |  ___
| || || |/ _ \ / ___) |/ _  |  |  ____/ _ \ / ___|_   _|____ | | /___)
| || || | |_| | |   | ( (_| |  | |   | |_| | |     | |_/ ___ | ||___ |
 \_____/ \___/|_|    \_)____|  |_|    \___/|_|      \__)_____|\_|___/


Punch holes in space! See through and walk through those holes!

Version 1.1.0
Copyright 2015 Zen Fulcrum LLC
https://www.assetstore.unity3d.com/#!/content/45383

Need help? support@zenfulcrum.com

===========
Quick Start
===========

Load up a scene. Preferably one with some terrain and a FPS controller.
Find the prefab in ZFPortals/Prefabs called "BidirectionalPortalPair".
Drag it into your scene somewhere. Under the prefab you added, grab {PairB} and move it somewhere else.
Hit play and walk through it!

Congratulations! You now have a portal in your game!

The default configuration will teleport moving colliders and rigidbodies through the portal automatically.

==========================
Using the included prefabs
==========================

The following prefabs are included:
	BidirectionalPortalPair - A framed set of portals that can be entered from either side.
	BidirectionalRoundPortalPair - Same as above, but the portals and frames are round.
	OneSidedPortalPair - A framed set of portals that can be entered from one side, but appears transparent from the other.
	UnframedOneSidedPortalPair - A OneSidedPortalPair without the frame around it.
	OneWayPortal - An unframed portal that can be entered from one side, with no return.

These prefabs come with all the relevant meshes, scripts, and links set up.

For most use cases, you'll need to change only a few things:
	- Naturally, you'll want to move portal pairs to their respective locations.
	- To change the destination of a portal, set the {destination} property to the transform you wish to act as the exit. The positive z plane of the destination maps to the negative z plane of the origin. (Destinations may be changed dynamically via scripting.)
	- You may want to replace the included portal frame with one that matches your art style.
	- Make sure there's some empty space behind the portals.

If you plan to allow cameras, objects, or characters to go through the portal, be sure to read the "Stepping through Portals" section below!

==================
Looking at Portals
==================

Portals are rendered by creating a carefully positioned camera, rendering its view to a texture, and then applying that texture to the portal's mesh using a special 1:1 screen position shader.

Here's some information:
	- Portals are smoke and mirrors and, like most graphical effects, only work well is certain circumstances.
	- The "slave" camera will mimic the settings of the source camera, including anti-aliasing settings, as closely as possible.
	- Lower-resolution slave renders may be used if desired. Try changing the {Texture Size} property on a portal.
	- Portals can be rendered inside of portals (inside of portals) as deeply as desired, limited only by available render time and render texture memory.
	- In edit mode, you will see a live preview of portals. In edit mode, the portal depth is limited to 1 deep to prevent catastrophic recursion while setting up scenes.
	- Maximum portal (recursion) depth can be configured by changing the {Maximum Portal Depth} property.
	- When rendering gets too deep, subsequent portals will be rendered with the flat, unlit appearance of {Fallback Color}.
	- The front of a portal is -z. Portals are not usually visible when the camera is behind them.
	- Scenes are rendered with a camera view frustum (https://en.wikipedia.org/wiki/Viewing_frustum). When running a slave render, the camera's near view plane will be altered to align with the destination plane. This is called an oblique view matrix. This causes any geometry between the slave camera and the destination portal to be clipped away.
	- Unity refuses to try rendering real time shadows when using an oblique view matrix. (Specifically, if the projection matrix's [2, 0] or [2, 1] values are not *exactly* zero.) When portals are visible in the scene consider using one of these workarounds:
		- Use only baked shadows.
		- Encourage Unity to fix the bug.
		- Disable the oblique view frustum. (See the "Disabling Oblique" section below.)
	- Along the same lines, though for different reasons (transformed z-depth assumptions), global fog (from the lighting panel) does not work correctly through portals either.
		- Use a different type of fog such as billboard or volumetric fog.
		- Replace all material shaders with versions that calculate fog density based on world distance instead of post-perspective z-depth.
		- Disable the oblique view frustum. (See the "Disabling Oblique" section below.)
	- Image post-processing scripts that rely on the depth pass, including GlobalFog, SSAO, and depth of field, will not work correctly with portals. I've yet to find a way to feed fake data to Unity's depth texture. :-(
	- Unity's implementation of occlusion culling is very broken when using a non-standard near plane distance/normal. Occlusion culling will be turned off when oblique running slave renders. If you have additional (non-visible) portals you do not want rendering, ensure they are beyond the far plane of the camera or disable them manually.
	- The materials for the portals are created dynamically. The preview window for portal prefabs will appear with the magenta "missing material" shader until you add them to the scene.
	- Portals don't work with Unity's VR. Vote on this issue so we can get the feature added! https://feedback.unity3d.com/suggestions/full-control-of-vr-rendering-api


==============================
Sending things through Portals
==============================

Rigidbodies that touch a portal will appear on both sides and teleport through as they pass. Appearance on both sides will automatically be cloned and synced for MeshRenderers and FramedMeshRenderers.

Here are some things to keep in mind:
	- IMPORTANT: Make sure there is a reasonable amount of empty space behind portals. Contrary to the illusion we are presenting to players, collision shapes are not carved up and sent across the aether. Instead, objects penetrate the portal and will hit any geometry that is behind it until the whole object is teleported to the other side.
		- On a related note: keep the exit to portals clear too. Objects will not "run into" colliders on the other side until they have teleported.
	- Rigidbodies with colliders that touch the portal will:
		- Automatically have a clone of their appearance created on the other side while they intersect the portal.
		- Automatically be teleported to the other side when the center of the object crosses the threshold.
		- The angular and linear velocity will automatically be adapted to the orientation of the exit portal.
		- If the object contains a camera, note that the camera will be used as the center instead of the actual center.
	- Pay attention to lighting. Objects inside portals will be lit half-and-half on each side. If an object is traveling through it, players will notice the portal because of lighting difference on the object.
	- Keep the physics environment identical on both sides of a portal. For example, if there's a floor on both sides, don't suddenly end the floor at the entrance. Continue the floor into the empty space behind the portal. This allows geometry to continue resting on the ground properly until it's teleported.
	- If you would like to be notified when an object teleports, or disable teleportation for certain objects, create a new class that extends TeleportController and attach it to your object.
		- Override BeforeTeleport and return false if you don't want the object to teleport.
		- Override AfterTeleport to find out when a teleport has happened. If the object has any internal direction or position information, this is a good place to update it. Use portal.RotateRelativeToDestination to rotate quaternions and vectors with respect to the portal you just traveled through.
	- When an FPS controller goes through a portal, we rotate it to match the new position. If this magically doesn't work on your mobile builds, verify that "CROSS_PLATFORM_INPUT" is defined for that platform.


========================
Stepping through Portals
========================

If a camera or visible objects will be traveling through the portal here are some additional things to keep in mind:
	- IMPORTANT: Make sure there is a reasonable amount of empty space behind portals. That is, in the empty space behind a portal, make sure nothing is visible. When the camera gets near the portal we can't reasonably use a "very very near clip distance" and have to fall back to rendering a bit of the space behind the portal.
	- Be mindful of the camera's viewing frustum: https://en.wikipedia.org/wiki/Viewing_frustum
		- When a camera is near a portal, the near clipping plane will intersect the portal. The included "thick" portal meshes can be used to ensure that all the player sees "inside" a portal is the portal.
		- Size the portal frame to the portal exactly. If the edge of the frame intersects the portal mesh, the player will sometimes see a "blip" of that frame when they should be seeing the other side.
		- Don't let the floor or walls cut through the portal. These can cause visual artifacts as you step through. Align the portal exactly on the edge or create a custom portal shaped for your needs.


=============
Portal Shapes
=============

You can create a portal of any shape and size. Rectangular and circular portal meshes (with simple frames) are included.

If you are viewing a portal from a distance, nearly anything goes so long as the end result meets your needs.

If you want to create a custom shape you can send a camera or objects through, however, here are some things to bear in mind:
	- The front face of the portal must be a flat plane that intersects the local origin.
	- A trigger collider attached with the portal script must trigger a hit *before* a camera or visible rigidbody touches the portal itself.
	- The portal mesh must be carefully constructed so a camera whose near plane is partially clipping through the portal's front plane can see nothing but more portal. See the included meshes for examples. Here's some tips for making your mesh:
		- In one mesh, create your frame of choice with a shape of your choice. Save this as your frame mesh.
		- In another mesh, create a portal shape that matches the inside of the frame exactly.
		- Have this main plane intersect (0, 0, 0) and face "toward the user" (0, 0, -1).
		- Create a copy of that plane and offset it by about (0, 0, 0.5). The exact number you should use depends on the camera's near plane distance, the aspect ratio of the screen, and the scaling applied. It's generally safe to err on the side of large. Keep this second plane facing toward the user (0, 0, -1).
		- Create a web/bounding edge between the edges of the two planes with the normals facing *inward*. Pull these web polygons inward a little bit to avoid z-fighting with the frame. This webbing will ensure the player can only seed the "opposite side" of a portal when standing inside a portal looking sideways.
		- Save this as your portal mesh.
		- Create a third mesh by extruding the main portal plane from (0, 0, 0) to (0, 0, -1). Save this as your collider mesh.
		- Duplicate one of the provided prefabs. Replace the frame, portal, and portal collider meshes and drop it in your scene!


=====
Notes
=====

- The portal destination can be any transform - even an empty. This is great for dumping characters into the middle of a desert with no way back.
 	- You can also apply a {Teleport Offset} when a user/object goes through.
- Portals are one-way. You can create a two-way portal by creating two portals and pointing them at each other.
 	- The included *PortalPair prefabs have this done for you already.
- Using portals you can implement some interesting effects, including: rooms that are bigger on the inside than the outside, impossible "five left turn" hallways, or "holographic" televisions. Be creative and make something interesting and memorable!
- Getting lighting to match up on both sides of a portal can be tricky. Light matching is beyond the scope of this document, but here's some tips:
	- Make sure all lights on the exit side have the same settings and positions, relative to the portal, as the entry side.
	- If you are using a general directional light, you may need to face all exit portals exactly 180° away from the entrance.
	- Unity 5 will have skybox-based reflections enabled by default. You may need to switch to a blank "custom" reflection source (Lighting » Scene » Environment Lighting » Reflection Source). Alternatively, you may get good results using a skybox that appears identical for any y-axis rotation.


=================
Disabling Oblique
=================

Some image effects, notably, Unity's built-in realtime shadowing and lighting panel fog, do not work or do not work correctly when we render using oblique view frustums on the slave cameras.

As a workaround, we can disable the oblique camera trick, but this causes an issue: You'll see stuff "behind" the portal when you look through it! Additionally, the farther away from the portal you are, the more objects you can accidentally "see" through the portal!

If you would like to use one of the image effects that requires disabling oblique view frustums, here's how to do it:
	- Disable {Use Oblique} on all relevant portals.
	- Option 1:
		- Have a huge space behind every portal that is completely devoid of visible objects.
	- Option 2:
		- For every object with a Renderer that could be seen behind the portal:
			- Attach the "HideBehindPortal" script.
				- The script only needs to be attached to the root object with a renderer. The origin of that object will be used to determine if it should be visible for any given portal render.
			- Some large objects, like rooms (with portals in them) and terrain can be both in front of AND behind the portal at the same time. For these objects:
				- Cut the mesh in half so we can hide the half that's behind the portal while still rendering the half that's in front.
				- Put them in separate renderers and attach HideBehindPortal to both halves.


===============
Troubleshooting
===============

It's not rendering? Make sure that:
- The portal script is attached
- The portal script is enabled
- A destination is assigned
- There's something worthwhile to see at the destination
- The portal is facing the right way
	- Portals should be invisible when viewed from the +Z side
- A mesh filter is attached and has a mesh (usually PortalObject_PortalMesh)
	- Created your own mesh? Make sure the main faces point along the -Z axis.
- A mesh renderer is attached

When I stand close to a portal I can sometimes see what's behind it! (camera near plane issue)
- Make sure the portal is using "thick" portal mesh like the provided mesh.
- If needed, adjust the z-scale a bit larger to allow more room between the camera and the viewing frustum.
- Make sure the portal doesn't intersect its frame or surrounding landscape. If your portal is non-square see Portal Shapes for instructions on making a custom mesh.

It's slow.
- Keep an eye your recursion and portal count! A test scene with four bi-directional portals and a render depth of 3 can end up rendering 28 times a frame!
- Keep in mind that a full-size render texture is needed for every portal render in a frame. If you get greedy and add too many portals, the video card driver will intermittently freeze your rendering and thrash things about.

Help!
- Email me: support@zenfulcrum.com
