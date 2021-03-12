--Fight to the Death: The Gamemode


function InitializeGamemode()
	Game_CreateRole("Red Team",{255,25,25},"Complete tasks within the time limit!",{},1,5,false,true,2)
	Game_CreateRole("Blue Team",{25,25,255},"Complete tasks within the time limit!",{},1,5,false,true,1)
	return {"Task Rush(Broken)",6} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
end

function OnTaskCompletionClient(totaltasks,completedtasks,player)
	if (math.random(1,15) == 5) then
	return false --haha
	end
	return true
end

function OnClientUpdate(timer,timesincelastround)
	Client_ClearMessages()
	Client_ShowMessage(Net_GetHost().luavalue1 .. " vs " .. Net_GetHost().luavalue2)
end

function OnTaskCompletionHost(totaltasks,completedtasks,player)
	local host = Net_GetHost()
	if (player.role == Game_GetRoleIDFromUUID("taskwar_Red Team")) then
		host.luavalue1 = host.luavalue1 + completedtasks
	end
	if (player.role == Game_GetRoleIDFromUUID("taskwar_Blue Team")) then
		host.luavalue2 = host.luavalue2 + completedtasks
	end
	Game_UpdatePlayerInfo(host)
	return false
end


function CanCallMeeting(reporter,isbody)
	return false
end

function DecideRoles(playerinfos)
	local redteamcount = math.ceil(#playerinfos / 2)
	local blueteamcount = #playerinfos - redteamcount
	local RolesToGive = {}
	for i=1,redteamcount do
	table.insert(RolesToGive,"taskwar_Red Team")
	end
	for i=1,blueteamcount do
	table.insert(RolesToGive,"taskwar_Blue Team")
	end
	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles}
end

function BeforeKill(killer,victim)
	return true
end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	return "none"
end

function CanKill(userinfo,targetinfo)
	return false --just incase
end



function DecideImpostors(impostorcount,playerinfos)
	return {} --no imps
end

