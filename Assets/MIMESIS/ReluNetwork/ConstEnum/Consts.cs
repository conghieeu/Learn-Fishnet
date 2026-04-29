namespace ReluNetwork.ConstEnum
{
	public class Consts
	{
		public const int PACKET_HEADER_SIZE = 4;

		public const int MAX_PACKET_SIZE = 65536;

		public const int TOTAL_RECV_BUFFER_SIZE = 4194304;

		public const int TOTAL_SEND_BUFFER_SIZE = 1048576;

		public const int MAX_ONE_RECV_BUFFER_SIZE = 64;

		public const int PACKET_LENGTH_SIZE = 4;

		public const int PACKET_TYPE_SIZE = 4;

		public const int PACKET_MIN_SIZE = 8;

		public const int DEFAULT_SENDRES_WAIT_TIME = 60000;

		public const int TIMEOUT_BACKEND_API_MINUTES = 10;

		public const int DEFAULT_MAX_API_CONNECTION = 100;

		public const string PLAYER_UID_HTTP_HEADER_STRINGKEY = "player_uid";

		public const string SESSION_ID_HTTP_HEADER_STRINGKEY = "session-id";

		public const string IS_ADMIN_HTTP_HEADER_STRINGKEY = "is-admin";

		public const string STEAM_ID_HTTP_HEADER_STRINGKEY = "steam-id";

		public const string MESSAGE_TYPE = "Message-Type";

		public const string HEADER_TRACE_ID = "TRACE-ID";

		public const int MAX_CONNECTION = 100;

		public const int MAX_NODE_RUDP_CONNECTION = 16;

		public const int DEFAULT_RUDP_PORT = 40443;

		public const int DEFAULT_NODE_HEALTHCHECK_INTERVAL = 5000;

		public const int MAX_NODE_CONNECTION_COUNT = 200;

		public const int DEFAULT_MAX_HTTP_TIMEOUT = 60;

		public const string RUDP_KEY = "RELURELU";

		public const int TCP_CLIENT_MAX_BUFFER_SIZE = 1048576;

		public const int PACKET_MSG_TYPE_SIZE = 4;

		public const float MILLISEC_TO_SEC_MAGNIFICANT = 0.001f;

		public const int SEC_TO_MILLISEC_MAGNIFICANT = 1000;

		public const int MAX_RECONNECT_COUNT = 5;

		public const float HEART_BEAT_PERIOD = 10f;

		public const int HEART_BEAT_DEAT_COUNT = 10;

		public const float LOGIN_RETRY_PERIOD = 5f;

		public const int VIRTUAL_SESSION_ID = 999999999;

		public const long HOLE_PUNCH_UPDATE_PERIOD = 10000L;

		public const long HOLE_PUNCH_CHECK_PERIOD = 10000L;

		public const int DEFAULT_SESSION_ID = 9999999;

		public const int MAX_MESSAGE_COUNT = 16;

		public const string REPLAY_STORAGE_SERVER_URL = "https://mimesisapi.relugameservice.com:22443";

		public const string API_LOG_SERVER_URL = "https://mimesisapi.relugameservice.com:22226";
	}
}
