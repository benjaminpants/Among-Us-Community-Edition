--amogus but swapped


function InitializeGamemode()
	Game_CreateRole("Impostor",{255,25,25},"Some of your [FF1919FF]Impostor[] friends \n\r are acting kind of suspicious, \n\r are you sure you killed every last [8CFFFFFF]Crewmate[]?",{0},0,0,true,false,0,false,"Find and kill the Crewmate(s)")
	Game_CreateRole("Crewmate",{114,195,250},"You are one of the last Crewmates on the ship. \n\r Complete all your tasks before the \r\n [FF1919FF]Impostors[] find out.",{},1,3,false,true,255,false,"Complete your tasks before the [FF1919FF]Impostors[] find out")
	Settings_CreateByte("Crewmates",1,3,1) -- 0
	return {"Swapped",13} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
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
	return voteid
end


function OnHostRecieveSimple(id)

end

function OnHostRecieve(id,data)

end

function OnHostUpdate(timer,timesincelastround)

end

function OnClientUpdate(timer,timesincelastround)
	
end

function CanSneak(player,allowall)
	return true
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

function OnMeetingStart()

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
	math.randomseed(Misc_GetCurrentTime())
end



function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	impostors = {}
	sw_crewmates = {}
	
	for i=1,#crewmates do
		if (crewmates[i].role == Game_GetRoleIDFromUUID("swapped_Impostor")) then
			table.insert(impostors,crewmates[i])
		end
	end
	for i=1,#crewmates do
		if (crewmates[i].role == Game_GetRoleIDFromUUID("swapped_Crewmate")) then
			table.insert(sw_crewmates,crewmates[i])
		end
	end
	
	if (#sw_crewmates == 0) then --crewmates can't win
		return "impostors"
	end
	if (#impostors <= #sw_crewmates) then --impostors can't win
		return "crewmates"
	end
	if (#crewmates == 0) then --this could probably happen when an impostor kills an impostor at the same time as that impostor kills the last crewmate
		return "stalemate"
	end
	if (taskscomplete) then --task win
		return "crewmates"
	end
	if (sab) then
		return "impostors"
	end
	return "none" --no win conditions have been met.
end

function CanKill(userinfo,targetinfo)
	return true
end


function BeforeKill(killer,victim)
	if (victim.role == Game_GetRoleIDFromUUID("swapped_Impostor")) then
		Game_KillPlayer(killer,true)
		Game_KillPlayer(victim,true)
	else
		return true
	end
end

function OnGameEnd()

end

function DecideRoles(playerinfos)  --imagine copying someone else's code; couldn't be me
	local RolesToGive = {}
	for i=1, Settings_GetNumber(0) do
		table.insert(RolesToGive,"swapped_Crewmate")
	end
	for i=1, (#playerinfos - Settings_GetNumber(0)) do
		table.insert(RolesToGive,"swapped_Impostor")
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
