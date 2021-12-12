using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace SampleGame.UIView
{
    public class RoomUIView : MonoBehaviour
    {
        [SerializeField] TMP_InputField _roomId;
        [SerializeField] TMP_InputField _username;
        [SerializeField] Button _joinButton;
        [SerializeField] Button _leaveButton;

        public IObservable<(string RoomId, string Username)> OnTriggerJoinRoom => _joinRoomTrigger.TakeUntilDestroy(this);
        private readonly Subject<(string RoomId, string Username)> _joinRoomTrigger = new Subject<(string RoomId, string Username)>();

        public IObservable<Unit> OnTriggerLeaveRoom => _leaveRoomTrigger.TakeUntilDestroy(this);
        private readonly Subject<Unit> _leaveRoomTrigger = new Subject<Unit>();

        void Awake()
        {
            _joinButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => 
                {
                    _roomId.interactable = false;
                    _username.interactable = false;
                    _joinButton.interactable = false;
                    _leaveButton.interactable = true;

                    _joinRoomTrigger.OnNext((_roomId.text, _username.text));
                });

            _leaveButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => 
                {
                    _roomId.interactable = true;
                    _username.interactable = true;
                    _joinButton.interactable = true;
                    _leaveButton.interactable = false;

                    _leaveRoomTrigger.OnNext(Unit.Default);
                });
        }
    }
}