using System;
using UnityEngine;

namespace PrefabSwapper
{
	public class SwapPrefabList : ScriptableObject
	{
		[Serializable]
		public class SwapPrefabProperties
		{
			public string prefabName;

			public string category;

			public string[] variations;

			public string[] swapToPrefab;

			public string[] swapToPrefabButtonName;

			public string[] addAdditional;

			public string[] addAdditionalButtonName;

			public Vector3[] addAdditionalPos;

			public Vector3[] addAdditionalRot;

			public string[] spawnAlong;

			public Vector3[] spawnAlongPos;

			public Vector3[] spawnAlongRot;

			public SwapPrefabProperties(string _PrefabName, string _Category, string[] _Variations, string[] _AwapToPrefab, string[] _SwapToPrefabButtonName, string[] _AddAdditional, string[] _AddAdditionalButtonName, Vector3[] _AddAdditionalPos, Vector3[] _AddAdditionalRot, string[] _spawnAlong, Vector3[] _spawnAlongPos, Vector3[] _spawnAlongRot)
			{
				prefabName = _PrefabName;
				category = _Category;
				variations = _Variations;
				swapToPrefab = _AwapToPrefab;
				swapToPrefabButtonName = _SwapToPrefabButtonName;
				addAdditional = _AddAdditional;
				addAdditionalButtonName = _AddAdditionalButtonName;
				addAdditionalPos = _AddAdditionalPos;
				addAdditionalRot = _AddAdditionalRot;
				spawnAlong = _spawnAlong;
				spawnAlongPos = _spawnAlongPos;
				spawnAlongRot = _spawnAlongRot;
			}
		}

		public static string[] prefabsFolders = new string[5] { "Assets/AtmosphericHouse/Prefabs/Building", "Assets/AtmosphericHouse/Prefabs/Doors", "Assets/AtmosphericHouse/Prefabs/Props", "Assets/AtmosphericHouse/Prefabs/Lights", "Assets/AtmosphericHouse/Prefabs/Decals" };

