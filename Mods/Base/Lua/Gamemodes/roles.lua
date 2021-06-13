--bee movie

local invent_timer = 15
local timedif = 0

local poslength = 13
local sni = 14
local dkp = 15
local sii = 16
local sic = 17
local csb = 18
local isj = 19
local amc = 20
local ssm = 21


function InitializeGamemode()
	Game_CreateRole("Sheriff",{255,216,0},"Find and Kill the [FF1919FF]Impostor[].",{0},1,0,false,true,255)
	Game_CreateRole("Joker",{129,41,139},"Trick the crewmates into thinking \n\r you are the [FF1919FF]Impostor[].",{},2,4,false,false,255,false,"[81298BFF]Get Ejected.[]")
	Game_CreateRole("Hawk-Eyed",{120, 86, 60},"Use your increased sight\n\r to find the [FF1919FF]Impostor[].",{},1,0,true,true)
	Game_CreateRole("Shielded",{107, 107, 107},"Be able to avoid being killed one time.",{},1,0,false,true)
	Game_CreateRole("Witch",{170, 102, 173},"Poison everyone, including [FF1919FF]Impostors[].",{0},2,0,true,false,255,false,"[AA66ADFF]Poison everyone.[]")
	Game_CreateRole("Clown",{243, 222, 255},"If you get ejected, \n\ryou kill someone else randomly.\n\rYou win with the [8CFFFFFF]Crewmates[].",{},1,0,false,true)
	Game_CreateRole("Doctor",{25, 255, 25},"Cure [8CFFFFFF]Crewmates[] poisoned by a [AA66ADFF]Witch[].",{0},1,3,false,true,1)
	Game_CreateRole("Griefer",{87, 85, 42},"Make it look like someone killed you.\nYou win with the [FF1919FF]Impostors[].",{0,2},0,1,true,false,255,true,"[57552AFF]Frame someone for a kill.[]")
	Game_CreateRole("Spy",{98, 91, 112},"Prevent people from speaking and voting.\nYou win with the [FF1919FF]Impostors[].",{0,2},0,1,true,false,255,true,"[625B70FF]Silence possible threats.[]")
	Game_CreateRole("Seer",{73, 235, 232},"Discover the identity of the crew.",{0},1,0,false,true)
	Game_CreateRole("Yeller",{109, 107, 232},"Report the body of anyone nearby\nEven if they are alive.",{0},1,0,false,true)
	Game_CreateRole("Twin",{118, 133, 163},"Stay alive with your twin.\n\rIf you die, your twin dies as well.",{},2,3,false,false,69,false,"[7685A3FF]Stay alive and win with your twin.[]")
	Game_CreateRole("Assassin",{110,22,22},"Whoever you vote for dies. Limited uses.\n\rYou win with the [8CFFFFFF]Crewmates[].",{},1,0,false,true,255)
	--Game_CreateRole("Snatcher",{255, 255, 255},"Steal someone's identity and survive for 4 meetings.",{},2,3,false,false,69,false,"[7685A3FF]Stay alive and win with your twin.[]")
	--roles you aren't supposed to see
	Game_CreateRole("Shielded(Broken)",{107, 107, 107},"you shouldn't see this lol",{},1,1,false,true) --impostors can see people with broken shields
	Game_CreateRole("Spy(Finished)",{98, 91, 112},"if you see this you are dumb",{2},0,1,true,false,255,true,"[625B70FF]Silence possible threats.[]")
	--counts
	Settings_CreateByte("Sheriff Count",0,2,0) -- 0
	Settings_CreateByte("Joker Count",0,3,0) -- 1
	Settings_CreateByte("Hawk-Eyed Count",0,2,0) -- 2
	Settings_CreateByte("Shielded Count",0,2,0) -- 3
	Settings_CreateByte("Witch Count",0,3,0) -- 4
	Settings_CreateByte("Clown Count",0,4,0) -- 5
	Settings_CreateByte("Doctor Count",0,3,0) -- 6
	Settings_CreateByte("Griefer Count",0,4,0) -- 7
	Settings_CreateByte("Seer Count",0,4,0) -- 8
	Settings_CreateByte("Yeller Count",0,4,0) -- 9
	Settings_CreateBool("Enable Twins Role",false) -- 10
	Settings_CreateByte("Assassin Count",0,3,0) -- 11
	Settings_CreateByte("Spy Count",0,3,0) -- 12
	Settings_CreateByte("Poison Length",5,120,15,5) -- 13
	Settings_CreateBool("Shields Kill Attackers",false) -- 14
	Settings_CreateBool("Doctors Know Poisoned",false) -- 15
	Settings_CreateBool("Seers can identify Impostors",false) -- 16
	Settings_CreateByte("Seer Inspect Count",1,4,1) -- 17
	Settings_CreateBool("Classic Sheriff Behavior",false) -- 18
	Settings_CreateBool("Impostors See Jokers",true) -- 19
	Settings_CreateByte("Assassin Kill Count",1,4,1) -- 20
	Settings_CreateByte("Spy Silence Count",1,4,1) -- 21
	
	return {"Roles",2}
