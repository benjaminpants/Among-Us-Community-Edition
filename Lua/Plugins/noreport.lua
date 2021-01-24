function InitializePlugin()
	return {"No Body Reporting",3,true} --Plugins can either override functions or run stuff ontop.
end

function CanCallMeeting(reporter,isbody)
	Debug_Log("is body:" .. isbody)
	return (not isbody)
end
