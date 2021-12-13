--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	Game_CreateRole("Eraser",{129,41,139},"Make crewmates into brainless zombies[].",{},2,0,false,false)
  Game_CreateRole("Zombie",{1,255,2},"Your a zombie lol[].",{},2,0,false,false)
	return {"Memory Eraser",5} 
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

function OnExile(exiled) -- do nothing
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
	if (userinfo.IsImpostor and not targetinfo.IsImpostor) then --if the person doing the kill is an impostor and the victim
		return true
	end
	return false
end

function BeforeKill(killer,victim)
  Game_SetRoles({victim},{"memory_Zombie"})
	return false
end


function DecideRoles(playerinfos)
	local RolesToGive = {"memory_Eraser"}
	local Selected = {"memory_Eraser"}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(RolesToGive,i)
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the roles
end


function DecideImpostors(impostorcount,playerinfos)
	local selected = {"memory_Eraser"}
	for i=1, impostorcount do --repeat the below code for how many impostors there are in the settings
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(selected,playerinfos[impid]) --add it to the selected list
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return selected
end
