function InitializePlugin()
	return {"Swap-kill",2,false} --Plugins can either override functions or run stuff ontop.
end

function BeforeKill(killer,victim)
	local killername = killer.PlayerName
	local killercolor = killer.ColorId
	local killerskin = killer.SkinId
	local killerhat = killer.HatId
	killer.PlayerName = victim.PlayerName
	killer.ColorId = victim.ColorId
	killer.SkinId = victim.SkinId
	killer.HatId = victim.HatId
	victim.PlayerName = killername
	victim.ColorId = killercolor
	victim.SkinId = killerskin
	victim.HatId = killerhat
	Game_UpdatePlayerInfo(victim)
	Game_UpdatePlayerInfo(killer)
	--doesn't overwrite so no need to return any value
end
