--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	Game_CreateRole("The Killer",{255,20,25},"Kill all crewmates with assistance from the\n[FF1919FF]Saboteur[] and the [FF1919FF]Venteer[].",{0},0,0,true,false)
	Game_CreateRole("Saboteur",{255,20,25},"Sabotage and help [FF1919FF]The Killer[]",{1},0,0,true,false)
	Game_CreateRole("Venteer",{255,20,25},"Open vents for both the [FF1919FF]Saboteur[] and [FF1919FF]The Killer[]",{2},0,0,true,false)
	return {"The Trio of Chaos",10} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
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
end

function OnExile(exiled)
	
end

function OnExileSkip()

end

function OnPlayerDC(playerinfo)

end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return true
end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (not sab) then --If the check isn't due to a sabotage
		if (#impostors >= #crewmates) then --crewmates can't win
			return "impostors"
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
	if (targetinfo.role == Game_GetRoleIDFromUUID("three_Saboteur") or targetinfo.role == Game_GetRoleIDFromUUID("three_Venteer")) then --if the person doing the kill is an impostor and the victim
		return true
	end
	return false
end


function BeforeKill(killer,victim)
	return true
end

function OnGameEnd()

end

function DecideRoles(playerinfos)
	local RolesToGive = {"three_The Killer","three_Saboteur","three_Venteer"}
	Debug_Log("Work please")
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


function DecideImpostors(impostorcount,playerinfos)
	return {}
end

