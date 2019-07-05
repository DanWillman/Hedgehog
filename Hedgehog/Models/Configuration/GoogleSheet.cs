namespace Hedgehog.Models.Configuration
{
    /// <summary>
    /// Configuration model for GoogleSheet secrets
    /// </summary>
    public class GoogleSheet
    {
        /// <summary>
        /// Target sheet ID
        /// </summary>
        public string SheetId { get; set; }

        /// <summary>
        /// Target Sheet name for data destination
        /// </summary>
        public string SheetName { get; set; }
    }
}
