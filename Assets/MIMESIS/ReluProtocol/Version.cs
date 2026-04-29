namespace ReluProtocol
{
	public static class Version
	{
		public const string ProtocolVersion = "123456789";

		public const string MasterDataVersion = "49175378726";

		public static bool IsEnable { get; set; }

		public static int GetProtocolHashCode()
		{
			if (int.TryParse("123456789", out var result) && int.TryParse("49175378726", out var result2))
			{
				return result ^ result2;
			}
			return 0;
		}
	}
}
