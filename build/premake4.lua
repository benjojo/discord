solution "discord"
	language "C#"
	framework "4.0"
	location ( os.get() .. "/" .. _ACTION )
	targetdir ( "../bin/" .. os.get() .. "/" .. _ACTION .. "/" )
	flags { "ExtraWarnings" }
	defines { "TRACE" }
	libdirs { "..\\libs" }
	
	links { "System", "System.Core", "System.Xml.Linq", "System.Data.DataSetExtensions", "System.Data", "System.Xml" }
	
	configurations { "Debug", "Release" }
		
	configuration "Debug"
		defines { "DEBUG" }
		flags { "Symbols" }
		buildoptions { "-debug" }
		
	configuration "Release"
		flags { "Optimize" }

	project "discord"
		uuid "19067008-561a-4f1b-a3a1-005e8d441326"
		files { "../discord/**.cs" }
		kind "ConsoleApp"
		links { "core" }
		
	project "core"
		uuid "caaf6c82-27fe-4274-8945-c3162e045726"
		files { "../core/**.cs" }
		kind "SharedLib"
		links { "SteamKit2" }
		
solution "discord-plugins"
	language "C#"
	framework "4.0"
	location ( os.get() .. "/" .. _ACTION )
	targetdir ( "../bin/" .. os.get() .. "/" .. _ACTION .. "/plugins/" )
	flags { "ExtraWarnings" }
	defines { "TRACE" }
	libdirs { "..\\libs" }
	
	links { "System", "System.Core", "System.Xml.Linq", "System.Data.DataSetExtensions", "System.Data", "System.Xml" }
	
	configurations { "Debug", "Release" }
		
	configuration "Debug"
		defines { "DEBUG" }
		flags { "Symbols" }
		buildoptions { "-debug" }
		
	configuration "Release"
		flags { "Optimize" }
	
	project "test"
		files { "../plugins/test/**.cs" }
		kind "SharedLib"
		links { "core", "SteamKit2" }