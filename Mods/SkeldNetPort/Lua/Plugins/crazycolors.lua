local delay = 45

local update_timer = delay
local timedif = 0


function InitializePlugin()
	return {"Crazy Colors",10,false} --Plugins can either override functions or run stuff ontop.
end



local function GetNonDisconnectedPlayers()
	local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
	for i=#players,1,-1 do
		if (players[i].Disconnected) then
			table.remove(players,i)
		end
	end
	return players
end



function OnGameStart() --FOR THE LOVE OF GOD I NEED TO FIGURE OUT WHY THE FUCK NOTHING NETWORK WISE WORKS PROPERLY HERE LIKE COME ON
	update_timer = delay
	timedif = 0
end

function DecideImpostors(impostorcount,playerinfos)
	local pl = GetNonDisconnectedPlayers()
	local decidedids = {math.random(0,(Game_GetGlobalNum("hats_max") - 1)),math.random(0,(Game_GetGlobalNum("skins_max") - 1))}
	for i=1, #pl do
		pl[i].ColorId = math.random(0,(Game_GetGlobalNum("colors_max") - 1))
		pl[i].HatId = decidedids[1]
		pl[i].SkinId = decidedids[2]
		pl[i].PlayerName = " "
		Game_UpdatePlayerInfo(pl[i])
	end
	
end


function OnHostUpdate(timer,timesincelastround)
	update_timer = update_timer - (timer - timedif)
	local pl = GetNonDisconnectedPlayers()
	if (update_timer < 0) then
		update_timer = delay
		for i=1, #pl do
			pl[i].ColorId = math.random(0,(Game_GetGlobalNum("colors_max") - 1))
			Game_UpdatePlayerInfo(pl[i])
		end
	end
	timedif = timer
end
