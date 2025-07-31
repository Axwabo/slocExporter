# slocExporter

Allows for exporting primitive objects in Unity scenes to use in SCP: Secret Laboratory.

Use the [slocLoader](https://github.com/Axwabo/slocLoader/) plugin to load them in the game.

![Logo](https://github.com/Axwabo/slocLoader/blob/main/logo%20small.png?raw=true)

[Watch the tutorial](https://youtu.be/4_-viU0HBBU)

# MapEditorReborn

Before this project was started, I had no knowledge of the MapEditorReborn plugin.

This is not meant to be a clone of MER, but a different approach to be able to load Unity scenes in SCP: Secret
Laboratory.

[Check out MapEditorReborn here](https://discord.gg/JwAfeSd79u)

# What is sloc?

**sloc** stands for **S**ecret **L**aboratory **O**bject **C**ontainer.

An **sloc** file can contain various primitive Unity objects, including spheres, cubes etc., and also lights.
See also: [other objects](#other-objects)

To create an **sloc** file, add the **ObjectExporter** script to an empty object. Select the path to export to and click **Export Objects**

**_sloc files are not text_**, but raw bytes.
This means that opening one in a text editor will produce gibberish, since it's not meant to be interpreted as text.
It is a sequence of **int**egers and **float**s.

**sloc** aims to retain the object hierarchy exactly as is in the Unity editor, meaning
you can use GetComponent(s)InChildren on the server to find child objects.

Any object that cannot be identified but isn't ignored will be exported as empty.
This ensures that transform parents are always present.

# Setup

> [!IMPORTANT]
> Unity 6 (6000.x) is required.

1. Download the archive from the [releases page](https://github.com/Axwabo/slocExporter/releases/latest/)
2. Extract the `slocExporter-x.x.x` directory from archive to your Unity project's `Assets` folder
    - Open the directory and copy its contents to `Assets`
3. Build your scene using primitive objects ang lights
4. On the toolbar, navigate to `sloc`, then `Export`
5. Enter the path to export to (or click **Select File**)
6. Click `Export All`

> [!TIP]
> Use export presets to save options to a file to be used later.

## Default Primitive Flags

Default flags specify what flags will primitive objects have.

By default (pun not intended), there are no flags, meaning that the collider and mesh renderer components
determine which flags to use per primitive.

For example, setting `Visible` as a default flag will make sure that primitives without a `MeshRenderer` in the editor
will still be visible on the client and on the server. Disabling the `MeshRenderer` will always remove the flag.

# Importing

If you want to edit an existing **sloc** file, you can use the **Importer**.

1. Navigate to `sloc`, then `Import`
2. Enter the path to the **sloc** file to import (or click **Select File**)
3. Click `Import Objects`
4. If a material doesn't exist in your Assets folder, you will be asked if you want to create it, or skip the material;
   you can select an option to execute for all objects.
5. The objects will be imported into the scene contained within an empty object in front of the camera.

# Use References

For better positioning, you can extract assets from SL.

1. Use [AssetRipper](https://assetripper.github.io/AssetRipper/) to extract the assets
    1. Include all files from the `SCPSL_Data/Managed` directory
    2. Open the extracted project using the Unity Hub
    3. Select the required assets in the `GameObjects` folder
    4. Right click, `Export Package` -> `Custom Package`
2. Import the package in your **sloc** project
3. Put the room you need into the scene
4. Make sure the prefab's position and rotation are `0, 0, 0`
5. Position the objects accordingly
6. Ensure that the room's root object is tagged either as `Room` or `slocExporter Ignored`
7. Export the objects as detailed in [setup](#setup)

# Versioning

Each exported **sloc** file has a version number.

Even if a new version of **sloc** is released, old **sloc** files will still be compatible with the current version of
the plugin due to the versioning system.

All versions have their own object readers to read the objects from the **sloc** file. This way, no **sloc** file will
be broken after a new version is released.

# Primitive Flags

> [!NOTE]
> Collider modes have been replaced by primitive flags.
> To migrate, select object(s) with the `Collider Mode Setter` script and click `To PrimitiveFlagsSetter`

> [!IMPORTANT]
> For triggers to work, the `Server Collider` must be **enabled**, and `Client Collider` has to be **off** for players to walk in it.

Some primitive flags are resolved automatically based on the components.

## Colliders

By setting `isTrigger` to true in the collider, the `Trigger` flag will be enabled.
If the collider is enabled, `Server Collider` will be enabled and `Client Collider` will be removed.

If the collider is disabled, `Server Collider` and `Client Collider` will be removed,
even if specified as [default flags](#default-primitive-flags).

## MeshRenderers

If a `MeshRenderer` is present and is enabled, the `Visible` flag will be added.
If the renderer is disabled, the `Visible` flag will be removed, even if specified as a [default flag](#default-primitive-flags).

## Full Override

To specifically set primitive flags, add the `Primitive Flags Setter` script onto an object.
This takes priority over flags detected from components and also overrides [default flags](#default-primitive-flags).

## NotSpawned

`Not Spawned` means the object will not exist on the client at all.

# Trigger Actions

Various actions can be added onto trigger colliders (or `non-spawned triggers`) upon a specific trigger event (**enter**, **stay**, **exit**).

For example, `TeleportToRoom` will find the room with that name and teleport the object interacting with it.

Room names differ from what you expect them to be, see [references](#use-references)

To create a trigger action, add the `Trigger Action` script to a primitive

## Teleporter Immunity

Teleporter immunity is a special trigger action.
It makes interacting objects immune to this or all teleporters for a specified amount of time.

A maximum of 60 seconds can be specified but this can be stacked by using the `Add` duration mode.

> [!WARNING]
> If you use the `Add` duration mode in `Stay` it may result in very long durations of teleporter immunity.

A way to clear immunity is by setting the duration mode to `Absolute` and the duration to 0 seconds.

# Other Objects

## Camera

Drag-and-drop an extracted camera prefab to spawn an camera for SCP-079:

- LczCameraToy
- HczCameraToy
- EzArmCameraToy
- EzCameraToy
- SzCameraToy

Add the `Scp 079 Camera Properties` script.

If you haven't imported the prefab, set the `Type Override` field.

## Capybara

Drag-and-drop the extracted `CapybaraToy` to spawn a capybara.

To disable collisions, set any collider to be disabled, or remove all colliders.

## Collider-Only Cube

If a GameObject only has a `Box Collider` (besides its transform), it will be treated
as an invisible cube primitive with collision flags resolved as described [here](#colliders)

> [!CAUTION]
> Do not change the collider's size or center. Move and scale the object instead.

## Invisible Interactable

> [!NOTE]
> A plugin needs to subscribe to the toy's events for the toy to do something.

Add the `Invisible Interactable Properties` script to an empty object.

You can also add a `Box Collider` or `Sphere Collider` or `Capsule Collider`

If no collider is present, select a shape. This will also be used if `Override Shape` is ticked.

> [!IMPORTANT]
> Sphere colliders must have a radius of 0.5.

> [!IMPORTANT]
> Capsule colliders must have a height of 2, radius of 0.5, and must be aligned on the Y-axis.

> [!CAUTION]
> Do not change the collider's size, radius or center apart from as noted above.
> Move and scale the object instead.

## Speaker

> [!NOTE]
> sloc doesn't include audio playback. Use a different plugin to play sound through the created SpeakerToys.

Create an `Audio Source` to spawn a speaker toy.

The `Priority` will be the speaker's controller ID.
Multiple speakers with the same controller ID play the same sound.

If `Spatial Blend` > 0.5, the speaker will be treated as spatial (3D), it will be 2D otherwise.

Other configurable properties are:

- Volume
- Min Distance
- Max Distance

## SL Structures

Structures are spawnable things in SL, such as doors, shooting targets, lockers, etc.

Add the imported prefab into the scene.

To mark another object as a structure, add the `Structure Override` script.
This can also be used to remove the default loot of lockers.
Setting the type to `None` on a prefab will retain the original object type.

## Text

> [!IMPORTANT]
> The font size must be 1 for it to scale properly in SL.
> The text area has a horizontal alignment of center and a vertical alignment of middle.

Right click in the hierarchy, and select `UI` -> `Text - TextMeshPro`

Resize the text to match the display size you need.
