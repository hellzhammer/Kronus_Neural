namespace Kronus_Neural.Activations
{
    public interface IActivation<T>
    {
        T Activate(T input, bool deriv);
    }
}
