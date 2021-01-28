namespace AppLayer.Commands
{
    public interface ICommand
    {
        void Execute(IExecutor executor);
    }
}