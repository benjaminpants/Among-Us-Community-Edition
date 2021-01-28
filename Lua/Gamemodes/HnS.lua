


function InitializeGamemode()
	Game_CreateRole("Seeker",{255,25,25},"Kill all the remaining [1919FFFF]Hiders[].",{0},0,5,true,false,1)
	Game_CreateRole("Hider",{25,25,255},"Complete all tasks and \n\rescape the [FF1919FF]Seekers[].",{},1,5,false,true,2)
	UI_AddLangEntry("UI_CantCallMeeting","Ejecting it won't work.")
	UI_AddLangEntry("Meeting_WhoIsImpostor","[FF0000FF]YOU DIRTY HACKER.[]")
	UI_AddLangEntry("Game_ImpsRemain","[FF0000FF]DIE.[]")
	--hey
	Settings_CreateByte("Seeker Count",1,5,1) -- 0
	return {"Hide And Seek",4}
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
	Game_KillPlayer(Client_GetLocalPlayer(),false)
end

function OnExileSkip()
	Game_KillPlayer(Client_GetLocalPlayer(),false)
end

function OnPlayerDC(playerinfo)

end

function CanVent(default,playerinfo)
	return false
end

function CanCallMeeting(reporter,isbody)
	return false
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return true
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


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (#crewmates == GetRoleAmount(Game_GetRoleIDFromUUID("HnS_Seeker"))) then --crewmates can't win
		return "impostors"
	end
	if (taskscomplete) then --task win
		return "crewmates"
	end
	return "none" --no win conditions have been met.
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
	local RolesToGive = {}
	for i=1, Settings_GetNumber(0) do
		table.insert(RolesToGive,"HnS_Seeker")
	end
	for i=1, (#playerinfos - Settings_GetNumber(0)) do
		table.insert(RolesToGive,"HnS_Hider")
	end
	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the roles
end


function DecideImpostors(impostorcount,playerinfos)
	return {}
end

