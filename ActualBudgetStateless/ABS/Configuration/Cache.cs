namespace ABS.Configuration;

public class Cache<T>(Func<Task<T>> retriever, TimeSpan validity)
    where T : class
{
    private T? _value;
    private DateTime? _expires;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<T> GetValue()
    {
        if (_value == null || DateTime.Now > _expires)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_value == null || DateTime.Now > _expires)
                {
                    _value = await retriever();
                    _expires = DateTime.Now.Add(validity);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return _value;
    }

    public async Task Invalidate()
    {
        await _semaphore.WaitAsync();
        try
        {
            _value = null;
            _expires = null;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}