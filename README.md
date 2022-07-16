# slocExporter

Allows for exporting primitive objects in Unity scenes to use in SCP: Secret Laboratory.

Use the [slocLoader](https://github.com/Axwabo/slocLoader/) plugin to load them in game.

![Logo](https://github.com/Axwabo/slocLoader/blob/main/logo%20small.png?raw=true)

# MapEditorReborn

Before this project was started, I had no knowledge of the MapEditorReborn plugin.

This is not meant to be a clone of MER, but a different approach to be able to load Unity scenes in SCP: Secret
Laboratory.

[Check out MapEditorReborn here](https://discord.gg/JwAfeSd79u)

# What is an sloc?

**sloc** stands for **S**ecret **L**aboratory **O**bject **C**ontainer.

An **sloc** file can contain various primitive Unity objects, including spheres, cubes etc., and also lights.

Note: SCP:SL only supports spawning **point lights** remotely, therefore **it's not yet possible to create spot,
directional or area lights.**

To create an **sloc** file, add the **ObjectExporter** script to an empty object. Select the path to export to and
click **Export Objects**

**_sloc files are not text_**, but raw bytes. This means that opening one in a text editor will produce gibberish, since
it's not meant to be interpreted as text. It is a sequence of **int**egers and **float**s.

# Setup

1. Download the archive from the [releases page](https://github.com/Axwabo/slocExporter/releases/latest/)
2. Extract the archive to your Unity project's **Assets** folder
3. Build your scene using primitive objects ang lights
4. On the top, navigate to **Window**, **sloc**, then **Export**
5. Enter the path to export to (or click **Select File**)
6. Click **Export All**

# Importing

If you want to edit an existing **sloc** file, you can use the **Importer**.

1. Navigate to **Window**, **sloc**, then **Import**
2. Enter the path to the **sloc** file to import (or click **Select File**)
3. Click **Import Objects**
4. If a material doesn't exist in your Assets folder, you will be asked if you want to create it, or skip the material;
   you can select an option to execute for all objects.
5. The objects will be imported into the scene contained within an empty object in front of the camera.

# Use References

For better positioning,
download [the room assets you need](https://drive.google.com/drive/folders/1693Lf8OkXKdar8Ni2W5TfNHWoOHIiFpt?usp=sharing)

1. Drag and drop the Unity packages into the Unity project.
2. Put the room you need into the scene from the RoomPrefabs folder.
3. Make sure the prefab's position is `0, 0, 0`
4. Position the objects accordingly.
5. Ensure that the room's root object is tagged either `Room` or `slocExporter Ignored`
6. Export the objects as detailed in **Setup**.

# Versioning

Each exported **sloc** file has a version number.

Even if a new version of **sloc** is released, old **sloc** files will still be compatible with the current version of
the plugin due to the versioning system.

All versions have their own object readers to read the objects from the **sloc** file. This way, no **sloc** file will
be broken after a new version is released.
