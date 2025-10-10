namespace KSW.UI.WPF.Controls
{
    public interface IToast : IMessage
    {
        /// <summary>
        /// Gets the toast message.
        /// </summary>
        string? Content { get; }
    }
}
