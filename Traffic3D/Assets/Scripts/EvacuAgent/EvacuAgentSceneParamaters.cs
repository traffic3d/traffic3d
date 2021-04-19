public static class EvacuAgentSceneParamaters
{
    // Tags and names
    public static string SCENE_NAME = "Evacu-agent";
    public static string SHOOTER_TAG = "shooter";
    public static string PEDESTRIAN_TAG = "pedestrian";
    public static string FRIEND_TAG = "friend";
    public static string WORKER_TAG = "worker";
    public static string SHOOTER_HIGHLIGHT_TAG = "shooterHighlight";
    public static string WORKER_HIGHLIGHT_TAG = "workerHighlight";
    public static string FRIEND_GROUP_HIGHLIGHT_TAG = "friendGroupHighlight";
    public static string OBSTACLE_LAYER_NAME = "Obstacle";
    public static bool IS_FOV_VISUAL_ENABLED = true;

    // Highlights
    public static bool IS_SHOOTER_HIGHTLIGHT_VISUAL_ENABLED = false;
    public static bool IS_WORKER_HIGHTLIGHT_VISUAL_ENABLED = false;
    public static bool IS_FRIEND_GROUP_LEADER_HIGHTLIGHT_VISUAL_ENABLED = false;
    public static bool IS_FRIEND_GROUP_FOLLOWER_HIGHTLIGHT_VISUAL_ENABLED = false;

    // Prefabs
    public static string RESEOURCES_PREFABS_PREFIX = "EvacuAgent/Prefabs/";
    public static string BEHAVIOUR_CONTROLLER_PREFAB = "EvacuAgent/Prefabs/Behaviour_Structure/BehaviourController";
    public static string SHOOTER_HIGHLIGHT_PREFAB = "EvacuAgent/Prefabs/Pedestrian_Highlights/ShooterHighlight";

    // Number of agent type
    public static int NUMBER_OF_SHOOTER_AGENTS = 0;
    public static int NUMBER_OF_WORKER_AGENTS = 100;
    public static int NUMBER_OF_FRIEND_GROUPS = 50;

    // Worker pedestrians
    public static float WORKER_CHANCE_TO_VISIT_HOSPITALITY_POINT_IN_ROUTE = 0.5f;

    // Wait time for hospitality
    public static int HOSPITALITY_WAIT_TIME_LOWER_BOUND = 1;
    public static int HOSPITALITY_WAIT_TIME_UPPER_BOUND = 5;

    // Wait time for work
    public static int WORK_WAIT_TIME_LOWER_BOUND = 20;
    public static int WORK_WAIT_TIME_UPPER_BOUND = 30;

    // Wait time for recreation
    public static int RECREATION_WAIT_TIME_LOWER_BOUND = 1;
    public static int RECREATION_WAIT_TIME_UPPER_BOUND = 5;

    // Wait time for landmark
    public static int LANDMARK_WAIT_TIME_LOWER_BOUND = 1;
    public static int LANDMARK_WAIT_TIME_UPPER_BOUND = 5;

    // Wait time for shopping
    public static int SHOPPING_WAIT_TIME_LOWER_BOUND = 1;
    public static int SHOPPING_WAIT_TIME_UPPER_BOUND = 5;

    // Friend group min and max bounds
    public static int FRIEND_GROUP_FOLLOWER_COUNT_MINIMUM = 2;
    public static int FRIEND_GROUP_FOLLOWER_COUNT_MAXIMUM = 5;

    // Worker group min and max bounds
    public static int WORKER_GROUP_FOLLOWER_COUNT_MINIMUM = 0;
    public static int WORKER_GROUP_FOLLOWER_COUNT_MAXIMUM = 0;

    // Friend group boid weights
    public static float FRIEND_GROUP_BOID_COHESION_WEIGHT = 0.0008f; // was 0.002
    public static float FRIEND_GROUP_BOID_SEPARATION_WEIGHT = 0.004f; // was 0.004
    public static float FRIEND_GROUP_BOID_TARGET_SEEKING_WEIGHT = 0.0004f; // was 0.00002
    public static float FRIEND_GROUP_BOID_INTER_GROUP_SEPARATION_WEIGHT = 0.0008f;

    // Worker group boid weights
    public static float WORKER_GROUP_BOID_COHESION_WEIGHT = 0.0008f;
    public static float WORKER_GROUP_BOID_SEPARATION_WEIGHT = 0.006f;
    public static float WORKER_GROUP_BOID_TARGET_SEEKING_WEIGHT = 0.0002f; // was 0.006 was good
    public static float WORKER_GROUP_BOID_INTER_GROUP_SEPARATION_WEIGHT = 0.0008f;

}
