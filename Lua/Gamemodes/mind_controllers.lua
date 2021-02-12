function InitializeGamemode()
	Game_CreateRole("Mind Controller",{130,255,130},"Control everyone",{0,1,2},0,0,true,false,1,false,"[CCFFCCFF]Control everyone...[]")
	Game_CreateRole("Controlled",{130,0,130},"Kill everyone",{0},5,5,false,false,1,false,"[CC0000FF]Kill everyone...[]")
	Game_CreateRole("Crewmate",{130,255,255},"You can now give [CC00CCFF]Controlled[] people their minds\nback by hitting them with a bat",{0,2},0,0,false,true,2,true,"Find out who the mind controller is.")
	
	Settings_CreateByte("How many Mind Controllers?",1,2,1)
	return {"Mind Controllers VS Crewmates",14} --Code optimized and fixed by Ben, original made by KYUU.
end

local function GetAlivePlayers(nametoavoid)
	local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
	for i=#players,1,-1 do
		if (players[i].IsDead or players[i].PlayerName == nametoavoid) then
			table.remove(players,i)
		end
	end
	return players
end

local function GetDeadPlayers(nametoavoid,roletoavoid)
	local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
	for i=#players,1,-1 do
		if ((not players[i].IsDead) or players[i].PlayerName == nametoavoid or players[i].role == roletoavoid) then
			table.remove(players,i)
		end
	end
	return players
end

local function GetAllPlayersOfSameRole(id)
	local players = Game_GetAllPlayers()
	for i=1,#players do
		if (not players[i].role == id) then
			table.remove(players,i)
		end
	end
end

local function GetAllPlayersWithNotThisRole(id)
	local players = Game_GetAllPlayers()
	for i=1,#players do
		if (players[i].role == id) then
			table.remove(players,i)
		end
	end
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
	if(player.role == Game_GetRoleIDFromUUID("mind_controllers_Controlled")) then
		return false
	end
	return true
end

function OnExile(exiled)
	if (exiled.role == Game_GetRoleIDFromUUID("mind_controllers_Mind Controller")) then
		if (Net_AmHost()) then
			Game_ActivateCustomWin(GetAllPlayersOfSameRole(Game_GetRoleIDFromUUID("mind_controllers_Crewmate")),"crewcontroller_win")
		end
	end
end

function OnDeath(dead,reason)

end

function OnExileSkip()

end

function OnRecieveRole(rolename,player)

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

function OnGameStart()

end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	evilcrewmates = {}
	goodcrewmates = {}
	for i=1,#crewmates do
		if (crewmates[i].role == Game_GetRoleIDFromUUID("mind_controllers_Mind Controller") or crewmates[i].role == Game_GetRoleIDFromUUID("mind_controllers_Controlled")) then
			table.insert(evilcrewmates,crewmates[i])
		end
	end
	for i=1,#crewmates do
		if (crewmates[i].role == Game_GetRoleIDFromUUID("mind_controllers_Crewmate")) then
			table.insert(goodcrewmates,crewmates[i])
		end
	end
	if (not sab) then --If the check isn't due to a sabotage
		if (#evilcrewmates >= #goodcrewmates) then --crewmates can't win
			return {evilcrewmates,"mindcontroller_win"}
		end
		if (#evilcrewmates <= 0) then --no more impostors, crewmates win
			return {goodcrewmates,"crewcontroller_win"}
		end
		if (#crewmates == 0) then
			return "stalemate"
		end
		if (taskscomplete) then --task win	
			return {goodcrewmates,"crewcontroller_win"}
		end

		return "none" --no win conditions have been met.
	else
		
		return {evilcrewmates,"mindcontroller_win"} --sab loose
		
	end
end

function CanKill(userinfo,targetinfo)
	if (userinfo.IsImpostor and not targetinfo.IsImpostor) then --if the person doing the kill is an impostor and the victim
		return true
	end
	if (userinfo.role == Game_GetRoleIDFromUUID("mind_controllers_Mind Controller")) then
		if (targetinfo.role == Game_GetRoleIDFromUUID("mind_controllers_Crewmate")) then
			return true
		end
	end

	if (userinfo.role == Game_GetRoleIDFromUUID("mind_controllers_Crewmate")) then
		if (targetinfo.role == Game_GetRoleIDFromUUID("mind_controllers_Controlled")) then
			return true
		end
	end

	if (userinfo.role == Game_GetRoleIDFromUUID("mind_controllers_Controlled")) then
		if (targetinfo.role == Game_GetRoleIDFromUUID("mind_controllers_Crewmate")) then
			return true
		end
	end
	return false
end


function BeforeKill(killer,victim)
	if (killer.role == Game_GetRoleIDFromUUID("mind_controllers_Mind Controller")) then
		Game_SetRoles({victim},{"None"})
		Game_SetRoles({victim},{"mind_controllers_Controlled"})
		Game_UpdatePlayerInfo(victim)
		local middle = ""
		if (GetRoleAmount(Game_GetRoleIDFromUUID("mind_controllers_Controlled")) == 1) then
			middle = " person is"
		else
			middle = " people are"
		end
		
		Client_ShowMessage(GetRoleAmount(Game_GetRoleIDFromUUID("mind_controllers_Controlled")) .. middle .. " now [CCFFCCFF]controlled[]")
		
		return false
	end
	if (killer.role == Game_GetRoleIDFromUUID("mind_controllers_Crewmate")) then
		
		Game_SetRoles({victim},{"None"})
		Game_SetRoles({victim},{"mind_controllers_Crewmate"})
		Game_UpdatePlayerInfo(victim)
		
		return false
	end
	if (killer.role == Game_GetRoleIDFromUUID("mind_controllers_Controlled")) then
		return true
	end
	return true
end

function OnGameEnd()

end

function DecideRoles(playerinfos)
	return DecideRolesFunction(playerinfos)
end

function DecideRolesFunction(playerinfos)
	local RolesToGive = {}
	
	table.insert(RolesToGive,"mind_controllers_Mind Controller")
	
	local StartingCrewmateCount = #playerinfos - Settings_GetNumber(0)

	for i=1,StartingCrewmateCount do
		table.insert(RolesToGive,"mind_controllers_Crewmate")
	end

	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the sheriff's role

end

function DecideImpostors(impostorcount,playerinfos)
	local selected = {}
	
	return selected
end

