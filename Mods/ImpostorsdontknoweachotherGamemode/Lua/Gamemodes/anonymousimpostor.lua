


function InitializeGamemode()
	Game_CreateRole("Impostor",{255,25,25},"There is [FF1919FF]another Impostor []among you",{0,1,2},0,0,true,false,255,false,"[FF1919FF]Sabotage and kill everyone")
	Settings_CreateByte("Impostors:",2,2,2) -- 0
	return {"Anonymous Impostors",16}
end


function OnTaskCompletionClient(totaltasks,completedtasks,player) --use this to cancel tasks
	if (not player.IsImpostor) then
		return true
	end
	return false
end

function OnTaskCompletionHost(totaltasks,completedtasks,player) --this is ran on the hosts end, usually used to trigger host only events like Winning, we don't need to do anything with this so we're going to make it return false
	return false --this isn't used
end

function OnHostUpdate(timer,timesincelastround)

end

function OnClientUpdate(timer,timesincelastround)
	
end

function OnChat(message, player)
	return true
end

function OnExile(exiled)
)
end

function OnExileSkip()

end

function OnPlayerDC(playerinfo)

end

function CanVent(default,playerinfo)
	return true
end

function CanCallMeeting(reporter,isbody)
	return true
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return true
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


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (#crewmates == GetRoleAmount(Game_GetRoleIDFromUUID("anonymousimpostor_Impostor"))) then --crewmates can't win
		return "impostors"
	end
	if (GetRoleAmount(Game_GetRoleIDFromUUID("anonymousimpostor_Impostor")) == 0) then
		return "crewmates"
	end
	if (taskscomplete) then --task win
		return "crewmates"
	end
	if (sab) then
		return "impostors"
	end
	return "none"
end

function CanKill(userinfo,targetinfo)
	return true
end

function BeforeKill(killer,victim)
	return true
end

function OnGameEnd()

end

function DecideRoles(playerinfos)
	local RolesToGive = {}
	for i=1, Settings_GetNumber(0) do
		table.insert(RolesToGive,"anonymousimpostor_Impostor")
	end
	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the roles
end


function DecideImpostors(impostorcount,playerinfos)
	return {}
end

