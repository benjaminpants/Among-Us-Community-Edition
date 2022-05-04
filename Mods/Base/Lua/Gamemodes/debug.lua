--debug gamemode, the game is currently hardwired to allow this gamemode to have chat enabled midround

function InitializeGamemode()
	UI_AddLangEntry("Game_WasNotImp","{0} is {0}, not {1} Impostor.")
	UI_AddLangEntry("Meeting_WhoIsImpostor","[FF00FFFF]What.[]")
	UI_AddLangEntry("Game_ImpsRemain","{0} remains a sussy baka")
	UI_AddLangEntry("UI_CantCallMeeting","You thought you could call\n\rA meeting? You are wrong lol.")
	Settings_CreateBool("Allow Self Suicide",true) -- 0
	return {"Debug",9} --Initialize a Gamemode with the name "Lua Test" and the ID of 6. In the future, the ID will be determined by the server/loader.
end

function OnTaskCompletionClient(totaltasks,completedtasks,player)
	return true
end

function OnTaskCompletionHost(totaltasks,completedtasks,player)
	return false --dumb
end

function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (sab) then
		return "stalemate"
	end
	return "none"
end

function DecideRoles(playerinfos)
	return {{},{}} -- no roles, also don't question
end

function BeforeKill(killer,victim)
	return true
end

function OnVote(voter,voteid,isskip)
	--voter is a player
	--vote id is a vote id
	--isskip is self explanitory
	if (isskip) then
		return 0
	end
	return voteid
end

function OnGameEnd()
	
end

function OnHostRecieve(id,data)
	if (id == 0) then
		Client_ShowMessage(data[1])
	end
	if (id == 1) then
		if (not (data[2] >= 101)) then
			for i=1, data[2] do
			Client_ShowMessage(data[1])
			end
		else
			Client_ShowMessage("some dumby tried crashing your client \nbe thankful I thought of this")
		end
	end
end


function OnChat(message, player, imponly)
	local cmd = {}
	for i in string.gmatch(message, "%S+") do
		table.insert(cmd,i)
	end
	if (cmd[1] == "tele") then
		Player_SnapPosTo(tonumber(cmd[2]),tonumber(cmd[3]),player)
	end
	if (cmd[1] == "sab") then
		Game_SabSystem(cmd[2],player,false,tonumber(cmd[3]))
	end
	if (cmd[1] == "fixsab") then
		Game_SabSystem(cmd[2],player,true,tonumber(cmd[3]))
	end
	if (cmd[1] == "broadcast") then
		Client_ShowMessage(cmd[2])
	end
	if (cmd[1] == "broadcastrandom") then
		Client_ShowMessage(math.random(1,20))
	end
	
	if (cmd[1] == "broadcasttohost" and player.PlayerId == Client_GetLocalPlayer().PlayerId) then
		Net_SendMessageToHost(0,{cmd[2]})
	end
	if (cmd[1] == "broadcasttohostdual" and player.PlayerId == Client_GetLocalPlayer().PlayerId) then
		Net_SendMessageToHost(1,{cmd[2], tonumber(cmd[3])})
	end
	
	if (cmd[1] == "killme" and Net_AmHost() and Settings_GetBool(0)) then
		Game_KillPlayer(player,false)
                Client_ShowMessage("You are dead.")
	end

	if (cmd[1] == "revive" and Net_AmHost() and Settings_GetBool(0)) then
		Game_RevivePlayer(player,true)
                Client_ShowMessage("You gat revoived by a temmie")
	end
	return true
end

function OnHostUpdate(timer,timesincelastround)
	
end

function OnExile(exiled)
	
end

function OnExileSkip()

end

function CanKill(userinfo,targetinfo)
	return true --the API already prevents the slaying of dead people so make this always return true
end


function DecideImpostors(impostorcount,playerinfos)
	return {playerinfos[1]}
end

