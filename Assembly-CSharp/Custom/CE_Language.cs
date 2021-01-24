using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class CE_LanguageManager
{
    private static Dictionary<string, CE_Language> BaseLanguages = new Dictionary<string, CE_Language>();
    private static Dictionary<byte, CE_Language> GMLanguages = new Dictionary<byte, CE_Language>();
    public static bool AddGMLanguage(byte key, CE_Language lang)
    {
        return GMLanguages.TryAdd(key, lang);
    }

    public static CE_Language GetGMLanguage(byte key)
    {
        CE_Language lang;
        if (GMLanguages.TryGetValue(key, out lang))
        {
            return lang;
        }
        return null;
    }

}

public class CE_Language
{
    private Dictionary<string, CE_LanguageEntry> LanguageIndex = new Dictionary<string, CE_LanguageEntry>();

    public CE_Language(bool adddefaultvalues)
    {
        if (adddefaultvalues)
        {
            AddEntry("Game_ImpsRemain", "{0} impostor{1} remains.");
            AddEntry("Game_WasNotImp", "{0} was not {1} Impostor.");
            AddEntry("Meeting_WhoIsImpostor", "Who Is The Impostor?");
            AddEntry("UI_CantCallMeeting", "EMERGENCY MEETINGS CANNOT\n\rBE CALLED RIGHT NOW");
        }
    }
    public string GetFormatted(string key, params object[] arg)
    {
        CE_LanguageEntry ent;
        if (LanguageIndex.TryGetValue(key, out ent))
        {
            if (ent.IsLua)
            {
                throw new NotImplementedException();
            }
            return String.Format(ent.GetText(), arg);
        }
        return "Missing/Invalid Key: " + key;
    }
    public string GetText(string key)
    {
        CE_LanguageEntry ent;
        if (LanguageIndex.TryGetValue(key, out ent))
        {
            if (ent.IsLua)
            {
                throw new NotImplementedException();
            }
            return ent.GetText();
        }
        return "Missing/Invalid Key: " + key;
    }

    public bool AddEntry(string key, string text)
    {
        if (LanguageIndex.TryAdd(key,new CE_LanguageEntry(text)))
        {
            return true;
        }
        return false;
    }
}
public class CE_LanguageEntry
{
    public bool IsLua { get; private set; }
    public string Text { get; private set; }
    public CE_LanguageEntry()
    {
        IsLua = false;
        Text = "Bob";
    }
    public CE_LanguageEntry(bool lua)
    {
        IsLua = lua;
    }
    public CE_LanguageEntry(string text)
    {
        Text = text;
    }
    public CE_LanguageEntry(string defaulttext,bool lua)
    {
        Text = defaulttext;
        IsLua = lua;
    }

    public string GetText()
    {
        return Text; //placeholder
    }
}

