public static class EvacuAgentSceneParamaters
{
    public static string SCENE_NAME = "Evacu-agent";
    public static string SHOOTER_TAG = "shooter";
    public static string PEDESTRIAN_TAG = "pedestrian";
    public static string SHOOTER_HIGHLIGHT_TAG = "shooterHighlight";
    public static string OBSTACLE_LAYER_NAME = "Obstacle";
    public static bool IS_FOV_VISUAL_ENABLED = true;

    // Highlights
    public static bool IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED = false;
    public static bool IS_WORKER_HIGHTLIGHT_VISUAL_ENABLED = false;
    public static bool IS_FRIEND_GROUP_LEADER_HIGHTLIGHT_VISUAL_ENABLED = false;
    public static bool IS_FRIEND_GROUP_FOLLOWER_HIGHTLIGHT_VISUAL_ENABLED = false;

    // Prefabs
    public static string RESEOURCES_PREFABS_PREFIX = "EvacuAgent/Prefabs/";
    public static string BEHAVIOUR_CONTROLLER_PREFAB = "EvacuAgent/Prefabs/BehaviourController";
    public static string SHOOTER_HIGHLIGHT_PREFAB = "EvacuAgent/Prefabs/ShooterHighlight";

    // Number of agent type
    public static int NUMBER_OF_SHOOTER_AGENTS = 1;
    public static int NUMBER_OF_WORKER_AGENTS = 20;
    public static int NUMBER_OF_FRIEND_GROUPS = 1;

    // Worker pedestrians
    public static float WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 0.5f;

    // Wait time for hospitality
    public static int HOSPITALITY_WAIT_TIME_LOWER_BOUND = 1;
    public static int HOSPITALITY_WAIT_TIME_UPPER_BOUND = 5;

    // Wait time for work
    public static int WORK_WAIT_TIME_LOWER_BOUND = 60;
    public static int WORK_WAIT_TIME_UPPER_BOUND = 60;

    // Group min and max bounds
    public static int FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM = 3;
    public static int FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM = 3;

    // Friend group boid weights
    public static float FRIEND_GROUP_BOID_COHESION_WEIGHT = 0.002f; // was 0.004
    public static float FRIEND_GROUP_BOID_SEPARATION_WEIGHT = 0.004f; // was 0.006 was good

}
