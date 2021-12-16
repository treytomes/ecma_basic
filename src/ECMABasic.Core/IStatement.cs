namespace ECMABasic.Core
{
    /// <summary>
    /// Represents a single executable statement.
    /// </summary>
    public interface IStatement : IListable
    {
        /// <summary>
        /// Execute this statement inside of an environment.
        /// </summary>
        /// <param name="env">The environment to execute the statement inside of.</param>
        public abstract void Execute(IEnvironment env);
    }
}