end

function OnRecieveRole(rolename,player)
	if (rolename == "Seer") then
		player.luavalue2 = Settings_GetNumber(sic)
		Game_UpdatePlayerInfo(player)
	end
	if (rolename == "Assassin") then
		player.luavalue2 = Settings_GetNumber(amc)
		Game_UpdatePlayerInfo(player)
	end
	if (rolename == "Spy") then
		player.luavalue2 = Settings_GetNumber(ssm)
		Game_UpdatePlayerInfo(player)
	end
end


function OnChat(message, player)
	return player.luavalue3 ~= 1
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


local function GetRoleAmountOfType(typee)
	if (typee == "noncrew") then
		return (GetRoleAmount(Game_GetRoleIDFromUUID("roles_Griefer")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Joker")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Witch")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Spy")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Twin")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Spy(Finished)")))
	end
	if (typee == "noncrew twinless") then
		return (GetRoleAmount(Game_GetRoleIDFromUUID("roles_Griefer")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Joker")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Witch")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Spy")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Spy(Finished)")))
	end
	if (typee == "imp") then
		return (GetRoleAmount(Game_GetRoleIDFromUUID("roles_Griefer")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Spy")) + GetRoleAmount(Game_GetRoleIDFromUUID("roles_Spy(Finished)")))
	end
	Debug_Log("invalid thingy, returing 0(GetRoleAmountOfType)")
	return 0
end

local function GetPlayersWithRole(id)
	local pl = Game_GetAllPlayers()
	local pls = {}
	for i=1, #pl do
		if (pl[i].role == id and not pl[i].IsDead) then
		table.insert(pls,pl[i])
		end
	end
	return pls
end

local function GetCrewWithoutBadRoles(og)
	local pl = og
	local count = 0
	for i=1, #pl do
		local bool = (pl[i].role == Game_GetRoleIDFromUUID("roles_Griefer") or pl[i].role == Game_GetRoleIDFromUUID("roles_Joker"))
		if ((not bool) and not pl[i].IsDead) then
		count = count + 1
		end
	end
	return count
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



function OnGameStart()
	invent_timer = Settings_GetNumber(poslength)
	timedif = 0
end




function ShouldSeeRole(rolename,playerinfo)
	if (rolename == "Joker" and Settings_GetBool(isj) and playerinfo.IsImpostor) then
		return true
	end
	return false
end


function OnClientUpdate(timer,timesincelastround)
	if (Client_GetLocalPlayer().luavalue1 == 255) then
		invent_timer = invent_timer - (timer - timedif)
		if (not (math.ceil(timer) == math.ceil(timedif)) and math.ceil(invent_timer) == 5) then
			Client_ShowMessage("You don't feel so well...")
		end
		if (invent_timer < 0) then
			Game_KillPlayer(Client_GetLocalPlayer(),false)
		end
	else
		invent_timer = Settings_GetNumber(poslength)
	end
	timedif = timer
end

function OnPlayerDC(playerinfo)
	if (playerinfo.role == Game_GetRoleIDFromUUID("roles_Sheriff") and Net_AmHost()) then
		if (Settings_GetBool(csb)) then
			return
		end
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == playerinfo.PlayerName or players[i].IsDead or players[i].IsImpostor or (players[i].role > 0)) then
				table.remove(players,i)
			end
		end
		if (#players == 0) then
			return
		end
		local selected = {players[math.random(1,#players)]}
		Game_SetRoles(selected,{"roles_Sheriff"})
	end
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
	return true
end

local function GetAlivePlayers(nametoavoid)
	local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
	for i=#players,1,-1 do
		if (players[i].IsDead or players[i].PlayerName == nametoavoid) then
			table.remove(players,i)
		end
	end
	return players
end

local function RemoveAllSilences()
	local players = GetAlivePlayers("MYNAMEISJEFFHAHAHAA")
	for i=1, #players do
		if (players[i].luavalue3 == 1 and Net_AmHost()) then
			players[i].luavalue3 = 0
			Game_UpdatePlayerInfo(players[i])
		end
	end

end

function OnExileSkip()
	RemoveAllSilences()

end

function OnExile(exiled)
	RemoveAllSilences()
	
	
	if (exiled.role == Game_GetRoleIDFromUUID("roles_Clown")) then
		if (Net_AmHost()) then
			local aliveplayers = GetAlivePlayers(exiled.PlayerName)
			Game_KillPlayer(aliveplayers[math.random(1,#aliveplayers)],false)
		end
	end
	
	
	if (exiled.role == Game_GetRoleIDFromUUID("roles_Joker")) then
		if (Net_AmHost()) then
			Game_ActivateCustomWin({exiled},"joker_win")
		end
	end
end


function OnMeetingStart() --this is called on every client
	local players = GetAlivePlayers("MYNAMEISJEFFHAHAHAA")
	for i=1, #players do
		if (players[i].luavalue3 == 1) then
			Client_ShowMessage(players[i].PlayerName .. " has been silenced!")
		end
	end
end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (#crewmates == 0 and #impostors == 0) then
		return "stalemate" --just incase
	end
	if (Settings_GetBool(10)) then
		if (#crewmates - GetRoleAmountOfType("noncrew twinless") == GetRoleAmount(Game_GetRoleIDFromUUID("roles_Twin"))) then
			if (GetRoleAmount(Game_GetRoleIDFromUUID("roles_Twin")) == 2) then
				return {GetPlayersWithRole(Game_GetRoleIDFromUUID("roles_Twin")),"twin_win"}
			end
		end
	end
	if (not sab) then --If the check isn't due to a sabotage
		if (GetRoleAmount(Game_GetRoleIDFromUUID("roles_Witch")) == 0) then
			if ((#impostors + GetRoleAmountOfType("imp")) >= #crewmates) then --crewmates can't win
				return "impostors"
			end
			if (#impostors <= 0) then --no more impostors, crewmates win
				return "crewmates"
			end
		else
			if (#crewmates == 1 and #impostors == 0) then
				if (crewmates[1].role == Game_GetRoleIDFromUUID("roles_Witch")) then
					return {{crewmates[1]},"witch_win"}
				else
					return "crewmates" --a sheriff most likely took the win
				end
			end
		end
		if (taskscomplete) then --task win
			return "crewmates"
		end
		return "none" --no win conditions have been met.
	else
		return "impostors" --sab loose
	end
end

function CanSneak(player,allowall)
	if (allowall) then
		return true
	end
	return (player.IsImpostor or player.role == Game_GetRoleIDFromUUID("roles_Witch") or player.role == Game_GetRoleIDFromUUID("roles_Twin"))
end


function CanKill(userinfo,targetinfo)
	if (userinfo.role == Game_GetRoleIDFromUUID("roles_Seer") or userinfo.role == Game_GetRoleIDFromUUID("roles_Yeller")) then
		return true
	end
	
	if (userinfo.role == Game_GetRoleIDFromUUID("roles_Spy") and not targetinfo.IsImpostor) then
		return true
	end
	
	if (userinfo.role == Game_GetRoleIDFromUUID("roles_Doctor")) then
		if (Settings_GetBool(dkp)) then
			return (targetinfo.luavalue1 == 255)
		else
			return true
		end
	end
	
	if (userinfo.IsImpostor) then
		if (targetinfo.IsImpostor) then
			return false
		else
			return not ((targetinfo.role == Game_GetRoleIDFromUUID("roles_Joker") and Settings_GetBool(isj)) or targetinfo.luavalue1 == 255 or (targetinfo.role == Game_GetRoleIDFromUUID("roles_Spy") or targetinfo.role == Game_GetRoleIDFromUUID("roles_Spy(Finished)")))
		end
	end
	
	if (userinfo.role == Game_GetRoleIDFromUUID("roles_Griefer")) then
		if (targetinfo.IsImpostor) then
			return false
		else
			return not ((targetinfo.role == Game_GetRoleIDFromUUID("roles_Joker") and Settings_GetBool(isj)) or targetinfo.luavalue1 == 255)
		end
	end
	
	if (userinfo.role == Game_GetRoleIDFromUUID("roles_Sheriff")) then
		if (targetinfo.luavalue1 == 255) then
			return false
		else
			return true
		end
	end
	
	if (userinfo.role == Game_GetRoleIDFromUUID("roles_Witch")) then
		if (targetinfo.luavalue1 == 255) then
			return false
		else
			return true
		end
	end
	
	return false
end


function OnVote(voter,voteid,isskip)
	if (isskip) then
		return -1 --skipping should skip
	end
	if (voter.role == Game_GetRoleIDFromUUID("roles_Assassin")) then
		voter.luavalue2 = voter.luavalue2 - 1
		Game_UpdatePlayerInfo(voter)
		if (voter.luavalue2 == 0) then
			Game_SetRoles({voter},{"None"})
		end
		OnExile(Game_GetPlayerFromID(voteid)) --call the exile logic on the person killed
		Game_KillPlayer(Game_GetPlayerFromID(voteid),false)
		return -1
	end
	if (voter.luavalue3 == 1) then
		return -1
	end
	return voteid
end

function OnDeath(victim,reason)
	victim.luavalue3 = 0
	Game_UpdatePlayerInfo(victim)
	if (victim.role == Game_GetRoleIDFromUUID("roles_Sheriff") and Net_AmHost()) then
		if (Settings_GetBool(csb)) then
			return
		end
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if ((players[i].PlayerName == victim.PlayerName or players[i].IsDead or players[i].IsImpostor) or players[i].role > 0) then
				table.remove(players,i)
			end
		end
		if (#players == 0) then
			return
		end
		local selected = {players[math.random(1,#players)],victim}
		Game_SetRoles(selected,{"roles_Sheriff","None"})
	end
	if (victim.role == Game_GetRoleIDFromUUID("roles_Twin") and Net_AmHost()) then
		if (#GetPlayersWithRole(Game_GetRoleIDFromUUID("roles_Twin")) == 0) then
			Debug_Log("How did this happen? Disconnect perhaps?")
		else
			Game_KillPlayer(GetPlayersWithRole(Game_GetRoleIDFromUUID("roles_Twin"))[1],false)
		end
	end
end


function BeforeKill(killer,victim)
	if (killer.role == Game_GetRoleIDFromUUID("roles_Spy")) then
		killer.luavalue2 = killer.luavalue2 - 1
		Game_UpdatePlayerInfo(killer)
		if (killer.luavalue2 == 0) then
			Game_SetRoles({killer},{"roles_Spy(Finished)"})
		end
		victim.luavalue3 = 1
		Game_UpdatePlayerInfo(victim)
		return false
	end
	if (killer.role == Game_GetRoleIDFromUUID("roles_Seer")) then
		killer.luavalue2 = killer.luavalue2 - 1
		Game_UpdatePlayerInfo(killer)
		if (killer.luavalue2 == 0) then
			Game_SetRoles({killer},{"None"})
		end
		local RoleName = Game_GetRoleNameFromID(victim.role)
		if (victim.role == 0) then
			if (Settings_GetBool(sii)) then
				if (victim.IsImpostor) then
					Client_ShowMessage(victim.PlayerName .. " is an Impostor.")
				else
					Client_ShowMessage(victim.PlayerName .. " is a Crewmate.")
				end
			else
				Client_ShowMessage(victim.PlayerName .. " has no special roles.")
			end
		else
			local conjoin = " the "
			if (not GetRoleAmount(victim.role) == 1) then
				cjoin = " a "
			end
			Client_ShowMessage(victim.PlayerName .. " is" .. conjoin .. RoleName .. ".")
		end
		return false
	end
	
	if (killer.role == Game_GetRoleIDFromUUID("roles_Yeller")) then
		Game_CallMeeting(killer,victim)
		return false
	end
	
	if (killer.role == Game_GetRoleIDFromUUID("roles_Doctor")) then
		victim.luavalue1 = 0
		Game_UpdatePlayerInfo(victim)
		return false
	end
	if (victim.role == Game_GetRoleIDFromUUID("roles_Shielded")) then
		if (Settings_GetBool(sni)) then
			Game_KillPlayer(killer,false)
		else
			Client_ShowMessage("You broke their shield.")
		end
		Game_SetRoles({victim},{"roles_Shielded(Broken)"})
		return false
	end
	if (killer.role == Game_GetRoleIDFromUUID("roles_Griefer")) then
		Player_SnapPosTo(killer.PosX,killer.PosY,victim)
		Game_KillPlayer(killer,false)
		return false
	end
	if (killer.role == Game_GetRoleIDFromUUID("roles_Witch")) then
		victim.luavalue1 = 255
		Game_UpdatePlayerInfo(victim)
		return false
	end
	if (killer.role == Game_GetRoleIDFromUUID("roles_Sheriff")) then
		if (Settings_GetBool(csb)) then
			if (victim.role == Game_GetRoleIDFromUUID("roles_Joker") or victim.role == Game_GetRoleIDFromUUID("roles_Witch") or victim.role == Game_GetRoleIDFromUUID("roles_Griefer") or victim.role == Game_GetRoleIDFromUUID("roles_Twin") or victim.IsImpostor) then
				
			else
				Game_KillPlayer(killer,false)
			end
			return true
		end
		local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
		for i=#players,1,-1 do
			if (players[i].PlayerName == victim.PlayerName or players[i].IsDead or players[i].IsImpostor or players[i].PlayerName == killer.PlayerName or (players[i].role > 0)) then
				table.remove(players,i)
			end
		end
		if (#players == 0) then
			return true
		end
		local selected = {players[math.random(1,#players)],killer}
		Game_SetRoles(selected,{"roles_Sheriff","None"})
	end
	return true
end

function DecideRolesFunction(playerinfos)
	math.randomseed(Misc_GetCurrentTime())
	local RolesToGive = {}
	for i=1, Settings_GetNumber(0) do
		table.insert(RolesToGive,"roles_Sheriff")
	end
	for i=1, Settings_GetNumber(1) do
		table.insert(RolesToGive,"roles_Joker")
	end
	for i=1, Settings_GetNumber(2) do
		table.insert(RolesToGive,"roles_Hawk-Eyed")
	end
	for i=1, Settings_GetNumber(3) do
		table.insert(RolesToGive,"roles_Shielded")
	end
	for i=1, Settings_GetNumber(4) do
		table.insert(RolesToGive,"roles_Witch")
	end
	for i=1, Settings_GetNumber(5) do
		table.insert(RolesToGive,"roles_Clown")
	end
	for i=1, Settings_GetNumber(6) do
		table.insert(RolesToGive,"roles_Doctor")
	end
	for i=1, Settings_GetNumber(7) do
		table.insert(RolesToGive,"roles_Griefer")
	end
	for i=1, Settings_GetNumber(8) do
		table.insert(RolesToGive,"roles_Seer")
	end
	for i=1, Settings_GetNumber(9) do
		table.insert(RolesToGive,"roles_Yeller")
	end
	if (Settings_GetBool(10)) then
		for i=1, 2 do
		table.insert(RolesToGive,"roles_Twin")
		end
	end
	for i=1, Settings_GetNumber(11) do
		table.insert(RolesToGive,"roles_Assassin")
	end
	for i=1, Settings_GetNumber(12) do
		table.insert(RolesToGive,"roles_Spy")
	end
	local Selected = {}
	local SelectedRoles = {}
	for i=1, #RolesToGive do
		local impid = math.random(0,#playerinfos) --randomly set the impostor id
		table.insert(Selected,playerinfos[impid]) --add it to the selected list
		table.insert(SelectedRoles,RolesToGive[i])
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return {Selected,SelectedRoles} -- sets the sheriff's role

end

function DecideRoles(playerinfos)
	return DecideRolesFunction(playerinfos) -- sets the sheriff's role
end


function DecideImpostors(impostorcount,playerinfos)
	math.randomseed(Misc_GetCurrentTime())
	local selected = {}
	for i=1, impostorcount do --repeat the below code for how many impostors there are in the settings
		local impid = math.random(#playerinfos) --randomly set the impostor id
		table.insert(selected,playerinfos[impid]) --add it to the selected list
		table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
	end
	return selected
end

