--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	Settings_CreateByte("Cyborgs",1,4,1,1)
	Game_CreateRole("Robot",{128,68,218},"Kill Every [8CFFFFFF]Crewmate[].",{0},2,0,true,false,255,false,"[8CFFFFFF]Kill The Crewmates[]")
	Game_CreateRole("Cyborg",{128,68,218},"Kill Every [8CFFFFFF]Crewmate[].",{0},2,0,true,false,255,false,"[8CFFFFFF]Become A Cyborg[]")
	return {"Cyborg",14} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
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


function OnExile(exiled)
	
end

function OnDeath(dead)

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
	if (userinfo.role == Game_GetRoleIDFromUUID("cyborg_Cyborg")) then
		return true
	end
	if (userinfo.role == Game_GetRoleIDFromUUID("cyborg_Robot")) then
		return true
	end
	if (userinfo.IsImpostor and not targetinfo.IsImpostor) then --if the person doing the kill is an impostor and the victim
		return true
	end
	return false
end


function BeforeKill(killer,victim)
	if (killer.role == Game_GetRoleIDFromUUID("cyborg_Robot")) then
		return true	
	end
	if (killer.role == Game_GetRoleIDFromUUID("cyborg_Cyborg")) then

		killer.PlayerName = "robot"
		killer.ColorId = 12
		killer.HatId = 78
		Game_SetRoles({killer},{"cyborg_Robot"})
		Game_UpdatePlayerInfo(killer)
		return false
	end
	return true
end

function OnGameEnd()

end

function DecideRoles(playerinfos)
	local RolesToGive = {"cyborg_Cyborg"}
	local Selected = {}
	local SelectedRoles = {}
	for i=1, Settings_GetNumber(0) do
		local impid = math.random(1,#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the sheriff's role
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

