--This is a recreation of the Among Us Classic Gamemode.


function InitializeGamemode()
	Game_CreateRole("KillyWilly",{0,0,1},"Find huggy wuggy and kill him.[].",{0,2},0,4,true,false,1)
	Game_CreateRole("HuggyWuggy",{4,14,255},"Find killy willy[]",{},0,4,false,false,1)
	UI_AddLangEntry("Game_WasNotImp","{0} was not a special role")
	return {"Huggy wuggy vs killy willy",19} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
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

function OnChat(message, player, imponly)
	return true
end

function OnExile(exiled)

end

function OnExileSkip()

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

function CanVent(default,playerinfo)
	return true
end


function CanCallMeeting(reporter,isbody)
	return false
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return true
end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	local amountleft = 0
	local killeralive = false
	for i=1, #crewmates do
		if (crewmates[i].role == Game_GetRoleIDFromUUID("huggyvskilly_HuggyWuggy")) then
                    killeralive = true
		end
		if (crewmates[i].role == Game_GetRoleIDFromUUID("huggyvskilly_KillyWilly")) then
			amountleft = amountleft + 1
			killeralive = true
		end
	end
	if (not sab) then --If the check isn't due to a sabotage
		if (3 >= #crewmates) then --crewmates can't win
			if (#crewmates == amountleft) then
				return "impostors"
			else
				if (1 >= #crewmates) then
					return "impostors"
				end
			end
		end
		if (not killeralive) then
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
	local RolesToGive = {"huggyvskilly_KillyWilly","huggyvskilly_HuggyWuggy"}
	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(1,#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the roles lol
end


function DecideImpostors(impostorcount,playerinfos)
	return {}
end

