--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	return {"Splatoon",7} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
end


function OnTaskCompletionClient(totaltasks,completedtasks,player) --use this to cancel tasks
	if (not userinfo.IsImpostor) then
		return true
	end
	return false
end

function OnTaskCompletionHost(totaltasks,completedtasks,player) --this is ran on the hosts end, usually used to trigger host only events like Winning, we don't need to do anything with this so we're going to make it return false
	return false --this isn't used
end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	return "none"
end

function CanKill(userinfo,targetinfo)
	return (not (userinfo.ColorId == targetinfo.ColorId))
end

local function GetColorAmount(id)
	local pl = Game_GetAllPlayers()
	local idamount = 0
	for i=1, #pl do
		if (pl[i].ColorId == id) then
		idamount = idamount + 1
		end
	end
	Debug_Log("id amount: " .. idamount)
	return idamount
end

function BeforeKill(killer,victim)
	plys = Game_GetAllPlayers()
	victim.ColorId = killer.ColorId
	Game_UpdatePlayerInfo(victim)
	Net_SendMessageToHostSimple(0)
	return false
end

function OnExile(exiled)
	
end

function OnExileSkip()

end


function OnHostRecieveSimple(id)
	if (id == 0) then
		plys = Game_GetAllPlayers()
		local winlist = {}
		for i=1, 24 do
			table.insert(winlist,0)
		end
		local winners = {}
		for i=1, #plys do
			if (GetColorAmount(plys[i].ColorId) == #plys) then
				table.insert(winners,plys[i])
				Debug_Log("I got here!")
				Game_ActivateCustomWin(Game_GetAllPlayers(),"splatoon_win")
			end
			Debug_Log("playercount:" .. #plys)
		end
	end
end

function OnGameEnd()
	
end

function DecideRoles(playerinfos)
	return {{},{}} -- no roles, also don't question
end


function DecideImpostors(impostorcount,playerinfos)
	OGPlayers = playerinfos
	return playerinfos
end

