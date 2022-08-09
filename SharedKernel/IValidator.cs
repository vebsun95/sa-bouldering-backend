namespace backend.SharedKernel;

public interface IValidator<T>
{
    (bool IsValid, string Error) isValid(T item);
}