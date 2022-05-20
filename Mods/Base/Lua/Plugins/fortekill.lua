
function InitializePlugin()
	return {"ForteKill",6,false} --sets up
end
function BeforeKill(killer,victim)
	victim.PlayerName = "???"
	victim.ColorId = 0
	Game_UpdatePlayerInfo(victim)
end