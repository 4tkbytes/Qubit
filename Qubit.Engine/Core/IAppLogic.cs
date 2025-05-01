namespace Qubit.Engine.Core
{
    public interface IAppLogic
    {
        void OnLoad();

        void OnUpdate(double deltaTime);

        void OnRender(double deltaTime);
    }
}
