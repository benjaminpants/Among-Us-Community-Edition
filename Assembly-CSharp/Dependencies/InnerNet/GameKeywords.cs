using System;

namespace InnerNet
{
	[Flags]
	public enum GameKeywords : uint
	{
		All = 0x0u,
		AllLanguages = 0x1Fu,
		English = 0x1u,
		Spanish = 0x2u,
		Korean = 0x4u,
		Russian = 0x8u,
		Portuguese = 0x10u
	}
}
