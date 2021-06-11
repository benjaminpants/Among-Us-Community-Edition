using UnityEngine;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.IO;
public class CE_Extensions
{
	private static bool hasPlayed;

	private static CE_Intro Intro;

	private static bool TitleChanged = false;

	//Import the following.
	[DllImport("user32.dll", EntryPoint = "SetWindowText")]
	public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
	[DllImport("user32.dll", EntryPoint = "FindWindow")]
	public static extern System.IntPtr FindWindow(System.String className, System.String windowName);
	[DllImport("user32.dll")]
	private static extern System.IntPtr GetActiveWindow();

	public static string GetGameDirectory()
    {
		return System.IO.Directory.GetParent(Application.dataPath).FullName;
    }

	public static string GetTexturesDirectory(string ExtraDir = null)
    {
		if (string.IsNullOrEmpty(ExtraDir)) return System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures");
		else return System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", ExtraDir);
	}

	public static void UpdateWindowTitle()
    {
		if (!TitleChanged)
        {
			bool isFocused = Application.isFocused;
			var windowPtr = FindWindow(null, "Among Us");
			if (windowPtr == GetActiveWindow() && isFocused)
			{
				SetWindowText(windowPtr, "Among Us: Community Edition");
				TitleChanged = true;
			}
		}
	}

	private static void PlayIntro()
	{
		if (!SaveManager.HideIntro)
		{
			Intro = UnityEngine.Object.Instantiate(new GameObject()).AddComponent<CE_Intro>();
			CE_Intro.IsShown = true;
		}

	}

	

	public static void OnStartup()
	{
		Debug.Log("OnStartup");
        CE_UIHelpers.LoadDebugConsole();
        new GameObject().AddComponent<CE_ModUI>().name = "ModUI";
        new GameObject().AddComponent<CE_ModErrorUI>().name = "ModErrorUI";
        if (!hasPlayed)
        {
			ResolutionManager.SetVSync(SaveManager.EnableVSync);
			CE_RoleManager.AddRole(new CE_Role());
			//CE_LuaLoader.LoadLua();
			CE_ModLoader.LoadMods();
            PlayIntro();
            CE_CustomMapManager.Initialize();
            hasPlayed = true;
		}

	}
}

internal static class ExtensionMethods
{

	private static bool IsCharacterAVowel(char c)
	{
		string vowels = "aeiou";
		return vowels.IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
	}


	public static string AOrAn(this string input, bool firstlettercap)
	{
        return (firstlettercap ? "A" : "a") + (IsCharacterAVowel(input[0]) ? "n" : ""); //thanks rose, developer on polus.gg!!!!


    }
    /*public static List<string> ToList(this StringBuilder stringBuilder)
	{
		char[] split = new char[2]
				{
					'\r','\n'
				};
		return stringBuilder.ToString().Split(split, StringSplitOptions.RemoveEmptyEntries).ToList();
	}*/
}



namespace Steam_acf_File_Reader
{
    class AcfReader
    {
        public string FileLocation { get; private set; }

        public AcfReader(string FileLocation)
        {
            if (File.Exists(FileLocation))
                this.FileLocation = FileLocation;
            else
                throw new FileNotFoundException("Error", FileLocation);
        }

        public bool CheckIntegrity()
        {
            string Content = File.ReadAllText(FileLocation);
            int quote = Content.Count(x => x == '"');
            int braceleft = Content.Count(x => x == '{');
            int braceright = Content.Count(x => x == '}');

            return ((braceleft == braceright) && (quote % 2 == 0));
        }

        public ACF_Struct ACFFileToStruct()
        {
            return ACFFileToStruct(File.ReadAllText(FileLocation));
        }

        private ACF_Struct ACFFileToStruct(string RegionToReadIn)
        {
            ACF_Struct ACF = new ACF_Struct();
            int LengthOfRegion = RegionToReadIn.Length;
            int CurrentPos = 0;
            while (LengthOfRegion > CurrentPos)
            {
                int FirstItemStart = RegionToReadIn.IndexOf('"', CurrentPos);
                if (FirstItemStart == -1)
                    break;
                int FirstItemEnd = RegionToReadIn.IndexOf('"', FirstItemStart + 1);
                CurrentPos = FirstItemEnd + 1;
                string FirstItem = RegionToReadIn.Substring(FirstItemStart + 1, FirstItemEnd - FirstItemStart - 1);

                int SecondItemStartQuote = RegionToReadIn.IndexOf('"', CurrentPos);
                int SecondItemStartBraceleft = RegionToReadIn.IndexOf('{', CurrentPos);
                if (SecondItemStartBraceleft == -1 || SecondItemStartQuote < SecondItemStartBraceleft)
                {
                    int SecondItemEndQuote = RegionToReadIn.IndexOf('"', SecondItemStartQuote + 1);
                    string SecondItem = RegionToReadIn.Substring(SecondItemStartQuote + 1, SecondItemEndQuote - SecondItemStartQuote - 1);
                    CurrentPos = SecondItemEndQuote + 1;
                    ACF.SubItems.Add(FirstItem, SecondItem);
                }
                else
                {
                    int SecondItemEndBraceright = RegionToReadIn.NextEndOf('{', '}', SecondItemStartBraceleft + 1);
                    ACF_Struct ACFS = ACFFileToStruct(RegionToReadIn.Substring(SecondItemStartBraceleft + 1, SecondItemEndBraceright - SecondItemStartBraceleft - 1));
                    CurrentPos = SecondItemEndBraceright + 1;
                    ACF.SubACF.Add(FirstItem, ACFS);
                }
            }

            return ACF;
        }

    }

    class ACF_Struct
    {
        public Dictionary<string, ACF_Struct> SubACF { get; private set; }
        public Dictionary<string, string> SubItems { get; private set; }

        public ACF_Struct()
        {
            SubACF = new Dictionary<string, ACF_Struct>();
            SubItems = new Dictionary<string, string>();
        }

        public void WriteToFile(string File)
        {

        }

        public override string ToString()
        {
            return ToString(0);
        }

        private string ToString(int Depth)
        {
            StringBuilder SB = new StringBuilder();
            foreach (KeyValuePair<string, string> item in SubItems)
            {
                SB.Append('\t', Depth);
                SB.AppendFormat("\"{0}\"\t\t\"{1}\"\r\n", item.Key, item.Value);
            }
            foreach (KeyValuePair<string, ACF_Struct> item in SubACF)
            {
                SB.Append('\t', Depth);
                SB.AppendFormat("\"{0}\"\n", item.Key);
                SB.Append('\t', Depth);
                SB.AppendLine("{");
                SB.Append(item.Value.ToString(Depth + 1));
                SB.Append('\t', Depth);
                SB.AppendLine("}");
            }
            return SB.ToString();
        }
    }

    static class Extension
    {
        public static int NextEndOf(this string str, char Open, char Close, int startIndex)
        {
            if (Open == Close)
                throw new Exception("\"Open\" and \"Close\" char are equivalent!");

            int OpenItem = 0;
            int CloseItem = 0;
            for (int i = startIndex; i < str.Length; i++)
            {
                if (str[i] == Open)
                {
                    OpenItem++;
                }
                if (str[i] == Close)
                {
                    CloseItem++;
                    if (CloseItem > OpenItem)
                        return i;
                }
            }
            throw new Exception("Not enough closing characters!");
        }
    }
}