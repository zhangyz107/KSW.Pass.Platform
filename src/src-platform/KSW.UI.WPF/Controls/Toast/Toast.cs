using KSW.UI.WPF.Enums;

namespace KSW.UI.WPF.Controls
{
    public class Toast : BindableBase, IToast
    {
        private string? _content;

        public Toast(string? content, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, bool showClose = true, Action? onClick = null, Action? onClose = null)
        {
            Content = content;
            Type = type;
            Expiration = expiration ?? TimeSpan.FromSeconds(3);
            ShowClose = showClose;
            OnClick = onClick;
            OnClose = onClose;
        }

        public Toast() : this(null)
        {

        }

        public string? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        /// <inheritdoc/>
        public NotificationType Type { get; set; }

        /// <inheritdoc/>
        public bool ShowIcon { get; set; }

        /// <inheritdoc/>
        public bool ShowClose { get; set; }

        /// <inheritdoc/>
        public TimeSpan Expiration { get; set; }

        /// <inheritdoc/>
        public Action? OnClick { get; set; }

        /// <inheritdoc/>
        public Action? OnClose { get; set; }
    }
}
