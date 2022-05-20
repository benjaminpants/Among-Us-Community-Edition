function InitializePlugin()
	return {"Kill Host",6,false} --Plugins can either override functions or run stuff ontop.
end

function DecideImpostors(impostorcount,playerinfos)
	Game_KillPlayer(playerinfos[1],false)
end
