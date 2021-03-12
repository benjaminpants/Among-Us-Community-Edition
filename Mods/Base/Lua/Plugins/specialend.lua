function InitializePlugin()
	return {"Only End when 0 Crewmates",5,true} --Plugins can either override functions or run stuff ontop.
end

function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
	if (not sab) then --If the check isn't due to a sabotage
		if (#crewmates == 0) then --crewmates can't win
			return "impostors"
		end
		if (#impostors <= 0) then --no more impostors, crewmates win
			return "crewmates"
		end
		if (taskscomplete) then --task win
			return "crewmates"
		end
		return "none" --no win conditions have been met.
	else
		return "impostors" --sab loose
	end
end
