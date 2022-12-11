using Manager;
using TMPro;
using UnityEngine;

namespace UISystem
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text CountText;
        private const string Title = "남은 학생 수 : ";

        private void Update()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            int count = AutoManager.Manager.Get<GameManager>().ZombieCount;
            CountText.text = Title + count;
        }
    }
}