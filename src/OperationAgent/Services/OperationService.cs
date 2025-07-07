namespace AddAgent.Services;

public interface IOperationService
{
    int Add(int a, int b);
    int Subtract(int a, int b);
}

public class OperationService : IOperationService
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Subtract(int a, int b)
    {
        return a - b;
    }
}