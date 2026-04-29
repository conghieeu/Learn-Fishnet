using System;

public static class VWorldConsts
{
	public const string GAME_NAME_CODE = "MIMESIS-MIMIC-MENOTME";

	public const int PRE_DEFINED_EXECUTOR_START = 10000000;

	public const int VWORLD_MANAGER_ID = 10000001;

	public const int SESSION_MAMAGER_ID = 10000002;

	public const int VROOM_MANAGER_ID = 10000003;

	public const int HOST_DEFAULT_PORTNUM = 30010;

	public const int DEFAULT_MAP_SEARCH_RANGE = 2000;

	public const int DEFAULT_MAP_MAX_SEARCH_RANGE = 1000000;

	public const double SINGULARITY_THRESHOLD = 0.4999994933605194;

	public const double RAD_TO_DEG = 180.0 / Math.PI;

	public const double DEG_TO_RAD = Math.PI / 180.0;

	public const double HALF_DEG_TO_RAD = Math.PI / 360.0;

	public static double[] DEG_TO_RAD_HALF_ARRAY = new double[4]
	{
		Math.PI / 360.0,
		Math.PI / 360.0,
		Math.PI / 360.0,
		Math.PI / 360.0
	};

	public static double[] DEG_TO_RAD_ARRAY = new double[4]
	{
		Math.PI / 180.0,
		Math.PI / 180.0,
		Math.PI / 180.0,
		Math.PI / 180.0
	};

	public static double[] RAD_TO_DEG_ARRAY = new double[4]
	{
		180.0 / Math.PI,
		180.0 / Math.PI,
		180.0 / Math.PI,
		180.0 / Math.PI
	};

	public const double INV_PI = 0.31830988618;

	public const double HALF_PI = 1.57079632679;

	public const double FASTASIN_HALF_PI = 1.570796305;

	public const double SMALLVAL = 1E-08;

	public const double KINDA_SMALLVAL = 0.0001;

	public const long FORCE_MOVE_SYNC_RATE = 2L;

	public const int LOBBY_ROOM_MASTERID = 1;

	public const float MAX_DIRECT_MOVE_CREDIT_SEC = 5f;

	public const float DIRECT_MOVE_CRERDIT_ERROR_CORRECT_RATE = 1.1f;

	public const float DIRECT_MOVE_PREVIOUS_POSITION_THRESHOLD = 10f;

	public const float MAX_DIRECT_MOVE_FUTURE_TIME = 1000f;

	public const float DIRECT_MOVE_CREDIT_CORRECT_RATE = 1.5f;

	public const string AI_ACTIVE_TEMPLATE = "active";

	public const string AI_PASSIVE_TEMPLATE = "passive";

	public const string AI_ATTACK_TEMPLATE = "attack";

	public const string DEFAULT_BOT_AI_DATA_NAME = "botavatar";

	public const int DEFAULT_INVENTORY_SLOT_COUNT = 4;

	public const float REFRESH_INVALID_POSITION_DISTANCE = 500f;

	public const float TRACE_DISTANCE = 100f;

	public const float AI_DL_DISTANCE_THRESHOLD = 1f;

	public const float AI_ARRIVE_DISTANCE_THRESHOLD = 100f;

	public const string AI_DATA_SUBPATH = "aidata/bt/";

	public const string AI_PARAM_SUBPATH = "aidata/param/";

	public const string ANIM_DATA_SUBPATH = "anim/";

	public const int MAX_GAME_SESSION_COUNT = 3;

	public const int MAX_LOOPCOUNT_FIND_RANDOM_POSITION = 5;

	public const double DISTANCE_ARRIVED = 0.5;

	public const float DEFAULT_LOOTING_OBJECT_SPAWN_DISTANCE = 1f;

	public const float DEFAULT_BUY_ITEM_SPAWN_DISTANCE = 0.6f;

	public const float DEFAULT_BUY_ITEM_RANDOM_RADIUS = 0.3f;

	public const int MAX_AI_GROUPCOMPOSITE_SELECT_INDEX = 100000;

	public const int MAX_BT_ITERATION_COUNT = 100;

	public const int MAX_BT_FAIL_COUNT = 10;

	public const int ACTOR_DEAD_EVENT_CHECK_INTERVAL = 1000;

	public const int DUNGEON_END_DELAY = 3000;

	public const int DUNGEON_START_DELAY = 1000;

	public const int SESSION_END_DELAY = 5000;

	public const int SESSION_RESTART_DELAY = 2000;

	public const int WAITINGROOM_RETURN_DELAY = 1000;

	public const int TEMP_SOUND_CLIP_DELAY = 10000;

	public const int TEMP_RESERVE_ACTOR_ID = 999999;

	public const long FALL_CHECK_EXPIRE_MILLISEC = 3000L;

	public const long LEVER_ENABLE_DELAY = 1000L;

	public const int FRAMETIME = 33;

	public const long FALL_CHECK_STOP_TIME = 2000L;

	public const long FPS = 30L;

	public const float ROOT_MOTION_MIN_DISTANCE = 6f;

	public const float VCREATURE_DEFAULT_HEIGHT = 2f;

	public const int DEFAULT_MIMIC_MASTER_ID = 20000001;

	public const long CUTSCENE_PLAY_TIME_MARGIN = 2000L;

	public const long CUTSCENE_BROADCAST_PERIOD = 500L;

	public const string INVALID_ACTOR_NAME = "no name reluman";

	public const long NETWORK_GRADE_BROADCAST_PERIOD = 5000L;

	public const long NETWORK_GRADE_EXPIRED_PERIOD = 60000L;

	public const long GAME_START_WAIT_TIME_LIMIT = 40000L;

	public const long PERIODIC_ALL_MEMBER_ENTER_PKT_BROADCAST_TIME = 10000L;

	public const int DEFAULT_AURA_DURATION = 600000;

	public const string DEFAULT_NO_BATTLE_ACTION = "NO_ACTION";

	public const string BATTLE_ACTION_BASE_KEY = "BATTLE_ACTION";

	public const long PERIODIC_SYNC_TIME = 5000L;

	public const float MAX_DISTANCE_HOLE_POINT_NEAREST_POLY = 2f;

	public const float MAX_DISTANCE_BOTTOM_RAYCAST = 50f;

	public const float GRAVITY_FORCE = 9.81f;

	public const float DISTANCE_TO_DETERMINE_LANDING = 0.2f;

	public const long JUMP_DURATION = 800L;

	public const int AGGRO_BASE_VALUE = 100;

	public const long PATH_MOVE_UPDATE_INTERVAL = 100L;

	public const long AI_UPDATE_TIME = 200L;

	public const int VROM_STASH_MAX_COUNT = 4;
}
