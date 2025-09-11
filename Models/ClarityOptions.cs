namespace TaskManagementMvc.Models
{
    /// <summary>
    /// Configuration options for Microsoft Clarity analytics integration
    /// </summary>
    public class ClarityOptions
    {
        /// <summary>
        /// Microsoft Clarity Project ID
        /// </summary>
        public string ProjectId { get; set; } = string.Empty;

        /// <summary>
        /// Whether Clarity tracking is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// Analytics configuration container
    /// </summary>
    public class AnalyticsOptions
    {
        /// <summary>
        /// Microsoft Clarity configuration
        /// </summary>
        public ClarityOptions MicrosoftClarity { get; set; } = new ClarityOptions();
    }
}
