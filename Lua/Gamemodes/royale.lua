--Fight to the Death: The Gamemode


function InitializeGamemode()
	return {"Battle Royale",5} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
end

function OnTaskCompletionClient(totaltasks,completedtasks,player)
	print("how") --how
	return false
end

function OnTaskCompletionHost(totaltasks,completedtasks,player)
	return false --dumb
end

function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (#impostors == 1) then
		return {{impostors[1]},"battleroyale_win"} --victory array, victory sound(minus the .wav)
	end
	return "none"
end

function DecideRoles(playerinfos)
	return {{},{}} -- no roles, also don't question
end

function BeforeKill(killer,victim)
	return true
end

function CanKill(userinfo,targetinfo)
	return true --the API already prevents the slaying of dead people so make this always return true
end


function DecideImpostors(impostorcount,playerinfos)
	return playerinfos --everyones impostor!!!
end

