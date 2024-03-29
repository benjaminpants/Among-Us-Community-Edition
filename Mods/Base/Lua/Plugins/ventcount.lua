local invent_timer = 10
local timedif = 0

function InitializePlugin()
	return {"Vent Timer",4,false} --Plugins can either override functions or run stuff ontop.
end

function OnClientUpdate(timer,timesincelastround)
	if (Game_CheckPlayerInVent(Client_GetLocalPlayer())) then
		invent_timer = invent_timer - (timer - timedif)
		if (not (math.ceil(timer) == math.ceil(timedif))) then
		if (math.ceil(invent_timer) > -1) then
			Client_ShowMessage("Time until suffocation:" .. math.ceil(invent_timer))
		end
		end
		if (invent_timer < 0) then
			Game_KillPlayer(Client_GetLocalPlayer(),false)
		end
	else
		invent_timer = 10
	end
	timedif = timer
end

function OnGameEnd()
	timedif = 0
end
