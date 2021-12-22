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
        /// <param name="isImmediate">Is the statement running in immediate-mode, or as part of a program?</param>
        public abstract void Execute(IEnvironment env, bool isImmediate);
    }
}
