using UnityEngine;

public class Managers
{
    // --------------------------------------------------
    // Variables
    // --------------------------------------------------
    private readonly UIManager _uiManager = new();
    private readonly SoundManager _soundManager = new();
    private readonly ToastManager _toastManager = new();
    private readonly ReviewSystem _reviewSystem = new();
    private readonly HapticManager _hapticManager = new();
    private readonly ResourceManager _resourceManager = new();
    
    // --------------------------------------------------
    // Properties
    // --------------------------------------------------
    public static UIManager UI => Instance._uiManager;
    public static SoundManager Sound => Instance._soundManager;
    public static ToastManager Toast => Instance._toastManager;
    public static ReviewSystem Review => Instance._reviewSystem;
    public static HapticManager Haptic => Instance._hapticManager;
    public static ResourceManager Resource => Instance._resourceManager;

    // --------------------------------------------------
    // Singleton
    // --------------------------------------------------
    private static Managers _instance;
    public static bool IsInit = false;

    public static Managers Instance
    {
        get
        {
            if (IsInit == false)
            {
                IsInit = true;
                if (_instance == null)
                {
                    _instance = new Managers();
                }

                InitForce();
            }

            return _instance;
        }
    }
    
    // --------------------------------------------------
    // Functions - Nomal
    // --------------------------------------------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        if (IsInit == false)
        {
            IsInit = true;
            if (Instance == null)
            {
                _instance = new Managers();
            }

            InitForce();
        }
    }

    private static void InitForce()
    {
        CoroutineHelper.Init();
    }
}