		public static SwapPrefabProperties[] swapPrefabProperties = new SwapPrefabProperties[151]
		{
			new SwapPrefabProperties("WallOut_1x", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_1x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_1x" }, new Vector3[1]
			{
				new Vector3(-1f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_2x", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_2x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_2x" }, new Vector3[1]
			{
				new Vector3(-2f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_3x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_door", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_3x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_door" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_door_windows", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_3x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_doordouble" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_doordouble", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_3x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_doordouble" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_window_static", "WallOut", new string[0], new string[0], new string[0], new string[2] { "Foundation_3x", "Windowshutters" }, new string[2] { "Foundation", "Windowshutters" }, new Vector3[2]
			{
				new Vector3(0f, -0.5f, 0f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_window" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_window_vinyl", "WallOut", new string[0], new string[0], new string[0], new string[2] { "Foundation_3x", "Windowshutters" }, new string[2] { "Foundation", "Windowshutters" }, new Vector3[2]
			{
				new Vector3(0f, -0.5f, 0f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_window" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_windowdouble_static", "WallOut", new string[0], new string[0], new string[0], new string[2] { "Foundation_3x", "Windowshutters_windowdouble" }, new string[2] { "Foundation", "Windowshutters" }, new Vector3[2]
			{
				new Vector3(0f, -0.5f, 0f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_windowdouble" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_windowdouble_vinyl", "WallOut", new string[0], new string[0], new string[0], new string[2] { "Foundation_3x", "Windowshutters_windowdouble" }, new string[2] { "Foundation", "Windowshutters" }, new Vector3[2]
			{
				new Vector3(0f, -0.5f, 0f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_windowdouble" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_3x_windowsmall_vinyl", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_3x" }, new string[1] { "Foundation" }, new Vector3[2]
			{
				new Vector3(0f, -0.5f, 0f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_3x_windowsmall" }, new Vector3[1]
			{
				new Vector3(-3f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_6x", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_6x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_6x" }, new Vector3[1]
			{
				new Vector3(-6f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_6x_windowtriple", "WallOut", new string[0], new string[0], new string[0], new string[1] { "Foundation_6x" }, new string[1] { "Foundation" }, new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[1] { "WallIn_A_6x_windowtriple" }, new Vector3[1]
			{
				new Vector3(-6f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 180f, 0f)
			}),
			new SwapPrefabProperties("WallOut_corner", "WallOut", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, -0.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_1x", "WallIn_A", new string[0], new string[1] { "WallIn_B_1x" }, new string[1] { "WallIn_B" }, new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_2x", "WallIn_A", new string[2] { "WallIn_A_2x_bent_A", "WallIn_A_2x_broken_A" }, new string[1] { "WallIn_B_2x" }, new string[1] { "WallIn_B" }, new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_3x", "WallIn_A", new string[5] { "WallIn_A_3x_bent_A", "WallIn_A_3x_broken_A", "WallIn_A_3x_broken_B", "WallIn_A_3x_broken_C", "WallIn_A_3x_broken_D" }, new string[1] { "WallIn_B_3x" }, new string[1] { "WallIn_B" }, new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_3x_door", "WallIn_A", new string[4] { "WallIn_A_3x_door_bent_A", "WallIn_A_3x_door_broken_A", "WallIn_A_3x_door_broken_B", "WallIn_A_3x_door_broken_C" }, new string[1] { "WallIn_B_3x_door" }, new string[1] { "WallIn_B" }, new string[4] { "Doorframe_in_single", "Door_parent_A_L", "Floor_trim_door", "Decal_wallIn_A_3x_door" }, new string[4] { "Doorframe", "Door", "Floor_trim", "Decal" }, new Vector3[4]
			{
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_3x_doordouble", "WallIn_A", new string[2] { "WallIn_A_3x_doordouble_bent_A", "WallIn_A_3x_doordouble_broken_A" }, new string[1] { "WallIn_B_3x_doordouble" }, new string[1] { "WallIn_B" }, new string[5] { "Doorframe_in_double", "Door_parent_A_double", "Floor_trim_doordouble", "Decal_wallIn_A_3x_doordouble", "AreaLight_door_double_prefab" }, new string[5] { "Doorframe", "Doors", "Floor_trim", "Decal", "Area Light" }, new Vector3[5]
			{
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[5]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_3x_recessed", "WallIn_A", new string[0], new string[1] { "WallIn_B_3x_recessed" }, new string[1] { "WallIn_B" }, new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_3x_window", "WallIn_A", new string[4] { "WallIn_A_3x_window_bent_A", "WallIn_A_3x_window_broken_A", "WallIn_A_3x_window_broken_B", "WallIn_A_3x_window_broken_C" }, new string[1] { "WallIn_B_3x_window" }, new string[1] { "WallIn_B" }, new string[4] { "Windowblinds_open_A", "Curtains_short_singlewindow", "Curtains_long_singlewindow", "AreaLight_window_single_prefab" }, new string[4] { "Windowblinds", "Curtains Short", "Curtains Long", "Area Light" }, new Vector3[4]
			{
				new Vector3(-1.5f, 2f, 0f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_3x_windowdouble", "WallIn_A", new string[2] { "WallIn_A_3x_windowdouble_bent_A", "WallIn_A_3x_windowdouble_broken_A" }, new string[1] { "WallIn_B_3x_windowdouble" }, new string[1] { "WallIn_B" }, new string[5] { "Windowblinds_open_A", "Windowblinds_open_A", "Curtains_short_doublewindow", "Curtains_long_doublewindow", "AreaLight_window_double_prefab" }, new string[5] { "Windowblinds Left", "Windowblinds Right", "Curtains Short", "Curtains Long", "Area Light" }, new Vector3[5]
			{
				new Vector3(-1f, 2f, 0f),
				new Vector3(-2f, 2f, 0f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[5]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_3x_windowsmall", "WallIn_A", new string[1] { "WallIn_A_3x_windowsmall_broken_A" }, new string[1] { "WallIn_B_3x_windowsmall" }, new string[1] { "WallIn_B" }, new string[1] { "AreaLight_window_singlesmall_prefab" }, new string[1] { "Area Light" }, new Vector3[1]
			{
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_6x", "WallIn_A", new string[3] { "WallIn_A_6x_bent_A", "WallIn_A_6x_broken_A", "WallIn_A_6x_broken_B" }, new string[1] { "WallIn_B_6x" }, new string[1] { "WallIn_B" }, new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_6x_windowtriple", "WallIn_A", new string[2] { "WallIn_A_6x_windowtriple_bent_A", "WallIn_A_6x_windowtriple_broken_A" }, new string[1] { "WallIn_B_6x_windowtriple" }, new string[1] { "WallIn_B" }, new string[4] { "Windowblinds_windowtriple", "Curtains_short_triplewindow", "Curtains_long_triplewindow", "AreaLight_window_triple_prefab" }, new string[4] { "Windowblinds", "Curtains Short", "Curtains Long", "Area Lights" }, new Vector3[4]
			{
				new Vector3(-3f, 2f, 0f),
				new Vector3(-3f, 2.33f, 0.1f),
				new Vector3(-3f, 2.33f, 0.1f),
				new Vector3(-3f, 1.5f, 0f)
			}, new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_column_small", "WallIn_A", new string[2] { "WallIn_A_column_small_broken_A", "WallIn_A_column_small_broken_B" }, new string[1] { "WallIn_B_column_small" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_column_small" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_column_medium", "WallIn_A", new string[2] { "WallIn_A_column_medium_broken_A", "WallIn_A_column_medium_broken_B" }, new string[1] { "WallIn_B_column_medium" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_column_medium" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_columnhalf_small", "WallIn_A", new string[2] { "WallIn_A_columnhalf_small_broken_A", "WallIn_A_columnhalf_small_broken_B" }, new string[1] { "WallIn_B_columnhalf_small" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_columnhalf_small" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_columnhalf_medium", "WallIn_A", new string[2] { "WallIn_A_columnhalf_medium_broken_A", "WallIn_A_columnhalf_medium_broken_B" }, new string[1] { "WallIn_B_columnhalf_medium" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_columnhalf_medium" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_columnhalf_large", "WallIn_A", new string[2] { "WallIn_A_columnhalf_large_broken_A", "WallIn_A_columnhalf_large_broken_B" }, new string[1] { "WallIn_B_columnhalf_large" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_columnhalf_large" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_corner_outSmall", "WallIn_A", new string[0], new string[1] { "WallIn_B_corner_outSmall" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_corner_outSmall" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_corner_outLarge", "WallIn_A", new string[2] { "WallIn_A_corner_outLarge_broken_A", "WallIn_A_corner_outLarge_broken_B" }, new string[1] { "WallIn_B_corner_outLarge" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_corner_outLarge" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_A_corner_outPatch", "WallIn_A", new string[0], new string[1] { "WallIn_B_corner_outPatch" }, new string[1] { "WallIn_B" }, new string[1] { "Decal_wallIn_A_corner_outPatch" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_1x", "WallIn_B", new string[0], new string[1] { "WallIn_A_1x" }, new string[1] { "WallIn_A" }, new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_2x", "WallIn_B", new string[2] { "WallIn_B_2x_bent_A", "WallIn_B_2x_broken_A" }, new string[1] { "WallIn_A_2x" }, new string[1] { "WallIn_A" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_3x", "WallIn_B", new string[5] { "WallIn_B_3x_bent_A", "WallIn_B_3x_broken_A", "WallIn_B_3x_broken_B", "WallIn_B_3x_broken_C", "WallIn_B_3x_broken_D" }, new string[1] { "WallIn_A_3x" }, new string[1] { "WallIn_A" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_3x_door", "WallIn_B", new string[4] { "WallIn_B_3x_door_bent_A", "WallIn_B_3x_door_broken_A", "WallIn_B_3x_door_broken_B", "WallIn_B_3x_door_broken_C" }, new string[1] { "WallIn_A_3x_door" }, new string[1] { "WallIn_A" }, new string[4] { "Doorframe_in_single", "Door_parent_A_L", "Floor_trim_door", "Decal_wallIn_B_3x_door" }, new string[4] { "Doorframe", "Door", "Floor_trim", "Decal" }, new Vector3[4]
			{
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_3x_doordouble", "WallIn_B", new string[2] { "WallIn_B_3x_doordouble_bent_A", "WallIn_B_3x_doordouble_broken_A" }, new string[1] { "WallIn_A_3x_doordouble" }, new string[1] { "WallIn_A" }, new string[5] { "Doorframe_in_double", "Door_parent_A_double", "Floor_trim_doordouble", "Decal_wallIn_B_3x_doordouble", "AreaLight_door_double_prefab" }, new string[5] { "Doorframe", "Doors", "Floor_trim", "Decal", "Area Light" }, new Vector3[5]
			{
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[5]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_3x_recessed", "WallIn_B", new string[0], new string[1] { "WallIn_A_3x_recessed" }, new string[1] { "WallIn_A" }, new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_3x_window", "WallIn_B", new string[4] { "WallIn_B_3x_window_bent_A", "WallIn_B_3x_window_broken_A", "WallIn_B_3x_window_broken_B", "WallIn_B_3x_window_broken_C" }, new string[1] { "WallIn_A_3x_window" }, new string[1] { "WallIn_A" }, new string[4] { "Windowblinds_open_A", "Curtains_short_singlewindow", "Curtains_long_singlewindow", "AreaLight_window_single_prefab" }, new string[4] { "Windowblinds", "Curtains Short", "Curtains Long", "Area Light" }, new Vector3[4]
			{
				new Vector3(-1.5f, 2f, 0f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_3x_windowdouble", "WallIn_B", new string[2] { "WallIn_B_3x_windowdouble_bent_A", "WallIn_B_3x_windowdouble_broken_A" }, new string[1] { "WallIn_A_3x_windowdouble" }, new string[1] { "WallIn_A" }, new string[5] { "Windowblinds_open_A", "Windowblinds_open_A", "Curtains_short_doublewindow", "Curtains_long_doublewindow", "AreaLight_window_double_prefab" }, new string[5] { "Windowblinds Left", "Windowblinds Right", "Curtains Short", "Curtains Long", "Area Light" }, new Vector3[5]
			{
				new Vector3(-1f, 2f, 0f),
				new Vector3(-2f, 2f, 0f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 2.33f, 0.1f),
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[5]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_3x_windowsmall", "WallIn_B", new string[1] { "WallIn_B_3x_windowsmall_broken_A" }, new string[1] { "WallIn_A_3x_windowsmall" }, new string[1] { "WallIn_A" }, new string[1] { "AreaLight_window_singlesmall_prefab" }, new string[1] { "Area Light" }, new Vector3[1]
			{
				new Vector3(-1.5f, 1.5f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_6x", "WallIn_B", new string[3] { "WallIn_B_6x_bent_A", "WallIn_B_6x_broken_A", "WallIn_B_6x_broken_B" }, new string[1] { "WallIn_A_6x" }, new string[1] { "WallIn_A" }, new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_6x_windowtriple", "WallIn_B", new string[2] { "WallIn_B_6x_windowtriple_bent_A", "WallIn_B_6x_windowtriple_broken_A" }, new string[1] { "WallIn_A_6x_windowtriple" }, new string[1] { "WallIn_A" }, new string[4] { "Windowblinds_windowtriple", "Curtains_short_triplewindow", "Curtains_long_triplewindow", "AreaLight_window_triple_prefab" }, new string[4] { "Windowblinds", "Curtains Short", "Curtains Long", "Area Lights" }, new Vector3[4]
			{
				new Vector3(-3f, 2f, 0f),
				new Vector3(-3f, 2.33f, 0.1f),
				new Vector3(-3f, 2.33f, 0.1f),
				new Vector3(-3f, 1.5f, 0f)
			}, new Vector3[4]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_column_small", "WallIn_B", new string[2] { "WallIn_B_column_small_broken_A", "WallIn_B_column_small_broken_B" }, new string[1] { "WallIn_A_column_small" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_column_small" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_column_medium", "WallIn_B", new string[2] { "WallIn_B_column_medium_broken_A", "WallIn_B_column_medium_broken_B" }, new string[1] { "WallIn_A_column_medium" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_column_medium" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_columnhalf_small", "WallIn_B", new string[2] { "WallIn_B_columnhalf_small_broken_A", "WallIn_B_columnhalf_small_broken_B" }, new string[1] { "WallIn_A_columnhalf_small" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_columnhalf_small" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_columnhalf_medium", "WallIn_B", new string[2] { "WallIn_B_columnhalf_medium_broken_A", "WallIn_B_columnhalf_medium_broken_B" }, new string[1] { "WallIn_A_columnhalf_medium" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_columnhalf_medium" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_columnhalf_large", "WallIn_B", new string[2] { "WallIn_B_columnhalf_large_broken_A", "WallIn_B_columnhalf_large_broken_B" }, new string[1] { "WallIn_A_columnhalf_large" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_columnhalf_large" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_corner_outSmall", "WallIn_B", new string[0], new string[1] { "WallIn_A_corner_outSmall" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_corner_outSmall" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_corner_outLarge", "WallIn_B", new string[2] { "WallIn_B_corner_outLarge_broken_A", "WallIn_B_corner_outLarge_broken_B" }, new string[1] { "WallIn_A_corner_outLarge" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_corner_outLarge" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_B_corner_outPatch", "WallIn_B", new string[0], new string[1] { "WallIn_A_corner_outPatch" }, new string[1] { "WallIn_A" }, new string[1] { "Decal_wallIn_B_corner_outPatch" }, new string[1] { "Decal" }, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_1x", "WallIn_C", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_2x", "WallIn_C", new string[1] { "WallIn_C_2x_bent_A" }, new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_3x", "WallIn_C", new string[1] { "WallIn_C_3x_bent_A" }, new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_3x_door", "WallIn_C", new string[0], new string[0], new string[0], new string[3] { "Doorframe_in_single", "Door_parent_A_L", "Floor_trim_door" }, new string[3] { "Doorframe", "Door", "Floor_trim" }, new Vector3[3]
			{
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f)
			}, new Vector3[3]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_3x_doordouble", "WallIn_C", new string[0], new string[0], new string[0], new string[3] { "Doorframe_in_double", "Door_parent_A_double", "Floor_trim_doordouble" }, new string[3] { "Doorframe", "Doors", "Floor_trim" }, new Vector3[3]
			{
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f)
			}, new Vector3[3]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(-1.5f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_6x", "WallIn_C", new string[1] { "WallIn_C_6x_bent_A" }, new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_column_small", "WallIn_C", new string[0], new string[0], new string[0], new string[1] { "Decal_wallIn_C_column_small" }, new string[1] { "Decal" }, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_column_medium", "WallIn_C", new string[0], new string[0], new string[0], new string[1] { "Decal_wallIn_C_column_medium" }, new string[1] { "Decal" }, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_columnhalf_small", "WallIn_C", new string[0], new string[0], new string[0], new string[1] { "Decal_wallIn_C_columnhalf_small" }, new string[1] { "Decal" }, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_columnhalf_medium", "WallIn_C", new string[0], new string[0], new string[0], new string[1] { "Decal_wallIn_C_columnhalf_medium" }, new string[1] { "Decal" }, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_columnhalf_large", "WallIn_C", new string[0], new string[0], new string[0], new string[1] { "Decal_wallIn_C_columnhalf_large" }, new string[1] { "Decal" }, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_corner_outSmall", "WallIn_C", new string[0], new string[0], new string[0], new string[1] { "Decal_wallIn_C_corner_outSmall" }, new string[1] { "Decal" }, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_corner_outLarge", "WallIn_C", new string[0], new string[0], new string[0], new string[1] { "Decal_wallIn_C_corner_outLarge" }, new string[1] { "Decal" }, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("WallIn_C_corner_outPatch", "WallIn_C", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_1x3", "Floor", new string[0], new string[0], new string[0], new string[1] { "Ceiling_1x3" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_2x3", "Floor", new string[0], new string[0], new string[0], new string[1] { "Ceiling_2x3" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_3x3", "Floor", new string[2] { "Floor_3x3_broken_A", "Floor_3x3_broken_B" }, new string[0], new string[0], new string[1] { "Ceiling_3x3" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_4x3", "Floor", new string[0], new string[0], new string[0], new string[1] { "Ceiling_4x3" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_6x3", "Floor", new string[1] { "Floor_6x3_broken_A" }, new string[0], new string[0], new string[1] { "Ceiling_6x3" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_6x3_stairs_L", "Floor", new string[0], new string[0], new string[0], new string[1] { "Ceiling_6x3" }, new string[0], new Vector3[1]
			{
				new Vector3(6f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_6x3_stairs_R", "Floor", new string[0], new string[0], new string[0], new string[1] { "Ceiling_6x3" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Floor_6x6", "Floor", new string[0], new string[0], new string[0], new string[1] { "Ceiling_6x6" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, 3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_1x3", "Ceiling", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_2x3", "Ceiling", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_3x3", "Ceiling", new string[3] { "Ceiling_3x3_broken_A", "Ceiling_3x3_broken_B", "Ceiling_3x3_broken_C" }, new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_4x3", "Ceiling", new string[1] { "Ceiling_4x3_broken_A" }, new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_6x3", "Ceiling", new string[1] { "Ceiling_6x3_broken_A" }, new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_6x3_stairs_L", "Ceiling", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_6x3_stairs_R", "Ceiling", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_6x6", "Ceiling", new string[2] { "Ceiling_6x6_broken_A", "Ceiling_6x6_broken_B" }, new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_3x_trim", "Ceiling", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_6x", "Roof", new string[0], new string[3] { "Roof_A_9x", "Roof_A_12x", "Roof_A_6x_gable" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_6x_corner", "Roof", new string[0], new string[2] { "Roof_A_9x_corner", "Roof_A_12x_corner" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_6x_corner_T", "Roof", new string[0], new string[2] { "Roof_A_9x_corner_T", "Roof_A_12x_corner_T" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_6x_gable", "Roof", new string[0], new string[3] { "Roof_A_9x_gable", "Roof_A_12x_gable", "Roof_A_6x" }, new string[0], new string[6] { "Gutter_pipe_1story", "Gutter_pipe_2story", "Gutter_pipe_abovePorch", "Gutter_pipe_1story", "Gutter_pipe_2story", "Gutter_pipe_abovePorch" }, new string[6] { "Gutter_pipe_1story_R", "Gutter_pipe_2story_R", "Gutter_pipe_abovePorch_R", "Gutter_pipe_1story_L", "Gutter_pipe_2story_L", "Gutter_pipe_abovePorch_L" }, new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(-0.5f, 0f, -6f),
				new Vector3(-0.5f, 0f, -6f),
				new Vector3(-0.5f, 0f, -6f)
			}, new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 180f, 0f),
				new Vector3(0f, 180f, 0f),
				new Vector3(0f, 180f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_9x", "Roof", new string[0], new string[3] { "Roof_A_6x", "Roof_A_12x", "Roof_A_9x_gable" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_9x_corner", "Roof", new string[0], new string[2] { "Roof_A_6x_corner", "Roof_A_12x_corner" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_9x_corner_T", "Roof", new string[0], new string[2] { "Roof_A_6x_corner_T", "Roof_A_12x_corner_T" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_9x_gable", "Roof", new string[0], new string[3] { "Roof_A_6x_gable", "Roof_A_12x_gable", "Roof_A_9x" }, new string[0], new string[6] { "Gutter_pipe_1story", "Gutter_pipe_2story", "Gutter_pipe_abovePorch", "Gutter_pipe_1story", "Gutter_pipe_2story", "Gutter_pipe_abovePorch" }, new string[6] { "Gutter_pipe_1story_R", "Gutter_pipe_2story_R", "Gutter_pipe_abovePorch_R", "Gutter_pipe_1story_L", "Gutter_pipe_2story_L", "Gutter_pipe_abovePorch_L" }, new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(-0.5f, 0f, -9f),
				new Vector3(-0.5f, 0f, -9f),
				new Vector3(-0.5f, 0f, -9f)
			}, new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 180f, 0f),
				new Vector3(0f, 180f, 0f),
				new Vector3(0f, 180f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_12x", "Roof", new string[0], new string[3] { "Roof_A_6x", "Roof_A_9x", "Roof_A_12x_gable" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_12x_corner", "Roof", new string[0], new string[2] { "Roof_A_6x_corner", "Roof_A_9x_corner" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_12x_corner_T", "Roof", new string[0], new string[2] { "Roof_A_6x_corner_T", "Roof_A_9x_corner_T" }, new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Roof_A_12x_gable", "Roof", new string[0], new string[3] { "Roof_A_6x_gable", "Roof_A_9x_gable", "Roof_A_12x" }, new string[0], new string[6] { "Gutter_pipe_1story", "Gutter_pipe_2story", "Gutter_pipe_abovePorch", "Gutter_pipe_1story", "Gutter_pipe_2story", "Gutter_pipe_abovePorch" }, new string[6] { "Gutter_pipe_1story_R", "Gutter_pipe_2story_R", "Gutter_pipe_abovePorch_R", "Gutter_pipe_1story_L", "Gutter_pipe_2story_L", "Gutter_pipe_abovePorch_L" }, new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(-0.5f, 0f, -12f),
				new Vector3(-0.5f, 0f, -12f),
				new Vector3(-0.5f, 0f, -12f)
			}, new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 180f, 0f),
				new Vector3(0f, 180f, 0f),
				new Vector3(0f, 180f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x", "Porch_A", new string[4] { "Porch_A_3x_broken_A", "Porch_A_3x_broken_B", "Porch_A_3x_broken_C", "Porch_A_3x_broken_D" }, new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x_noColumn", "Porch_A", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x_corner", "Porch_A", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x_end_entrance_L", "Porch_A", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x_end_entrance_R", "Porch_A", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x_end_L", "Porch_A", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x_end_R", "Porch_A", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_3x_entrance", "Porch_A", new string[1] { "Porch_A_3x_entrance_broken_A" }, new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Porch_A_6x_entrance_separate", "Porch_A", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Foundation_1x", "Foundation", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Foundation_2x", "Foundation", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Foundation_3x", "Foundation", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Foundation_6x", "Foundation", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Stairs_basement", "Stairs", new string[0], new string[0], new string[0], new string[1] { "Floor_6x3" }, new string[0], new Vector3[1]
			{
				new Vector3(0f, -3f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, -90f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Stairs_main_L", "Stairs", new string[0], new string[0], new string[0], new string[2] { "Floor_6x3_stairs_L", "Ceiling_6x3_stairs_L" }, new string[2] { "Floor", "Ceiling" }, new Vector3[2]
			{
				new Vector3(0f, 3f, 0f),
				new Vector3(0f, 3f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Stairs_main_R", "Stairs", new string[0], new string[0], new string[0], new string[2] { "Floor_6x3_stairs_R", "Ceiling_6x3_stairs_R" }, new string[2] { "Floor", "Ceiling" }, new Vector3[2]
			{
				new Vector3(0f, 3f, 0f),
				new Vector3(0f, 3f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Stairs_straight_L", "Stairs", new string[0], new string[0], new string[0], new string[2] { "Floor_6x3_stairs_L", "Ceiling_6x3_stairs_L" }, new string[2] { "Floor", "Ceiling" }, new Vector3[2]
			{
				new Vector3(0f, 3f, 0f),
				new Vector3(0f, 3f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Stairs_straight_R", "Stairs", new string[0], new string[0], new string[0], new string[2] { "Floor_6x3_stairs_R", "Ceiling_6x3_stairs_R" }, new string[2] { "Floor", "Ceiling" }, new Vector3[2]
			{
				new Vector3(0f, 3f, 0f),
				new Vector3(0f, 3f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_A_L", "Doors", new string[0], new string[5] { "Door_parent_A_R", "Door_parent_B_L", "Door_parent_B_R", "Door_parent_C_L", "Door_parent_C_R" }, new string[5] { "DoorA Right", "DoorB Left", "DoorB Right", "DoorC Left", "DoorC Right" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_A_R", "Doors", new string[0], new string[5] { "Door_parent_A_L", "Door_parent_B_L", "Door_parent_B_R", "Door_parent_C_L", "Door_parent_C_R" }, new string[5] { "DoorA Left", "DoorB Left", "DoorB Right", "DoorC Left", "DoorC Right" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_B_L", "Doors", new string[0], new string[5] { "Door_parent_A_L", "Door_parent_A_R", "Door_parent_B_R", "Door_parent_C_L", "Door_parent_C_R" }, new string[5] { "DoorA Left", "DoorA Right", "DoorB Right", "DoorC Left", "DoorC Right" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_B_R", "Doors", new string[0], new string[5] { "Door_parent_A_L", "Door_parent_A_R", "Door_parent_B_L", "Door_parent_C_L", "Door_parent_C_R" }, new string[5] { "DoorA Left", "DoorA Right", "DoorB Left", "DoorC Left", "DoorC Right" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_C_L", "Doors", new string[0], new string[5] { "Door_parent_A_L", "Door_parent_A_R", "Door_parent_B_L", "Door_parent_B_R", "Door_parent_C_R" }, new string[5] { "DoorA Left", "DoorA Right", "DoorB Left", "DoorB Right", "DoorC Right" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_C_R", "Doors", new string[0], new string[5] { "Door_parent_A_L", "Door_parent_A_R", "Door_parent_B_L", "Door_parent_B_R", "Door_parent_C_L" }, new string[5] { "DoorA Left", "DoorA Right", "DoorB Left", "DoorB Right", "DoorC Left" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_A_double", "Doors", new string[0], new string[3] { "Door_parent_B_double", "Door_parent_C_double", "Door_parent_sliding_double" }, new string[3] { "DoorB double", "DoorC double", "Door Sliding double" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_B_double", "Doors", new string[0], new string[3] { "Door_parent_A_double", "Door_parent_C_double", "Door_parent_sliding_double" }, new string[3] { "DoorA double", "DoorC double", "Door Sliding double" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_C_double", "Doors", new string[0], new string[3] { "Door_parent_A_double", "Door_parent_B_double", "Door_parent_sliding_double" }, new string[3] { "DoorA double", "DoorB double", "Door Sliding double" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Door_parent_sliding_double", "Doors", new string[0], new string[3] { "Door_parent_A_double", "Door_parent_B_double", "Door_parent_C_double" }, new string[3] { "DoorA double", "DoorB double", "DoorC double" }, new string[0], new string[0], new Vector3[0], new Vector3[0], new string[0], new Vector3[0], new Vector3[0]),
			new SwapPrefabProperties("Ceiling_wood_3x_single", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Ceiling_wood_3x3", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Ceiling_wood_9x3", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Ceiling_wood_pilar", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Fireplace", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_cabinet_A", "Extras", new string[0], new string[0], new string[0], new string[2] { "Kitchen_cabinet_end_L", "Kitchen_cabinet_end_R" }, new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(-1f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_cabinet_B", "Extras", new string[0], new string[0], new string[0], new string[2] { "Kitchen_cabinet_end_L", "Kitchen_cabinet_end_R" }, new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(-1f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_cabinet_C", "Extras", new string[0], new string[0], new string[0], new string[2] { "Kitchen_cabinet_end_L", "Kitchen_cabinet_end_R" }, new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(-1f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_cabinet_corner", "Extras", new string[0], new string[0], new string[0], new string[2] { "Kitchen_cabinet_end_L", "Kitchen_cabinet_end_R" }, new string[0], new Vector3[2]
			{
				new Vector3(1f, 0f, 0f),
				new Vector3(0f, 0f, 1f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 90f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_cabinet_shelf_L", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(1f, 0f, 0f),
				new Vector3(0f, 0f, 1f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 90f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_cabinet_shelf_R", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(1f, 0f, 0f),
				new Vector3(0f, 0f, 1f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 90f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_counter_A", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_counter_B", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_counter_dishwasher", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_counter_sink", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_counter_corner", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Kitchen_counter_separate", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new Vector3[2]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Pipe_1x", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Pipe_3x", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Pipe_3x_valve", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Pipe_corner_L", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Pipe_corner_R", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Pipe_corner_T", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Pipe_end", "Extras", new string[0], new string[0], new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Windowblinds_open_A", "Extras", new string[0], new string[3] { "Windowblinds_open_B", "Windowblinds_open_C", "Windowblinds_closed" }, new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Windowblinds_open_B", "Extras", new string[0], new string[3] { "Windowblinds_open_A", "Windowblinds_open_C", "Windowblinds_closed" }, new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Windowblinds_open_C", "Extras", new string[0], new string[3] { "Windowblinds_open_A", "Windowblinds_open_B", "Windowblinds_closed" }, new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}),
			new SwapPrefabProperties("Windowblinds_closed", "Extras", new string[0], new string[3] { "Windowblinds_open_A", "Windowblinds_open_B", "Windowblinds_open_C" }, new string[0], new string[0], new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new string[0], new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			}, new Vector3[1]
			{
				new Vector3(0f, 0f, 0f)
			})
		};
	}
}
