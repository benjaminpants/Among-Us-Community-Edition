--Fight to the Death: The Gamemode


function InitializeGamemode()
	return {"Task War",8} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
end

function OnTaskCompletionClient(totaltasks,completedtasks,player)
	if (math.random(1,15) == 5) then
	return false --haha
	end
	return true
end

function OnTaskCompletionHost(totaltasks,completedtasks,player)
	print("sup")
	if (completedtasks >= totaltasks) then
		Game_ActivateCustomWin({player},"taskwar_win")
	end
	return false
end

function DecideRoles(playerinfos)
	return {{},{}} -- no roles, also don't question
end

function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	return "none"
end

function CanKill(userinfo,targetinfo)
	return false --just incase
end


function DecideImpostors(impostorcount,playerinfos)
	return {} --no imps
end

