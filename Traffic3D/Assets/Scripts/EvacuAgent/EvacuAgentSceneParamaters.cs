public static class EvacuAgentSceneParamaters
{
    public static string SCENE_NAME = "Evacu-agent";
    public static string SHOOTER_TAG = "shooter";
    public static string SHOOTER_HIGHLIGHT_TAG = "shooterHighlight";
    public static bool IS_FOV_VISUAL_ENABLED = true;
    public static bool IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED = false;
    public static string RESEOURCES_PREFABS_PREFIX = "EvacuAgent/Prefabs/";

    // Number of agent type
    public static int NUMBER_OF_SHOOTER_AGENTS = 1;
    public static int NUMBER_OF_WORKER_AGENTS = 10;

    // Worker pedestrians
    public static float WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 0.5f;

    // Wait time for hospitality
    public static int HOSPITALITY_WAIT_TIME_LOWER_BOUND = 0;
    public static int HOSPITALITY_WAIT_TIME_UPPER_BOUND = 5;

    // Wait time for work
    public static int WORK_WAIT_TIME_LOWER_BOUND = 60;
    public static int WORK_WAIT_TIME_UPPER_BOUND = 60;
}
