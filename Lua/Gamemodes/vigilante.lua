
function InitializeGamemode()
	Game_CreateRole("Vigilante",{53,53,57},"Eliminate all of the [FF1919FF]Impostors[].",{0,1,2},1,0,true,false,255)
	Game_CreateRole("Impostor",{255,25,25},"Discover and eliminate the [9C9C9CFF]Vigilante[].",{0,2},0,0,false,false,0)
	Settings_CreateByte("Vigilante Count",1,4,1) -- 0
	return {"Vigilante",3} 
end


function OnTaskCompletionClient(totaltasks,completedtasks,player) --use this to cancel tasks
	if (not player.IsImpostor) then
		return false
	end
	return false
end

function OnTaskCompletionHost(totaltasks,completedtasks,player) --this is ran on the hosts end, usually used to trigger host only events like Winning, we don't need to do anything with this so we're going to make it return false
	return false --this isn't used
end


function ShouldSeeRole(rolename,playerinfo)
	return false
end

function OnExile(exiled)
	
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
	if (not sab) then --If the check isn't due to a sabotage
		if (GetRoleAmount(Game_GetRoleIDFromUUID("vigilante_Vigilante")) == 0) then
			return "impostors"
		end
		if (GetRoleAmount(Game_GetRoleIDFromUUID("vigilante_Impostor")) <= GetRoleAmount(Game_GetRoleIDFromUUID("vigilante_Vigilante"))) then
			return "crewmates"
		end
		return "none" --no win conditions have been met.
	else
		return "crewmates" --sab loose
	end
end


function CanKill(userinfo,targetinfo)
	
	return true
end


function BeforeKill(killer,victim)

	if (victim.role == Game_GetRoleIDFromUUID("vigilante_Impostor") and killer.role == Game_GetRoleIDFromUUID("vigilante_Impostor")) then
        Game_KillPlayer(killer,false)
    end
	return true

end


function DecideRolesFunction(playerinfos)
	local RolesToGive = {}
	for i=1, Settings_GetNumber(0) do
		table.insert(RolesToGive,"vigilante_Vigilante")
	end
	local Selected = {}
	local SelectedRoles = {}
	for i=1, (#playerinfos - Settings_GetNumber(0)) do
		table.insert(RolesToGive,"vigilante_Impostor")
	end
	for i=1, #RolesToGive do
		local impid = math.random(1,#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the vigilante's role
end

function DecideRoles(playerinfos)
	return DecideRolesFunction(playerinfos) -- sets the vigilante's role
end


function DecideImpostors(impostorcount,playerinfos)
	return {}
end

