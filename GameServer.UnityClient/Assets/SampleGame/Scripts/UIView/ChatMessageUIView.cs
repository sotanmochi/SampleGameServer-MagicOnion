using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using SampleGame.Utility;

namespace SampleGame.UIView
{
    public class ChatMessageUIView : MonoBehaviour
    {
        [SerializeField] TMP_InputField _messageInput;
        [SerializeField] Button _sendButton;
        [SerializeField] TMP_Text _chatMessageDisplay;
        [SerializeField] ContentSizeFitter _contentSizeFitter;
        [SerializeField] ScrollRect _scrollRect;

        public IObservable<string> OnTriggerSendingMessage => _sendMessageTrigger.TakeUntilDestroy(this);
        private readonly Subject<string> _sendMessageTrigger = new Subject<string>();

        void Awake()
        {
            _sendButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => 
                {
                    _sendMessageTrigger.OnNext(_messageInput.text);
                });

            _messageInput
                .ObserveEveryValueChanged(inputField => inputField.isFocused)
                .TakeUntilDestroy(this)
                .Subscribe(isFocused => 
                {
                    KeyInput.SetEnable(!isFocused);
                });
        }

        public void AppendMessage(string message)
        {
            _chatMessageDisplay.text = $"{_chatMessageDisplay.text}{message}\n";
            _contentSizeFitter.SetLayoutVertical();
            _scrollRect.verticalNormalizedPosition = 0;
        }
    }
}