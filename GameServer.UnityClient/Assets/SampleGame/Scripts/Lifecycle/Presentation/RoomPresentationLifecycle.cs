using UnityEngine;
using Cysharp.Threading.Tasks;
using SampleGame.Context;
using SampleGame.Presenter;
using SampleGame.UIView;

namespace SampleGame.Lifecycle.Presentation
{
    public class RoomPresentationLifecycle : LifecycleBase
    {
        [SerializeField] private RoomUIView _uiView;
        private RoomPresenter _presenter;

        public override async UniTask InitializeAsync()
        {
            var context = ServiceLocator.GetInstance<NetworkServiceContext>();
            _presenter = new RoomPresenter(_uiView, context);
            _presenter.Initialize();
        }

        public override async UniTask DisposeAsync()
        {
            _presenter.Dispose();
        }
    }
}