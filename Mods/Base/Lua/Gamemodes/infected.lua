--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	Game_CreateRole("Infected",{98, 167, 74},"Infect all the crewmates.",{0,1,2},0,3,true,false,1)
	return {"Infected",3} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
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

function OnDeath(dead,reason)
	if (reason == 1 and Net_AmHost()) then
		Game_SetRoles({dead},{"infected_Infected"})
		Game_RevivePlayer(dead)
	end
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
	local zombienum = GetRoleAmount(Game_GetRoleIDFromUUID("infected_Infected"))
	local crewmatesnum = #crewmates - zombienum
	if (not sab) then --If the check isn't due to a sabotage
		if (zombienum >= crewmatesnum) then --crewmates can't win
			return "impostors"
		end
		if (zombienum <= 0) then --no more impostors, crewmates win
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
	return true
end


function BeforeKill(killer,victim)
	return true
end

function OnGameEnd()

end

function DecideRoles(playerinfos)
	local RolesToGive = {"infected_Infected"}
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
	return {}
end

