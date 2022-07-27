namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IViewer : IRaycastable
    {
        /// <summary>
        /// Last seen target
        /// </summary>
        ISeeable CurrentTarget { get; }

        /// <summary>
        /// See target
        /// </summary>
        /// <param name="target"></param>
        void See(ISeeable target);

        /// <summary>
        /// Is currently seeing any target
        /// </summary>
        bool Seeing { get; set; }
    }
}