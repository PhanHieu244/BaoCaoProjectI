public interface IValidator
{
#if UNITY_EDITOR
    void Validate();
#endif
}