# JF Helper

Can be used to easily fix some things. For programmers it also adds some usefull utils.
If you have any questions or suggestions please message me in disscord: ```justafrogger```

### Merging the DLLs into your mod

Download the JFHelper.dll and the ServerSync.dll from the release section to the right.
Including the DLLs is best done via ILRepack (https://github.com/ravibpatel/ILRepack.Lib.MSBuild.Task). You can load
this package (ILRepack.Lib.MSBuild.Task) from NuGet.

If you have installed ILRepack via NuGet, simply create a file named `ILRepack.targets` in your project and copy the
following content into the file

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <Target Name="ILRepacker" AfterTargets="Build">
        <ItemGroup>
            <InputAssemblies Include="$(TargetPath)"/>
            <InputAssemblies Include="$(OutputPath)\JFHelper.dll"/>
        </ItemGroup>
        <ILRepack Parallel="true" DebugInfo="true" Internalize="true" InputAssemblies="@(InputAssemblies)"
                  OutputFile="$(TargetPath)" TargetKind="SameAsPrimaryAssembly" LibraryPath="$(OutputPath)"/>
    </Target>
</Project>
```

Make sure to set the JFHelper.dll in your project to "Copy to output directory" in the properties of the DLLs and to add
a reference to it.
After that, simply add `using JustAFrogger;`.
Then initialize JFHelper by this line of code `JFHelperLite.Initialize(Info);`.

## Example project

This code fixes MusicLocation in prefab `TestTown` located in bundle `npsssystem` and `SecondTown` in bundle `vilage`.

```csharp
using BepInEx;
using JustAFrogger;

namespace Test
{
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	public class Weapons : BaseUnityPlugin
	{
		private const string ModName = "Test";
		private const string ModVersion = "1.0";
		private const string ModGUID = "org.bepinex.plugins.Test";
		
		public void Awake()
		{
		    npsssystem = ItemManager.PrefabManager.RegisterAssetBundle("npsssystem");
		    LocationManager.Location TestTown = new(bundle, "TestTown")
		    {
		        Biome = Heightmap.Biome.Meadows,
		        Count = 15,
		        Prioritize = true,
		        Rotation = Rotation.Random,
		        CanSpawn = true,
		        MapIcon = "WoodNPSHouse",
		        ShowMapIcon = ShowIcon.Always,
		        GroupName = "Towns",
		        MinimumDistanceFromGroup = 500,
		        PreferCenter = true,
		        SpawnAltitude = new(10, 200)
		    };
		    LocationManager.Location SecondTown = new("vilage", "SecondTown")
		    {
		        Biome = Heightmap.Biome.BlackForest,
		        Count = 50,
		        Prioritize = true,
		        Rotation = Rotation.Random,
		        CanSpawn = true,
		        GroupName = "Towns",
		        MinimumDistanceFromGroup = 500,
		        PreferCenter = true,
		        SpawnAltitude = new(10, 200)
		    };
			
		    JFHelperLite.Initialize(Logger);
		    JFHelperLite.FixMusicLocation(bundle, "TestTown");
		    JFHelperLite.FixMusicLocation("vilage", "SecondTown");
		}
	}
}
```


In this example npc is looking for the nearest drop on the ground

```csharp
Collider[] colliderArray = Physics.OverlapSphere(transform.position, 8, MonsterAI.m_itemMask);
List<ItemDrop> drops = new();
foreach (Collider collider in colliderArray)
{
    if (collider.attachedRigidbody)
    {
        ItemDrop component = collider.attachedRigidbody.GetComponent<ItemDrop>();
        if (component && component.GetComponent<ZNetView>().IsValid()) drops.Add(component); 
    }
}

if(drops.Count == 0) return;
var drop = JFHelper.Nearest(drops, transform.position);
human.Pickup(drop.gameObject); 
```

In this example I'm geting vanila audio clip and output

```csharp
MusicLocation musicLocation;
musicLocation.m_audioSource.outputAudioMixerGroup = GetVanilaAudioMixer(musicLocation.m_audioSource.outputAudioMixerGroup.name);
musicLocation.m_audioSource.clip = GetVanilaMusic(musicLocation.m_audioSource.clip.name, showErrorIfCantFindAudioClip);
```
