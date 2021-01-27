--This is a recreation of the Among Us Classic Gamemode.

local invent_timer = 15
local timedif = 0

function InitializeGamemode()
	Game_CreateRole("Poisoned",{255,255,255},"you shouldn't see this",{},1,0,false,true)
	return {"Witch",11} 
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

function OnExile(exiled)
end

function OnClientUpdate(timer,timesincelastround)
	if (Client_GetLocalPlayer().role == Game_GetRoleIDFromUUID("witch_Poisoned")) then
		invent_timer = invent_timer - (timer - timedif)
		if (not (math.ceil(timer) == math.ceil(timedif))) then
		end
		if (invent_timer < 0) then
			Game_KillPlayer(Client_GetLocalPlayer(),false)
		end
	else
		invent_timer = 15
	end
	timedif = timer
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return true
end

function OnPlayerDC(playerinfo)

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
		if (not (targetinfo.role == Game_GetRoleIDFromUUID("witch_Poisoned"))) then
			return true
		end
	end
	return false
end

function BeforeKill(killer,victim)
	Game_SetRoles({victim},{"witch_Poisoned"})
	return false
end


function DecideRoles(playerinfos)
	return {{},{}}
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

