function InitializePlugin()
	return {"Disable End Criteria",1,true} --Plugins can either override functions or run stuff ontop.
end

function CheckWinCondition(impostors,crewmates,sab,taskscomplete)
	if (not sab) then --If the check isn't due to a sabotage
		return "none" --no win conditions have been met.
	else
		return "impostors" --sab loose
	end
end
