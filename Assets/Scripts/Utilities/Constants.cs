using System.IO;
using UnityEngine;

public class Constants
{
#if UNITY_STANDALONE//PC����
    public const string RUNTIME_ENVIRONMENT = RUNTIME_ENVIRONMENT_PC;
#elif UNITY_ANDROID//ANDROID����
    public const string RUNTIME_ENVIRONMENT =RUNTIME_ENVIRONMENT_ANDROID;
#elif UNITY_WEBGL && !UNITY_EDITOR//WEBGL��ҳ����
    public const string RUNTIME_ENVIRONMENT = RUNTIME_ENVIRONMENT_WEBGL;
#elif UNITY_EDITOR//�༭����������PC��������
    public const string RUNTIME_ENVIRONMENT = RUNTIME_ENVIRONMENT_PC;
#endif

    public const string RUNTIME_ENVIRONMENT_PC = "PC";
    public const string RUNTIME_ENVIRONMENT_ANDROID = "ANDROID";
    public const string RUNTIME_ENVIRONMENT_WEBGL = "WEBGL";
    public const string RUNTIME_ENVIRONMENT_WEBGL_PC = "WEBGL_PC";
    public const string RUNTIME_ENVIRONMENT_WEBGL_MOBILE = "WEBGL_MOBILE";

    public const string SAVE_DATA_FILE_NAME = "saveData.json";
    public const string COLLECTION_DATA_FILE_NAME = "collectionList.json";
    public const string BUTTON_SETTINGS_FILE_NAME = "buttonSettings.json";
    public const string SETTINGS_FILE_NAME = "settings.json";
    public const string GAME_SCENE_SO_PATH = "Assets/DataSO/GameScenes/";
    public const string PREFAB_PATH = "Assets/Prefabs/";
    public const string BOAR_PREFAB_PATH = "Assets/Prefabs/Boar.prefab";
    public const string SNAIL_PREFAB_PATH = "Assets/Prefabs/Snail.prefab";
    public const string KEY_PREFAB_PATH = "Assets/Prefabs/Key.prefab";

    public const string KEY_PREFAB_NAME = "Key";

    public const float IMAGE_DEFAULT_HALF_RGBA = 0.2f;


    //��ҳ�ʼ����
    public const int PLAYER_INIT_HP = 100;
    public const int PLAYER_INIT_ENERGY = 100;


    //����������������������
    public const float DEAD_ZONE_WIDTH = 0.05f;
    public const float DEAD_ZONE_HEIGHT = 0.1f;
    public const float SOFT_ZONE_WIDTH = 0.8f;
    public const float SOFT_ZONE_HEIGHT = 0.8f;

    //Menu����Title��Btn�ĳ�ʼ�����С
    public const float MENU_TITLE_FONT_SIZE = 140;
    public const float MENU_BTN_FONT_SIZE = 60;

}
