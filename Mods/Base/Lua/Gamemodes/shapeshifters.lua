local ShapeShifterText = ""

function InitializeGamemode()
    Game_CreateRole("Shapeshifter",{110,1,41},"Shapeshift",{0,1,2},0,0,true,false,1,false,"[6E0129FF]Turn into them.[]")
    Game_CreateRole("Dead",{0,0,0},"How",{},0,0,false,true,1,false,"You are dead now.")
    Game_CreateRole("Crewmate",{130,255,255},"There are [6E0129FF]Shapeshifters[] among us.",{},0,0,false,true,1,true)

    Settings_CreateByte("How many Shapeshifters?",1,3,1)
    return {"Shapeshifters",15} --Collab KstarSus and Kyuu
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

local function GetDeadPlayers(nametoavoid,roletoavoid)
    local players = Game_GetAllPlayers() --They need to be alive and they can't be an impostor
    for i=#players,1,-1 do
        if ((not players[i].IsDead) or players[i].PlayerName == nametoavoid or players[i].role == roletoavoid) then
            table.remove(players,i)
        end
    end
    return players
end

local function GetAllPlayersOfSeuameRole(id)
    local players = Game_GetAllPlayers()
    for i=1,#players do
        if (not players[i].role == id) then
            table.remove(players,i)
        end
    end
end

local function GetAllPlayersWithNotThisRole(id)
    local players = Game_GetAllPlayers()
    for i=1,#players do
        if (players[i].role == id) then
            table.remove(players,i)
        end
    end
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

function OnTaskCompletionClient(totaltasks,completedtasks,player) --use this to cancel tasks
    if (not player.IsImpostor) then
        return true
    end
    return false
end

function OnTaskCompletionHost(totaltasks,completedtasks,player) --this is ran on the hosts end, usually used to trigger host only events like Winning, we don't need to do anything with this so we're going to make it return false
    return false --this isn't used
end

function OnHostUpdate(timer,timesincelastround)

end

function OnClientUpdate(timer,timesincelastround)

end

function OnChat(message, player)
    if(player.role == Game_GetRoleIDFromUUID("shapeshifters_Dead")) then
        return false
    end
    return true
end

function OnExile(exiled)
    if (exiled.role == Game_GetRoleIDFromUUID("shapeshifters_Shapeshifters")) then
        if (Net_AmHost()) then
            Game_ActivateCustomWin(GetAllPlayersOfSameRole(Game_GetRoleIDFromUUID("shapeshifters_Crewmate")))
        end
    end
end

function OnDeath(dead,reason)

end

function OnExileSkip()

end

function OnRecieveRole(rolename,player)

end

function OnPlayerDC(playerinfo)

end

function CanVent(default,playerinfo)
    return true
end

function CanCallMeeting(reporter,isbody)
    return true
end

function GiveTasks(playerinfo) --Whether or not to assign tasks to a player, this function is a placeholder for proper task assignment control
    return true
end

function OnGameStart()

end


function CheckWinCondition(impostors,crewmates,sab,taskscomplete) --required
    evilcrewmates = {}
    goodcrewmates = {}
    for i=1,#crewmates do
        if (crewmates[i].role == Game_GetRoleIDFromUUID("shapeshifters_Shapeshifter")) then
            table.insert(evilcrewmates,crewmates[i])
        end
    end
    for i=1,#crewmates do
        if (crewmates[i].role == Game_GetRoleIDFromUUID("shapeshifters_Crewmate")) then
            table.insert(goodcrewmates,crewmates[i])
        end
    end
    if (not sab) then --If the check isn't due to a sabotage
        if (#evilcrewmates >= #goodcrewmates) then --crewmates can't win
            return {evilcrewmates,"shapeshifter_win"}
        end
        if (#evilcrewmates <= 0) then --no more impostors, crewmates win
            return {goodcrewmates}
        end
        if (#crewmates == 0) then
            return "stalemate"
        end
        if (taskscomplete) then --task win
            return {goodcrewmates}
        end

        return "none" --no win conditions have been met.
    else

        return {evilcrewmates,"shapeshifter_win"} --sab loose

    end
end

function CanKill(userinfo,targetinfo)
    if (userinfo.IsImpostor and not targetinfo.IsImpostor) then --if the person doing the kill is an impostor and the victim
        return true
    end
    if (userinfo.role == Game_GetRoleIDFromUUID("shapeshifters_Shapeshifter")) then
        if (not (targetinfo.role == Game_GetRoleIDFromUUID("shapeshifters_Shapeshifter"))) then
            return true
        end
    end
    return false
end


function BeforeKill(killer,victim)
    if (killer.role == Game_GetRoleIDFromUUID("shapeshifters_Shapeshifter")) then
        Client_ShowMessage("[6E0129FF]You turned into " .. victim.PlayerName .. ". []")
        killer.PlayerName = victim.PlayerName
        killer.ColorId = victim.ColorId
        killer.HatId = victim.HatId
        killer.SkinId = victim.SkinId
        victim.PlayerName = "Noone"
        victim.ColorId = 16
        victim.HatId = 0
        victim.SkinId = 0
        Game_SetRoles({victim},{Game_GetRoleIDFromUUID("shapeshifters_Dead")})
        Game_UpdatePlayerInfo(killer)
        Game_UpdatePlayerInfo(victim)

        return true
    end
    return true
end

function OnGameEnd()

end

function DecideRoles(playerinfos)
    return DecideRolesFunction(playerinfos)
end

function DecideRolesFunction(playerinfos)
    local RolesToGive = {}

    for i=1,Settings_GetNumber(0) do
        table.insert(RolesToGive,"shapeshifters_Shapeshifter")
    end

    local StartingCrewmateCount = #playerinfos - Settings_GetNumber(0)

    for i=1,StartingCrewmateCount do
        table.insert(RolesToGive,"shapeshifters_Crewmate")
    end

    local Selected = {}
    local SelectedRoles = {}
    for i=1, #RolesToGive do
        local impid = math.random(#playerinfos) --randomly set the impostor id
        table.insert(Selected,playerinfos[impid]) --add it to the selected list
        table.insert(SelectedRoles,RolesToGive[i])
        table.remove(playerinfos,impid) --remove the chosen item from the playerinfo list
    end
    return {Selected,SelectedRoles} -- sets the sheriff's role

end

function DecideImpostors(impostorcount,playerinfos)
    return {}
end
