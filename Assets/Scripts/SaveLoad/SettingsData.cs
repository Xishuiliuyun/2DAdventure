using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData 
{
    public GeneralSettingsData generalSettingsData;
    public VolumeSettingsData volumeSettingsData;
    public ScreenSettingsData screensettingsData;
}

public class GeneralSettingsData
{
    public bool showEnemyHP;
    public bool showDamage;
    public bool isScreenShock;
    public float handleShock;
    public Language language;
}

public class VolumeSettingsData
{
    public float masterVolume;
    public float BGMVolume;
    public float FXVolume;
}

public class ScreenSettingsData
{
    public bool fullScreen;
    public bool dynamicBlur;
    public bool verticalSynchronization;
    public float brightness;
    public float contrastRatio;
    public ResolvingPower resolvingPower;
}




