namespace WeatherApp.Wrapper
{

    public class ResultModel
    {
        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; }
    }

    public class ResultModel<T>
    {
        public T? Value { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
}

