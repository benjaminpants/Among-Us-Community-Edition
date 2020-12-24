--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	Game_CreateRole("Red Team",{255,15,15},"Convert the entire [0F0FFFFF]Blue Team[].",{0},0,0,false)
	Game_CreateRole("Blue Team",{15,15,255},"Convert the entire [FF0F0FFF]Red Team[].",{0},0,0,false)
	return {"Splatoon Teams",8} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
end


function OnTaskCompletionClient(totaltasks,completedtasks,player) --use this to cancel tasks
	return false
end

function OnTaskCompletionHost(totaltasks,completedtasks,player) --this is ran on the hosts end, usually used to trigger host only events like Winning, we don't need to do anything with this so we're going to make it return false
	return false --this isn't used
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return false
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



function OnHostRecieveSimple(id)
	if (id == 0) then
		plys = Game_GetAllPlayers()
		local winlist = {}
		for i=1, 24 do
			table.insert(winlist,0)
		end
		local winners = {}
		local arewin = false
		for i=1, #plys do
			if (GetColorAmount(plys[i].ColorId) == #plys) then
				if ((plys[i].ColorId == 0 and plys[i].role == Game_GetRoleIDFromName("Red Team")) or plys[i].ColorId == 1 and plys[i].role == Game_GetRoleIDFromName("Blue Team")) then
					table.insert(winners,plys[i])
					arewin = true
					Debug_Log("I got here!")
				end
			end
			Debug_Log("playercount:" .. #plys)
		end
		if (arewin) then
			Game_ActivateCustomWin(winners,"splatoon_win")
		end
	end
end

local function GetRoleAmount(id)
	local pl = Game_GetAllPlayers()
	local idamount = 0
	for i=1, #pl do
		if (pl[i].role == id) then
		idamount = idamount + 1
		end
	end
	Debug_Log("id amount: " .. idamount)
	return idamount
end

function OnGameEnd()
	
end

function DecideRoles(playerinfos)
	local redteamcount = math.ceil(#playerinfos / 2)
	local blueteamcount = #playerinfos - redteamcount
	local RolesToGive = {}
	for i=1,redteamcount do
	table.insert(RolesToGive,"splatoon_teams_Red Team")
	end
	for i=1,blueteamcount do
	table.insert(RolesToGive,"splatoon_teams_Blue Team")
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
	local allplayers = Game_GetAllPlayers()
	for i=1, #allplayers do
		if (allplayers[i].role == Game_GetRoleIDFromName("Blue Team")) then
			allplayers[i].ColorId = 1
		else
			allplayers[i].ColorId = 0
		end
		Game_UpdatePlayerInfo(allplayers[i])
	end
	return {}
end

