using UnityEngine;


namespace Assets.Game.Presentation.Views
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField]
        private Transform _hooksParent;

        [SerializeField]
        private HookView _hookPrefab;

        [SerializeField]
        private KeyView _keyPrefab;


    }
}