--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	Game_CreateRole("Sheriff",{255,216,0},"Find and Kill the [FF1919FF]Impostor[].",{0},1,0,false,true)
	return {"Murder",3} 
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


function ShouldSeeRole(rolename,playerinfo)
	return false
end

function OnPlayerDC(playerinfo)
	if (playerinfo.role == Game_GetRoleIDFromName("Sheriff") and Net_AmHost()) then
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == playerinfo.PlayerName or players[i].IsDead or players[i].IsImpostor) then
				table.remove(players,i)
			end
		end
		local selected = {players[math.random(1,#players)]}
		Game_SetRoles(selected,{"Sheriff"})
	end
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return true
end

function OnExile(exiled)
	if (exiled.role == Game_GetRoleIDFromName("Sheriff") and Net_AmHost()) then
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == exiled.PlayerName or players[i].IsDead or players[i].IsImpostor) then
				table.remove(players,i)
			end
		end
		local selected = {players[math.random(1,#players)],exiled}
		Game_SetRoles(selected,{"Sheriff","None"})
	end
end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (not sab) then --If the check isn't due to a sabotage
		if (#impostors >= #crewmates) then --crewmates can't win
			return "impostors"
		end
		if (#impostors <= 0) then --no more impostors, crewmates win
			return "crewmates"
		end
		if (taskscomplete) then --task win
			return "crewmates"
		end
		return "none" --no win conditions have been met.
	else
		return "impostors" --sab loose
	end
end


function CanKill(userinfo,targetinfo)
	if ((userinfo.IsImpostor and not targetinfo.IsImpostor) or userinfo.role == Game_GetRoleIDFromName("Sheriff")) then --if the person doing the kill is an impostor and the victim
		return true
	end
	return false
end

function BeforeKill(killer,victim)
	if (killer.role == Game_GetRoleIDFromUUID("sheriff_Sheriff")) then
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == victim.PlayerName or players[i].IsDead or players[i].IsImpostor or players[i].PlayerName == killer.PlayerName) then
				table.remove(players,i)
			end
		end
		local selected = {players[math.random(1,#players)],killer}
		Game_SetRoles(selected,{"sheriff_Sheriff","None"})
	end
	
	if (victim.role == Game_GetRoleIDFromUUID("sheriff_Sheriff")) then
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == victim.PlayerName or players[i].IsDead or players[i].IsImpostor or players[i].PlayerName == killer.PlayerName) then
				table.remove(players,i)
			end
		end
		local selected = {players[math.random(1,#players)],victim}
		Game_SetRoles(selected,{"sheriff_Sheriff","None"})
	end
	return true
end

function DecideRolesFunction(playerinfos)
	local RolesToGive = {"sheriff_Sheriff"}
	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(RolesToGive,i)
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the sheriff's role

end

function DecideRoles(playerinfos)
	return DecideRolesFunction(playerinfos) -- sets the sheriff's role
end


function DecideImpostors(impostorcount,playerinfos)
	local selected = {}
	for i=1, impostorcount do --repeat the below code for how many impostors there are in the settings
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(selected,playerinfos[impid]) --add it to the selected list
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return selected
end

