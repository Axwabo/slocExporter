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

> [!NOTE]
> Currently, only point lights are supported.

To create an **sloc** file, add the **ObjectExporter** script to an empty object. Select the path to export to and click **Export Objects**

**_sloc files are not text_**, but raw bytes. This means that opening one in a text editor will produce gibberish, since it's not meant to be interpreted as text. It is a sequence of **int**egers and **float**s.

# Setup

1. Download the archive from the [releases page](https://github.com/Axwabo/slocExporter/releases/latest/)
2. Extract the archive to your Unity project's `Assets` folder
3. Build your scene using primitive objects ang lights
4. On the toolbar, navigate to `sloc`, then `Export`
5. Enter the path to export to (or click **Select File**)
6. Click `Export All`

# Importing

If you want to edit an existing **sloc** file, you can use the **Importer**.

1. Navigate to `sloc`, then `Import`
2. Enter the path to the **sloc** file to import (or click **Select File**)
3. Click `Import Objects`
4. If a material doesn't exist in your Assets folder, you will be asked if you want to create it, or skip the material; you can select an option to execute for all objects.
5. The objects will be imported into the scene contained within an empty object in front of the camera.

# Use References

For better positioning, download [the room and structure assets you need](https://drive.google.com/drive/folders/1693Lf8OkXKdar8Ni2W5TfNHWoOHIiFpt?usp=sharing)

1. Drag and drop the Unity packages into the Unity project.
2. Put the room you need into the scene from the RoomPrefabs folder.
3. Make sure the prefab's position is `0, 0, 0`
4. Position the objects accordingly.
5. Ensure that the room's root object is tagged either as `Room` or `slocExporter Ignored`
6. Export the objects as detailed in [setup](#setup)

# Versioning

Each exported **sloc** file has a version number.

Even if a new version of **sloc** is released, old **sloc** files will still be compatible with the current version of
the plugin due to the versioning system.

All versions have their own object readers to read the objects from the **sloc** file. This way, no **sloc** file will
be broken after a new version is released.

# Collider Modes

The collider behavior can be modified on primitives. To change the collider mode, add the `Collider Mode Setter` script onto an object.

Multiple collider modes can be selected; information about the selected mode is shown in the Inspector.

For example, `Trigger` mode will remove the client-side collider and add a collider on the server with `isTrigger` set to true.

A non-spawned object means the object will not exist on the client at all.

# Trigger Actions

Various actions can be added onto trigger colliders (or `non-spawned triggers`) upon a specific trigger event (**enter**, **stay**, **exit**).

For example, `TeleportToRoom` will find the room with that name and teleport the object interacting with it.

Room names differ from what you expect them to be, see [references](#use-references)

To create a trigger action, add the `Trigger Action` script to a primitive

## Teleporter Immunity

Teleporter immunity is a special trigger action. It makes interacting objects immune to this or all teleporters for a specified amount of time.

A maximum of 60 seconds can be specified but this can be stacked by using the `Add` duration mode.

**WARNING:** if you use the `Add` duration mode in `Stay` it may result in very long durations of teleporter immunity.

A way to clear immunity is by setting the duration mode to `Absolute` and the duration to 0 seconds.

# Structures

Add the imported prefab into the scene.

To mark another object as a structure, add the `Structure Override` script. This can also be used to remove the default loot of lockers. Setting the type to `None` on a prefab will retain the original object type. 
