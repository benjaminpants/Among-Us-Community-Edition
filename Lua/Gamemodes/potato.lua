--This is a recreation of the Among Us Classic Gamemode.

local invent_timer = 15
local timedif = 0

function InitializeGamemode()
	Game_CreateRole("Potatoed",{102,72,49},"That potato looks pretty hot...",{0,2},0,5,false,true)
	UI_AddLangEntry("UI_CantCallMeeting","No time for meetings.")
	Settings_CreateByte("Time Until Death",0,60,15,5) -- 0
	return {"Hot Potato",12} 
end


function OnTaskCompletionClient(totaltasks,completedtasks,player) --use this to cancel tasks
	return false
end

function OnTaskCompletionHost(totaltasks,completedtasks,player) --this is ran on the hosts end, usually used to trigger host only events like Winning, we don't need to do anything with this so we're going to make it return false
	return false --this isn't used
end

function OnExile(exiled)
end

function OnClientUpdate(timer,timesincelastround)
	if (Client_GetLocalPlayer().role == Game_GetRoleIDFromUUID("potato_Potatoed")) then
		invent_timer = invent_timer - (timer - timedif)
		if (not (math.ceil(timer) == math.ceil(timedif))) then
			if (math.ceil(invent_timer) > -1 and (math.ceil(invent_timer) % 5 == 0 or math.ceil(invent_timer) < 6)) then
			Client_ClearMessages()
			Client_ShowMessage(math.ceil(invent_timer) .. " Seconds left!")
			end
		end
		if (invent_timer < 0) then
			Game_KillPlayer(Client_GetLocalPlayer(),false)
		end
	else
		invent_timer = Settings_GetNumber(0)
	end
	timedif = timer
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return false
end

function OnPlayerDC(playerinfo)
	if (playerinfo.role == Game_GetRoleIDFromUUID("potato_Potatoed") and Net_AmHost()) then
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == playerinfo.PlayerName or players[i].IsDead) then
				table.remove(players,i)
			end
		end
		local selected = {players[math.random(1,#players)]}
		Game_SetRoles(selected,{"potato_Potatoed"})
	end
end

function CanCallMeeting(reporter,isbody)
	return false
end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (#crewmates == 1) then --no more impostors, crewmates win
		return {{crewmates[1]},"hotpotato_win"} --victory array, victory sound(minus the .wav)
	end	
end

function OnGameEnd()
	invent_timer = 20
	timedif = 0
end

function CanKill(userinfo,targetinfo)
	return true
end

function BeforeKill(killer,victim)
	if (killer.PlayerName == Client_GetLocalPlayer().PlayerName) then
		Game_SetRoles({victim,killer},{"potato_Potatoed","None"})
	end
	return false
end

local function GetRoleAmount(id)
	local pl = Game_GetAllPlayers()
	local idamount = 0
	for i=1, #pl do
		if (pl[i].role == id and not pl[i].IsDead) then
		idamount = idamount + 1
		end
	end
	return idamount
end


function OnDeath(playerinfo)
	if (not GetRoleAmount(Game_GetRoleIDFromUUID("potato_Potatoed")) == 0) then
		return
	end
	if (playerinfo.role == Game_GetRoleIDFromUUID("potato_Potatoed") and Net_AmHost()) then
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == playerinfo.PlayerName or players[i].IsDead or players[i].IsImpostor) then
				table.remove(players,i)
			end
		end
		local selected = players[math.random(1,#players)]
		Game_SetRoles({selected,playerinfo},{"potato_Potatoed","undefined_undefined"})
	end
end


function DecideRoles(playerinfos)
	return {{playerinfos[math.random(1,#playerinfos)]},{"potato_Potatoed"}}
end


function DecideImpostors(impostorcount,playerinfos)
	return {}
end

