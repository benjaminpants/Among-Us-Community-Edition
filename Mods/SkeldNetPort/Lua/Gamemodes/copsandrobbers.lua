
function InitializeGamemode()
	Game_CreateRole("Cop",{15,15,255},"Capture all [FC8803FF]Robbers[].",{0},2,5,true,false,1)
	Game_CreateRole("Cop Leader",{15,15,255},"Capture all [FC8803FF]Robbers[].\nYou can sabotage.",{0,1},2,5,true,false,1)
	Game_CreateRole("Robber",{252, 136, 3},"Complete all tasks.\nFree any captured Robbers.",{0},2,5,false,true,2)
	Game_CreateRole("Captured",{255, 255, 15},"If you see this, what are you doing?",{},2,5,false,true,2)
	
	UI_AddLangEntry("UI_CantCallMeeting","This button doesn't exist. \n[FF0000FF]Ignore it.[]")
	
	
	Settings_CreateBool("Cops Auto-Capture",false) -- 0
	Settings_CreateBool("Robbers Auto-Free",true) -- 1
	Settings_CreateBool("Cop Leader Enabled",true) -- 2
	
	
	return {"Cops And Robbers",11}
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


function OnVote(voter,voteid,isskip)
	--voter is a player
	--vote id is a vote id
	--isskip is self explanitory
	--returning -1 results in a skip
	return -1
end

local function distance_from_points ( x1, y1, x2, y2 ) --thanks LOVE2D community!
  local dx = x1 - x2
  local dy = y1 - y2
  return math.sqrt ( dx * dx + dy * dy )
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

function OnHostUpdate(timer,timesincelastround)
	if ((timer <= 1.0) and not (timer >= 1.1)) then
		local yes = GetAlivePlayers("SUSSY SUSSY IMPOSTOR HACKER CARD FORNITE")
		Debug_Log("stop being sus")
		for i=1, #yes do
			if (yes[i].role == Game_GetRoleIDFromUUID("copsandrobbers_Robber")) then
				Player_SnapPosTo(-20,-5,yes[i])
			else
				Player_SnapPosTo(9,1,yes[i])
			end
		end
	end
end


local function GetPlayersWithRole(id)
	local pl = Game_GetAllPlayers()
	local pls = {}
	for i=1, #pl do
		if (pl[i].role == id and not pl[i].IsDead) then
		table.insert(pls,pl[i])
		end
	end
	return pls
end

function OnClientUpdate(timer,timesincelastround)
	local locplayer = Client_GetLocalPlayer()
	if (locplayer.role == Game_GetRoleIDFromUUID("copsandrobbers_Captured")) then
		if (distance_from_points(-8,-8,locplayer.PosX,locplayer.PosY) >= 4) then
			Player_SnapPosTo(-8,-8,locplayer)
		end
		if (Settings_GetBool(1)) then
			local aliveplayers = GetAlivePlayers(locplayer.PlayerName)
			for i=1, #aliveplayers do
				if ((distance_from_points(locplayer.PosX,locplayer.PosY,aliveplayers[i].PosX,aliveplayers[i].PosY) <= 0.5) and aliveplayers[i].role == Game_GetRoleIDFromUUID("copsandrobbers_Robber")) then
					Game_SetRoles({locplayer},{"copsandrobbers_Robber"})
				end
			end
		end
	end
	
	if (locplayer.role == Game_GetRoleIDFromUUID("copsandrobbers_Robber")) then
		if (Settings_GetBool(0)) then
			local aliveplayers = GetAlivePlayers(locplayer.PlayerName)
			for i=1, #aliveplayers do
				if ((distance_from_points(locplayer.PosX,locplayer.PosY,aliveplayers[i].PosX,aliveplayers[i].PosY) <= 0.5) and (aliveplayers[i].role == Game_GetRoleIDFromUUID("copsandrobbers_Cop") or aliveplayers[i].role == Game_GetRoleIDFromUUID("copsandrobbers_Cop Leader"))) then
					Player_SnapPosTo(-8,-8,locplayer)
					Game_SetRoles({locplayer},{"copsandrobbers_Captured"})
				end
			end
		end
	end
end

function OnChat(message, player)
	return true
end

function OnExile(exiled)
	
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
	return false
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return (playerinfo.role == Game_GetRoleIDFromUUID("copsandrobbers_Robber"))
end

function OnGameStart()

end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (not sab) then --If the check isn't due to a sabotage
		if (taskscomplete) then --task win
			Game_ActivateCustomRolesWin({"copsandrobbers_Robber","copsandrobbers_Captured"},"robber_win")
			return "none"
		else
			if (GetRoleAmount(Game_GetRoleIDFromUUID("copsandrobbers_Robber")) == 0) then
				Game_ActivateCustomRolesWin({"copsandrobbers_Cop","copsandrobbers_Cop Leader"},"cops_win")
				return "none"
			end
		end
	else
		Game_ActivateCustomRolesWin({"copsandrobbers_Cop","copsandrobbers_Cop Leader"},"cops_win")
		return "none"
	end
	if (crewmates == 0) then
		return "stalemate"
	end
	return "none"
end

function CanKill(userinfo,targetinfo)
	return (((targetinfo.role == Game_GetRoleIDFromUUID("copsandrobbers_Robber") and (userinfo.role == Game_GetRoleIDFromUUID("copsandrobbers_Cop") or userinfo.role == Game_GetRoleIDFromUUID("copsandrobbers_Cop Leader"))) and not Settings_GetBool(0)) or (targetinfo.role == Game_GetRoleIDFromUUID("copsandrobbers_Captured") and (userinfo.role == Game_GetRoleIDFromUUID("copsandrobbers_Robber"))) and not Settings_GetBool(1))
end


function BeforeKill(killer,victim)
	if (victim.role == Game_GetRoleIDFromUUID("copsandrobbers_Robber")) then
		Game_SetRoles({victim},{"copsandrobbers_Captured"})
		Player_SnapPosTo(-8,-8,victim)
	end
	if (victim.role == Game_GetRoleIDFromUUID("copsandrobbers_Captured")) then
		Game_SetRoles({victim},{"copsandrobbers_Robber"})
	end
	return false
end

function OnGameEnd()

end

function DecideRoles(playerinfos)
	math.randomseed(Misc_GetCurrentTime())
	local redteamcount = math.ceil(#playerinfos / 2)
	local blueteamcount = #playerinfos - redteamcount
	local RolesToGive = {}
	if (not Settings_GetBool(2)) then
		blueteamcount = blueteamcount + 1
	end
	for i=1,blueteamcount - 1 do
	table.insert(RolesToGive,"copsandrobbers_Cop")
	end
	for i=1,redteamcount do
	table.insert(RolesToGive,"copsandrobbers_Robber")
	end
	if (Settings_GetBool(2)) then
		table.insert(RolesToGive,"copsandrobbers_Cop Leader")
	end
	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[impid])
		table.remove(RolesToGive,impid)
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the roles
end


function DecideImpostors(impostorcount,playerinfos)
	return {} --no.
end

