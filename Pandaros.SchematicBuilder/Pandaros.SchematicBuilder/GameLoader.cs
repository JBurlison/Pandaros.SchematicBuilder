﻿using Chatting;
using Pandaros.API;
using Pipliz.JSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace Pandaros.SchematicBuilder
{
    [ModLoader.ModManager]
    public static class GameLoader
    {
        public const string NAMESPACE = "Pandaros.SchematicBuilder";
        public const string SETTLER_INV = "Pandaros.SchematicBuilder.Inventory";
        public const string ALL_SKILLS = "Pandaros.SchematicBuilder.ALLSKILLS";
        public static string MESH_PATH = "Meshes/";
        public static string AUDIO_PATH = "Audio/";
        public static string ICON_PATH = "icons/";
        public static string BLOCKS_ALBEDO_PATH = "Textures/albedo/";
        public static string BLOCKS_EMISSIVE_PATH = "Textures/emissive/";
        public static string BLOCKS_HEIGHT_PATH = "Textures/height/";
        public static string BLOCKS_NORMAL_PATH = "Textures/normal/";
        public static string BLOCKS_NPC_PATH = "Textures/npc/";
        public static string TEXTURE_FOLDER_PANDA = "Textures";
        public static string MOD_FOLDER = @"";
        public static string MODS_FOLDER = @"";
        public static string GAMEDATA_FOLDER = @"";
        public static string GAME_ROOT = @"";
        public static string SAVE_LOC = "";
        public static string MACHINE_JSON = "";
        public static string Schematic_SAVE_LOC = "";
        public static string Schematic_DEFAULT_LOC = "";
        public static readonly Version MOD_VER = new Version(0, 1, 2, 2);
        public static bool RUNNING { get; private set; }
        public static bool WorldLoaded { get; private set; }
        public static Colony StubColony { get; private set; }
        public static JSONNode ModInfo { get; private set; }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.AfterSelectedWorld, NAMESPACE + ".AfterSelectedWorld")]
        public static void AfterSelectedWorld()
        {
            WorldLoaded = true;
            SAVE_LOC = GAMEDATA_FOLDER + "savegames/" + ServerManager.WorldName + "/";
            MACHINE_JSON = $"{SAVE_LOC}/{NAMESPACE}.Machines.json";
            Schematic_SAVE_LOC = $"{SAVE_LOC}/Schematics/";

            if (!Directory.Exists(Schematic_SAVE_LOC))
                Directory.CreateDirectory(Schematic_SAVE_LOC);

            StubColony = Colony.CreateStub(-99998);
        }

        [ModLoader.ModCallback(ModLoader.EModCallbackType.OnAssemblyLoaded, NAMESPACE + ".OnAssemblyLoaded")]
        public static void OnAssemblyLoaded(string path)
        {
            MOD_FOLDER = Path.GetDirectoryName(path).Replace("\\", "/");
            Schematic_DEFAULT_LOC = $"{MOD_FOLDER}/Schematics/";

            if (!Directory.Exists(Schematic_DEFAULT_LOC))
                Directory.CreateDirectory(Schematic_DEFAULT_LOC);

            GAME_ROOT = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"/../../";
            GAMEDATA_FOLDER = Path.Combine(GAME_ROOT, "gamedata").Replace("\\", "/") + "/";
            MODS_FOLDER = MOD_FOLDER + @"/../../";

            ICON_PATH = Path.Combine(MOD_FOLDER, "icons").Replace("\\", "/") + "/";
            MESH_PATH = Path.Combine(MOD_FOLDER, "Meshes").Replace("\\", "/") + "/";
            AUDIO_PATH = Path.Combine(MOD_FOLDER, "Audio").Replace("\\", "/") + "/";
            TEXTURE_FOLDER_PANDA = Path.Combine(MOD_FOLDER, "Textures").Replace("\\", "/") + "/";
            BLOCKS_ALBEDO_PATH = Path.Combine(TEXTURE_FOLDER_PANDA, "albedo").Replace("\\", "/") + "/";
            BLOCKS_EMISSIVE_PATH = Path.Combine(TEXTURE_FOLDER_PANDA, "emissive").Replace("\\", "/") + "/";
            BLOCKS_HEIGHT_PATH = Path.Combine(TEXTURE_FOLDER_PANDA, "height").Replace("\\", "/") + "/";
            BLOCKS_NORMAL_PATH = Path.Combine(TEXTURE_FOLDER_PANDA, "normal").Replace("\\", "/") + "/";
            BLOCKS_NPC_PATH = Path.Combine(TEXTURE_FOLDER_PANDA, "npc").Replace("\\", "/") + "/";

            ModInfo = JSON.Deserialize(MOD_FOLDER + "/modInfo.json")[0];

            SchematicBuilderLogger.Log("Found mod in {0}", MOD_FOLDER);
            SchematicBuilderLogger.Log("GAME_ROOT in {0}", GAME_ROOT);
            SchematicBuilderLogger.Log("GAMEDATA_FOLDER in {0}", GAMEDATA_FOLDER);
            SchematicBuilderLogger.Log("MODS_FOLDER in {0}", MODS_FOLDER);

            ModInfo = JSON.Deserialize(MOD_FOLDER + "/modInfo.json")[0];
        }
    }
}
