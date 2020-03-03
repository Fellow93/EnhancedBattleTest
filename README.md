# Enhanced Battle Test

A mod for Mount&Blade Bannerlord that can test battle locally.

## Features
- Test Battle Mode: You can choose where to spawn troops and they will be all spawned instantly.

- Custom Battle Mode: Use built-in mechanism to spawn troops. Troops that exceeds the battle size limit will be spawned later.

- Map selection. Including sergeant maps, skirmish maps, tdm map, etc.

  However, I removed siege maps because they are buggy.

  Custom battle mode contains sergeant maps only, because only sergeant maps contains corrrect spawning positions that custom battle mode requires.

  If you want more maps, you can edit the config file yourself, details below.

- Character selection. You can specify at most three types of troops for each team.

  Also you can select **perks** that is consistent with those in Multiplayer mode.

- Configuration saving. The battle configuration is saved in "(user directory)\Documents\Mount and Blade II Bannerlord\Configs\EnhancedBattleTest\".

  The configuration for Test Battle mode is saved in "EnhancedTestBattleConfig.xml" and that for Custom Battle mode is saved in "EnhancedCustomBattleConfig.xml".

  You can modify it to add more maps, but if you edit it incorrectly, the configuration will be reset to default, or the game may crash. I don't guarantee anything.

- Switching player's team. You can switch between player agent and the enemy commander to control their troops respectively.

- Controlling your bots after dead.

- Switching free camera.

- Undead mode. Any agent will not die after switched on.

- Adjusting combat AI between 0 and 100.

- Customizing player characters, details below.

## How to install
1. Copy `bin` and `Modules` into Bannerlord installation folder(For example `C:\Program Files\Steam\steamapps\common\Mount & Blade II Bannerlord - Beta`).

2. Remember to reinstall the mod after game updating. This may solve some problems.

## How to use
- Start the mod by clicking `EnhancedBattleTest.bat` in `bin\Win64_Shipping_Client` that you have copied into Bannerlord installation folder.

- You can select troops for each side of teams.

- Press and hold `TAB` key for a while to exit the battle scene.

- Press `numpad5` key to switch your team.

- Press `numpad6` key to switch between free camera and main agent camera.

- Press `f` key or `numpad6` key to control one of your troops after you being killed.

- Press `numpad7` to disable dying.

- Press `L` key to teleport player when in free camera mode.

### How to add more maps
- You can go to `Modules\Native\SceneObj` to find available maps.

- To add more maps, you need to edit the configuartion file(in folder "(user directory)\Documents\Mount and Blade II Bannerlord\Configs\EnhancedBattleTest\").

- The maps available in mod are under the xml element `sceneList`.

  The element `SceneInfo` represents a map and related config.

  So you need to add a `SceneInfo` element as a child of `sceneList`, just like the other maps.

  For example you can copy the text between `<SceneInfo>` and `</SceneInfo>`, then replace the text between `<name>` and `</name>` with the folder name of the map you want to add.

  The other config like formation positions can be configured in the game.

### How to customize characters
- You can customize your characters by modifying the xml elements with id `player_character_1`, `player_character_2` and `player_character_3` in `Modules\EnhancedBattleTest\ModuleData\mpcharacters.xml`.

- This character is referred in another file `Modules\EnhancedBattleTest\ModuleData\mpclassdivisions.xml`, in which armors, movement speed and other properties of the character are defined.

- **However**, currently in Bannerlord b0.8.0 (also in b0.8.1), merging those two `mpclassdivisions.xml` files (one in `native` and one in this mod), and parsing them is **NOT** correctly implemented: the spaces between xml elements are not ignored, and the game will crash.

  This is a bug in Bannerlord b0.8.0/b0.8.1, and the work-around is to remove all the spaces between xml elements in both files.
  
  I have done this for you. So you don't need to worry about it if you don't modify those two `mpclassdivisions.xml` files.

  If you need to modify any of those two files, please remember to remove all the spaces between the xml elements just like I do.

  I use vscode with xml extension to remove spaces automatically.

- If you modified those files or the game updated, and the mod could not start, try to reinstall the mod.

  If it does not work, then try to remove the following content in `Modules\EnhancedBattleTest\SubModule.xml`:
  ```
  <XmlNode>
		<XmlName id="MPClassDivisions" path="mpclassdivisions"/>
	</XmlNode>
  ```
  This should make the game not to load `mpclassdivisions.xml` in this mod anymore, and do not merge it with the one in `native`. So it shound be impossible to trigger the bug mentioned above. However, the characters defined in `mpcharacters.xml` can no longer be spawned in the game.

- Don't blame me, blame the code that TaleWorlds wrote.

- Hope to see this bug fixed soon.


## Build from source:
The source code is located in the `source` folder or available at https://gitlab.com/lzh_mb_mod/enhancedbattletest.

1. install .net core sdk

2. modify 6th line of `EnhancedBattleTest.csproj`, change `Mb2Bin` property to your bannerlord installation location

3. open a termial (powershell or cmd), run `dotnet msbuild -t:install`. This step will build `EnhancedBattleTest.dll` and copy it to `bin\Win64_Shipping_Client`

## Bug:
Some people say they can't launch the mod, if you are among them, try building from source code.

I guess there will be compilation error, If you are a programmer, it's not difficult to fix.

If you are a normal user, google the compilation error or post it to forum to ask for help.

If you find the cause of crash, please tell me.

## Contact with me:
* Please mail to: lizhenhuan1019@qq.com

* This mod is originated from mod "Battle Test" written by "Modbed", who does not maintain "Battle Test" anymore.
  
  Way to contact him:
  
  TaleWorlds forum: modbed
  
  youtube: modbed
  
  bilibili: modbed帅
  
  website: modbed.cn
