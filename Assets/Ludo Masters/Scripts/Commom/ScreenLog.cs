using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class ScreenLog : MonoBehaviour
    {
        public static ScreenLog instance;
        bool updateLog;
        string currentLog = "";
        public Text text;
        public Button clearButton;

        void Awake()
        {
            instance = this;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Create()
        {
            if (TestingMode.logs)
                Instantiate(Resources.Load("ScreenLog") as GameObject);
        }

        public static void Remove()
        {
            if (instance) Destroy(instance.gameObject);
        }

        public static void ForceLog(string log)
        {
            if (TestingMode.logs)
            {
                instance.currentLog += "\n\n" + log;
                instance.updateLog = true;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Application.logMessageReceived += LogMessageReceived;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= LogMessageReceived;
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(text.text))
            {
                if (clearButton.gameObject.activeInHierarchy)
                {
                    clearButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!clearButton.gameObject.activeInHierarchy)
                {
                    clearButton.gameObject.SetActive(true);
                }
            }

            if (updateLog)
            {
                text.text = currentLog;
            }
        }

        public void OnClearButtonClicked()
        {
            currentLog = "";
            updateLog = true;
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    currentLog += "\n\n<color=red>" + condition + ": " + stackTrace + "</color>";
                    break;

                case LogType.Exception:
                    currentLog += "\n\n<color=red>" + condition + ": " + stackTrace + "</color>";
                    break;

                case LogType.Warning:
                    currentLog += "\n\n<color=yellow>" + condition + "\n" + "</color>";
                    break;

                default:
                    currentLog += "\n\n" + condition;
                    break;
            }

            updateLog = true;
        }
    }
}