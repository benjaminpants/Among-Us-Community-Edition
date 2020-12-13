using Hazel;

public interface ISystemType
{
	bool Detoriorate(float deltaTime);

	void RepairDamage(PlayerControl player, byte amount);

	void Serialize(MessageWriter writer, bool initialState);

	void Deserialize(MessageReader reader, bool initialState);
}
